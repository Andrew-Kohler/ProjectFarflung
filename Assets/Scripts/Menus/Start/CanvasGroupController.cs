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
        _currentFadeOut = DoFadeOut(time);
        if (_currentFadeIn != null)
            StopCoroutine(_currentFadeIn);
        StartCoroutine(_currentFadeOut);
    }

    public void ToggleInteractable(bool interact)
    {
        _group.interactable = interact;
    }
    public void ToggleBlocker(bool block)
    {
        _group.blocksRaycasts = block;
    }

    private IEnumerator DoFadeIn(float time)
    {
        _group = GetComponent<CanvasGroup>();
        float timeToTake = time;
        float timeElapsed = 0;
        float initial = _group.alpha;
        while (time > 0f)
        {
            _group.alpha = Mathf.Lerp(initial, 1, timeElapsed / timeToTake);
            yield return new WaitForSeconds(Time.deltaTime);
            time -= Time.deltaTime;
            timeElapsed += Time.deltaTime;
        }
        _group.alpha = 1.0f;
    }

    private IEnumerator DoFadeOut(float time)
    {
        _group = GetComponent<CanvasGroup>();

        float timeToTake = time;
        float timeElapsed = 0;
        float initial = _group.alpha;
        while (time > 0f)
        {
            _group.alpha = Mathf.Lerp(initial, 0, timeElapsed / timeToTake);
            yield return new WaitForSeconds(Time.deltaTime);
            time -= Time.deltaTime;
            timeElapsed += Time.deltaTime;
        }
        _group.alpha = 0.0f;
    }
}
