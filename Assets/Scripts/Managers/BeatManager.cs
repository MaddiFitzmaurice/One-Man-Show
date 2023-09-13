using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // TEST
        IEnumerator coroutine = BroadcastBeat();
        StartCoroutine(BroadcastBeat());
    }

    // TEST
    IEnumerator BroadcastBeat()
    {
        // TODO: Put song length in meta
        float totalBeats = 53f * Conductor.currentBPS;
        Debug.Log(totalBeats);

        // Broadcast until song finishes
        while (Conductor.songBeat < totalBeats)
        {
            Debug.Log(Conductor.songBeat);
            // Wait for the equivalent of a half-beat in seconds before broadcasting again
            yield return new WaitForSeconds(0.375f);
        }
    }
}
