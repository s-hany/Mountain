using UnityEngine;

[RequireComponent( typeof( Rigidbody ) )]
public class PullObject : MonoBehaviour
{
    private Camera m_MainCamera = null;
    private Transform m_MainCameraTransform = null;
    private Rigidbody m_Physics = null;

    private Vector3 m_CurrentForce = Vector3.zero; // 発射方向の力
    private Vector3 m_DragStart = Vector3.zero; // ドラッグ開始地点
    const float MaxMagnitude = 1f; // 最大力量
    const float FixForce = 10f; // 加える力量の調整

    void Start()
    {
        this.m_Physics = this.GetComponent<Rigidbody>();

        this.m_MainCamera = Camera.main;
        this.m_MainCameraTransform = this.m_MainCamera.transform;
    }

    /// <summary>
    /// マウス座標をワールド座標に変換して返す
    /// </summary>
    /// <returns>ワールド座標</returns>
    Vector3 GetMousePosition()
    {
        var pos = Input.mousePosition;

        pos.z = this.m_MainCameraTransform.position.z;
        pos = this.m_MainCamera.ScreenToWorldPoint( pos );
        pos.z = 0;

        return pos;
    }

    /// <summary>
    /// ドラック開始のイベントハンドラ
    /// </summary>
    void OnMouseDown()
    {
        this.m_DragStart = this.GetMousePosition();
    }

    /// <summary>
    /// ドラッグ中のイベントハンドラ
    /// </summary>
    void OnMouseDrag()
    {
        var pos = this.GetMousePosition();

        this.m_CurrentForce = pos - this.m_DragStart;
        if (this.m_CurrentForce.magnitude > MaxMagnitude* MaxMagnitude)
        {
            this.m_CurrentForce *= MaxMagnitude / this.m_CurrentForce.magnitude;
        }
    }

    /// <summary>
    /// ドラッグ終了イベントハンドラ
    /// </summary>
    void OnMouseUp()
    {
        this.Flip( this.m_CurrentForce * FixForce );
    }

    /// <summary>
    /// 力を加える
    /// </summary>
    /// <param name="force">力量</param>
    void Flip( Vector3 force )
    {
        this.m_Physics.AddForce( force, ForceMode.Impulse );
    }
}