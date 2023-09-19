using UnityEngine;

[System.Serializable]
public struct SpawnData
{
    [SerializeField] private StageDirection _direction;
    [SerializeField] private EnemyType _type;

    public StageDirection Direction { get => _direction; }
    public EnemyType Type { get => _type; }

    public SpawnData(StageDirection dir, EnemyType type)
    {
        _direction = dir;
        _type = type;
    }
}
