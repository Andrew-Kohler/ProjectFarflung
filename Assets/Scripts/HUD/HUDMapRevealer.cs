using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDMapRevealer : MonoBehaviour
{
    public Vector2 AdjoiningRoomIndicies;
    // Whenever this trigger is passed through, it tells the HUD and GM that the player has visited both sides of that door
    public delegate void OnPassthrough(int side1, int side2);
    public static event OnPassthrough onPassthrough;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player"))
            onPassthrough?.Invoke((int)AdjoiningRoomIndicies.x, (int)AdjoiningRoomIndicies.y);
    }
}
