using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainPoolManager : MonoBehaviour
{
    // シングルトンインスタンス
    public static TerrainPoolManager Instance { get; private set; }

    // 地形セグメントのプレハブ（パターン）の配列
    public GameObject[] terrainPrefabs;
    // 各プレハブのプールに作成しておく初期オブジェクト数
    public int initialPoolCount = 5;

    // プール用のディクショナリ (プレハブインデックス -> オブジェクトキュー)
    private Dictionary<int, Queue<GameObject>> poolDictionary;

    void Awake()
    {
        // シングルトンパターンの設定
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // プールディクショナリを初期化し、初期オブジェクトを生成
        poolDictionary = new Dictionary<int, Queue<GameObject>>();
        for (int i = 0; i < terrainPrefabs.Length; i++)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            // 初期オブジェクトを生成して非アクティブ状態にしておく
            for (int j = 0; j < initialPoolCount; j++)
            {
                GameObject obj = Instantiate(terrainPrefabs[i]);
                obj.SetActive(false);

                // プレハブにTerrainSegmentTimerが付与されていれば、プールインデックスを設定
                TerrainSegmentTimer timer = obj.GetComponent<TerrainSegmentTimer>();
                if (timer != null)
                {
                    timer.poolIndex = i;
                }

                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(i, objectPool);
        }
    }

    // プールから地形セグメントを取得（プールが空の場合はインスタンス化）
    public GameObject GetTerrainSegment(int prefabIndex, Vector3 position)
    {
        if (prefabIndex < 0 || prefabIndex >= terrainPrefabs.Length)
        {
            Debug.LogError("Invalid prefab index for terrain: " + prefabIndex);
            return null;
        }

        GameObject obj;
        Queue<GameObject> objectPool = poolDictionary[prefabIndex];

        if (objectPool.Count > 0)
        {
            // プールにあるオブジェクトを再利用
            obj = objectPool.Dequeue();
        }
        else
        {
            // プールにオブジェクトがない場合は新規作成
            obj = Instantiate(terrainPrefabs[prefabIndex]);
            // 生成時にTerrainSegmentTimerがあればプールインデックスを設定
            TerrainSegmentTimer timer = obj.GetComponent<TerrainSegmentTimer>();
            if (timer != null)
            {
                timer.poolIndex = prefabIndex;
            }
        }

        // 取得したオブジェクトを配置してアクティブ化
        obj.transform.position = position;
        obj.transform.rotation = Quaternion.identity;
        obj.SetActive(true);

        return obj;
    }

    // 地形セグメントをプールに戻す
    public void ReturnTerrainSegment(GameObject obj)
    {
        // オブジェクトを非アクティブにする
        obj.SetActive(false);

        // どのプールに戻すか判断
        TerrainSegmentTimer timer = obj.GetComponent<TerrainSegmentTimer>();
        if (timer == null)
        {
            Debug.LogError("Returned object has no TerrainSegmentTimer component.");
            return;
        }

        int index = timer.poolIndex;
        if (!poolDictionary.ContainsKey(index))
        {
            Debug.LogError("No pool exists for index: " + index);
            return;
        }

        poolDictionary[index].Enqueue(obj);
    }
}
