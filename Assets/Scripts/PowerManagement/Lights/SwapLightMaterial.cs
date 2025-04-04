using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// swaps light materials to match enabled/disabled state of light component
public class SwapLightMaterial : MonoBehaviour
{
    [SerializeField, Tooltip("Brighter light surface material.")]
    private Material _litMaterial;
    [SerializeField, Tooltip("Not Bright light surface material.")]
    private Material _unlitMaterial;
    [SerializeField, Tooltip("Light element to determine if light is on or off.")]
    private Light _light;
    [SerializeField, Tooltip("Used to actually swap the material on the light mesh.")]
    private MeshRenderer[] _renderers;

    private bool _isLightOn;

    // Start is called before the first frame update
    void Start()
    {
        // initial config
        UpdateMaterial();
    }

    // Update is called once per frame
    void Update()
    {
        if (_light.enabled != _isLightOn)
            UpdateMaterial();
    }

    private void UpdateMaterial()
    {
        _isLightOn = _light.enabled;
        foreach (MeshRenderer mesh in _renderers)
        {
            Material[] mats = mesh.materials;
            // change LAST material in list (needed for certain meshes that have multiple)
            mats[mats.Length-1] = _isLightOn ? _litMaterial : _unlitMaterial;
            mesh.materials = mats;
        }
    }
}
