using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameObjectPool : MonoBehaviour
{
    public class QueueItem
    {
        public long destroyTime;
        public GameObject gameObject;

        public QueueItem(GameObject gameObject)
        {
            this.gameObject = gameObject;
            destroyTime = 0;
        }
    }

    /// <summary>
    /// Arrays element for Unity's built-in serialization. Contains parameters for each object queue and queues them self (private) 
    /// </summary>
    [Serializable]
    public class PoolablePrefab
    {
        public string prefabName;
        public double timeToLive;
        public int prewarmCount;

        [NonSerialized] public Queue<QueueItem> sleepingGameObjects;
        [NonSerialized] public Queue<QueueItem> activeGameObjects;

        public PoolablePrefab()
        {
            sleepingGameObjects = new Queue<QueueItem>();
            activeGameObjects = new Queue<QueueItem>();
        }
    }

    [SerializeField] private PoolablePrefab[] poolablePrefabs;

    /// <summary>
    /// Key is path to prefab (string)
    /// </summary>
    private static Dictionary<string, PoolablePrefab> _pool;

    /// <summary>
    /// Parent for initial objects storage is Pool's GameObject 
    /// </summary>
    private static Transform _container;

    /// <summary>
    /// Preparing instances of sleeping objects queues, according to initial values in the Pool's inspector 
    /// </summary>
    private void Awake()
    {
        _container = transform;
        _pool = new Dictionary<string, PoolablePrefab>(poolablePrefabs.Length);

        foreach (var poolablePrefab in poolablePrefabs)
        {
            int prewarmCount = poolablePrefab.prewarmCount;
            _pool.Add(poolablePrefab.prefabName, poolablePrefab);

            for (var i = 0; i < prewarmCount; i++)
            {
                AddItemToSleepingPool(poolablePrefab.prefabName);
            }
        }
    }

    /// <summary>
    /// Checking queues of active objects for the end of activity time. Because the objects in queue are ordered by it's age
    /// and each queue contains one type and lifetime, it's enough to check 'till first queue element, whose activity has not yet expired.
    /// This will save resources. 
    /// </summary>
    private void Update()
    {
        long now = DateTime.Now.ToUnixTime();

        foreach (var poolable in poolablePrefabs)
        {
            if (poolable.activeGameObjects.Count > 0)
            {
                long itemTime = poolable.activeGameObjects.Peek().destroyTime;

                while (itemTime <= now && poolable.activeGameObjects.Count > 0)
                {
                    PushActiveItemToSleepItemsQueue(poolable);
                    if (poolable.activeGameObjects.Count > 0)
                    {
                        itemTime = poolable.activeGameObjects.Peek().destroyTime;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Moves oldest element in queue into sleeping objects queue, resets binding to the pool's container and deactivates an object
    /// </summary>
    /// <param name="poolablePrefab"> Type of element in queue that need to moved. Type is matching with prefab's path in resources
    /// </param>
    private static void PushActiveItemToSleepItemsQueue(PoolablePrefab poolablePrefab)
    {
        var tQueueItem = poolablePrefab.activeGameObjects.Dequeue();

        if (tQueueItem.gameObject == null)
        {
            return;
        }

        tQueueItem.gameObject.SetActive(false);
        tQueueItem.gameObject.transform.localScale = Vector3.one;
        tQueueItem.gameObject.transform.parent = _container;
        poolablePrefab.sleepingGameObjects.Enqueue(tQueueItem);
    }

    /// <summary>
    /// Creates new element in queue and moves it into sleeping objects queue of given type.
    /// </summary>
    /// <param name="prefabName"> Type of element that needs to be created and put to sleep. Type is matching with prefab's path in resources
    /// </param>
    private static void AddItemToSleepingPool(string prefabName)
    {
        var goForAdd = Resources.Load<GameObject>(prefabName);

        if (goForAdd == null)
        {
            DebugWrapper.LogWarning($"[GameObjectPool::AddItemToSleepingPool] Object with name {prefabName} is null ");
        }

        var go = Instantiate(goForAdd, _container);
        go.name = prefabName;
        go.SetActive(false);

        if (!_pool.ContainsKey(prefabName))
        {
            _pool.Add(prefabName, new PoolablePrefab());
        }

        _pool[prefabName].sleepingGameObjects.Enqueue(new QueueItem(go));
    }

    /// <summary>
    /// Returns inactive element from sleeping objects queue, moves it to active objects queue and sets time to return to sleep
    /// </summary>
    /// <param name="prefabName"> Type of element that needs to be waken up and returned. Type is matching with prefab's path in resources
    /// </param>
    /// <param name="timeBeforeDestroy"> 
    /// </param>
    /// <returns> Returns inactive element from sleeping objects queue</returns>
    private static QueueItem GetItemFromSleepingPool(string prefabName, double timeBeforeDestroy)
    {
        var tQueueItem = _pool[prefabName].sleepingGameObjects.Dequeue();

        if (tQueueItem == null)
        {
            DebugWrapper.LogWarning($"[GameObjectPool::GetItemFromSleepingPool] tQueueItem invalid {prefabName}");
        }

        if (tQueueItem != null && tQueueItem.gameObject == null)
        {
            DebugWrapper.LogWarning(
                $"[GameObjectPool::GetItemFromSleepingPool] tQueueItem GameObject invalid {prefabName}");
        }


        long timeToLive = -1;
        if (timeBeforeDestroy > 0.0f)
        {
            timeToLive = DateTime.Now.AddSeconds(timeBeforeDestroy).ToUnixTime();
        }
        else if (_pool[prefabName].timeToLive != 0.0d)
        {
            timeToLive = DateTime.Now.AddSeconds(_pool[prefabName].timeToLive).ToUnixTime();
        }

        if (timeToLive > 0.0f)
        {
            tQueueItem.destroyTime = timeToLive;
            _pool[prefabName].activeGameObjects.Enqueue(tQueueItem);
        }

        tQueueItem?.gameObject.SetActive(true);
        return tQueueItem;
    }

    /// <summary>
    /// Pushing gameObject back to sleeping pool
    /// </summary>
    /// <param name="gameObject">GameObject to push back to pool </param>
    public static void PushBackGameObject(GameObject gameObject)
    {
        if (_container == null || gameObject == null)
        {
            return;
        }

        gameObject.SetActive(false);
        gameObject.transform.parent = _container;
        var queueItem = new QueueItem(gameObject);

        if (_pool.ContainsKey(gameObject.name))
        {
            var activeItem = _pool[gameObject.name].activeGameObjects
                .FirstOrDefault(item => item.gameObject == gameObject);

            if (activeItem == default)
            {
                _pool[gameObject.name].sleepingGameObjects.Enqueue(queueItem);
            }
            else
            {
                activeItem.destroyTime = 0;
            }
        }
    }

    /// <summary>
    /// Get object from pool by prefab name. If pool doesn't have enough needed objects, it will be added
    /// </summary>
    /// <param name="prefabName"> Type of element to get. Type is matching with prefab's path in resources </param>
    /// <returns> GameObject of requested object from pool </returns>
    public static GameObject GetObjectFromPool(string prefabName) =>
        GetObjectFromPool(prefabName, Vector3.zero, Quaternion.identity);

    /// <summary>
    /// Get object from pool by prefab name. If pool doesn't have enough needed objects, it will be added </summary>
    /// <param name="prefabName"> Type of element to get. Type is matching with prefab's path in resources </param>
    /// <param name="position">Desired position </param>
    /// <param name="rotation">Desired orientation </param>
    /// <param name="timeBeforeDestroy"> Desired life time </param>
    /// <returns> GameObject of requested object from pool </returns>
    public static GameObject GetObjectFromPool(string prefabName, Vector3 position, Quaternion rotation,
        double timeBeforeDestroy = -1.0f)
    {
        if (!_pool.ContainsKey(prefabName))
        {
            DebugWrapper.LogWarning($"PrefabName was not present in dictionary: {prefabName} ");
            AddItemToSleepingPool(prefabName);
        }

        if (_pool[prefabName].sleepingGameObjects.Count > 0)
        {
            return GetItemFromPoolAndSetTransform();
        }

        AddItemToSleepingPool(prefabName);
        return GetItemFromPoolAndSetTransform();

        GameObject GetItemFromPoolAndSetTransform()
        {
            var go = GetItemFromSleepingPool(prefabName, timeBeforeDestroy).gameObject;
            go.transform.position = position;
            go.transform.rotation = rotation;
            return go;
        }
    }
}