using UnityEngine;
using System.Collections.Generic;

namespace GunFireHeroes.Utils
{
    /// <summary>
    /// 对象池管理器，用于管理弹幕、敌人等频繁创建销毁的对象
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool Instance { get; private set; }
        
        [Header("池配置")]
        public PoolConfig[] poolConfigs;
        
        private Dictionary<string, Queue<GameObject>> pools;
        private Dictionary<string, GameObject> prefabs;
        private Dictionary<string, Transform> poolParents;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializePools();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializePools()
        {
            pools = new Dictionary<string, Queue<GameObject>>();
            prefabs = new Dictionary<string, GameObject>();
            poolParents = new Dictionary<string, Transform>();
            
            foreach (var config in poolConfigs)
            {
                CreatePool(config);
            }
        }
        
        private void CreatePool(PoolConfig config)
        {
            if (config.prefab == null)
            {
                Debug.LogError($"对象池配置错误：{config.poolName} 的预制体为空");
                return;
            }
            
            string poolName = config.poolName;
            pools[poolName] = new Queue<GameObject>();
            prefabs[poolName] = config.prefab;
            
            // 创建池父对象
            GameObject poolParent = new GameObject($"Pool_{poolName}");
            poolParent.transform.SetParent(transform);
            poolParents[poolName] = poolParent.transform;
            
            // 预创建对象
            for (int i = 0; i < config.initialSize; i++)
            {
                GameObject obj = CreateNewObject(poolName);
                obj.SetActive(false);
                pools[poolName].Enqueue(obj);
            }
            
            Debug.Log($"创建对象池：{poolName}，初始大小：{config.initialSize}");
        }
        
        private GameObject CreateNewObject(string poolName)
        {
            if (!prefabs.ContainsKey(poolName))
                return null;
                
            GameObject obj = Instantiate(prefabs[poolName], poolParents[poolName]);
            
            // 添加池对象组件
            PooledObject pooledObj = obj.GetComponent<PooledObject>();
            if (pooledObj == null)
            {
                pooledObj = obj.AddComponent<PooledObject>();
            }
            pooledObj.poolName = poolName;
            
            return obj;
        }
        
        /// <summary>
        /// 从池中获取对象
        /// </summary>
        public GameObject GetObject(string poolName)
        {
            if (!pools.ContainsKey(poolName))
            {
                Debug.LogError($"对象池不存在：{poolName}");
                return null;
            }
            
            GameObject obj;
            
            if (pools[poolName].Count > 0)
            {
                obj = pools[poolName].Dequeue();
            }
            else
            {
                obj = CreateNewObject(poolName);
            }
            
            if (obj != null)
            {
                obj.SetActive(true);
                obj.transform.SetParent(null);
            }
            
            return obj;
        }
        
        /// <summary>
        /// 从池中获取对象并设置位置和旋转
        /// </summary>
        public GameObject GetObject(string poolName, Vector3 position, Quaternion rotation)
        {
            GameObject obj = GetObject(poolName);
            if (obj != null)
            {
                obj.transform.position = position;
                obj.transform.rotation = rotation;
            }
            return obj;
        }
        
        /// <summary>
        /// 将对象返回池中
        /// </summary>
        public void ReturnObject(GameObject obj)
        {
            if (obj == null)
                return;
                
            PooledObject pooledObj = obj.GetComponent<PooledObject>();
            if (pooledObj == null)
            {
                Debug.LogWarning($"对象 {obj.name} 不是池对象，直接销毁");
                Destroy(obj);
                return;
            }
            
            string poolName = pooledObj.poolName;
            
            if (!pools.ContainsKey(poolName))
            {
                Debug.LogWarning($"对象池不存在：{poolName}，直接销毁对象");
                Destroy(obj);
                return;
            }
            
            obj.SetActive(false);
            obj.transform.SetParent(poolParents[poolName]);
            pools[poolName].Enqueue(obj);
        }
        
        /// <summary>
        /// 延迟返回对象
        /// </summary>
        public void ReturnObject(GameObject obj, float delay)
        {
            if (obj != null)
            {
                StartCoroutine(ReturnObjectDelayed(obj, delay));
            }
        }
        
        private System.Collections.IEnumerator ReturnObjectDelayed(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            ReturnObject(obj);
        }
        
        /// <summary>
        /// 清空指定池
        /// </summary>
        public void ClearPool(string poolName)
        {
            if (!pools.ContainsKey(poolName))
                return;
                
            while (pools[poolName].Count > 0)
            {
                GameObject obj = pools[poolName].Dequeue();
                if (obj != null)
                    Destroy(obj);
            }
        }
        
        /// <summary>
        /// 清空所有池
        /// </summary>
        public void ClearAllPools()
        {
            foreach (var poolName in pools.Keys)
            {
                ClearPool(poolName);
            }
        }
        
        /// <summary>
        /// 获取池状态信息
        /// </summary>
        public void LogPoolStatus()
        {
            foreach (var kvp in pools)
            {
                Debug.Log($"池 {kvp.Key}：可用对象数量 {kvp.Value.Count}");
            }
        }
    }
    
    /// <summary>
    /// 对象池配置
    /// </summary>
    [System.Serializable]
    public class PoolConfig
    {
        [Header("基础配置")]
        public string poolName;
        public GameObject prefab;
        public int initialSize = 10;
        public int maxSize = 100;
        
        [Header("自动管理")]
        public bool autoReturn = false;
        public float autoReturnTime = 5f;
    }
    
    /// <summary>
    /// 池对象组件
    /// </summary>
    public class PooledObject : MonoBehaviour
    {
        [HideInInspector]
        public string poolName;
        
        private float autoReturnTime;
        private bool autoReturn;
        
        private void OnEnable()
        {
            // 获取池配置
            var poolConfig = System.Array.Find(ObjectPool.Instance.poolConfigs, 
                config => config.poolName == poolName);
                
            if (poolConfig != null && poolConfig.autoReturn)
            {
                autoReturn = true;
                autoReturnTime = poolConfig.autoReturnTime;
                Invoke(nameof(AutoReturn), autoReturnTime);
            }
        }
        
        private void OnDisable()
        {
            if (autoReturn)
            {
                CancelInvoke(nameof(AutoReturn));
            }
        }
        
        private void AutoReturn()
        {
            ObjectPool.Instance.ReturnObject(gameObject);
        }
        
        /// <summary>
        /// 手动返回池
        /// </summary>
        public void ReturnToPool()
        {
            ObjectPool.Instance.ReturnObject(gameObject);
        }
        
        /// <summary>
        /// 延迟返回池
        /// </summary>
        public void ReturnToPool(float delay)
        {
            ObjectPool.Instance.ReturnObject(gameObject, delay);
        }
    }
}