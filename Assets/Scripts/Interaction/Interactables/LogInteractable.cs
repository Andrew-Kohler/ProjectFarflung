using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogInteractable : Interactable
{
    // Start is called before the first frame update
    new void Start()
    {

    }

    // Update is called once per frame
    new void Update()
    {
        
    }

    public override void InteractEffects()
    {
        Destroy(gameObject);
    }
}
