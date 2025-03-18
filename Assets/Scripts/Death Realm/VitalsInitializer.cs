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
    public void Awake()
    {
        // functionally decrement health
        GameManager.Instance.SceneData.RemainingLives--;

        // fill amount
        _healthFill.fillAmount = (float)GameManager.Instance.SceneData.RemainingLives / GameManager.MAX_LIVES;

        // color
        float healthR = math.remap(GameManager.MAX_LIVES, 0.7f, 1f, 0f, GameManager.Instance.SceneData.RemainingLives);
        _healthFill.color = new Color(0.7f, healthR, healthR);

        // update health text
        if (healthR > 0.66f)
            _healthText.text = ">vitals\nregular";
        else if (healthR > 0.33f)
            _healthText.text = "vitals\nmoderate";
        else
            _healthText.text = "vitals\ncritical";
    }
}
