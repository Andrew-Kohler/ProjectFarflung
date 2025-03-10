using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInteractable : Interactable
{
    [SerializeField, Tooltip("Key name / classification")] private PoweredDoor.KeyType keyName;
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
        GameManager.Instance.SceneData.Keys.Add(keyName.ToString());
        Destroy(gameObject);
    }
}
