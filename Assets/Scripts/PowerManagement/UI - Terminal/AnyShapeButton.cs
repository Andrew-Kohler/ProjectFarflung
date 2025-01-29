using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Permits a UI button to be the shape of the image alpha contents instead of the bounds of the image dimensions.
/// </summary>
public class AnyShapeButton : MonoBehaviour
{
    [SerializeField, Tooltip("Used to set the alpha hit test minimum threshold.")]
    private Image _img;

    // Start is called before the first frame update
    void Start()
    {
        _img.alphaHitTestMinimumThreshold = 0.5f;
    }
}
