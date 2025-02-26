using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogInteractable : Interactable
{
    [SerializeField, Tooltip("The log data associated with this pickup")] private Log data;
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        
    }

    public override void InteractEffects()
    {
        GameManager.Instance.AddLogData(data, data.name);
        Destroy(gameObject);
    }
}
