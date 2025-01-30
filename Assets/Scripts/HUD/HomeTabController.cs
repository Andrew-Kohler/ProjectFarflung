using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using Unity.Mathematics;

public class HomeTabController : MonoBehaviour
{
    [Header("Narrative Time")]
    // Narrative time passage
    [SerializeField] private Image _narrativeTime; // Top right indicator of time passage
    [SerializeField] private List<Sprite> _narrativeTimeImages;
    [SerializeField] private TextMeshProUGUI _narrativeTimeText;
    [SerializeField, TextArea(1,4)] private List<string> _narrativeTimeReadouts;

    [Header("Head Rotation")]
    // Rotation gauge
    [SerializeField] private Image _verticalRotationIndicator;
    [SerializeField] private TextMeshProUGUI _verticalRotationText;
    [SerializeField] private Transform _cameraRotTransform;

    [Header("Vitals")]
    // Health
    [SerializeField] private Image _healthFill;

    // Flashlight
    [SerializeField] private Image _lightFill;

    // Update is called once per frame
    void Update()
    {
        UpdateNarrativeTime();
        UpdateRotationGauge();
        UpdateVitals();
    }

    private void UpdateNarrativeTime() // Updates the indicator of narratively passed time
    {
        _narrativeTime.sprite = _narrativeTimeImages[GameManager.Instance.SceneData.NarrativeTimestamp];
        _narrativeTimeText.text = _narrativeTimeReadouts[GameManager.Instance.SceneData.NarrativeTimestamp];
    }

    private void UpdateRotationGauge()
    {
        float yPos = 0;
        if (_cameraRotTransform.localRotation.eulerAngles.x < 270) // downward angle
        {
            yPos = math.remap(90, 0, -60, 0, _cameraRotTransform.localRotation.eulerAngles.x);
            double angle = System.Math.Round((-1 * (_cameraRotTransform.localRotation.eulerAngles.x)), 1);
            _verticalRotationText.text = angle.ToString(".0");
        }
        else // upward angle
        {
            yPos = math.remap(360, 270, 0, 60, _cameraRotTransform.localRotation.eulerAngles.x);
            double angle = System.Math.Round((90 - (_cameraRotTransform.localRotation.eulerAngles.x - 270f)), 1);
            _verticalRotationText.text = angle.ToString(".0");
        }

        Vector3 gaugePos = new Vector3(0, yPos, 0);
        _verticalRotationIndicator.transform.localPosition = gaugePos;
    }

    private void UpdateVitals()
    {
        _healthFill.fillAmount = ((float)GameManager.Instance.SceneData.RemainingLives / GameManager.Instance.SceneData.MaxLives);
        float healthR = math.remap(GameManager.Instance.SceneData.MaxLives, 0, .7f, 0, GameManager.Instance.SceneData.RemainingLives);
        _healthFill.color = new Color(.7f, healthR, healthR);
    }
}
