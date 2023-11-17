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
    private void OnMouseDown()
    {
        SC_GameLogic.Instance.setKey(gameObject.name);
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
    private void OnTriggerEnter2D(Collider2D col)
    {
        // Add the GameObject collided with to the list.
        currentCollisions.Add(col.gameObject);

    }

    private void OnTriggerExit2D(Collider2D col)
    {

        // Remove the GameObject collided with from the list.
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
