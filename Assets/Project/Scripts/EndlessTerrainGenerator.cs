using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 2D横スクロール用の動的地形生成スクリプト（URP環境可）
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class EndlessTerrainGenerator : MonoBehaviour {
    // --- インスペクタで調整可能なパラメータ ---
    [SerializeField] private int pointsPerSegment = 10;   // 1セグメントを分割する頂点数
    [SerializeField] private float segmentWidth = 5f;     // 1セグメントの横幅
    [SerializeField] private float baseHeight = 0f;       // 地形の基準高さ（底面）
    [SerializeField] private float groundDepth = 5f;      // 底面からどれだけ下にメッシュを張るか
    [SerializeField] private float heightRange = 2f;      // 高低差の基本範囲
    [SerializeField] private float heightScale = 0.1f;    // プレイヤー位置に応じた高低差の増加係数

    private Mesh mesh;
    private List<Vector3> vertices;
    private List<int> triangles;
    private Transform player;        // プレイヤーTransform
    private float lastSegmentEndX;   // 最後に生成したセグメントの終端X座標
    private Vector3 lastLeftPoint;   // 前セグメントの左端（開始）点
    private int lastLeftTopIndex;    // 前セグメントの左上頂点インデックス
    private int lastLeftBottomIndex; // 前セグメントの左下頂点インデックス

    void Start() {
        // MeshFilterを取得し、新規Meshを割り当て
        mesh = new Mesh();
        mesh.name = "ProceduralTerrain";
        GetComponent<MeshFilter>().mesh = mesh;

        vertices = new List<Vector3>();
        triangles = new List<int>();

        // プレイヤーオブジェクトをタグで取得（シーンにPlayerタグ付きのオブジェクトが存在する想定）
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) {
            player = playerObj.transform;
        }

        // 初期の地形開始点を設定（左端0にベース高さで配置）
        lastLeftPoint = new Vector3(0f, baseHeight, 0f);
        // 最初のセグメントを生成
        GenerateNextSegment();
        UpdateMesh();
    }

    void Update() {
        if (player != null) {
            // プレイヤーが最後の地形セグメントの手前まで到達したら次セグメント生成
            if (player.position.x > lastSegmentEndX - segmentWidth) {
                GenerateNextSegment();
                UpdateMesh();
            }
        }
    }

    /// <summary>
    /// 次の地形セグメントを生成して頂点リストと三角形リストに追加する
    /// </summary>
    private void GenerateNextSegment() {
        // プレイヤー位置に応じて高低差の範囲をスケーリング
        float range = heightRange + heightScale * (player != null ? player.position.x : 0f);
        // 次セグメントの右端（新規終端）のY座標を乱数で決定
        float nextY = lastLeftPoint.y + Random.Range(-range, range);
        // 底面のY座標を基準に基準高さ分下げた位置とする
        float bottomY = baseHeight - groundDepth;
        // 底面より下に行かないように制限
        if (nextY < bottomY) nextY = bottomY;

        // 初回生成時は左端を頂点リストに追加
        if (vertices.Count == 0) {
            // 左上（x=0, lastLeftPoint.y）
            vertices.Add(new Vector3(lastLeftPoint.x, lastLeftPoint.y, 0f));
            // 左下（底面）
            vertices.Add(new Vector3(lastLeftPoint.x, bottomY, 0f));
            lastLeftTopIndex = 0;
            lastLeftBottomIndex = 1;
        }

        // 新セグメントの幅分だけx方向を移動する
        float startX = lastLeftPoint.x;
        float endX = lastLeftPoint.x + segmentWidth;
        // 分割数分だけ補間して頂点を生成
        for (int i = 1; i <= pointsPerSegment; i++) {
            float t = (float)i / pointsPerSegment;
            // 緩やかなS字補間（Mathf.SmoothStep）で滑らかな曲線
            float newX = Mathf.Lerp(startX, endX, t);
            float newY = Mathf.Lerp(lastLeftPoint.y, nextY, Mathf.SmoothStep(0f, 1f, t));
            // 上面頂点
            vertices.Add(new Vector3(newX, newY, 0f));
            // 下面頂点（底面）
            vertices.Add(new Vector3(newX, bottomY, 0f));

            // 新規頂点のインデックス
            int newTopIndex = vertices.Count - 2;
            int newBottomIndex = vertices.Count - 1;
            // 三角形を追加（前の頂点ペアと今回追加したペアで四角形を構成）
            triangles.Add(lastLeftTopIndex);
            triangles.Add(newTopIndex);
            triangles.Add(newBottomIndex);
            triangles.Add(lastLeftTopIndex);
            triangles.Add(newBottomIndex);
            triangles.Add(lastLeftBottomIndex);

            // 次の補間用にインデックスを更新
            lastLeftTopIndex = newTopIndex;
            lastLeftBottomIndex = newBottomIndex;
        }

        // 新しく生成したセグメントの終端を更新
        lastLeftPoint = new Vector3(endX, nextY, 0f);
        lastSegmentEndX = lastLeftPoint.x;
    }

    /// <summary>
    /// 頂点・三角形リストからMeshを更新する
    /// </summary>
    private void UpdateMesh() {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }
}
