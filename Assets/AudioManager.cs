using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    void Awake()
    {
        // Check if an AudioManager instance already exists.
        if (instance != null && instance != this)
        {
            // If an instance exists, destroy this GameObject to prevent duplicates.
            Destroy(gameObject);
            return;
        }

        // If no instance exists, set this as the instance and don't destroy it.
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Add the AudioSource and audio settings here.
}

