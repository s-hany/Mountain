using UnityEngine;
using System.Collections.Generic;

public class SegmentPoolManager : MonoBehaviour
{
    [SerializeField] private GameObject segmentPrefab;  // セグメントのプレハブ
    [SerializeField] private int initialPoolSize = 5;   // 初期プール数

    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Start()
    {
        // 初期プールを作成：必要数だけ生成し、非アクティブ化してキューに追加
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject obj = Instantiate(segmentPrefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    // プールからセグメントを取得し、指定位置に配置してアクティブ化
    public GameObject GetSegment(Vector3 position)
    {
        GameObject obj;
        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else
        {
            // プールが空の場合は新規生成（可能な限り再利用する運用とします）
            obj = Instantiate(segmentPrefab);
        }
        obj.transform.position = position;
        obj.SetActive(true);
        return obj;
    }

    // セグメントをプールに返却（非アクティブ化してキューに戻す）
    public void ReturnSegment(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
