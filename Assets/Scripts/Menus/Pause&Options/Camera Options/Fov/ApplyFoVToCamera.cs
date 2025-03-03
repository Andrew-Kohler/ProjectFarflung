using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ApplyFoVToCamera : MonoBehaviour
{
    [SerializeField, Tooltip("Used to apply FoV value to the virutal camera component itself.")]
    private CinemachineVirtualCamera _camera;

    private int _currFoV;

    // Start is called before the first frame update
    void Start()
    {
        _currFoV = GameManager.Instance.OptionsData.FoV;
        _camera.m_Lens.FieldOfView = _currFoV;
    }

    // Update is called once per frame
    void Update()
    {
        if (_currFoV != GameManager.Instance.OptionsData.FoV)
        {
            _camera.m_Lens.FieldOfView = GameManager.Instance.OptionsData.FoV;
            _currFoV = GameManager.Instance.OptionsData.FoV;
        }
    }
}
