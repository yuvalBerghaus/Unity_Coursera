using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static SC_Enums;
using AssemblyCSharp;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using System.Linq;

public class SC_GameLogic : MonoBehaviour
{
    #region globalVars
    private Dictionary<string, GameObject> unityShipObjects;
    private Dictionary<string, GameObject> unityGameObjects;
    // 
    private Dictionary<int, GameObject> player1SlotsDict; // the chosen slots of the player (the objects of the chosen indexes)
    private Dictionary<int, GameObject> unitySlotObjects; // the slots of all the player's board
    private Dictionary<int, GameObject> unityEnemySlotObjects;
    List<GameObject> player1Slots;
    private string currentKeyObject; // the name of the chosen ship that is being dragged
    private bool isMultiplayer = false;
    public bool isPlayerTurn = false;
    private int allshipsize = 14;
    public bool isReady = false;
    public bool isEnemyReady = false;
    private bool gameState = false;
    GameObject[] _objs_Ships;
    GameObject[] _objs_Game;
    GameObject[] _objs_Slots;
    GameObject[] _objs_Enemy_Slots;
    private int counter_hit_enemy = 0;
    private int counter_hit_player = 0;
    public string nextTurn;
    private float startTime;
    #endregion
    #region SingleTon
    private static SC_GameLogic instance;
    public bool isGameReady = false;

    public static SC_GameLogic Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.Find("SC_GameLogic").GetComponent<SC_GameLogic>();

