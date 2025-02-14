using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileNode : MonoBehaviour
{
    // The script that goes on each file node / the file node prefab; pretty much just used to control sprites and animations of the object.

    [SerializeField] private List<Sprite> _typeSprites; // The images representing log types

    private Animator _anim;
    private Image _img;

    void Start()
    {
       _anim = GetComponent<Animator>(); 
        _img = GetComponentInChildren<Image>();
    }

    void Update()
    {
        
    }

    public void SetType(Log.LogType type)
    {
        if(_img == null)
            _img = GetComponentInChildren<Image>();
        if (type == Log.LogType.Text)
            _img.sprite = _typeSprites[0];
        if (type == Log.LogType.Audio)
            _img.sprite = _typeSprites[1];
        if (type == Log.LogType.Image)
            _img.sprite = _typeSprites[2];
    }

    public void SetSmall()
    {
        _anim = GetComponent<Animator>();
        _anim.Play("StaticSmall");
    }

    public void Shrink() { _anim.Play("Shrink"); }
    public void Grow() { _anim.Play("Grow"); }
}
