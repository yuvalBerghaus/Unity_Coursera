using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SC_Enums;

public class SC_Board : MonoBehaviour
{
    List<GameObject> player1Slots;
    int i = 0;
    int board_size;
    
    // Start is called before the first frame update
    #region SingleTon
    private static SC_Board instance;
    public static SC_Board Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.Find("SC_Board").GetComponent<SC_Board>();
            }
            return instance;
        }
    }
    #endregion
    void Start()
    {
        board_size = 100;
    }
    public int getBoardSize()
    {
        return board_size;
    }
    public void setPlayer1Slots(List<GameObject> _player1Slots)
    {
        player1Slots = _player1Slots;
        foreach (GameObject slot in player1Slots)
        {
            Debug.Log(slot.name);
        }


    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
