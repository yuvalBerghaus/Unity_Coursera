using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Logic_Boat : MonoBehaviour
{
    #region globalVars
    /*    public delegate void ClickHandler(string _Index);
        public static event ClickHandler OnClick;*/
    public int length;
    private float startPosX;
    private float startPosY;
    private Vector2 mousePosition;
    private List<GameObject> currentCollisions = new List<GameObject>();
    private Collider2D[] results;
    #endregion
    // Start is called before the first frame update
    #region monoBehavior
    void Start()
    {
    }
    void Init()
    {

    }

    void Update()
    {

    }
    #endregion
    #region Events

    // once the chosen boat is decided for placement and press the mouse
    private void OnMouseDown()
    {
        SC_GameLogic.Instance.setKey(gameObject.name); // the chosen player's boat name will be the key
        startPosX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x;
        startPosY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y;
    }
    private void OnMouseDrag()
    {
        if(SC_GameLogic.Instance.isReady == false)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector2(mousePosition.x - startPosX, mousePosition.y - startPosY);
        }
    }
    // this function triggers whenever we drag the ships over any slots it will input its index to currentCollisions array
    private void OnTriggerEnter2D(Collider2D col)
    {
        // Add the GameObject collided with to the list.
        currentCollisions.Add(col.gameObject);

    }

    private void OnTriggerExit2D(Collider2D col)
    {

        // remove the slot that was collided with our ship so we can keep that current state of currentCollisions
        currentCollisions.Remove(col.gameObject);
    }
    #endregion
    #region Logic
    public List<GameObject> getPlayer1Slots()
    {
        return currentCollisions;
    }
    #endregion
    // Update is called once per fram
}
