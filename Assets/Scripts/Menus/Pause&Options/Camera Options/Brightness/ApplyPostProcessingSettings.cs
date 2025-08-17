using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Applies the game manager post-processing values to the post process profile linked to this script.
/// Currently only applies for brightness (could also add other effects such as saturation).
/// </summary>
public class ApplyPostProcessingSettings : MonoBehaviour
{
    [SerializeField, Tooltip("Used to set exposure value on post-processing volume.")]
    private Volume volume;

    private ColorAdjustments _colorAdjustments;
    private float _currVal;

    private void Awake()
    {
        if (volume.profile.TryGet(out _colorAdjustments))
        {
            _currVal = _colorAdjustments.postExposure.value;
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        // avoid applying value every frame
        if (_currVal != GameManager.Instance.OptionsData.Brightness)
        {
            _colorAdjustments.postExposure.value = GameManager.Instance.OptionsData.Brightness;
            _currVal = GameManager.Instance.OptionsData.Brightness;
        }
    }
}
