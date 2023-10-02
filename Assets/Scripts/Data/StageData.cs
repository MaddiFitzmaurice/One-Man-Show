using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageData
{
    [SerializeField] private float _length; // How many beats this stage lasts for
    [SerializeField] private SpawnBeat[] _spawns; // What to spawn during this stage

    public float Length { get => _length; }
    public SpawnBeat[] Spawns { get => _spawns; }
}
