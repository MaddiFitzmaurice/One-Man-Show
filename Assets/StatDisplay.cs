using TMPro;
using UnityEngine;

public class StatDisplay : MonoBehaviour
{
	public TextMeshProUGUI damageDisplay;
	public TextMeshProUGUI timingDisplay;
	public TextMeshProUGUI missDisplay;
	public TextMeshProUGUI hitDisplay;

	public string damageFormat;
	public string timingFormat;
	public string missFormat;
	public string hitFormat;

	void Start()
	{
		damageDisplay.text = string.Format(damageFormat, StatManager.damage_taken                );
		timingDisplay.text = string.Format(timingFormat, StatManager.avg_time_inaccuracy * 1000.0);
		missDisplay.text   = string.Format(missFormat,   StatManager.misses                      );
		hitDisplay.text    = string.Format(hitFormat,    StatManager.hits                        );

		enabled = false;
	}
}
