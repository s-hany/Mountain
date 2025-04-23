using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f; // 基本速度
    public float acceleration = 2f; // 加速量
    public float deceleration = 2f; // 減速量
    public float jumpForce = 5f; // ジャンプの力
    private Rigidbody rb; // Rigidbody コンポーネントへの参照

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Rigidbody コンポーネントを取得
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody がアタッチされていません！");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        // Spaceキーでジャンプ
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            Jump();
        }
    }

    private void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal"); // 入力を取得
        float currentSpeed = speed;

        if (horizontalInput > 0) // Dキーで加速
        {
            currentSpeed += acceleration * Time.deltaTime;
        }
        else if (horizontalInput < 0) // Wキーで減速
        {
            currentSpeed -= deceleration * Time.deltaTime;
        }

        // 常に右方向へ前進
        transform.Translate(Vector3.right * currentSpeed * Time.deltaTime);
    }

    void Jump()
    {
        // Rigidbody に上方向の力を加える
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    // 地面に接しているかを判定するメソッド
    bool IsGrounded()
    {
        // 地面との接触を判定する簡易的な方法
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
