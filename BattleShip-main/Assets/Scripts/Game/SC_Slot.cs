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
    private bool isMouseOver = false;
    Material SphereMaterial;
    GameObject highlightObject;
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
        if(!isMouseOver)
        {
            // Create a new GameObject for highlighting
            highlightObject = new GameObject("Highlight");

            // Set the position and scale of the highlightObject to match the slot
            highlightObject.transform.position = transform.position;
            highlightObject.transform.localScale = new Vector3(80f, 80f, 0);

            // Create a SpriteRenderer component for the highlightObject
            SpriteRenderer highlightRenderer = highlightObject.AddComponent<SpriteRenderer>();

            // Set the sprite to a white square
            highlightRenderer.sprite = Sprite.Create(
                Texture2D.whiteTexture,
                new Rect(0, 0, 1, 1),
                new Vector2(0.5f, 0.5f)
            );

            // Optionally, adjust the sorting order if needed
            highlightRenderer.sortingOrder = 1;

            // Set the color with transparency
            highlightRenderer.color = new Color(1f, 1f, 1f, 0.5f); // Adjust the alpha channel for transparency (0.0f to 1.0f)
            isMouseOver = true;
        }

    }
    // Called when the mouse pointer exits the collider
    private void OnMouseExit()
    {
        Debug.Log("Mouse has exited the object!");
        Destroy(highlightObject);
        isMouseOver=false;
        // Add your custom logic here
    }
    #endregion
    #region Logic
    public bool checkEmpty()
    {
        return isEmpty;
    }
    #endregion
}