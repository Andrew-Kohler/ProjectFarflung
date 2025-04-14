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
                _instance.ActiveZone = null; // no starting active zone
                _instance.IsAggro = false; // no aggro instantly
            }
            // return new/existing instance
            return _instance;
        }
    }
    #endregion

    #region Behavior
    // unfortunately these cannot be configured from inspector because I chose to make this a singleton manager
    private const float SPEED_INCREASE_FACTOR = 0.2f;
    private const float SPEED_DECREASE_FACTOR = 0.4f;
    private const float STUN_DURATION = 2.0f;

    public Transform PlayerTransform { get; private set; }

    public CreatureZone ActiveZone { get; private set; }

    /// <summary>
    /// Controls whether the creature is currently increasing or decreasing in speed.
    /// </summary>
    public bool IsAggro;

    private float _currentSpeed;
    public float CurrentSpeed {
        get
        {
            // return 0 speed if stunned so it stops moving
            if (IsStunned)
                return 0f;
            return _currentSpeed;
        }
        set
        {
            _currentSpeed = value;
        }
    }

    public bool IsStunned = false;

    // Update is called once per frame
    void Update()
    {
        // prevent building speed while stunned
        if (IsStunned)
            return;

        // increase / decrease speed
        if (IsAggro)
        {
            // increase speed without cap
            CurrentSpeed += SPEED_INCREASE_FACTOR * Time.deltaTime;
        }
        else
        {
            // new behavior: instantly reset aggro upon leaving a creature zone
            CurrentSpeed = 0;

            // OLD BEHAVIOR - keeping this here in case we want to revert
            // decrease speed without cap
            /*CurrentSpeed -= SPEED_DECREASE_FACTOR * Time.deltaTime;
            if (CurrentSpeed < 0)
                CurrentSpeed = 0;*/
        }
    }
    #endregion

    #region Public Interfacing
    /// <summary>
    /// Attaches provided zone as new active zone
    /// </summary>
    public void AttachCurrentZone(CreatureZone zone)
    {
        if (ActiveZone is not null)
            throw new System.Exception("Improper usage of AttachCurrentZone. existing zone MUST be detached first.");

        ActiveZone = zone;
    }

    /// <summary>
    /// Detaches current zone as active zone, allowing a replacement
    /// </summary>
    public void DetachCurrentZone()
    {
        if (ActiveZone is null)
            throw new System.Exception("Improper usage of DetachCurrentZone. There MUST be an active zone to remove.");

        ActiveZone = null;
    }

    public void TryStunCreature()
    {
        // conduct stun logic ONLY if it is able to be stunned again
        if (!IsStunned)
        {
            IsStunned = true;

            StartCoroutine(DoStunCreature());

            IEnumerator DoStunCreature()
            {
                yield return new WaitForSeconds(STUN_DURATION);

                IsStunned = false;
            }

            // stun causes aggro if creature was not already aggro
            Instance.IsAggro = true;
        }
    }
    #endregion
}
