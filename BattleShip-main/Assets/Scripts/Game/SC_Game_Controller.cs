using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Game_Controller : MonoBehaviour
{
    public void Btn_Start_Game()
    {
        SC_GameLogic.Instance.Btn_Start_Game();
    }
    #region MonoBehaviour
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion
}
