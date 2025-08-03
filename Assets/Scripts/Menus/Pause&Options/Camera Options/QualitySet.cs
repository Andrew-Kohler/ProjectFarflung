using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QualitySet : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown qualityDropdown;

    private void Start() {
        if (qualityDropdown != null)
        {
            qualityDropdown.value = QualitySettings.GetQualityLevel();
        }
    }

    public void SetQualityDropdown(int index)
    {
        QualitySettings.SetQualityLevel(index, false);
        Debug.Log("Switched to quality level: " + QualitySettings.names[index]);
    }

}
