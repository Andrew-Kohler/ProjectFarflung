using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureScroll : MonoBehaviour
{
    [SerializeField] private GameObject _obj;
    private Renderer _renderer;

    [SerializeField] private float scrollSpeed = 0.5f;

    void Start()
    {
        _renderer = _obj.GetComponent<Renderer>();
    }

    void Update()
    {
        Vector2 textureOffset = new Vector2(0, Time.time * scrollSpeed);
        _renderer.material.mainTextureOffset = textureOffset;
    }
}
