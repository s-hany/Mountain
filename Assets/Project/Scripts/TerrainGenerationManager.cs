using UnityEngine;
using System.Collections.Generic;

// TerrainGenerationManager: 進行に合わせて地形セグメントを生成するマネージャー
public class TerrainGenerationManager : MonoBehaviour
{
    // プールマネージャーで登録した地形プレハブのインデックスリスト
    public int[] terrainPrefabIndexes;

    // カメラからどれだけ先まで地形を生成しておくかの距離
    public float spawnThresholdDistance = 30.0f;
    // 地形セグメントの水平幅（X方向）※プレハブのサイズに合わせる
    public float segmentWidth = 10.0f;
    // 1セグメントごとに下方向（Y軸マイナス）に傾ける量
    public float slopeOffset = 0.5f;

    private float lastSpawnX;  // 最後に生成したセグメントのX座標
    private float lastSpawnY;  // 最後に生成したセグメントのY座標

    private Transform mainCamera;

    void Start()
    {
        mainCamera = Camera.main.transform;

        // 初期座標を設定して、最初のセグメントがX=0に来るようにする
        lastSpawnX = -segmentWidth;
        lastSpawnY = 0.0f;

        // ゲーム開始時にいくつかの地形を事前生成しておく
        for (int i = 0; i < 3; i++)
        {
            SpawnNextSegment();
        }
    }

    void Update()
    {
        // カメラが前進したら、一定距離先に地形を生成
        if (lastSpawnX - mainCamera.position.x < spawnThresholdDistance)
        {
            SpawnNextSegment();
        }
    }

    // 次の地形セグメントを生成する処理
    private void SpawnNextSegment()
    {
        // プレハブのインデックスリストが設定されているか確認
        if (terrainPrefabIndexes == null || terrainPrefabIndexes.Length == 0)
        {
            Debug.LogWarning("TerrainGenerationManagerに地形プレハブのインデックスが設定されていません。");
            return;
        }

        // 配列からランダムにインデックスを選択
        int randomIndex = terrainPrefabIndexes[Random.Range(0, terrainPrefabIndexes.Length)];

        // 前回の位置から右にsegmentWidthだけ進め、下方向にslopeOffset分下げた位置を計算
        Vector3 spawnPosition = new Vector3(lastSpawnX + segmentWidth, lastSpawnY, 0f);

        // プールマネージャーからセグメントを取得し、指定位置に配置
        GameObject segment = TerrainPoolManager.Instance.GetTerrainSegment(randomIndex, spawnPosition);

        if (segment != null)
        {
            // 生成後の座標を更新
            lastSpawnX = spawnPosition.x;
            lastSpawnY = spawnPosition.y - slopeOffset;
        }
    }
}
