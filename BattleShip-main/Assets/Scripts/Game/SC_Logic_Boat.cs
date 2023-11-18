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
    public float scaleFactor = 2.0f;

    private SpriteRenderer slotRenderer;
    private Color originalColor;
    public Color highlightColor = Color.green; // Change this to the color you want for highlighting

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

        if (col.CompareTag("UnitySlotObject"))
        {
            Debug.Log("the name is " + col.gameObject.name);
            // Highlight the slot by changing its material
            HighlightSlot(col.gameObject);
        }


    }
    private void HighlightSlot(GameObject slotObject)
    {
        // Create a new GameObject for highlighting
        GameObject highlightObject = new GameObject("Highlight");

        // Set the position and scale of the highlightObject to match the slot
        highlightObject.transform.position = slotObject.transform.position;
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
        highlightRenderer.sortingOrder = 3;

        // Set the color with transparency
        highlightRenderer.color = new Color(1f, 1f, 1f, 0.5f); // Adjust the alpha channel for transparency (0.0f to 1.0f)
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
