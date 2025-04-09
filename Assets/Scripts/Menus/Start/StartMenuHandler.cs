using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Contains all button functionality and credits swapping located on the start menu UI scene.
/// </summary>
public class StartMenuHandler : MonoBehaviour
{
    [Header("Options Navigation")]
    [SerializeField, Tooltip("Enabled/DIsabled to swap between start menu and options menu")]
    private GameObject _optionsContainer;

    [Header("Credits Navigation")]
    [SerializeField, Tooltip("Faded in/out to swap between main menu and credits.")]
    private CanvasGroupController _mainMenuGroup;
    [SerializeField, Tooltip("Faded in/out to swap between main menu and credits.")]
    private CanvasGroupController _creditsGroup;
    [SerializeField, Tooltip("Animator for credits")]
    private Animator _creditsAnim;
    [SerializeField, Tooltip("Fade time between main and credits")]
    private float _creditsFadeTime = 2f;
    private bool _creditsOver = true;

    [Header("Scene Transitions")]
    [SerializeField, Tooltip("Used to make calls to smooth scene transitions.")]
    private SceneTransitionHandler _transitionHandler;
    [SerializeField, Tooltip("Scene name of level select scene.")]
    private string _levelSceneName; // TODO: replace with detecting which scene to load based on save data floor num
    [SerializeField, Tooltip("Scene name of brightness configuration scene (for use on New Game press).")]
    private string _brightnessConfigSceneName;

    [Header("New Game Functionality")]
    [SerializeField, Tooltip("Used to enable/disable resume button based on whether there is a save to resume.")]
    private GameObject _resumeButton;
    [SerializeField, Tooltip("Used if new game would clear save data.")]
    private Animator _newGameConfirmationAnim;
    [SerializeField, Tooltip("Confirm clearing save data")]
    private Button _confirmButton;
    [SerializeField, Tooltip("Go back on clearing save data")]
    private Button _nevermindButton;
    private IEnumerator _currentCloseCoroutine;

    [Header("Menu Aesthetics")]
    [SerializeField, Tooltip("TMP object describing the function of a hovered button")]
    private TextMeshProUGUI _modeText;
    [SerializeField, Tooltip("Image providing a visual aid about the selected mode to fill space.")]
    private Image _modeImage;
    [SerializeField, Tooltip("List of different mode images")]
    private List<Sprite> _modeImages;

    [Header("Scene Intro Sequence")]
    [SerializeField, Tooltip("Toggle for whether this plays (for editor work)")]
    private bool _doOpen = true;
    [SerializeField, Tooltip("Parent of the cutscene (off by default)")]
    private GameObject _cutsceneParent;
    [SerializeField, Tooltip("CanvasGroupController for full logo")]
    private CanvasGroupController _fullLogoGroup;
    [SerializeField, Tooltip("CanvasGroupController for 'Games'")]
    private CanvasGroupController _gamesGroup;
    [SerializeField, Tooltip("CanvasGroupController for back backer")]
    private CanvasGroupController _backerGroup;

    private void Awake()
    {
        // free control over the mouse
        // necessary for when coming back from death realm death ending
        Cursor.lockState = CursorLockMode.None;

        // Only show resume button if there is save data to resume with
        if (!GameManager.Instance.SceneData.NewGameStarted)
        {
            _resumeButton.SetActive(false);
        }

        StartCoroutine(DoInitialLoad());
    }

    private void Update()
    {
        if(_creditsAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && !_creditsOver)
        {
            _creditsOver = true;
            BackButton();
        }
    }

    #region Main Menu Buttons
    /// <summary>
    /// Functionality for New Game Button.
    /// Either loads level select on new save (if no save data present) OR shows confirmation popup before overwriting save
    /// </summary>
    public void NewGameButton()
    {
        // overriding previous save -> confirmation popup
        if (GameManager.Instance.SceneData.NewGameStarted)
        {
            if(_currentCloseCoroutine != null)
                StopCoroutine(_currentCloseCoroutine);
            _newGameConfirmationAnim.gameObject.SetActive(true);
            _newGameConfirmationAnim.Play("Popup");
        }
        // no save data being overriden
        else
        {
            GameManager.Instance.ResetGameData(); // new progression data
            
            // load brightness config scene
            _transitionHandler.LoadScene(_brightnessConfigSceneName);
        }

        // Click SFX
        AudioManager.Instance.PlayClickUI();
    }

    // Displays text and an image related to starting a new game
    public void NewGameButtonHover()
    {
        _modeText.text = "> Begin a new playthrough.";
        _modeImage.sprite = _modeImages[0];
    }

    /// <summary>
    /// Resume Button Functionality.
    /// Loads level select scene.
    /// </summary>
    public void ResumeButton()
    {
        // Click SFX
        AudioManager.Instance.PlayClickUI();

        // load level scene functionality
        _transitionHandler.LoadScene(_levelSceneName);
    }

