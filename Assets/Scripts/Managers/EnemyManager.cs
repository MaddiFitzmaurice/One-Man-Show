using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] List<EnemySpawner> _enemySpawns;

    // Enemy prefab to pool
    [SerializeField] GameObject _enemyPrefab;

    // Maximum number of enemies per pool
    [SerializeField] int _maxNumToPool;

    // Parents that enemies are parented to so the hierarchy is clean and organised
    [SerializeField] Transform _leftEnemyParent;
    [SerializeField] Transform _rightEnemyParent;
    [SerializeField] Transform _centreEnemyParent;

    // Enemy pool lists
    List<GameObject> _leftEnemyList;
    List<GameObject> _rightEnemyList;
    List<GameObject> _centreEnemyList;

    private void Start()
    {
        InitEnemyPools();
    }

    // Create enemy pools
    void InitEnemyPools()
    {
        _leftEnemyList = ObjectPooler.CreateObjectPool(_maxNumToPool, _enemyPrefab, _leftEnemyParent);
        _rightEnemyList = ObjectPooler.CreateObjectPool(_maxNumToPool, _enemyPrefab, _rightEnemyParent);
        _centreEnemyList = ObjectPooler.CreateObjectPool(_maxNumToPool, _enemyPrefab, _centreEnemyParent);
    }
}
