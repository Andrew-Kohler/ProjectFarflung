using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.Mathematics;

public class MapRaycaster : MonoBehaviour
{
    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _inactiveColor;
    [SerializeField] private MapTabController _tabController;

    [Header("Dot Images")]
    [SerializeField] private Image _dot;
    [SerializeField] private Image _dotTri;
    [SerializeField] private Color _currentFloorColor;
    [SerializeField] private Color _otherFloorColor;

    private BoxCollider2D _col;

    public delegate void OnPassthrough(int currRoom);
    public static event OnPassthrough onPassthrough;

    void Start()
    {
        _col = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // This is left here as a reminder of pretty much everything that didn't work in
        // case anyone (me included) tries to think of a better way to do this
        //ray = cam.ScreenPointToRay(obj.position);


        //Set up the new Pointer Event
        /*_pointerEventData = new PointerEventData(m_EventSystem);
        //Set the Pointer Event Position to that of the position of the little dot
        float xCoord = math.remap(-960, 960, 0, 1920, obj.position.x);
        float yCoord = math.remap(-540, 540, 0, 1080, obj.position.y);
        _pointerEventData.position = new Vector2(xCoord, yCoord);
        //_pointerEventData.dragging = true;

        //Create a list of Raycast Results
        List<RaycastResult> results = new List<RaycastResult>();

        //Raycast using the Graphics Raycaster and the dot position
        _raycaster.Raycast(_pointerEventData, results);

        //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
        Debug.Log(results.Count);
        foreach (RaycastResult result in results)
        {
            Debug.Log("Hit " + result.gameObject.name);

        }*/

        //float xCoord = math.remap(-960, 960, 0, 1920, obj.position.x);
        //float yCoord = math.remap(-540, 540, 0, 1080, obj.position.y);

        /*Ray ray = cam.ScreenPointToRay(new Vector2(xCoord, yCoord));

        *//*RaycastHit hit;
        Physics.Raycast(cam.transform.position, ray.direction, out hit, Mathf.Infinity);
        Debug.DrawRay(cam.transform.position, ray.direction, Color.red);
        if(hit.collider != null)
          Debug.Log(hit.collider);*/
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.Instance.PlayerEnabled)
        {
            collision.GetComponent<Image>().color = _activeColor;

            // The index of a room is stored as the first character of its name
            onPassthrough?.Invoke(int.Parse(collision.gameObject.name.Substring(0, 1)));
            // The canon name of the room is stored in the rest of the string
            _tabController.SetLocationText(collision.gameObject.name.Substring(1, collision.gameObject.name.Length - 1));
        }
       
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(GameManager.Instance.PlayerEnabled)
            collision.GetComponent<Image>().color = _inactiveColor;
    }

    public void ToggleState(bool currentFloor)
    {
        _col.enabled = currentFloor;
        if (currentFloor)
        {
            _dot.color = _currentFloorColor;
            _dotTri.color = _currentFloorColor;
        }
        else
        {
            _dot.color = _otherFloorColor;
            _dotTri.color = _otherFloorColor;
        }
    }

    public void ToggleEnabled(bool enabled)
    {
        _dot.enabled = enabled;
        _dotTri.enabled = enabled;
    }
}
