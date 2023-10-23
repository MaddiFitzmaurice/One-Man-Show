using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class Fader : MonoBehaviour
{
	private float _start_time;

	private CanvasGroup _targetGroup;

	[Min(0)]
	public float delay = 0.0f;
	[Min(0)]
	public float lifetime = 1.0f;

	public AnimationCurve transparency;

	public bool destroyObject = false;
	public bool destroyScript = true;
	public bool destroyGroup = false;

	private void Awake()
	{
		_targetGroup = GetComponent<CanvasGroup>();
	}

	private void OnEnable()
	{
		_start_time = Time.time + delay;

		_targetGroup.alpha = transparency.Evaluate(0);
	}

	void Update()
	{
		if (Time.time <= _start_time) return;

		if (Time.time < _start_time + lifetime)
		{
			_targetGroup.alpha = transparency.Evaluate((Time.time - _start_time) / lifetime);
			return;
		}

		if (destroyObject)
		{
			_targetGroup = null;
			Destroy(gameObject);
			return;
		}

		_targetGroup.alpha = transparency.Evaluate(1);

		if (destroyScript)
		{
			Destroy(this);
			if (destroyGroup && _targetGroup != null)
			{
				Destroy(_targetGroup);
				_targetGroup = null;
			}
		}

		enabled = false;
	}
}
