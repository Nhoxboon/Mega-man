using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImagePool : NhoxMonoBehaviour
{
    [SerializeField] protected GameObject afterImagePrefab;

    [SerializeField] protected Queue<GameObject> availableObjects = new Queue<GameObject>();

    private static PlayerAfterImagePool instance;
    public static PlayerAfterImagePool Instance => instance;

    protected override void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Only 1 PlayerAfterImagePool allow to exist");
        }
        instance = this;
        GrowPool();
    }

    protected void GrowPool()
    {
        for (int i = 0; i < 10; i++)
        {
            var instanceToAdd = Instantiate(afterImagePrefab);
            instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);
        }
    }

    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        availableObjects.Enqueue(instance);
    }

    public GameObject GetFromPool()
    {
        if (availableObjects.Count == 0)
        {
            GrowPool();
        }

        var instance = availableObjects.Dequeue();
        instance.SetActive(true);
        return instance;
    }
}
