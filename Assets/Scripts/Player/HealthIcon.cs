using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
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

	private void SetImage()
	{
		if (_image == null) return;
		_image.color = _isFull ? Color.red : Color.gray;
	}

	public bool IsFull
	{
		get => _isFull;
		set
		{
			if (_isFull == value) return;
			_isFull = value;
			SetImage();
		}
	}
}
