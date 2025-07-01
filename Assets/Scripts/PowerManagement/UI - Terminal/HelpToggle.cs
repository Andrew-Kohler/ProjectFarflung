using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the showing, hiding, and animating of the terminal help menu through a button.
/// </summary>
public class HelpToggle : MonoBehaviour
{
    [SerializeField, Tooltip("Used to smoothly animate help menu in and out.")]
    private CanvasGroup _alphaGroup;
    [SerializeField, Tooltip("Rate at which fade in/out occurs.")]
    private float _fadeRate;
    [SerializeField, Tooltip("Whether the tutorial opens the help menu by default on terminal first open.")]
    private bool _isTutorialTerminal;

    private bool _isOpen;

    // Start is called before the first frame update
    void Start()
    {
        // tutorial terminal help menu open by default
        if (_isTutorialTerminal)
        {
            _alphaGroup.alpha = 1;
            _isOpen = true;
            _alphaGroup.blocksRaycasts = true;
        }
        // menu closed by default otherwise
        else
        {
            _alphaGroup.alpha = 0;
            _isOpen = false;
            _alphaGroup.blocksRaycasts = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isOpen)
        {
            _alphaGroup.alpha += _fadeRate * Time.deltaTime;

            // start blocking raycasts as soon as it starts opening
            _alphaGroup.blocksRaycasts = true;
        }
        else
        {
            _alphaGroup.alpha -= _fadeRate * Time.deltaTime;

            // stop blocking raycasts only once fully closed
            if (_alphaGroup.alpha <= 0)
                _alphaGroup.blocksRaycasts = false;
        }
    }

    public void ToggleHelpMenu()
    {
        _isOpen = !_isOpen;

        // terminal press SFX
        AudioManager.Instance.PlayTerminalArrow();
    }
}
