using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileNode : MonoBehaviour
{
    // The script that goes on each file node / the file node prefab; pretty much just used to control sprites and animations of the object.

    public int masterIndex; // The index of this log IN THE MASTER LIST (not in the HUD list that's only some logs)

    private Animator _anim;
    void Start()
    {
       _anim = GetComponent<Animator>(); 
    }

    void Update()
    {
        
    }

    public void SetSmall()
    {
        _anim = GetComponent<Animator>();
        _anim.Play("StaticSmall");
    }

    public void Shrink() { _anim.Play("Shrink"); }
    public void Grow() { _anim.Play("Grow"); }
}
