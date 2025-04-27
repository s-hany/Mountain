using UnityEngine;
using System.Collections.Generic;

public class TerrainManager : MonoBehaviour {
    public GameObject[] segmentPrefabs;    // 複数の地形パターンプレハブ
    public int initialPoolSize = 5;        // プールするセグメント数
    public float scrollSpeed = 5f;         // 地形スクロール速度

    private Queue<GameObject> pool = new Queue<GameObject>();
    private Vector3 nextSpawnPos = Vector3.zero;

    void Start() {
        // プールの初期化
        for (int i = 0; i < initialPoolSize; i++) {
            GameObject seg = Instantiate(segmentPrefabs[i % segmentPrefabs.Length]);
            seg.SetActive(false);
            pool.Enqueue(seg);
        }
        // 初回配置（例：画面上方に続けて配置）
        for (int i = 0; i < initialPoolSize; i++) {
            SpawnSegment();
        }
    }

    void Update() {
        // スクロール処理
        foreach (GameObject seg in pool) {
            if (seg.activeSelf) {
                seg.transform.position += Vector3.down * scrollSpeed * Time.deltaTime;
                // 画面外に出たら再利用
                if (seg.transform.position.y < -10f) {
                    RecycleSegment(seg);
                    SpawnSegment();
                    break; // 1つずつ再配置
                }
            }
        }
    }

    void SpawnSegment() {
        // プールから取得して配置
        GameObject seg = pool.Dequeue();
        seg.SetActive(true);
        seg.transform.position = nextSpawnPos;
        // 次のスポーン位置を更新（例：パターンの高さに応じて）
        nextSpawnPos += new Vector3(0, 5f, 0);
        pool.Enqueue(seg);
    }

    void RecycleSegment(GameObject seg) {
        seg.SetActive(false);
        // （必要ならセグメントを初期化）
    }
}