            return instance;
        }
    }
    #endregion
    // First functionality once the game starts
    private void Init()
    {
        if (!isMultiplayer)
        {
            isPlayerTurn = true;
        }
        Debug.Log("Call init");
        unityShipObjects = new Dictionary<string, GameObject>();
        unityEnemySlotObjects = new Dictionary<int, GameObject>();
        unitySlotObjects = new Dictionary<int, GameObject>();
        unityGameObjects = new Dictionary<string, GameObject>();
        player1SlotsDict = new Dictionary<int, GameObject>();
        _objs_Enemy_Slots = GameObject.FindGameObjectsWithTag("UnitySlotComputerObject");
        _objs_Ships = GameObject.FindGameObjectsWithTag("UnityShipObject");
        _objs_Game = GameObject.FindGameObjectsWithTag("UnityGameObject");
        _objs_Slots = GameObject.FindGameObjectsWithTag("UnitySlotObject");
        Debug.Log("_objs_Game count = " + _objs_Game.Length);
        foreach (GameObject g in _objs_Ships)
        {
            unityShipObjects.Add(g.name, g);
        }
        // TODO - add Btn_restart
        foreach (GameObject g in _objs_Game)
        {
            unityGameObjects.Add(g.name, g);
        }
        foreach (GameObject g in _objs_Slots)
        {
            unitySlotObjects.Add(g.GetComponent<SC_Slot>().slotIdx, g);
        }
        foreach (GameObject g in _objs_Enemy_Slots)
        {
            unityEnemySlotObjects.Add(g.GetComponent<SC_Slot>().slotIdx, g);
        }

    }

    #region Events
    private void OnEnable()
    {
        SC_Slot.OnClick += OnClick;
        Listener.OnGameStarted += OnGameStarted;
        Listener.OnMoveCompleted += OnMoveCompleted;
    }

    private void OnDisable()
    {
        SC_Slot.OnClick -= OnClick;
        Listener.OnGameStarted -= OnGameStarted;
        Listener.OnMoveCompleted -= OnMoveCompleted;
    }

    // OnClick function is whenever the player presses on the boxes of the enemy's board
    private void OnClick(int _slotIndex)
    {
        // single player procedure for each attack move
        if (isReady && !isMultiplayer)
        {
            playerMove(_slotIndex);
            checkState();
            changeTurn();
            StartCoroutine(WaitingTime());
        }
        // multiplayer game
        else if (isMultiplayer && isGameReady && nextTurn == SC_GlobalVars.userId)
        {
            nextTurn = null;
            changeTurn();
            playerMove(_slotIndex);
            Dictionary<string, object> _toSend = new Dictionary<string, object>();
            _toSend.Add("indexHit", _slotIndex);
            string _toJson = MiniJSON.Json.Serialize(_toSend);
            WarpClient.GetInstance().sendMove(_toJson);
        }
    }
    #endregion
    #region Callbacks





    // OnGameStarted function is when multiplayer game starts!
    private void OnGameStarted(string _Sender, string _RoomId, string _NextTurn)
    {
        isMultiplayer = true;
        Init();
        Debug.Log("OnGameStarted " + _Sender + " " + _RoomId + " " + _NextTurn);
        nextTurn = _NextTurn;
        startTime = Time.time;
        if(_NextTurn != SC_GlobalVars.userId)
        {
            changeTurn();
        }
    }
    private void OnMoveCompleted(MoveEvent _Move)
    {
        nextTurn = _Move.getNextTurn();
        if (isReady == false || isGameReady == false) // game status : organizing ships
        {
            if (nextTurn != null && nextTurn == SC_GlobalVars.userId)
            {
                Debug.Log(_Move.getMoveData());
                unityGameObjects["Txt_enemyReady"].GetComponent<TextMesh>().text = "Enemy is Ready!";
                isGameReady = true;
                setEnemySlots(_Move.getMoveData());
                changeTurn();
            }

        }
        else // game status : playing
        {
            if(nextTurn == SC_GlobalVars.userId) // then its my turn
            {
                changeTurn();
                Dictionary<string, object> _data = (Dictionary<string, object>)MiniJSON.Json.Deserialize(_Move.getMoveData());
                if(_data.ContainsKey("indexHit"))
                {
                    enemyMove(int.Parse(_data["indexHit"].ToString()));
                }
            }
        }
        unityGameObjects["Txt_Timer"].GetComponent<SC_Timer>().resetTimer();
        checkState();
    }

    // multiplayer
    private void setEnemySlots(string _Move)
    {
        string[] tokens = _Move.Split(',');
        for (int i = 0; i < tokens.Length;i++)
        {
            int numVal = int.Parse(tokens[i]);
            unityEnemySlotObjects[numVal].GetComponent<SC_Slot>().slotIdx = numVal;
            unityEnemySlotObjects[numVal].GetComponent<SC_Slot>().isEmpty = false;
        }
    }
    #endregion
    #region Logic
    private IEnumerator WaitingTime()
    {
        SC_Slot.clickable = false;
        yield return new WaitForSeconds(1f);
        SC_Slot.clickable = true;
        enemyMove();
        checkState();
        changeTurn();
    }
    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void LoadMenu()
    {
        SceneManager.LoadScene("Scene_Menu");
    }

    // after every move , we check the state of the board - who won and who lost?
    private void checkState()
    {
        Debug.Log("counter_hit_enemy = " + counter_hit_enemy + " and slots of enemy is " + allshipsize);
        Debug.Log("counter_hit_player = " + counter_hit_player + " and slots of enemy is " + allshipsize);
        // checking if all ships of the enemies were destroyed
        if (counter_hit_enemy == allshipsize)
        {
            Sprite winner = SC_GameModel.Instance.GetSprite("Sprite_winner");
            unityGameObjects["Sprite_state"].GetComponent<SpriteRenderer>().sprite = winner;
            unityGameObjects["music_state"].GetComponent<AudioSource>().clip = GetSFX("WinTheme");
            unityGameObjects["music_state"].GetComponent<AudioSource>().volume = 0.4f;
            unityGameObjects["music_state"].GetComponent<AudioSource>().Play();
            gameState = true;
        }
        if (counter_hit_player == allshipsize)
        {
            Sprite loser = SC_GameModel.Instance.GetSprite("Sprite_loser");
            unityGameObjects["Sprite_state"].GetComponent<SpriteRenderer>().sprite = loser;
            unityGameObjects["music_state"].GetComponent<AudioSource>().clip = GetSFX("LoseTheme");
            unityGameObjects["music_state"].GetComponent<AudioSource>().volume = 0.4f;
            unityGameObjects["music_state"].GetComponent<AudioSource>().Play();
            gameState = true;
        }
        if (gameState)
        {
            unityGameObjects["Sprite_state"].GetComponent<SpriteRenderer>().sortingOrder = 4;
            unityGameObjects["Sprite_state"].GetComponent<Transform>().position = Vector3.zero;
            unityGameObjects["Sprite_state"].GetComponent<Transform>().localScale = new Vector3(2f, 2f, 0f);
            unityGameObjects["Screen_game_object"].SetActive(false);

        }
    }
    // changing the sprite of the turn state to the corresponding current player
    private void changeTurn()
    {
        if(isMultiplayer)
        {
            if (SC_GlobalVars.userId == nextTurn)
            {
                unityGameObjects["Sprite_turn"].GetComponent<SpriteRenderer>().sprite = GetSprite("Sprite_me");
                unityGameObjects["Sprite_turn"].GetComponent<Transform>().transform.localScale= new Vector3(1f, 1f, 1.0f);
                Debug.Log("my x position is " + unityGameObjects["Sprite_turn"].GetComponent<Transform>().transform.position.x);
                unityGameObjects["Sprite_turn"].GetComponent<Transform>().transform.position = new Vector3(-4.17f, 7.77f, 1.0f);
                Debug.Log("my x position is " + unityGameObjects["Sprite_turn"].GetComponent<Transform>().transform.position.x);
            }
            else
            {
                unityGameObjects["Sprite_turn"].GetComponent<SpriteRenderer>().sprite = GetSprite("Sprite_enemy");
                unityGameObjects["Sprite_turn"].GetComponent<Transform>().transform.localScale = new Vector3(0.3f, 0.3f, 1.0f);
                unityGameObjects["Sprite_turn"].GetComponent<Transform>().transform.position = new Vector3(7.23f, 7.62f, 1.0f);
            }
        }
        // if its single player change to player's turn
        else if (isPlayerTurn == false && isMultiplayer == false)
        {
            unityGameObjects["Sprite_turn"].GetComponent<SpriteRenderer>().sprite = GetSprite("Sprite_me");
            unityGameObjects["Sprite_turn"].GetComponent<Transform>().transform.localScale = new Vector3(1f, 1f, 0f);
            unityGameObjects["Sprite_turn"].GetComponent<Transform>().transform.position = new Vector3(-6.5f, 7.5f, 1.0f);
        }
        // else change to computer's turn
        else
        {
            unityGameObjects["Sprite_turn"].GetComponent<SpriteRenderer>().sprite = GetSprite("Sprite_enemy");
            unityGameObjects["Sprite_turn"].GetComponent<Transform>().transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            unityGameObjects["Sprite_turn"].GetComponent<Transform>().transform.position = new Vector3(5f, 7.5f, 1.0f);
        }

        isPlayerTurn = !isPlayerTurn;
    }
    // once the player clicks one of the slots of the enemy it will generate the corresponding action
    private void playerMove(int _slotIdx)
    {
        unityEnemySlotObjects[_slotIdx].AddComponent<SpriteRenderer>();
        unityEnemySlotObjects[_slotIdx].AddComponent<Animator>();
        unityEnemySlotObjects[_slotIdx].GetComponent<SpriteRenderer>().sortingOrder = 3;
            if (!(unityEnemySlotObjects[_slotIdx].GetComponent<SC_Slot>().isEmpty)) // player hit!
            {
                unityEnemySlotObjects[_slotIdx].AddComponent<AudioSource>();
                unityEnemySlotObjects[_slotIdx].GetComponent<AudioSource>().clip = GetSFX("Sound_Explosion");
            unityEnemySlotObjects[_slotIdx].GetComponent<AudioSource>().volume = 0.1f;
                unityEnemySlotObjects[_slotIdx].GetComponent<AudioSource>().Play();
                RuntimeAnimatorController hitAnimator = SC_GameModel.Instance.GetAnimator("Sprite_Explosion/Explosion_29");
                Sprite hitSprite = SC_GameModel.Instance.GetSprite("Sprite_Explosion/Sprite_Explosion");
                unityEnemySlotObjects[_slotIdx].GetComponent<SpriteRenderer>().sprite = hitSprite;
                unityEnemySlotObjects[_slotIdx].GetComponent<Animator>().runtimeAnimatorController = hitAnimator;
                
                counter_hit_enemy++;
            }
        // if player missed
        else if ((unityEnemySlotObjects[_slotIdx].GetComponent<SC_Slot>()).isEmpty)
            {
                RuntimeAnimatorController hitAnimator = SC_GameModel.Instance.GetAnimator("Sprite_Explosion/Sprite_waterSplash_1");
                Sprite hitSprite = SC_GameModel.Instance.GetSprite("Sprite_Explosion/Sprite_waterSplash");
                unityEnemySlotObjects[_slotIdx].GetComponent<SpriteRenderer>().sprite = hitSprite;
                unityEnemySlotObjects[_slotIdx].GetComponent<Animator>().runtimeAnimatorController = hitAnimator;
            }
    }
    //  Btn_Start_Game() starts when multiplayer starts
    public void Btn_Start_Game()
    {
        if ((isMultiplayer && nextTurn == SC_GlobalVars.userId) || !isMultiplayer)
        {
            setPlayer1Slots();
            Debug.Log("amount of player1 slots are " + player1SlotsDict.Count);

            if (player1SlotsDict.Count == allshipsize)
            {
                isReady = true;

                if (!isMultiplayer)
                {
                    generateEnemy();
                    Debug.Log("enemy were generated!");
                }
                else if (SC_GlobalVars.userId == nextTurn)
                {
                    string _Indexes = string.Join(",", player1SlotsDict.Keys);
                    Debug.Log("turn of " + nextTurn);
                    WarpClient.GetInstance().sendMove(_Indexes);
                }
            }
        }

        if (isMultiplayer)
        {
            nextTurn = null;
            changeTurn();
            unityGameObjects["Txt_Timer"].GetComponent<SC_Timer>().resetTimer();
        }

        unityGameObjects["Sprite_BtnReady"].SetActive(false);
    }

    // setPlayer1Slots() function will run once the player has put all of his ships on his board
    // setPlayer1Slots() function locates its slots and assigns it to be not empty and player's slot
    private void setPlayer1Slots()
        {
            player1Slots = new List<GameObject>();
            foreach (GameObject a in _objs_Ships)
            {
                List<GameObject> s = a.GetComponent<SC_Logic_Boat>().getPlayer1Slots(); // Get the current slots which the player's ships are collided with
            // operation to allocate the chosen slots(indexes) where the player has allocated his ships on the board
                foreach (GameObject b in s)
                {
                    player1SlotsDict[b.GetComponent<SC_Slot>().slotIdx] = b;
                    b.GetComponent<SC_Slot>().isPlayerSlot = true;
                    b.GetComponent<SC_Slot>().isEmpty = false;
                    Debug.Log(b.GetComponent<SC_Slot>().slotIdx);
                }
            }
        }
    // generateEnemy() function occurs once "Im ready" button is clicked so it can generate the indexes of the enemy on his board
        private void generateEnemy()
        {
            int board_size = unityGameObjects["board"].GetComponent<SC_Board>().getBoardSize();
            int rand_desination = (int)UnityEngine.Random.Range(15, board_size - 15);
            for (int i = rand_desination - allshipsize; i < rand_desination; i++)
            {
                unityEnemySlotObjects[i].GetComponent<SC_Slot>().isEmpty = false;
                Debug.Log("we put the enemy in " + unityEnemySlotObjects[i].GetComponent<SC_Slot>().slotIdx);
            }

        }
    // enemyMove() function generates randomly a shot to any index in the player's board
    private void enemyMove(int indexHit = -1)
    {
        // Get the size of the player's board
        int size = unityGameObjects["board"].GetComponent<SC_Board>().getBoardSize();

        // If indexHit is not provided, choose a random index
        if (indexHit == -1)
        {
            indexHit = (int)UnityEngine.Random.Range(1, size);
        }

        // If the slot is not clicked (not already hit)
        if (!unitySlotObjects[indexHit].GetComponent<SC_Slot>().clicked)
        {
            // Add necessary components to the slot
            unitySlotObjects[indexHit].AddComponent<SpriteRenderer>();
            unitySlotObjects[indexHit].GetComponent<SpriteRenderer>().sortingOrder = 3;
            unitySlotObjects[indexHit].AddComponent<Animator>();

            // If the player's ship is hit
            if (player1SlotsDict.ContainsKey(indexHit))
            {
                // Add audio component, set audio clip, and play explosion sound
                unitySlotObjects[indexHit].AddComponent<AudioSource>();
                unitySlotObjects[indexHit].GetComponent<AudioSource>().clip = GetSFX("Sound_Explosion");
                unitySlotObjects[indexHit].GetComponent<AudioSource>().volume = 0.1f;
                unitySlotObjects[indexHit].GetComponent<AudioSource>().Play();

                // Set explosion sprite and animation controller
                unitySlotObjects[indexHit].GetComponent<SpriteRenderer>().sprite = GetSprite("Sprite_Explosion/Sprite_Explosion");
                unitySlotObjects[indexHit].GetComponent<Animator>().runtimeAnimatorController = GetAnimator("Sprite_Explosion/Explosion_29");

                // Increment hit counter and log the result
                counter_hit_player++;
                Debug.Log("counter_hit_player = " + counter_hit_player);
            }
            else
            {
                // Log water splash sprite and set water splash sprite and animation
                Debug.Log(GetSprite("Sprite_Explosion/Sprite_waterSplash"));
                unitySlotObjects[indexHit].GetComponent<SpriteRenderer>().sprite = GetSprite("Sprite_Explosion/Sprite_waterSplash");
                unitySlotObjects[indexHit].GetComponent<Animator>().runtimeAnimatorController = GetAnimator("Sprite_Explosion/Sprite_waterSplash_1");
            }

            // Update slot properties
            unitySlotObjects[indexHit].GetComponent<SC_Slot>().isEmpty = false;
            unitySlotObjects[indexHit].GetComponent<SC_Slot>().clicked = true;
        }
        else
        {
            // Handle the case when the same place is hit again (currently empty)
            // You can add specific handling for this scenario if needed
        }
    }

    private Sprite GetSprite(string _SpriteName)
    {
        return SC_GameModel.Instance.GetSprite(_SpriteName);
    }
    private RuntimeAnimatorController GetAnimator(string _AnimatorName)
    {
        return SC_GameModel.Instance.GetAnimator(_AnimatorName);
    }
    private AudioClip GetSFX(string _SFX)
    {
        return SC_GameModel.Instance.GetSFX(_SFX);
    }
    public void setKey(string _currentKeyObject)
    {
        currentKeyObject = _currentKeyObject;
    }
    #endregion
    //Called via animation event
    // Start is called before the first frame update
    void Start()
    {
        /*unityGameObjects["Txt_Timer"].SetActive(true);*/
        Sprite Btn_restart = SC_GameModel.Instance.GetSprite("Sprite_reset");
        if (Btn_restart.Equals(null))
        {
            Debug.Log("check");
        }
        if(unityGameObjects.ContainsKey("SinglePlayer"))
        {
            unityGameObjects["music_state"].GetComponent<AudioSource>().clip = GetSFX("GameTheme");
            unityGameObjects["music_state"].GetComponent<AudioSource>().volume = 0.1f;
            unityGameObjects["music_state"].GetComponent<AudioSource>().Play();


            unityGameObjects["Btn_restart"].AddComponent<SpriteRenderer>();
            unityGameObjects["Btn_restart"].GetComponent<SpriteRenderer>().sprite = Btn_restart;
            unityGameObjects["Btn_restart"].GetComponent<SpriteRenderer>().sortingOrder = 4;
            unityGameObjects["Btn_restart"].GetComponent<Transform>().position = Vector3.zero + new Vector3(4f, -7f, 0f);
            unityGameObjects["Btn_restart"].GetComponent<Transform>().localScale = new Vector3(0.5f, 0.5f, 0f);
            unityGameObjects["Btn_restart"].AddComponent<BoxCollider2D>().size = new Vector2(1f, 1f);


            Sprite Btn_menu = SC_GameModel.Instance.GetSprite("Sprite_btn_menu");
            unityGameObjects["Btn_menu"].AddComponent<SpriteRenderer>();
            unityGameObjects["Btn_menu"].GetComponent<SpriteRenderer>().sprite = Btn_menu;
            unityGameObjects["Btn_menu"].GetComponent<SpriteRenderer>().sortingOrder = 4;
            unityGameObjects["Btn_menu"].GetComponent<Transform>().position = Vector3.zero + new Vector3(-4f, -7f, 0f);
            unityGameObjects["Btn_menu"].GetComponent<Transform>().localScale = new Vector3(1.5f, 1.5f, 1.5f);
            unityGameObjects["Btn_menu"].AddComponent<BoxCollider2D>().size = new Vector2(1f, 1f);
        }
    }
    void Update()
        {
        /*
         checking if arrows right or left clicked in order to rotate the ships 90 degrees
         */
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                Debug.Log("right");
                unityShipObjects[currentKeyObject].GetComponent<SpriteRenderer>().transform.Rotate(new Vector3(0f, 0f, 90f), -90f);

            }
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                Debug.Log("left");
                unityShipObjects[currentKeyObject].GetComponent<SpriteRenderer>().transform.Rotate(new Vector3(0f, 0f, 90f), 90f);
            }
        }
    private void FixedUpdate()
    {
       
    }
    private void Awake()
    {
        Init();
    }




}
