using AssemblyCSharp;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SC_MenuLogic : MonoBehaviour
{
    private string apiKey = "c1dec05fa72fef6b16a677c13c874936fd0132a45023b8d04d5626e694edee94";
    private string apiSecret = "cb38afa602f545c1be39c1d1a544dc6d30a83d5b245eac790e0364d6ee78820c";
    private Dictionary<string, GameObject> unityObjects;
    private SC_Enums.Screens currentScreen;
    private SC_Enums.Screens prevScreen;
    private Listener listener;
    private Dictionary<string, object> passedParams;
    private string roomId;
    private List<string> roomIds;
    private int roomIndex;
    private string userId = System.DateTime.Now.Ticks.ToString();
    Stack<SC_Enums.Screens> back = new Stack<SC_Enums.Screens>();
    private Scene scene;
    #region Callbacks
    private void OnConnect(bool _IsSuccess)
    {
        Debug.Log("OnConnect " + _IsSuccess);
        if (_IsSuccess)
        {
            unityObjects["Btn_MultiPlayer_EnterRoom"].GetComponent<Button>().interactable = true;
            UpdateStatus("Connected.");
        }
        else
        {
            if(!(unityObjects["Btn_Play"].Equals(null)))
            {
                unityObjects["Btn_Play"].GetComponent<Button>().interactable = false;
                UpdateStatus("Failed to Connect.");
            }
        }
        //  WarpClient.GetInstance().chat
    }

    private void OnRoomsInRange(bool _IsSuccess, MatchedRoomsEvent eventObj)
    {
        Debug.Log("OnRoomsInRange " + _IsSuccess);
        if (_IsSuccess)
        {
            UpdateStatus("Parsing Rooms.");
            roomIds = new List<string>();
            foreach (var RoomData in eventObj.getRoomsData())
            {
                Debug.Log("Room Id: " + RoomData.getId());
                Debug.Log("Room Owner: " + RoomData.getRoomOwner());
                roomIds.Add(RoomData.getId());
            }
        }

        roomIndex = 0;
        DoRoomsSearchLogic();
    }

    private void OnCreateRoom(bool _IsSuccess, string _RoomId)
    {
        Debug.Log("OnCreateRoom: " + _IsSuccess + ", RoomId: " + _RoomId);
        if (_IsSuccess)
        {
            roomId = _RoomId;
            unityObjects["Txt_Status"].GetComponent<Text>().text = "RoomId: " + roomId;
            UpdateStatus("Room have been created, RoomId: " + roomId);
            WarpClient.GetInstance().JoinRoom(roomId);
            WarpClient.GetInstance().SubscribeRoom(roomId);
        }
    }

    private void OnJoinRoom(bool _IsSuccess, string _RoomId)
    {
        Debug.Log("OnJoinRoom: " + _IsSuccess + ", RoomId: " + _RoomId);
        if (_IsSuccess)
            UpdateStatus("Joined Room: " + _RoomId + ", Waiting for an opponent...");
        else UpdateStatus("Failed to join room: " + _RoomId);
    }
    private void OnGetLiveRoomInfo(LiveRoomInfoEvent eventObj)
    {
        Debug.Log("OnGetLiveRoomInfo " + eventObj.getProperties());
        if (eventObj != null && eventObj.getProperties() != null && eventObj.getProperties().ContainsKey("Password") &&
           eventObj.getProperties()["Password"].ToString() == passedParams["Password"].ToString())
        {
            Debug.Log("Matched Room!");
            roomId = eventObj.getData().getId();
            UpdateStatus("Recived Room info, joining room... (" + roomId + ")");
            WarpClient.GetInstance().JoinRoom(roomId);
            WarpClient.GetInstance().SubscribeRoom(roomId);
        }
        else
        {
            roomIndex++;
            DoRoomsSearchLogic();
        }
    }

    private void OnUserJoinRoom(RoomData eventObj, string _UserName)
    {
        Debug.Log("User joined room: " + _UserName);
        UpdateStatus("User joined room: " + _UserName);
        if (eventObj.getRoomOwner() == userId && userId != _UserName)
        {
            UpdateStatus("Starting game...");
            WarpClient.GetInstance().startGame();
        }
    }

    private void OnGameStarted(string _Sender, string _RoomId, string _NextTurn)
    {
        UpdateStatus("The game have started, nextTurn: " + _NextTurn);
        unityObjects["Screens"].SetActive(true);
        unityObjects["Scripts_Menu"].SetActive(false);
        unityObjects["Menu"].SetActive(false);
        /*        SceneManager.LoadScene(2);*/
    }

    #endregion
    #region SingleTon
    private static SC_MenuLogic instance;
    public static SC_MenuLogic Instance
    {
        get 
        { 
            if(instance == null)
            {
                instance = GameObject.Find("SC_MenuLogic").GetComponent<SC_MenuLogic>();
            }
            return instance;
        }
    }
    #endregion
    #region MonoBehavior
    private void OnEnable()
    {
        Listener.OnConnect += OnConnect;
        Listener.OnRoomsInRange += OnRoomsInRange;
        Listener.OnCreateRoom += OnCreateRoom;
        Listener.OnJoinRoom += OnJoinRoom;
        Listener.OnUserJoinRoom += OnUserJoinRoom;
        Listener.OnGetLiveRoomInfo += OnGetLiveRoomInfo;
        Listener.OnGameStarted += OnGameStarted;
    }
    private void OnDisable()
    {
        Listener.OnConnect -= OnConnect;
        Listener.OnRoomsInRange -= OnRoomsInRange;
        Listener.OnCreateRoom -= OnCreateRoom;
        Listener.OnJoinRoom -= OnJoinRoom;
        Listener.OnUserJoinRoom -= OnUserJoinRoom;
        Listener.OnGetLiveRoomInfo -= OnGetLiveRoomInfo;
        Listener.OnGameStarted -= OnGameStarted;
    }

    private void Awake()
    {
        Init();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion
    #region Controller
    #region Buttons
    //TODO
    public void Btn_Menu_SinglePlayer()
    {
        ChangeToScreen(SC_Enums.Screens.SinglePlayer, true);
    }
    public void Btn_Menu_MultiPlayer()
    {
        ChangeToScreen(SC_Enums.Screens.MultiPlayer, true);
    }
    public void Btn_Menu_StudentInfo()
    {
        ChangeToScreen(SC_Enums.Screens.StudentInfo, true);
    }
    public void Btn_Menu_Options()
    {
        ChangeToScreen(SC_Enums.Screens.Options, true);
    }
    public void Btn_StudentInfo_Back()
    {
        ChangeToScreen(back.Pop(), false);
    }
    public void Btn_MultiPlayer_Play()
    {
        ChangeToScreen(SC_Enums.Screens.Loading, true);
    }
    public void Btn_Loading_Back()
    {
        ChangeToScreen(back.Pop(), false);
    }
    public void Btn_StudentInfo_Options()
    {
        ChangeToScreen(SC_Enums.Screens.Options, true);
    }
    public void Btn_Options_Back()
    {
        ChangeToScreen(back.Pop(), false);
    }
    public void Btn_MultiPlayer_Back()
    {
        ChangeToScreen(back.Pop(), false);
    }
    public void Btn_MultiPlayer_EnterRoom()
    {
        Debug.Log("Btn_MultiPlayer_EnterRoom");
/*        unityObjects["Btn_Play"].GetComponent<Button>().interactable = false;*/
        WarpClient.GetInstance().GetRoomsInRange(1, 2);
        UpdateStatus("Searching for an available room...");
    }
    public void Btn_StudentInfo_CV()
    {
        Application.OpenURL("https://www.linkedin.com/in/yuval-berghaus-19b18a1a2/");
    }
    #endregion
    #region Sliders
    public void Slider_OnValueChanged()
    {
        unityObjects["Txt_Amount"].GetComponent<Text>().text = unityObjects["Slider_amount"].GetComponent<Slider>().value.ToString();
    }
    public void Slider_MusicVolChange()
    {
        unityObjects["Music_LikaSomBoDee"].GetComponent<AudioSource>().volume = unityObjects["Slider_Music"].GetComponent<Slider>().value;
    }
    #endregion
    #endregion
    #region Logic
    private void UpdateStatus(string message)
    {
        unityObjects["Txt_Status"].GetComponent<Text>().text = message;
    }
    private void DoRoomsSearchLogic()
    {
        //Check if there are rooms to lookup
        if (roomIndex < roomIds.Count)
        {
            UpdateStatus("Bring room Info (" + roomIds[roomIndex] + ")");
            WarpClient.GetInstance().GetLiveRoomInfo(roomIds[roomIndex]);
        }
        else //No rooms create a new room
        {
            UpdateStatus("Creating Room...");
            WarpClient.GetInstance().CreateTurnRoom("Test", userId, 2, passedParams, 60);
        }
    }
    private void Init()
    {
        currentScreen = SC_Enums.Screens.MainMenu;
        SC_GlobalVars.userId = userId;
        unityObjects = new Dictionary<string, GameObject>();
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("UnityObject");
        foreach(GameObject gameObject in gameObjects)
        {
            unityObjects.Add(gameObject.name, gameObject);
        }
        unityObjects["Screen_Loading"].SetActive(false);
        unityObjects["Screens"].SetActive(false);
        unityObjects["Screen_MultiPlayer"].SetActive(false);
        unityObjects["Screen_Options"].SetActive(false);
        unityObjects["Screen_StudentInfo"].SetActive(false);
    }
    private void ChangeToScreen(SC_Enums.Screens _ToScreen, bool recordBack)
    {
        if(SC_Enums.Screens.SinglePlayer == _ToScreen)
        {
            /*            unityObjects["SinglePlayer"].SetActive(true);
                        unityObjects["Menu"].SetActive(false);*/
            
            SceneManager.LoadScene(1);
        }
/*        else if (SC_Enums.Screens.MultiPlayer == _ToScreen)
        {
            *//*            unityObjects["SinglePlayer"].SetActive(true);
                        unityObjects["Menu"].SetActive(false);*//*
            SceneManager.LoadScene(2);
        }*/
        else
        {
            if (recordBack == true)
                back.Push(currentScreen);
            if(_ToScreen == SC_Enums.Screens.Loading)
            {
                connect_To_Server();
            }
            unityObjects["Screen_" + _ToScreen].SetActive(true);
            unityObjects["Screen_" + currentScreen].SetActive(false);
            currentScreen = _ToScreen;
        }
    }
    private void turnOffCanvas()
    {
        unityObjects["Canvas"].SetActive(false);
    }
    #endregion
    #region server
    private void connect_To_Server()
    {
        passedParams = new Dictionary<string, object>();
/*        passedParams.Add("Password", "Shenkar");*/
        passedParams.Add("Password", unityObjects["Slider_amount"].GetComponent<Slider>().value.ToString());

        if (listener == null)
            listener = new Listener();

        if (listener == null)
        {
            listener = new Listener();
        }
        WarpClient.initialize(apiKey, apiSecret);
        WarpClient.GetInstance().AddConnectionRequestListener(listener);
        WarpClient.GetInstance().AddChatRequestListener(listener);
        WarpClient.GetInstance().AddUpdateRequestListener(listener);
        WarpClient.GetInstance().AddLobbyRequestListener(listener);
        WarpClient.GetInstance().AddNotificationListener(listener);
        WarpClient.GetInstance().AddRoomRequestListener(listener);
        WarpClient.GetInstance().AddTurnBasedRoomRequestListener(listener);
        WarpClient.GetInstance().AddZoneRequestListener(listener);
        WarpClient.GetInstance().Connect(userId);
        unityObjects["Txt_UserId"].GetComponent<Text>().text = "UserID: " + userId;
        unityObjects["Txt_Status"].GetComponent<Text>().text = "Open Connection...";
    }
    #endregion
}
