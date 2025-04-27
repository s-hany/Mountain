using UnityEngine;
using System.Collections;

// TerrainSegmentTimer: 地形セグメントの寿命を管理し、一定時間後にプールに戻す
public class TerrainSegmentTimer : MonoBehaviour
{
    [Tooltip("プールマネージャーのプレハブ配列におけるインデックス")]
    public int poolIndex;

    [Tooltip("セグメントをプールに戻すまでの生存時間（秒）")]
    public float lifeTime = 5.0f;

    private Coroutine returnCoroutine;

    void OnEnable()
    {
        // セグメントがアクティブになったら、指定時間後にプールへ戻すコルーチンを開始
        returnCoroutine = StartCoroutine(ReturnToPoolAfterDelay());
    }

    void OnDisable()
    {
        // オブジェクトが非アクティブになったらコルーチンを停止
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
        }
    }

    // 指定時間待ってからオブジェクトをプールに戻すコルーチン
    private IEnumerator ReturnToPoolAfterDelay()
    {
        yield return new WaitForSeconds(lifeTime);

        // プールマネージャーに自身を返却
        TerrainPoolManager.Instance.ReturnTerrainSegment(this.gameObject);
    }
}
