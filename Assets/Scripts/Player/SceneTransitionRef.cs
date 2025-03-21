using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Just contains a reference to the SceneTransitionsHandler.
/// </summary>
public class SceneTransitionRef : MonoBehaviour
{
    [Header("Scene Transitions")]
    [SerializeField, Tooltip("Used to transition scenes when creature contacts the player")]
    public SceneTransitionHandler TransitionHandler;

    // Start is called before the first frame update
    void Start()
    {
        // sorry for the inconvenience on this one, I could not find a good way to get this reference to the Creature making the contact check
        // ideally SceneTransitionsHandler would have been made as a static singleton to begin with but I did not do that and making the change now is probably just harder than doing this ;-;
        if (TransitionHandler is null)
            throw new System.Exception("Incorrect Configuration: Player MUST have reference to SceneTransitionsHandler.");
    }
}
