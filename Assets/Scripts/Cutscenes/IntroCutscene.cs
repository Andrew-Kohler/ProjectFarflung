using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class IntroCutscene : MonoBehaviour
{
    private Animator _anim;

    // Control booleans
    private bool _interactPressed;

    private bool _forwardPressed;
    private bool _leftPressed;
    private bool _backPressed;
    private bool _rightPressed;
    private bool _jumpPressed;

    private bool _hudUpPressed;
    private bool _hudLeftPressed;
    private bool _hudDownPressed;
    private bool _hudRightPressed;
    private bool _hudTabPressed;

    private bool _flashlightPressed;

    [Header("Motion Tutorial")]
    [SerializeField] private GameObject _forwardBind;
    [SerializeField] private GameObject _leftBind;
    [SerializeField] private GameObject _backBind;
    [SerializeField] private GameObject _rightBind;
    [SerializeField] private GameObject _jumpBind;
    [Header("HUD Tutorial")]
    [SerializeField] private GameObject _hudUpBind;
    [SerializeField] private GameObject _hudLeftBind;
    [SerializeField] private GameObject _hudBackBind;
    [SerializeField] private GameObject _hudRightBind;
    [SerializeField] private GameObject _hudTabBind;
    [Header("Flashlight Tutorial")]
    [SerializeField] private GameObject _flashlightBind;
    [SerializeField] private Image _flashlightMeter;

    void Start()
    {
        if (GameManager.Instance.SceneData.IntroCutsceneWatched)
        {
            Destroy(gameObject);
        }
        else
        {
            _anim = GetComponent<Animator>();
            StartCoroutine(DoIntroCutscene());
        }
        
    }

    #region Controls Bindings
    private void OnEnable()
    {
        InputSystem.actions.FindAction("Interact").started += DoInteract;

        InputSystem.actions.FindAction("MoveForward").started += DoPressMoveForward;
        InputSystem.actions.FindAction("MoveRight").started += DoPressMoveRight;
        InputSystem.actions.FindAction("MoveBackward").started += DoPressMoveBack;
        InputSystem.actions.FindAction("MoveLeft").started += DoPressMoveLeft;
        InputSystem.actions.FindAction("Jump").started += DoPressJump;

        InputSystem.actions.FindAction("HUDUp").started += DoPressHUDUp;
        InputSystem.actions.FindAction("HUDLeft").started += DoPressHUDLeft;
        InputSystem.actions.FindAction("HUDRight").started += DoPressHUDRight;
        InputSystem.actions.FindAction("HUDDown").started += DoPressHUDDown;
        InputSystem.actions.FindAction("Tab").started += DoPressTab;

        InputSystem.actions.FindAction("ToggleFlashlight").started += DoPressFlashlight;
    }

    private void OnDisable()
    {
        InputSystem.actions.FindAction("Interact").started -= DoInteract;

        InputSystem.actions.FindAction("MoveForward").started -= DoPressMoveForward;
        InputSystem.actions.FindAction("MoveRight").started -= DoPressMoveRight;
        InputSystem.actions.FindAction("MoveBackward").started -= DoPressMoveBack;
        InputSystem.actions.FindAction("MoveLeft").started -= DoPressMoveLeft;
        InputSystem.actions.FindAction("Jump").started -= DoPressJump;

        InputSystem.actions.FindAction("HUDUp").started -= DoPressHUDUp;
        InputSystem.actions.FindAction("HUDLeft").started -= DoPressHUDLeft;
        InputSystem.actions.FindAction("HUDRight").started -= DoPressHUDRight;
        InputSystem.actions.FindAction("HUDDown").started -= DoPressHUDDown;
        InputSystem.actions.FindAction("Tab").started -= DoPressTab;
    }

    /// <summary>
    /// Simply calls Interact() function.
    /// Necessary to avoid memory leak.
    /// </summary>
    private void DoInteract(InputAction.CallbackContext context)
    {
        _interactPressed = true;
    }

    private void DoPressMoveForward(InputAction.CallbackContext context)
    {
        _forwardPressed = true;
    }

    private void DoPressMoveRight(InputAction.CallbackContext context)
    {
        if(_forwardPressed)
            _rightPressed = true;
    }

    private void DoPressMoveLeft(InputAction.CallbackContext context)
    {
        if(_forwardPressed && _rightPressed && _backPressed)
            _leftPressed = true;
    }

    private void DoPressMoveBack(InputAction.CallbackContext context)
    {
        if(_forwardPressed && _rightPressed)
        _backPressed = true;
    }

    private void DoPressJump(InputAction.CallbackContext context)
    {
        if (_forwardPressed && _rightPressed && _backPressed && _leftPressed)
            _jumpPressed = true;
    }

    private void DoPressHUDUp(InputAction.CallbackContext context)
    {
        _hudUpPressed = true;
    }

    private void DoPressHUDRight(InputAction.CallbackContext context)
    {
        if(_hudUpPressed)
            _hudRightPressed = true;
    }

    private void DoPressHUDLeft(InputAction.CallbackContext context)
    {
        if (_hudUpPressed && _hudRightPressed && _hudDownPressed)
            _hudLeftPressed = true;
    }

    private void DoPressHUDDown(InputAction.CallbackContext context)
    {
        if (_hudUpPressed && _hudRightPressed)
            _hudDownPressed = true;
    }

    private void DoPressTab(InputAction.CallbackContext context)
    {
        if (_hudUpPressed && _hudRightPressed && _hudDownPressed && _hudLeftPressed)
            _hudTabPressed = true;
    }

    private void DoPressFlashlight(InputAction.CallbackContext context)
    {
        if(_hudTabPressed)
            _flashlightPressed = true;
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        _flashlightMeter.fillAmount = GameManager.StunHoldRatio;
    }

    private IEnumerator DoIntroCutscene()
    {
        // Setup
        GameManager.Instance.PlayerEnabled = false;
        //TODO: Lock the HUD tabs out of changing for the moment

        // Part 1: Fun text scroll, no input
        _anim.Play("Intro1", 0, 0);
        yield return new WaitForSeconds(8.1667f);

        // Part 2: Welcome to calibration
        _anim.Play("Intro2", 0, 0);
        yield return new WaitForSeconds(1.667f);
        yield return new WaitUntil(() => _interactPressed);
        _interactPressed = false;

        // Part 3: Movement tutorial
        _anim.Play("Intro3", 0, 0);
        yield return new WaitForSeconds(1.1667f);
        yield return new WaitUntil(() => _forwardPressed);
        _rightBind.SetActive(true);
        yield return new WaitUntil(() => _rightPressed);
        _backBind.SetActive(true);
        yield return new WaitUntil(() => _backPressed);
        _leftBind.SetActive(true);
        yield return new WaitUntil(() => _leftPressed);
        _jumpBind.SetActive(true);
        yield return new WaitUntil(() => _jumpPressed);

        // Part 4: HUD tutorial
        _anim.Play("Intro4", 0, 0);
        yield return new WaitForSeconds(1.1667f);
        yield return new WaitUntil(() => _hudUpPressed);
        _hudRightBind.SetActive(true);
        yield return new WaitUntil(() => _hudRightPressed);
        _hudBackBind.SetActive(true);
        yield return new WaitUntil(() => _hudDownPressed);
        _hudLeftBind.SetActive(true);
        yield return new WaitUntil(() => _hudLeftPressed);
        _hudTabBind.SetActive(true);
        yield return new WaitUntil(() => _hudTabPressed);

        // Part 5: Flashlight tutorial
        _anim.Play("Intro5", 0, 0);
        yield return new WaitForSeconds(1.1667f);
        yield return new WaitUntil(() => _flashlightPressed);
        _flashlightBind.SetActive(true);
        yield return new WaitUntil(() => GameManager.StunHoldRatio == 1);

        // Part 6: Calibration conclusion
        _anim.Play("Intro6", 0, 0);
        yield return new WaitForSeconds(1.1667f);
        yield return new WaitUntil(() => _interactPressed);
        _interactPressed = false;

        // Part 7: Attempting to connect
        _anim.Play("Intro7", 0, 0);
        yield return new WaitForSeconds(25f);
        yield return new WaitUntil(() => _interactPressed);
        _interactPressed = false;

        // Part 8: Close and begin
        _anim.Play("Intro8", 0, 0);
        GameManager.FlashlightCharge = 1f;
        yield return new WaitForSeconds(1f);

        // Cleanup
        GameManager.Instance.PlayerEnabled = true;
        GameManager.Instance.SceneData.IntroCutsceneWatched = true;
        
    }
}
