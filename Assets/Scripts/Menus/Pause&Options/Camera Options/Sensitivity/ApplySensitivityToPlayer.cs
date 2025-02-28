using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

/// <summary>
/// Applies sensitivity from game manager to player controller.
/// </summary>
public class ApplySensitivityToPlayer : MonoBehaviour
{
    [SerializeField, Tooltip("Used to apply FoV value to the virutal camera component itself.")]
    private FirstPersonController _player;

    private float _currSensitivity;

    // Start is called before the first frame update
    void Start()
    {
        _currSensitivity = GameManager.Instance.OptionsData.Sensitivity;
        _player.RotationSpeed = _currSensitivity;
    }

    // Update is called once per frame
    void Update()
    {
        if (_currSensitivity != GameManager.Instance.OptionsData.Sensitivity)
        {
            _currSensitivity = GameManager.Instance.OptionsData.Sensitivity;
            _player.RotationSpeed = GameManager.Instance.OptionsData.Sensitivity;
        }
    }
}
