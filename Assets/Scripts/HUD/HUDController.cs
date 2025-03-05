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

    [Header("HUD Tab Navigation")]
    [SerializeField] private Animator _hudNavAnim;
    [SerializeField] private List<Image> _hudImages; // -2, -1, 0, 1, 2

    [Header("HUD Tabs")]
    [SerializeField] private List<GameObject> _tabs;

    private int currentTab = 0;

    bool active = false;

    #region Controls Bindings
    private void OnEnable()
    {
        InputSystem.actions.FindAction("Tab").started += DoTabSwitch;
        InputSystem.actions.FindAction("Tab").canceled += SetActiveFalse;
    }

    private void OnDisable()
    {
        InputSystem.actions.FindAction("Tab").started -= DoTabSwitch;
        InputSystem.actions.FindAction("Tab").canceled -= SetActiveFalse;
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
        active = false;
    }
    #endregion

    // Update is called once per frame
    void Update()
    {

    }

    private void TabSwitch()
    {
        if (!active)
        {
            StartCoroutine(DoTabSwitch());
        }
    }

    private IEnumerator DoTabSwitch()
    {
        active = true;

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
}
