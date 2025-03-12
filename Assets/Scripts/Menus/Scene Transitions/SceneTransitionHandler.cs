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
    [SerializeField, Tooltip("List of transforms used to correctly place the player in this scene")]
    private List<GameObject> _loadSpots;
    [SerializeField, Tooltip("Player transform")]
    private Transform _playerTransform;

    private bool _isDoneEnter = false; // becomes true once new scene is fully faded in
    private bool _isDoneTransitioning = false;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Function that should be used to activate any scene transition in the game.
    /// </summary>
    public void LoadScene(string sceneName)
    {
        _anim.Play("FadeExit");

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

    // Sets the player to their correct position
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _playerTransform.position = _loadSpots[GameManager.Instance.LoadPoint].transform.position;
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
    }

    /// <summary>
    /// Returns whether the current scene has finished fading IN.
    /// </summary>
    public bool IsDoneEnter()
    {
        return _isDoneEnter;
    }
}
