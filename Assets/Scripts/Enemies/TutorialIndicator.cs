using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialIndicator : MonoBehaviour
{
    [SerializeField] private Gradient _guideTint; // what to tint the enemy as based on their relative position in the timing window
    private SpriteRenderer _sprite;
    private float _earlyWindow;
    private float _lateWindow;

    // Start is called before the first frame update
    void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    // set the early/late windows
    public void SetTimings(float early, float late)
	{
        _earlyWindow = early;
        _lateWindow = late;
	}

    // Update is called once per frame
    void Update()
    {
        _sprite.color = _guideTint.Evaluate(Mathf.InverseLerp(_earlyWindow, _lateWindow, Conductor.SongBeat));
    }
}
