using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Stores the prefab and its associated parent
[System.Serializable]
public struct EnemyPrefabType
{
    public GameObject prefab;
    public Transform parent;
}

public class EnemyManager : MonoBehaviour
{
    // Enemy prefabs to pool
    [SerializeField] List<EnemyPrefabType> _enemyPrefabs;

    // Maximum number of enemies per pool
    [SerializeField] uint _maxNumToPool;

    // Enemy pool lists for all prefabs
    List<List<GameObject>> _typeEnemyList;
    
    // List of spawn locations
    public Vector3 leftStartPosition;
    public Vector3 leftEndPosition;
    public Vector3 forwardStartPosition;
    public Vector3 forwardEndPosition;
    public Vector3 rightStartPosition;
    public Vector3 rightEndPosition;

    // Track current beat number
    private float _currentBeat;

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.BEAT, BeatHandler);
        EventManager.EventSubscribe(EventType.SPAWN, SpawnHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.BEAT, BeatHandler);
        EventManager.EventUnsubscribe(EventType.SPAWN, SpawnHandler);
    }

    private void Start()
    {
        _typeEnemyList = new List<List<GameObject>>();
        InitEnemyPools();
    }

    // Create enemy pools
    void InitEnemyPools()
    {
        foreach (EnemyPrefabType ept in _enemyPrefabs)
        {
            _typeEnemyList.Add(ObjectPooler.CreateObjectPool(_maxNumToPool, ept.prefab, ept.parent));
        }
    }

    // Update current beat
    public void BeatHandler(object data)
    {
        _currentBeat = (float)data;
    }
    
    // Spawns an enemy from a type index and direction
    public void SpawnHandler(object data)
    {
        SpawnData spawnData = (SpawnData)data;

        if (spawnData == null)
        {
            Debug.Log($"Attempted to spawn null data!");
            return;
        }

        // Invalid type ID
        if ((int)spawnData.Type < 0 || (int)spawnData.Type >= _typeEnemyList.Count)
        {
            Debug.Log($"Attempted to spawn enemy with invalid ID! ({spawnData.Type})");
            return;
        }

        GameObject newEnemy = ObjectPooler.GetPooledObject(_typeEnemyList[(int)spawnData.Type]);

        // No more enemies in pool
        if (newEnemy == null)
        {
            Debug.Log($"Attempted to spawn enemy but pool was empty! ({spawnData.Type})");
            return;
        }

        Vector3 start = Vector3.zero;
        Vector3 end = Vector3.zero;

        switch (spawnData.Direction)
        {
            case StageDirection.LEFT:
                start = leftStartPosition;
                end = leftEndPosition;
                break;
            case StageDirection.FORWARD:
                start = forwardStartPosition;
                end = forwardEndPosition;
                break;
            case StageDirection.RIGHT:
                start = rightStartPosition;
                end = rightEndPosition;
                break;
        }

        newEnemy.GetComponent<Enemy>().Initialise(spawnData.Direction, spawnData.Beat, start, end);
        newEnemy.SetActive(true);

        Debug.Log($"Spawned enemy {spawnData.Type} on beat {spawnData.Beat} from {spawnData.Direction}");
    }
}
