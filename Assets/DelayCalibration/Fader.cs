using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
	private float _start_time;

	public float lifetime = 1.0f;
	public AnimationCurve transparency;

	public Graphic image;

	public bool destroyOnEnd = true;

	private void OnEnable()
	{
		_start_time = Time.time;
	}

	void Update()
	{
		if(Time.time - _start_time > lifetime)
		{
			if (destroyOnEnd)
			{
				Destroy(gameObject);
			}

			enabled = false;
			return;
		}

		Color c = image.color;
		c.a = transparency.Evaluate((Time.time - _start_time) / lifetime);
		image.color = c;
	}
}
