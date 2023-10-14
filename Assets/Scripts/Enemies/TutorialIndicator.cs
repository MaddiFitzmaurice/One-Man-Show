using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialIndicator : MonoBehaviour
{
    [SerializeField] private Gradient _guideTint; // what to tint the enemy as based on their relative position in the timing window
    private SpriteRenderer _sprite;
    [SerializeField] private GameObject _marker; // the marker that moves along the bar
    [SerializeField] private float _offset; // how far in units the bar extends in either direction
    private float _earlyWindow;
    private float _lateWindow;

    // Start is called before the first frame update
    void Awake()
    {
        _sprite = _marker.GetComponent<SpriteRenderer>();
    }

    // set the early/late windows
    public void SetTimings(float early, float late)
	{
        _earlyWindow = early;
        _lateWindow = late;
        _marker.SetActive(false); // hide to begin with
    }

    // Update is called once per frame
    void Update()
    {
        float progress = Mathf.InverseLerp(_earlyWindow, _lateWindow, Conductor.SongBeat);

        // if hidden, but progress has started, show the marker
        if (progress > 0 && !_marker.activeSelf)
		{
            _marker.SetActive(true);
		}

        float x_offset = Mathf.Lerp(-_offset, _offset, progress);
        _marker.transform.localPosition = new Vector3(0 + x_offset, 0, 0);
    }
}
