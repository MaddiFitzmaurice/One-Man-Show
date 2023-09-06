using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] List<EnemySpawner> _enemySpawns;

    // Enemy prefabs to pool
    [SerializeField] GameObject _type1EnemyPrefab;
    [SerializeField] GameObject _type2EnemyPrefab;
    [SerializeField] GameObject _type3EnemyPrefab;

    // Maximum number of enemies per pool
    [SerializeField] int _maxNumToPool;

    // Parents that enemies are parented to so the hierarchy is clean and organised
    [SerializeField] Transform _type1EnemyParent;
    [SerializeField] Transform _type2EnemyParent;
    [SerializeField] Transform _type3EnemyParent;

    // Enemy pool lists
    List<GameObject> _type1EnemyList;
    List<GameObject> _type2EnemyList;
    List<GameObject> _type3EnemyList;

    private void Start()
    {
        InitEnemyPools();
    }

    // Create enemy pools
    void InitEnemyPools()
    {
        _type1EnemyList = ObjectPooler.CreateObjectPool(_maxNumToPool, _type1EnemyPrefab, _type1EnemyParent);
        _type2EnemyList = ObjectPooler.CreateObjectPool(_maxNumToPool, _type2EnemyPrefab, _type2EnemyParent);
        _type3EnemyList = ObjectPooler.CreateObjectPool(_maxNumToPool, _type3EnemyPrefab, _type3EnemyParent);
    }
}
