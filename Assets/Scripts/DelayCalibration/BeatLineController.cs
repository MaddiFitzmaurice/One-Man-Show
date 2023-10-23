using UnityEngine;
using UnityEngine.UI;

public class BeatLineController : MonoBehaviour
{
	public Vector2 minAnchor;
	public Vector2 maxAnchor;

	public RectTransform lineTransform;

	public GameObject beatPrefab;
	public Transform beatParent;

	public Gradient beatGradient;

	private void OnDisable()
	{
		lineTransform.gameObject.SetActive(false);
	}

	private void OnEnable()
	{
		lineTransform.gameObject.SetActive(true);
	}

	void Update()
	{
		float t = Conductor.RawSongBeat - Conductor.RawNearestBeat + 0.5f;

		lineTransform.anchorMin = lineTransform.anchorMax = Vector2.Lerp(minAnchor, maxAnchor, t);

		if (Input.GetKeyDown(KeyCode.Space))
		{
			GameObject line = Instantiate(beatPrefab, beatParent);

			RectTransform rt = line.transform as RectTransform;
			rt.anchorMin = lineTransform.anchorMin;
			rt.anchorMax = lineTransform.anchorMax;

			Graphic graphic;

			if (!line.TryGetComponent(out graphic)) return;

			graphic.color = beatGradient.Evaluate(t);
		}
	}
}
