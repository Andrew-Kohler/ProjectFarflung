using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Configures death realm either for 
/// (1) 'respawn' version - with respawn player terminal.
/// (2) 'death' version - with a creture pursuing the player.
/// 
/// Also functionally decrements remaining lives
/// </summary>
public class DeathRealmInitializer : MonoBehaviour
{
    [SerializeField, Tooltip("Objects to be enabled only for the 'respawn' version of the death realm.")]
    private GameObject[] _respawnObjs;
    [SerializeField, Tooltip("Objects to be enabled only for the 'death' version of the death realm.")]
    private GameObject[] _deathObjs;

    void Awake()
    {
        // functionally decrement health
        GameManager.Instance.SceneData.RemainingLives--;

        // by default, respawnObjs are ENABLED, and deathObjs are DISABLED
        // flip objects so respawnObjs are DISABLED and deathObjs are ENABLED
        if (GameManager.Instance.SceneData.RemainingLives <= 0)
        {
            foreach (GameObject obj in _respawnObjs)
                obj.SetActive(false);
            foreach (GameObject obj in _deathObjs)
                obj.SetActive(true);
        }
    }
}
