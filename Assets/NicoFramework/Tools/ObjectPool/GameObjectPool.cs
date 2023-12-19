using System.Collections.Generic;
using UnityEngine;

namespace NicoFramework.Tools.ObjectPool
{
    public sealed class GameObjectPool : MonoBehaviour
    {
        public static GameObjectPool Instance { get; private set; }

        private Dictionary<string, List<GameObject>> pool = new Dictionary<string, List<GameObject>>();

        public int WaterLevel { get; private set; } = 10;

        private bool isNeedGenerate = false;
        private string generateName;
        private string generatePath;

        private void Awake()
        {
            if (Instance == null) {
                Instance = this;
            } else {
                Destroy(gameObject);
            }
        }

        public GameObject Get(string prefabName, string prefabPath, Vector3 position, Quaternion rotation)
        {
            var key = prefabName;

            // 如果对象池中没有这个物体，则第一次初始化到水位线
            if (!pool.ContainsKey(key)) {
                pool[key] = new List<GameObject>();
                InitializeToWaterLevel(key, prefabPath);
            }

            GameObject obj;
            // 有物体
            if (pool[key].Count > 0) {
                var list = pool[key];
                obj = list[0];
                list.RemoveAt(0);
                // initialize
                obj.SetActive(true);
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                // 小于水位线
                if (pool[key].Count < WaterLevel) {
                    isNeedGenerate = true;
                    generateName = prefabName;
                    generatePath = prefabPath;
                }
            } else {
                Debug.Log("水位线可能较低");
                obj = Instantiate(Resources.Load<GameObject>(prefabPath), position, rotation, transform);
            }

            return obj;
        }

        public void Store(GameObject obj)
        {
            var key = obj.name.Replace("(Clone)", string.Empty);
            if (pool.ContainsKey(key)) {
                pool[key].Add(obj);
            } else {
                pool[key] = new List<GameObject>() { obj };
            }

            obj.SetActive(false);
        }

        private void InitializeToWaterLevel(string key, string path)
        {
            GenerateToPool(key, WaterLevel, path);
        }

        private void GenerateToWaterLevel(string key, string path)
        {
            var currCount = pool[key].Count;
            GenerateToPool(key, WaterLevel - currCount, path);
        }

        private void GenerateToPool(string key, int num, string path)
        {
            for (int i = 0; i < num; i++)
            {
                var obj = Instantiate(Resources.Load<GameObject>(path), transform);
                obj.SetActive(false);
                pool[key].Add(obj);
            }
        }

        private void Update()
        {
            if (isNeedGenerate) {
                GenerateToWaterLevel(generateName, generatePath);
                isNeedGenerate = false;
            }
        }
    }
}