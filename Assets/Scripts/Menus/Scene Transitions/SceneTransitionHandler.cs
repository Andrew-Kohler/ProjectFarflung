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
        GameManager.Instance.PlayerEnabled = false;
        StartCoroutine(DoTransition(sceneName));
    }

    /// <summary>
    /// Handles waiting until animation completes before actually transitioning scenes
    /// </summary>
    private IEnumerator DoTransition(string sceneName)
    {
        yield return new WaitUntil(() => _isDoneTransitioning);

        SceneManager.LoadScene(sceneName);
    }

   

    /// <summary>
    /// Called by animation event at end of fade out animation.
    /// </summary>
    public void SetDoneTransitioning()
    {
        _isDoneTransitioning = true;
    }

    /// <summary>
    /// Called by animation event at end of fade out animation.
    /// </summary>
    public void SetDoneEnter()
    {
        _isDoneEnter = true;
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
