using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// The script that goes on each file node / the file node prefab; pretty much just used to control sprites and animations of the object.
/// </summary>
public class FileNode : MonoBehaviour
{

    [SerializeField, Tooltip("The images representing log types")]
    private List<Sprite> _typeSprites; 
    [SerializeField, Tooltip("GameObject for the unread indicator - for toggling on and off")] 
    private GameObject _unreadIndicator;
    [SerializeField, Tooltip("Image component of the prefab that displays the type image")] 
    private Image _typeImg;

    private Animator _anim; // Animator shrinks or grows the display icon

    void Start()
    {
       _anim = GetComponent<Animator>(); 
    }

    void Update()
    {
        
    }

    /// <summary>
    /// Sets the type image of the node.
    /// </summary>
    public void SetType(Log.LogType type)
    {
        if (type == Log.LogType.Text) 
            _typeImg.sprite = _typeSprites[0];
        if (type == Log.LogType.Audio)
            _typeImg.sprite = _typeSprites[1];
        if (type == Log.LogType.Image)
            _typeImg.sprite = _typeSprites[2];
    }

    public void SetRead(bool read)
    {
        _unreadIndicator.SetActive(!read);
    }

    public void SetSmall()
    {
        _anim = GetComponent<Animator>();
        _anim.Play("StaticSmall");
    }

    public void Shrink() { _anim.Play("Shrink"); }
    public void Grow() { _anim.Play("Grow"); }
}
