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
    void Start()
    {

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
        collision.GetComponent<Image>().color = _activeColor;
        _tabController.SetLocationText(collision.gameObject.name);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.GetComponent<Image>().color = _inactiveColor;
    }
}
