using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("HUD Tab Navigation")]
    [SerializeField] private Animator _hudNavAnim;
    [SerializeField] private List<Image> _hudImages; // -2, -1, 0, 1, 2
    [SerializeField] private List<Image> _hudSprites; // Map, main, file

    [Header("HUD Tabs")]
    [SerializeField] private GameObject _mainTab;
    [SerializeField] private GameObject _mapTab;
    [SerializeField] private GameObject _fileTab;

    bool active = true;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            StartCoroutine(DoTabSwitch());
        }
    }

    private IEnumerator DoTabSwitch()
    {
        active = false;
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

        yield return new WaitForSeconds(2f);
        active = true;
        yield return null;
    }
}
