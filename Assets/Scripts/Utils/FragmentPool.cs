using System.Collections.Generic;
using UnityEngine;

public class FragmentPool : MonoBehaviour
{
    public static FragmentPool Instance;
    public int poolSize = 20;

    private Dictionary<GameObject, Queue<FragmentedObject>> _pools = new Dictionary<GameObject, Queue<FragmentedObject>>();
    
    [System.Serializable]
    public class PoolConfig
    {
        public GameObject prefab;
        public int initialSize = 10;
    }
    public List<PoolConfig> initialPools;

    void Awake()
    {
        Instance = this;

        foreach (var config in initialPools)
        {
            CreatePool(config.prefab, config.initialSize);
        }
    }

    void CreatePool(GameObject prefab, int size)
    {
        var queue = new Queue<FragmentedObject>();
        _pools[prefab] = queue;

        for (int i = 0; i < size; i++)
        {
            var obj = Instantiate(prefab, transform).GetComponent<FragmentedObject>();
            obj.SetPoolKey(prefab);
            obj.gameObject.SetActive(false);
            queue.Enqueue(obj);
        }
    }


    public FragmentedObject Get(GameObject prefab)
    {
         if (!_pools.ContainsKey(prefab))
        {
            CreatePool(prefab, 5);
        }

        var pool = _pools[prefab];

        if (pool.Count > 0)
        {
            return pool.Dequeue();
        }

        var obj = Instantiate(prefab, transform).GetComponent<FragmentedObject>();
        obj.SetPoolKey(prefab);
        return obj;
    }

    public void Return(GameObject prefab, FragmentedObject obj)
    {
        obj.gameObject.SetActive(false);
        _pools[prefab].Enqueue(obj);
    }

}
