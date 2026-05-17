using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{

    [Header("Ç® ¼³Á¤")]
    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private int initialSize = 5;

    [SerializeField]
    private Transform poolContainer;

    private Queue<GameObject> pooledObjects = new Queue<GameObject>();

    private void Awake()
    {

        if (poolContainer == null)
        {

            GameObject containerObject = new GameObject($"{name}_Container");

            containerObject.transform.SetParent(transform);
            containerObject.transform.localPosition = Vector3.zero;
            containerObject.transform.localRotation = Quaternion.identity;

            poolContainer = containerObject.transform;

        }

        for (int i = 0; i < initialSize; i++)
        {

            GameObject pooledObject = CreateObject();

            pooledObjects.Enqueue(pooledObject);

        }

    }

    public GameObject GetObject(Vector3 position, Quaternion rotation)
    {

        GameObject pooledObject;

        if (pooledObjects.Count > 0)
        {

            pooledObject = pooledObjects.Dequeue();

        }
        else
        {

            pooledObject = CreateObject();

        }

        pooledObject.transform.SetPositionAndRotation(position, rotation);
        pooledObject.SetActive(true);

        return pooledObject;

    }

    public void ReturnObject(GameObject pooledObject)
    {

        if (pooledObject == null)
        {

            return;

        }

        pooledObject.SetActive(false);

        if (poolContainer != null)
        {

            pooledObject.transform.SetParent(poolContainer);

        }

        pooledObjects.Enqueue(pooledObject);

    }

    private GameObject CreateObject()
    {

        GameObject pooledObject = Instantiate(prefab, poolContainer);

        pooledObject.SetActive(false);

        return pooledObject;

    }

}