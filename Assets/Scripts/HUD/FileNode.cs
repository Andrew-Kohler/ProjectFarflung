using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileNode : MonoBehaviour
{
    // The script that goes on each file node / the file node prefab; pretty much just used to control sprites and animations of the object.

    [SerializeField] private List<Sprite> _typeSprites; // The images representing log types
    [SerializeField] private GameObject _unreadIndicator;
    [SerializeField] private Image _typeImg;

    private Animator _anim;

    void Start()
    {
       _anim = GetComponent<Animator>(); 
    }

    void Update()
    {
        
    }

    public void SetType(Log.LogType type)
    {
        if (type == Log.LogType.Text) 
            _typeImg.sprite = _typeSprites[0];
        if (type == Log.LogType.Audio)
            _typeImg.sprite = _typeSprites[1];
        if (type == Log.LogType.Image)
            _typeImg.sprite = _typeSprites[2];

        //_unreadIndicator.GetComponent<Image>().sprite = _typeImg.sprite;
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
