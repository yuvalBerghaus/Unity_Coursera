using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SC_Enums;

public class SC_Slot : MonoBehaviour
{
    #region globalVars
    public delegate void ClickHandler(int _slotIndex);
    public static event ClickHandler OnClick;
    MeshRenderer meshRenderer;
    public static bool clickable;
    public bool isPlayerSlot;
    public int slotIdx = 0;
    public bool clicked;
    public bool isEmpty;
    Material SphereMaterial;
    #endregion
    #region monoBehavior



    void Start()
    {
        clickable = true;
        isEmpty = true;
        clicked = false;

    }
    void init()
    {
        SphereMaterial = Resources.Load<Material>("Material_slot");
        meshRenderer = GetComponent<MeshRenderer>();
    }
    #endregion
    #region Events
    void OnMouseUp()
    {
        if(OnClick != null && isPlayerSlot == false && clicked == false && clickable == true && SC_GameLogic.Instance.isGameReady && SC_GameLogic.Instance.nextTurn == SC_GlobalVars.userId)
        {
            clicked = true;
            OnClick(slotIdx);
        }
        else if (OnClick != null && isPlayerSlot == false && clicked == false && clickable == true && SC_GameLogic.Instance.isReady)
        {
            clicked = true;
            OnClick(slotIdx);
        }
    }

    private void OnMouseOver()
    {
        /*        if (isPlayerSlot == false && clicked == false)
                {
                    // Set the new material on the GameObject
                    meshRenderer.material = SphereMaterial;
                }*/
        Debug.Log(slotIdx);
    }
    #endregion
    #region Logic
    public bool checkEmpty()
    {
        return isEmpty;
    }
    #endregion
}