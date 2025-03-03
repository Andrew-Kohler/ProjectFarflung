using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/// <summary>
/// Applies the game manager post-processing values to the post process profile linked to this script.
/// Currently only applies for brightness (could also add other effects such as saturation).
/// </summary>
public class ApplyPostProcessingSettings : MonoBehaviour
{
    [SerializeField, Tooltip("Used to set exposure value on post-processing volume.")]
    private PostProcessProfile _profile;

    private AutoExposure _exposure;
    private float _currVal;

    private void Awake()
    {
        // fetch exposure compnent of post process profile
        _profile.TryGetSettings(out _exposure);
        _currVal = _exposure.keyValue.value;
    }

    // Update is called once per frame
    void Update()
    {
        // avoid applying value every frame
        if (_currVal != GameManager.Instance.OptionsData.Brightness)
        {
            _exposure.keyValue.value = GameManager.Instance.OptionsData.Brightness;
            _currVal = GameManager.Instance.OptionsData.Brightness;
        }
    }
}