    // Displays text and an image related to resuming a saved game
    public void ResumeButtonHover()
    {
        _modeText.text = "> Continue from your last saved game.";
        _modeImage.sprite = _modeImages[1];
    }

    /// <summary>
    /// Options Button Functionality.
    /// Opens Controls/Audio options, skipping the pause menu segment of the Pause Canvas.
    /// </summary>
    public void OptionsButton()
    {
        // Click SFX
        AudioManager.Instance.PlayClickUI();

        // bypasses pause menu of pause/options canvas
        _optionsContainer.SetActive(true);
    }

    // Displays text and an image related to otpions
    public void OptionsButtonHover()
    {
        _modeText.text = "> Change control, audio, and camera settings.";
        _modeImage.sprite = _modeImages[2];
    }

    /// <summary>
    /// Credits Button functionality.
    /// Fades out main manu container and fades in credits container.
    /// </summary>
    public void CreditsButton()
    {
        // Click SFX
        AudioManager.Instance.PlayClickUI();

        // Restart the animation and fade the menus in and out
        _creditsAnim.Play("Scroll", 0, 0);
        _creditsGroup.FadeIn(_creditsFadeTime);
        _mainMenuGroup.FadeOut(_creditsFadeTime);

        // Correctly sets Canvas Group settings so appropriate layer can be interacted with
        _mainMenuGroup.ToggleInteractable(false);
        _mainMenuGroup.ToggleBlocker(false);
        _creditsGroup.ToggleInteractable(true);
        _creditsGroup.ToggleBlocker(true);

        StartCoroutine(DoCreditsLoadDelay());
    }

    // Displays text and an image related to otpions
    public void CreditsButtonHover()
    {
        _modeText.text = "> View the developer credits.";
        _modeImage.sprite = _modeImages[3];
    }

    /// <summary>
    /// Quit Button functionality.
    /// Exits play mode (in editor) or closes application (in build)
    /// </summary>
    public void QuitButton()
    {
#if UNITY_EDITOR
        // quits play mode if in editor
        EditorApplication.ExitPlaymode();
#endif
        Application.Quit();
    }

    // Displays text and an image related to otpions
    public void QuitButtonHover()
    {
        _modeText.text = "> Exit the game.";
        _modeImage.sprite = _modeImages[4];
    }
    #endregion

    #region Credits Buttons
    /// <summary>
    /// Button used for returning back to the main menu (from credits).
    /// Fades out credits container and fades in main menu container.
    /// </summary>
    public void BackButton()
    {
        _creditsGroup.FadeOut(_creditsFadeTime);
        _mainMenuGroup.FadeIn(_creditsFadeTime);

        // Correctly sets Canvas Group settings so appropriate layer can be interacted with
        _mainMenuGroup.ToggleInteractable(true);
        _mainMenuGroup.ToggleBlocker(true);
        _creditsGroup.ToggleInteractable(false);
        _creditsGroup.ToggleBlocker(false);

        // Click SFX
        AudioManager.Instance.PlayClickUI();
    }
    #endregion

    #region New Game Confirmation
    /// <summary>
    /// Loads new game without any stipulations, this is the confirmation function.
    /// </summary>
    public void ConfirmNewGame()
    {
        // Click SFX
        AudioManager.Instance.PlayClickUI();

        GameManager.Instance.ResetGameData(); // new progression data

        // load brightness config scene
        _transitionHandler.LoadScene(_brightnessConfigSceneName);
    }

    /// <summary>
    /// Cancels the confirmation popup for creating a new game.
    /// Can be used by other buttons to ensure popup is closed.
    /// </summary>
    public void AbortNewGame()
    {
        // Click SFX
        AudioManager.Instance.PlayClickUI();

        _currentCloseCoroutine = DoPopupClose();
        StartCoroutine(_currentCloseCoroutine);
    }
    #endregion

    #region Animated Transitions

    private IEnumerator DoInitialLoad()
    {
        if (_doOpen)
        {
            _cutsceneParent.SetActive(true);
            // Fade in Farflung Games
            yield return new WaitForSeconds(1f);
            _fullLogoGroup.FadeIn(4f);
            yield return new WaitForSeconds(6f);

            // Fade out Games
            _gamesGroup.FadeOut(4f);
            yield return new WaitForSeconds(6f);

            // Fade out backer so that you can see the main menu
            _backerGroup.FadeOut(4f);
            yield return new WaitForSeconds(2f);
            _backerGroup.ToggleBlocker(false);
        }
        

        yield return null;
    }

    private IEnumerator DoCreditsLoadDelay()
    {
        // Since update is being used, we have to be careful to not meet the end conditions at the same time when re-loading the credits
        yield return new WaitForEndOfFrame(); 
        _creditsOver = false;
    }

    private IEnumerator DoPopupClose()
    {
        _newGameConfirmationAnim.Play("Shrink");
        yield return new WaitForSeconds(.333f);
        _newGameConfirmationAnim.gameObject.SetActive(false);
    }

    #endregion
}
