using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StageData
{
    [SerializeField] private float _length; // How many beats this stage lasts for
    [SerializeField] private List<SpawnData> _spawns; // What to spawn during this stage

    public float Length { get => _length; }
    public List<SpawnData> Spawns { get => _spawns; }
}
