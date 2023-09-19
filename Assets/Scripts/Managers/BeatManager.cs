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
        // TODO: Get song length from clip
        double totalBeats = 53f * Conductor.CurrentBPS;
        Debug.Log(totalBeats);

		double currentTime = Conductor.SongTime;

        // Broadcast until song finishes
        while (Conductor.SongBeat < totalBeats)
        {
            // Wait for the equivalent of a half-beat in seconds passing before broadcasting again
            if (Conductor.SongTime >= currentTime + 0.375)
            {
                //Broadcast beat and reset current time
                Debug.Log(Conductor.SongBeat);
                currentTime = Conductor.SongTime;
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
