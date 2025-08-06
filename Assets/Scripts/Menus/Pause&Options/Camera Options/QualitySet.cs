using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QualitySet : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown qualityDropdown;

    private void Awake() {
        // When opening camera tab, sets drop down to match the unity default quality level
        if (qualityDropdown != null)
        {
            qualityDropdown.value = QualitySettings.GetQualityLevel();
        }
    }

    public void SetQualityDropdown(int index)
    {

        // Read value / remap
        int newQuality = qualityDropdown.value;

        // Update game manager
        GameManager.Instance.OptionsData.Brightness = newQuality;

        // Actually switch quality
        QualitySettings.SetQualityLevel(index, false);
        Debug.Log("Switched to quality level: " + QualitySettings.names[index]);

       
    }

}
