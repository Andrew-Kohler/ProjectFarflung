using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles functionality of clicking a wire to select it as the current wire to be used
/// </summary>
public class WireSelector : ClickableObject
{
    public override void OnObjectClick()
    {
        Debug.Log("TEST");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    override protected void Update()
    {
        // ensure click check occurs
        base.Update();
    }
}
