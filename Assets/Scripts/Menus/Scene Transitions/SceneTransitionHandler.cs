using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles smoothly animating and timing scene transitions.
/// TransitionIn plays automatically as default non-looping animation.
/// </summary>
public class SceneTransitionHandler : MonoBehaviour
{
    [SerializeField, Tooltip("Used to trigger animations for scene enter/exit")]
    private Animator _anim;

    private bool _isDoneEnter = false; // becomes true once new scene is fully faded in
    private bool _isDoneTransitioning = false;

    /// <summary>
    /// Function that should be used to activate any scene transition in the game.
    /// </summary>
    public void LoadScene(string sceneName)
    {
        _anim.Play("FadeExit");

        // lock player controls until end of next scene enter
        GameManager.Instance.PlayerEnabled = false;

        StartCoroutine(DoTransition(sceneName));
    }

    /// <summary>
    /// Handles waiting until animation completes before actually transitioning scenes
    /// </summary>
    private IEnumerator DoTransition(string sceneName)
    {
        yield return new WaitUntil(() => _isDoneTransitioning);

        // special case of dynamically picking scene based on save data (used for resume from start and respawn from Death Realm).
        if (sceneName == "Resume")
        {
            // Resume functionality requires that no override load point is being used
            GameManager.Instance.LoadPoint = -1;

            // load death realm
            if (GameManager.Instance.SceneData.IsInDeathRealm)
            {
                // return mouse to first-person mode if not already
                Cursor.lockState = CursorLockMode.Locked;

                SceneManager.LoadScene("DeathRealm");
            }
            // load specific level scene
            else
            {
                switch (GameManager.Instance.SceneData.SaveScene)
                {
                    case 0:
                        // return mouse to first-person mode if not already
                        Cursor.lockState = CursorLockMode.Locked;

                        SceneManager.LoadScene("Hangar");
                        break;
                    case 1:
                        // return mouse to first-person mode if not already
                        Cursor.lockState = CursorLockMode.Locked;

                        SceneManager.LoadScene("Floor1");
                        break;
                    case 2:
                        // return mouse to first-person mode if not already
                        Cursor.lockState = CursorLockMode.Locked;

                        SceneManager.LoadScene("Floor2");
                        break;
                    case 3:
                        // return mouse to first-person mode if not already
                        Cursor.lockState = CursorLockMode.Locked;

                        SceneManager.LoadScene("Command");
                        break;
                    default:
                        throw new System.Exception("Invalid GameManager SaveScene index: must be between -1 and 3.");
                }
            }
        }
        // otherwise load scene normally
        else
            SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Called by animation event at end of FADE-OUT animation.
    /// </summary>
    public void SetDoneTransitioning()
    {
        _isDoneTransitioning = true;
    }

    /// <summary>
    /// Called by animation event at end of FADE-IN animation.
    /// </summary>
    public void SetDoneEnter()
    {
        _isDoneEnter = true;
        // restore controls for new scene that was just entered
        GameManager.Instance.PlayerEnabled = true;
    }

    /// <summary>
    /// Returns whether the current scene has finished fading IN.
    /// </summary>
    public bool IsDoneEnter()
    {
        return _isDoneEnter;
    }
}
