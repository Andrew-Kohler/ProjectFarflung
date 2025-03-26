using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasGroupController : MonoBehaviour
{
    private CanvasGroup _group; // Canvas groups can be used to give any UI element an alpha channel

    private IEnumerator _currentFadeIn;
    private IEnumerator _currentFadeOut;    
    void Start()
    {
        _group = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FadeIn(float time)
    {
        _currentFadeIn = DoFadeIn(time);
        if(_currentFadeOut != null)
                StopCoroutine(_currentFadeOut);
        StartCoroutine(_currentFadeIn);
    }

    public void FadeOut(float time)
    {

    }

    private IEnumerator DoFadeIn(float time)
    {
        float increment = (1 - _group.alpha) / time * 60;
        // 1 unit, over 3 seconds (with 60 frames a second)
        while (_group.alpha < 1.0f)
        {
            _group.alpha += increment;
            yield return null;
        }
        _group.alpha = 1.0f;
    }

    private IEnumerator DoFadeOut(float time)
    {
        float increment = (0 + _group.alpha) / time * 60;
        // 1 unit, over 3 seconds (with 60 frames a second)
        while (_group.alpha > 0f)
        {
            _group.alpha -= increment;
            yield return null;
        }
        _group.alpha = 0.0f;
    }
}
