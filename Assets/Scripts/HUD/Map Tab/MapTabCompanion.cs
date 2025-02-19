using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTabCompanion : MonoBehaviour
{
    // Exists to be able to track the active status of the map tab from the player's perspective

    [SerializeField] MapTabController _control;

    private void OnEnable()
    {
        _control.RevealHUDMapStart();
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
