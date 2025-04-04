using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInteractable : Interactable
{
    [SerializeField, Tooltip("Key name / classification")] private PoweredDoor.KeyType keyName;
    new void Start()
    {
        if (GameManager.Instance.SceneData.Keys.Contains(keyName.ToString()))
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
        GameManager.Instance.SceneData.Keys.Add(keyName.ToString());
        Destroy(gameObject);
    }
}
