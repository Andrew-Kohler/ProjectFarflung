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
    [SerializeField, Tooltip("Enabled/Disabled to swap between main menu and credits.")]
    private GameObject _mainMenuContainer;
    [SerializeField, Tooltip("Enabled/Disabled to swap between main menu and credits.")]
    private GameObject _creditsContainer;

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
    [SerializeField, Tooltip("Enabled if new game would clear save data.")]
    private GameObject _newGameConfirmation;

    [Header("Menu Aesthetics")]
    [SerializeField, Tooltip("TMP object describing the function of a hovered button")]
    private TextMeshProUGUI _modeText;
    [SerializeField, Tooltip("Image providing a visual aid about the selected mode to fill space.")]
    private Image _modeImage;
    [SerializeField, Tooltip("List of different mode images")]
    private List<Sprite> _modeImages;

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
            _newGameConfirmation.SetActive(true);
        }
        // no save data being overriden
        else
        {
            GameManager.Instance.ResetGameData(); // new progression data
            
            // load brightness config scene
            _transitionHandler.LoadScene(_brightnessConfigSceneName);
        }
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
        // bypasses pause menu of pause/options canvas
        _optionsContainer.SetActive(true);
    }

    // Displays text and an image related to otpions
    public void OptionsButtonHover()
    {
        _modeText.text = "> Change brightness, volume, and camera settings.";
        _modeImage.sprite = _modeImages[2];
    }

    /// <summary>
    /// Credits Button functionality.
    /// Disables main manu container and enables credits container.
    /// </summary>
    public void CreditsButton()
    {
        _creditsContainer.SetActive(true);
        _mainMenuContainer.SetActive(false);
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
    /// Disables credits container and enables main menu container.
    /// </summary>
    public void BackButton()
    {
        _creditsContainer.SetActive(false);
        _mainMenuContainer.SetActive(true);
    }
    #endregion

    #region New Game Confirmation
    /// <summary>
    /// Loads new game without any stipulations, this is the confirmation function.
    /// </summary>
    public void ConfirmNewGame()
    {
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
        _newGameConfirmation.SetActive(false);
    }
    #endregion

    #region Animated Transitions

    private IEnumerator DoInitialLoad()
    {
        // Fade in Farflung Games
        // Fade out Games
        // Fade out backer so that you can see the main menu
        yield return null;
    }

    #endregion
}
