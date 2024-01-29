using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HealthIcon : MonoBehaviour
{
	bool _isFull;
	Image _image;
	[SerializeField] List<Sprite> _HPFull;
	[SerializeField] List<Sprite> _HPEmpty;
	[SerializeField] float _timeBetweenFrames;
	private float _timer;
	private List<Sprite> _currentAnim;
	private int _indexSprite;

	private void Awake()
	{
		_isFull = true;
		_timer = 0;
		_image = GetComponent<Image>();
	}

	private void Start()
	{
		ChangeStatus();
	}

	private void Update()
	{
		if (_timer > _timeBetweenFrames)
		{
			_timer = 0;

			if (_indexSprite >= _currentAnim.Count)
			{
				_indexSprite = 0;
			}

			_image.sprite = _currentAnim[_indexSprite];
			_indexSprite++;
		}
		else
		{
			_timer += Time.deltaTime;
		}
	}

	private void ChangeStatus()
	{
		if (_image == null) return;
		
		_currentAnim = _isFull ? _HPFull : _HPEmpty;
		_indexSprite = 0;
	}

	public bool IsFull
	{
		get => _isFull;
		set
		{
			if (_isFull == value) return;
			_isFull = value;
			ChangeStatus();
		}
	}
}
