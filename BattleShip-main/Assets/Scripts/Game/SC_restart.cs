using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_restart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseUp()
    {
        SC_GameLogic.Instance.Reset();
    }
}
