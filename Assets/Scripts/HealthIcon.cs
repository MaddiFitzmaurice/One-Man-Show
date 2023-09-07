using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthIcon : MonoBehaviour
{
    bool _active;
    Image _image; 

    private void Awake()
    {
        _active = true;
        _image = GetComponent<Image>();
    }

    private void Start()
    {
        SetImage();
    }

    public void SetImage()
    {
        _image.color = _active == true ? Color.red : Color.gray;
    }

    public void SetActive(bool flag)
    {
        _active = flag;
        SetImage();
    }

    public bool GetActive()
    {
        return _active;
    }
}
