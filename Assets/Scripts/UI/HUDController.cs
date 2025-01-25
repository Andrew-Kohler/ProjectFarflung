using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using Unity.Mathematics;

public class HUDController : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;

    [Header("HUD Tab Navigation")]
    [SerializeField] private Animator _hudNavAnim;
    [SerializeField] private List<Image> _hudImages; // -2, -1, 0, 1, 2

    [Header("HUD Tabs")]
    [SerializeField] private List<GameObject> _tabs;

    

    private int currentTab = 0;

    bool active = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        


        if (_playerInput.actions.FindAction("Tab").IsPressed() && !active)
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
        active = false;
        yield return null;
    }
}
