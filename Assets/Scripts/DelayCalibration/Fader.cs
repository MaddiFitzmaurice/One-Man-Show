using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
	private float _start_time;

	public Graphic targetGraphic;

	[Min(0)]
	public float delay = 0.0f;
	[Min(0)]
	public float lifetime = 1.0f;
	public bool destroyScript = true;
	public bool destroyGraphic = true;
	public bool destroyObject = false;

	public AnimationCurve transparency;

	private void OnEnable()
	{
		_start_time = Time.time + delay;

		Color c = targetGraphic.color;
		c.a = transparency.Evaluate(0);
		targetGraphic.color = c;
	}

	void Update()
	{
		if (Time.time <= _start_time) return;

		if (Time.time - _start_time > lifetime)
		{
			if (destroyGraphic)
			{
				Destroy(targetGraphic);
			}
			if (destroyScript)
			{
				Destroy(this);
			}
			if(destroyObject)
			{
				Destroy(gameObject);
			}

			enabled = false;
		}
		else
		{
			Color c = targetGraphic.color;
			c.a = transparency.Evaluate((Time.time - _start_time) / lifetime);
			targetGraphic.color = c;
		}
	}
}
