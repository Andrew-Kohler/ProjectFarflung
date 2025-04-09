using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using Unity.Mathematics;

public class HUDController : MonoBehaviour
{
    // Routing all of the player components through here so we don't have to dig around in the tabs
    public Transform PlayerTransform;
    public Transform CameraRotTransform;
    [HideInInspector] public FlashlightController FlashlightController;

    [Header("HUD Tab Navigation")]
    [SerializeField] private Animator _hudNavAnim;
    [SerializeField] private List<Image> _hudImages; // -2, -1, 0, 1, 2
    [SerializeField] private GameObject _tabSwapParent;
    [SerializeField] private GameObject _tabSwapText;


    [Header("HUD Tabs")]
    [SerializeField] private List<GameObject> _tabs;

    [Header("Interact Tab")]
    [SerializeField] private GameObject _interactTab;
    private Animator _hudControlAnim;
    private bool _canSwitchTabs = true;    // Disables the ability to change tabs when interacting with stuff

    private int currentTab = 0;

    bool _active = false;

    #region Controls Bindings
    private void OnEnable()
    {
        InputSystem.actions.FindAction("Tab").started += DoTabSwitch;
        InputSystem.actions.FindAction("Tab").canceled += SetActiveFalse;

        TerminalInteractable.onLockedInteractionTerminal += HUDBlink;
        WireBoxInteractable.onLockedInteractionWirebox += HUDBlink;
    }

    private void OnDisable()
    {
        InputSystem.actions.FindAction("Tab").started -= DoTabSwitch;
        InputSystem.actions.FindAction("Tab").canceled -= SetActiveFalse;

        TerminalInteractable.onLockedInteractionTerminal -= HUDBlink;
        WireBoxInteractable.onLockedInteractionWirebox -= HUDBlink;
    }

    private void Start()
    {
        _hudControlAnim = GetComponent<Animator>();
        FlashlightController = PlayerTransform.GetComponentInChildren<FlashlightController>();
    }

    /// <summary>
    /// Simply calls TabSwitch() function.
    /// Necessary to avoid memory leak
    /// </summary>
    private void DoTabSwitch(InputAction.CallbackContext context)
    {
        TabSwitch();
    }

    /// <summary>
    /// Necessary to avoid memory leak.
    /// </summary>
    private void SetActiveFalse(InputAction.CallbackContext context)
    {
        _active = false;
    }
    #endregion

    // Update is called once per frame
    void Update()
    {

    }

    private void TabSwitch()
    {
        if (!_active && _canSwitchTabs)
        {
            // tab cycle SFX
            AudioManager.Instance.PlayTabCycle();

            StartCoroutine(DoTabSwitch());
        }
    }

    private void HUDBlink(bool start)
    {
        StartCoroutine(DoHUDBlink(start));
    }

    private IEnumerator DoTabSwitch()
    {
        _active = true;

        _tabs[currentTab].SetActive(false);
        currentTab++;
        if(currentTab >= _tabs.Count)
        {
            currentTab = 0;
        }
        _tabs[currentTab].SetActive(true);

        _hudNavAnim.Play("Right", 0, 0);
        Sprite end = _hudImages[2].sprite;
        // 1 becomes 0
        _hudImages[3].sprite = _hudImages[2].sprite;
        // 0 becomes -1
        _hudImages[2].sprite = _hudImages[1].sprite;
        // -1 becomes -2
        _hudImages[1].sprite = _hudImages[0].sprite;
        // -2 becomes 2
        _hudImages[0].sprite = end;

        yield return new WaitForSeconds(.25f);
        
        yield return null;
    }

    private IEnumerator DoHUDBlink(bool start)
    {
        _hudControlAnim.Play("BlinkDown");
        yield return new WaitForSeconds(7f / 12);

        // Turn everything on or off between blinks
        if (start)
        {
            _interactTab.SetActive(true);

            _tabs[currentTab].SetActive(false);
            _tabSwapParent.SetActive(false);
            _tabSwapText.SetActive(false);

            _canSwitchTabs = false;
        }
        else
        {
            _interactTab.SetActive(false);

            _tabs[currentTab].SetActive(true);
            _tabSwapParent.SetActive(true);
            _tabSwapText.SetActive(true);

            _canSwitchTabs = true;
        }
        _hudControlAnim.Play("BlinkUp");

    }
}
