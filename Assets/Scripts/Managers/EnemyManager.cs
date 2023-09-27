using System.Collections;
using System.Collections.Generic;
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

    [SerializeField]
    private bool _debugTestSpawn = true;
    
    // List of spawn locations
    public Vector3 leftStartPosition;
    public Vector3 leftEndPosition;
    public Vector3 forwardStartPosition;
    public Vector3 forwardEndPosition;
    public Vector3 rightStartPosition;
    public Vector3 rightEndPosition;

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
        Spawn(0, StageDirection.LEFT, 4);
    }
    
    // Spawns an enemy from a type index and direction
    public GameObject Spawn(int typeIndex, StageDirection direction, float startBeat)
    {
        // Invalid type ID
        if (typeIndex < 0 || typeIndex >= _typeEnemyList.Count)
        {
            Debug.Log($"Attempted to spawn enemy with invalid ID! ({typeIndex})");
            return null;
        }

        GameObject newEnemy = ObjectPooler.GetPooledObject(_typeEnemyList[typeIndex]);

        // No more enemies in pool
        if (newEnemy == null)
        {
            Debug.Log($"Attempted to spawn enemy but pool was empty! ({typeIndex})");
            return null;
        }

        Vector3 start = Vector3.zero;
        Vector3 end = Vector3.zero;

        switch (direction)
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

        newEnemy.GetComponent<Enemy>().Initialise(direction, startBeat, start, end);
        newEnemy.SetActive(true);

        Debug.Log($"Spawned enemy {typeIndex} on beat {startBeat} from {direction}");
        return newEnemy;
    }
}
