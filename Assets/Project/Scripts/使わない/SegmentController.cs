using UnityEngine;
using System.Collections;

public class SegmentController : MonoBehaviour
{
    [SerializeField] private float activeDuration = 5f;  // セグメントの存続時間

    private void OnEnable()
    {
        // セグメントが有効化されたらタイマーを開始
        StartCoroutine(DisableAfterTime());
    }

    private IEnumerator DisableAfterTime()
    {
        // activeDuration秒待機後に非アクティブ化
        yield return new WaitForSeconds(activeDuration);
        gameObject.SetActive(false);
    }
}
