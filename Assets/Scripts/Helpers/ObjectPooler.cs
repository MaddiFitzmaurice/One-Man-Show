using System.Collections.Generic;
using UnityEngine;

// Helper class for object pooling
public static class ObjectPooler
{
    // Create object pool using a prefab and amount to pool
    public static List<GameObject> CreateObjectPool(uint amountToPool, GameObject objectToPool)
    {
        List<GameObject> pooledObjects = new List<GameObject>();

        for (uint i = 0; i < amountToPool; i++)
        {
            GameObject obj = GameObject.Instantiate(objectToPool);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }

        return pooledObjects;
    }

    // Create object pool using a prefab and amount to pool under a given parent
    public static List<GameObject> CreateObjectPool(uint amountToPool, GameObject objectToPool, Transform parent)
    {
        List<GameObject> pooledObjects = new List<GameObject>();

        for (uint i = 0; i < amountToPool; i++)
        {
            GameObject obj = GameObject.Instantiate(objectToPool, parent);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }

        return pooledObjects;
    }

    //Retrieve a pooled object if it is not active in the hierarchy
    public static GameObject GetPooledObject(this List<GameObject> pooledObjects)
    {
        foreach (GameObject obj in pooledObjects)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        return null;
    }

    // Return objects to pool
    public static void ReturnObjectsToPool(this List<GameObject> pooledObjects)
    {
        foreach (GameObject obj in pooledObjects)
        {
            if (obj.activeInHierarchy)
            {
                obj.SetActive(false);
            }
        }
    }
}
