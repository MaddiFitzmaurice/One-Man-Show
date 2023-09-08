using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthIcon : MonoBehaviour
{
    bool _isFull;
    Image _image; 

    private void Awake()
    {
        _isFull = true;
        _image = GetComponent<Image>();
    }

    private void Start()
    {
        SetImage();
    }

    public void SetImage()
    {
        _image.color = _isFull == true ? Color.red : Color.gray;
    }

    public void SetIsFull(bool flag)
    {
        _isFull = flag;
        SetImage();
    }

    public bool GetIsFull()
    {
        return _isFull;
    }
}
