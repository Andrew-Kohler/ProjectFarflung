using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using Unity.Mathematics;

public class HomeTabController : MonoBehaviour
{

    [Header("HUD Controller")]
    [SerializeField] private HUDController _mainHUD;

    [Header("Narrative Time")] 
    // Narrative time passage
    [SerializeField] private Image _narrativeTime; // Top right indicator of time passage
    [SerializeField] private Sprite[] _narrativeTimeImages = new Sprite[5];
    [SerializeField] private TextMeshProUGUI _narrativeTimeText;
    [SerializeField, TextArea(1,4)] private string[] _narrativeTimeReadouts = new string[5];

    [Header("Head Rotation")]
    // Rotation gauge
    [SerializeField] private Image _verticalRotationIndicator;
    [SerializeField] private TextMeshProUGUI _verticalRotationText;
    

    [Header("Vitals")]
    // Health
    [SerializeField] private Image _healthFill;
    [SerializeField] private TextMeshProUGUI _healthReadout;

    // Flashlight
    [SerializeField] private Image _lightFill;

    // Update is called once per frame
    void Update()
    {
        UpdateNarrativeTime();
        UpdateRotationGauge();
        UpdateVitals();
        UpdateFlashlightBattery();
    }

    private void UpdateNarrativeTime() // Updates the indicator of narratively passed time
    {
        _narrativeTime.sprite = _narrativeTimeImages[GameManager.Instance.SceneData.NarrativeTimestamp];
        _narrativeTimeText.text = _narrativeTimeReadouts[GameManager.Instance.SceneData.NarrativeTimestamp];
    }

    private void UpdateRotationGauge()
    {
        float yPos = 0;
        if (_mainHUD.CameraRotTransform.localRotation.eulerAngles.x < 270) // downward angle
        {
            yPos = math.remap(90, 0, -60, 0, _mainHUD.CameraRotTransform.localRotation.eulerAngles.x);
            double angle = System.Math.Round((-1 * (_mainHUD.CameraRotTransform.localRotation.eulerAngles.x)), 1);
            _verticalRotationText.text = angle.ToString(".0");
        }
        else // upward angle
        {
            yPos = math.remap(360, 270, 0, 60, _mainHUD.CameraRotTransform.localRotation.eulerAngles.x);
            double angle = System.Math.Round((90 - (_mainHUD.CameraRotTransform.localRotation.eulerAngles.x - 270f)), 1);
            _verticalRotationText.text = angle.ToString(".0");
        }

        Vector3 gaugePos = new Vector3(0, yPos, 0);
        _verticalRotationIndicator.transform.localPosition = gaugePos;
    }

    private void UpdateVitals()
    {
        _healthFill.fillAmount = (float)GameManager.Instance.SceneData.RemainingLives / GameManager.MAX_LIVES;
        float healthR = math.remap(GameManager.MAX_LIVES, 0, .7f, 0, GameManager.Instance.SceneData.RemainingLives);
        _healthFill.color = new Color(.7f, healthR, healthR);

        if (_healthFill.fillAmount > .9f) // 1 
        {
            _healthReadout.text = ">vitals normal";
        }
        else if (_healthFill.fillAmount > .8f) // Below 90%
        {
            _healthReadout.text = ">vitals normal\n>minor leisons on left arm";
        }
        else if (_healthFill.fillAmount > .7f) // Below 80%
        {
            _healthReadout.text = ">vitals normal\n>minor leisons across upper body";
        }
        else if (_healthFill.fillAmount > .6f) // Below 70%
        {
            _healthReadout.text = ">vitals normal\n>minor leisons\n>bruising across ribs 8-12";
        }
        else if (_healthFill.fillAmount > .5f) // Below 60%
        {
            _healthReadout.text = ">vitals normal\n>unhealed leisons\n>bruising across ribcage";
        }
        else if (_healthFill.fillAmount > .4f) // Below 50%
        {
            _healthReadout.text = ">vitals irregular\n>brusing across body\n>blood viscocity abnormal" +
                "\n>seek medical assistance";
        }
        else if (_healthFill.fillAmount > .3f) // Below 40%
        {
            _healthReadout.text = ">vitals irregular\n>degraded muscle mass\n>blood viscocity highly abnormal" +
                "\n>seek medical assistance";
        }
        else if (_healthFill.fillAmount > .2f) // Below 30%
        {
            _healthReadout.text = ">vitals irregular\n>blood too thin to congeal\n>chromosonal irregularities detected" +
                "\n>seek medical assistance";
        }
        else if (_healthFill.fillAmount > .1f) // Below 20%
        {
            _healthReadout.text = ">vitals irregular\n>brain activity irregular\n>chromosonal mutation ongoing" +
                "\n>seek medical assistance";
        }
        else // Below 10%
        {
            _healthReadout.text = ">vitals critical\n>damage to frontal lobe\n>genetic basis corrupted" +
                "\n>urgently seek medical assistance";
        }
    }

    private void UpdateFlashlightBattery()
    {
        _lightFill.fillAmount = GameManager.FlashlightCharge;
    }
}
