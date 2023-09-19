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
    //[SerializeField] List<EnemySpawner> _enemySpawns;

    // Enemy prefabs to pool
    [SerializeField] List<EnemyPrefabType> _enemyPrefabs;

    // Maximum number of enemies per pool
    [SerializeField] uint _maxNumToPool;

    // Enemy pool lists for all prefabs
    List<List<GameObject>> _typeEnemyList;

    // List of spawn locations
    [SerializeField] List<Transform> _spawnLocations; 

    private bool _debugTestSpawn = true;

    private float _currentBeat;

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

	// TESTING ONLY: DELETE WHEN DONE
	private void Update()
	{
        if (_debugTestSpawn)
        {
            TestSpawn();
            _debugTestSpawn = false;
        }
    }

	// spawn an enemy, just to test that it works
	public void TestSpawn()
    {
        GameObject newEnemy = ObjectPooler.GetPooledObject(_typeEnemyList[0]);
        newEnemy.GetComponent<Enemy>().Initialise(StageDirection.LEFT, 10);
        
        // Assign spawn pos then activate
        newEnemy.transform.position = _spawnLocations[(int)StageDirection.LEFT].position;
        newEnemy.SetActive(true);
    }

    // Data will come from the TrackManager, and should contain Stage Direction and Enemy Type
    // Handler to spawn enemies according to TrackManager
    public void SpawnHandler(object data)
    {
        // Enemy type will come from data 
        GameObject newEnemy = ObjectPooler.GetPooledObject(_typeEnemyList[0]);
        newEnemy.GetComponent<Enemy>().Initialise(StageDirection.LEFT, Conductor.RawSongBeat);

        // StageDirection will come from data
        // Assign spawn pos then activate
        newEnemy.transform.position = _spawnLocations[(int)StageDirection.LEFT].position;
        newEnemy.SetActive(true);
    }
}
