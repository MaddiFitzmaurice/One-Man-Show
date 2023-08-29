using System.Collections.Generic;
using UnityEngine;

// Helper class for object pooling
public static class ObjectPooler
{
    // Create object pool using a prefab and amount to pool
    public static List<GameObject> CreateObjectPool(int amountToPool, GameObject objectToPool)
    {
        List<GameObject> pooledObjects = new List<GameObject>();

        for (int i = 0; i < amountToPool; i++)
        {
            GameObject obj = GameObject.Instantiate(objectToPool);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }

        return pooledObjects;
    }

    //Retrieve a pooled object if it is not active in the hierarchy
    public static GameObject GetPooledObject(List<GameObject> pooledObjects)
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
    public static void ReturnObjectsToPool(List<GameObject> pooledObjects)
    {
        foreach (GameObject obj in pooledObjects)
        {
            if (obj.activeInHierarchy)
            {
                obj.SetActive(false);
            }
        }
    }

    // Assign parent transform for pooled objects to organise hierarchy
    public static void AssignParentGroup(List<GameObject> pooledObjects, Transform parent)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            pooledObjects[i].transform.parent = parent;
        }
    }
}
