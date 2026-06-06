using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private bool expandable = true;

    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            pool.Enqueue(obj);
        }
    }

    public GameObject Get()
    {
        GameObject obj;

        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else if (expandable)
        {
            obj = Instantiate(prefab);
            obj.transform.SetParent(transform);
        }
        else
        {
            Debug.LogWarning($"Pool for {prefab.name} is empty!");
            return null;
        }

        obj.SetActive(true);
        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        pool.Enqueue(obj);
    }

    public GameObject Get(float autoReturnAfter)
    {
        GameObject obj = Get();
        if (obj != null)
        {
            StartCoroutine(AutoReturn(obj, autoReturnAfter));
        }
        return obj;
    }

    private System.Collections.IEnumerator AutoReturn(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null && obj.activeSelf)
            ReturnToPool(obj);
    }
}