using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
	private void Start()
	{
		Conductor.instance.StartTracking(120,0);
	}

	// Update is called once per frame
	void Update()
    {
        Debug.Log(Conductor.instance.beatNum);
    }
}
