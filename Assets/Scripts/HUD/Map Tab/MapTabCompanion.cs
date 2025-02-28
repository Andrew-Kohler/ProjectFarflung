using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTabCompanion : MonoBehaviour
{
    // Exists to be able to track the active status of the map tab from the player's perspective

    [SerializeField] MapTabController _control;
    [SerializeField] private KeycardDisplayController _display;

    private void OnEnable()
    {
        _control.RevealHUDMapStart();
        _display.TabOpen();
    }

    private void OnDisable()
    {
        _control.HideHUDMapEnd();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
