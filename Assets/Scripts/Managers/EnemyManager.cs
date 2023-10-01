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
    [SerializeField] List<Transform> _spawnLocations;

    //Player Pos Cache
    [SerializeField] private Player _player;

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

    // Handler to spawn enemies according to LevelManager's SpawnData
    public void SpawnHandler(object data)
    {
        SpawnData spawnData = (SpawnData)data;

        GameObject newEnemy = ObjectPooler.GetPooledObject(_typeEnemyList[(int)spawnData.Type]);
        newEnemy.transform.position = _spawnLocations[(int)spawnData.Direction].position;

        // move a portion of your total distance from spawn to enemy (for now)
        Vector3 dist = newEnemy.transform.position - _player.transform.position;
        dist = dist * 0.25f;
        newEnemy.GetComponent<Enemy>().Initialise(spawnData.Direction, spawnData.Beat, newEnemy.transform.position, _player.transform.position + dist);
    }
}
