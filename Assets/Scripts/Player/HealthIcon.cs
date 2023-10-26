using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class HealthIcon : MonoBehaviour
{
    private bool _isFull = true;
    private Animator _anim;

    private void Awake()
	{
		_isFull = true;
        _anim = GetComponent<Animator>();
	}

	private void SetAnimation()
	{
		if (_anim == null) return;
		_anim.SetTrigger(_isFull ? "activate" : "deactivate");
		Debug.Log($"Set health trigger '{(_isFull ? "activate" : "deactivate")}'");
	}

	public bool IsFull
	{
		get => _isFull;
		set
		{
			if (_isFull == value) return;
			_isFull = value;
            SetAnimation();
		}
	}
}
