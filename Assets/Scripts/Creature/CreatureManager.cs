using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles tracking global creature data, including speed data and player reference
/// </summary>
public class CreatureManager : MonoBehaviour
{
    #region Singleton
    // private singleton instance
    private static CreatureManager _instance;

    // public accessor of instance
    public static CreatureManager Instance
    {
        get
        {
            // setup GameManager as a singleton class
            if (_instance == null)
            {
                // create new game manager object
                GameObject newManager = new();
                newManager.name = "[Creature Manager]";
                newManager.AddComponent<CreatureManager>();
                _instance = newManager.GetComponent<CreatureManager>();

                // not a perfect solution, but you can't give a reference to a singleton.
                // This still prevents us needing to drag a reference to the player on every creature zone that we configure.
                // TODO: replace with ACTUAL player tracking object name (probably whatever our player prefab name will become)
                GameObject player = GameObject.Find("PlayerCapsule");
                if (player is null)
                    throw new System.Exception("There should not be a creature in a scene without a player. Is the player named correctly?");
                _instance.PlayerTransform = player.transform;

                _instance.CurrentSpeed = 0; // start at 0 speed
            }
            // return new/existing instance
            return _instance;
        }
    }
    #endregion

    #region Behavior
    // unfortunately these cannot be configured from inspector because I chose to make this a singleton manager
    private const float SPEED_CHANGE_FACTOR = 1.0f;
    private const float STUN_DURATION = 1.5f;

    public Transform PlayerTransform { get; private set; }

    public CreatureZone ActiveZone { get; private set; } = null;

    private float _currentSpeed;
    public float CurrentSpeed {
        get
        {
            // return 0 speed if stunned so it stops moving
            if (_isStunned)
                return 0f;
            return _currentSpeed;
        }
        private set
        {
            _currentSpeed = value;
        }
    }

    private bool _isAggro = false;
    private bool _isStunned = false;

    // Update is called once per frame
    void Update()
    {
        // prevent building speed while stunned
        if (_isStunned)
            return;

        // increase / decrease speed
        if (_isAggro)
        {
            // increase speed without cap
            CurrentSpeed += SPEED_CHANGE_FACTOR * Time.deltaTime;
        }
        else
        {
            // decrease speed without cap
            CurrentSpeed -= SPEED_CHANGE_FACTOR * Time.deltaTime;
            if (CurrentSpeed < 0)
                CurrentSpeed = 0;
        }
    }
    #endregion

    #region Public Interfacing
    /// <summary>
    /// Starts ramping velocity that creature uses to track player.
    /// </summary>
    public void ActivateCreatureAggro(CreatureZone zone)
    {
        _isAggro = true;

        ActiveZone = zone;
    }

    /// <summary>
    /// Sets creature speed to start decreasing over time.
    /// </summary>
    public void DeactivateCreatureAggro()
    {
        _isAggro = false;

        ActiveZone = null;
    }

    public void StunCreature()
    {
        _isStunned = true;

        StartCoroutine(DoStunCreature());

        IEnumerator DoStunCreature()
        {
            yield return new WaitForSeconds(STUN_DURATION);

            _isStunned = false;
        }
    }
    #endregion
}
