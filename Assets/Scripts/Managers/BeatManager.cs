using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // TEST
        StartCoroutine(BroadcastBeat());
    }

    // TEST
    IEnumerator BroadcastBeat()
    {
        // TODO: Put song length in meta
        float totalBeats = 53f * Conductor.currentBPS;
        Debug.Log(totalBeats);

        float currentTime = Conductor.songTime;

        // Broadcast until song finishes
        while (Conductor.songBeat < totalBeats)
        {
            // Wait for the equivalent of a half-beat in seconds passing before broadcasting again
            if (Conductor.songTime > currentTime + 0.375)
            {
                //Broadcast beat and reset current time
                Debug.Log(Conductor.songBeat);
                currentTime = Conductor.songTime;
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
