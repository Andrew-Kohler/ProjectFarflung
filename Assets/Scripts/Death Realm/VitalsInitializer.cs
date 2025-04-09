using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
using TMPro;

/// <summary>
/// Handles functional decrement and initialization of vitals indicator within the Death Realm.
/// </summary>
public class VitalsInitializer : MonoBehaviour
{
    // Health
    [SerializeField] private Image _healthFill;
    [SerializeField, Tooltip("Used to update vitals health text.")]
    private TextMeshProUGUI _healthText;

    /// <summary>
    /// Called during respawn process, handling the functional update of health and update of the display
    /// </summary>
    public void Start()
    {
        // fill amount
        _healthFill.fillAmount = (float)GameManager.Instance.SceneData.RemainingLives / GameManager.MAX_LIVES;

        // color
        float healthR = math.remap(GameManager.MAX_LIVES, 0.7f, 1f, 0f, GameManager.Instance.SceneData.RemainingLives);
        _healthFill.color = new Color(0.7f, healthR, healthR);

        // update health text
        if (_healthFill.fillAmount > .9f) // 1 
        {
            _healthText.text = ">vitals normal";
        }
        else if (_healthFill.fillAmount > .8f) // Below 90%
        {
            _healthText.text = ">vitals normal\n>minor leisons on left arm";
        }
        else if (_healthFill.fillAmount > .7f) // Below 80%
        {
            _healthText.text = ">vitals normal\n>minor leisons across upper body";
        }
        else if (_healthFill.fillAmount > .6f) // Below 70%
        {
            _healthText.text = ">vitals normal\n>minor leisons\n>bruising across ribs 8-12";
        }
        else if (_healthFill.fillAmount > .5f) // Below 60%
        {
            _healthText.text = ">vitals normal\n>unhealed leisons\n>bruising across ribcage";
        }
        else if (_healthFill.fillAmount > .4f) // Below 50%
        {
            _healthText.text = ">vitals irregular\n>brusing across body\n>blood viscocity abnormal" +
                "\n>seek medical assistance";
        }
        else if (_healthFill.fillAmount > .3f) // Below 40%
        {
            _healthText.text = ">vitals irregular\n>degraded muscle mass\n>blood viscocity highly abnormal" +
                "\n>seek medical assistance";
        }
        else if (_healthFill.fillAmount > .2f) // Below 30%
        {
            _healthText.text = ">vitals irregular\n>blood too thin to congeal\n>chromosonal irregularities detected" +
                "\n>seek medical assistance";
        }
        else if (_healthFill.fillAmount > .1f) // Below 20%
        {
            _healthText.text = ">vitals irregular\n>brain activity irregular\n>chromosonal mutation ongoing" +
                "\n>seek medical assistance";
        }
        else // Below 10%
        {
            _healthText.text = ">vitals critical\n>damage to frontal lobe\n>genetic basis corrupted" +
                "\n>urgently seek medical assistance";
        }
    }
}
