using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogInteractable : Interactable
{
    [SerializeField, Tooltip("The log data associated with this pickup")] private Log data;
    new void Start()
    {
        // destroy log if already picked up
        if (GameManager.Instance.SceneData.FoundLogNames.Contains(data.name))
        {
            Destroy(this.gameObject);
            // return to prevent rest of start logic from ever computing
            return;
        }

        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        
    }

    public override void InteractEffects()
    {
        // pickup SFX
        AudioManager.Instance.PlayPickup();

        GameManager.Instance.AddLogData(data, data.name);
        Destroy(gameObject);
    }
}
