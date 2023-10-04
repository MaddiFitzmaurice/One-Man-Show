using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TitleLighting : MonoBehaviour
{
    [SerializeField] private AnimationCurve _openinglightAnim;
    [SerializeField] private float _openingLightAnimTiming;

    // Components
    private Light2D _light;
    private Animation _anim;

    private void Awake()
    {
        _light = GetComponent<Light2D>();
        _anim = GetComponent<Animation>();
    }

    private void Start()
    {
        StartCoroutine(OpeningLightAnim());         
    }

    IEnumerator OpeningLightAnim()
    {
        float time = 0;
        while (time < _openingLightAnimTiming)
        {
            _light.intensity = _openinglightAnim.Evaluate(time / _openingLightAnimTiming);
            time += Time.deltaTime;
            yield return null;
        }

        _anim.Play();
    }
}
