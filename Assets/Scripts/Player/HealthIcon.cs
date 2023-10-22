using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HealthIcon : MonoBehaviour
{
	bool _isFull;
	Image _image;
	[SerializeField] Sprite _HPFull;
	[SerializeField] Sprite _HPEmpty;

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
		_image.sprite = _isFull ? _HPFull : _HPEmpty;
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
