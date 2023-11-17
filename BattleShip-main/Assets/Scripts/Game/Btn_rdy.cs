using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Btn_rdy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnMouseUp()
    {
           SC_GameLogic.Instance.Btn_Start_Game();
    }
}
