using UnityEngine;

[System.Serializable]
public struct SpawnData
{
    [SerializeField] private float _beat;
    [SerializeField] private StageDirection _direction;
    [SerializeField] private EnemyType _type;

    public float Beat { get => _beat; set { _beat = value; } }
    public StageDirection Direction { get => _direction; }
    public EnemyType Type { get => _type; }

    public SpawnData(float beat, StageDirection dir, EnemyType type)
    {
        _beat = beat;
        _direction = dir;
        _type = type;
    }
}
