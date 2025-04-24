using UnityEngine;


public class Player : MonoBehaviour
{
    // 移動スピード関連
    public float baseSpeed = 5f;            // 基本速度（何も入力がないときの目安）
    public float currentSpeed = 5f;         // 現在の速度。入力や地形で変化する
    public float accelerationRate = 3f;     // Dキー（正入力）による加速率
    public float decelerationRate = 3f;     // Aキー（負入力）による減速率
    public float maxSpeed = 15f;
    public float minSpeed = 3f;

    // 地形（坂）による加速
    public float slopeMultiplier = 5f;      // 地面の傾斜が右下の場合に加算されるスピード係数
    public float raycastDistance = 1.0f;    // 地面判定用Raycastの距離
    public LayerMask groundLayer;           // 地面判定に使用するレイヤー

    // ジャンプ関連
    public float jumpForce = 7f;
    public int maxJumps = 2;                // 二段ジャンプ可能
    private int jumpCount = 0;              // 現在のジャンプ回数

    // 障害物衝突時のラグドール変換
    public float obstacleSpeedThreshold = 10f;  // この速度以上の衝突でラグドール化
    public GameObject ragdollPrefab;             // ラグドールPrefab（プレイヤーと同じ位置、回転で生成）

    // コンポーネント参照
    private Rigidbody rb;
    private Animator animator;

    // 地面接触フラグ
    private bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        currentSpeed = baseSpeed;
    }

    void Update()
    {
        // Horizontal入力の取得（A, Dキー：-1～+1）
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        // 右入力（Dキー）→加速
        if (horizontalInput > 0)
        {
            currentSpeed += accelerationRate * Time.deltaTime;
            animator.SetBool("isRunning", true);
            animator.SetBool("isSlowing", false);
        }
        // 左入力（Aキー）→減速（右方向移動は維持）
        else if (horizontalInput < 0)
        {
            currentSpeed -= decelerationRate * Time.deltaTime;
            animator.SetBool("isSlowing", true);
            animator.SetBool("isRunning", false);
        }
        // 入力が無い場合は基本速度に戻す
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, baseSpeed, decelerationRate * Time.deltaTime);
            animator.SetBool("isRunning", false);
            animator.SetBool("isSlowing", false);
        }
        
        // 速度の範囲を制限
        currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);

        // ジャンプの処理（二段ジャンプまで可能）
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            // 縦方向速度をリセットしてから力を加算
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpCount++;
            animator.SetTrigger("Jump");
        }
        
        // Animatorに速度の情報（オプションで正規化した値）を渡す
        animator.SetFloat("Speed", currentSpeed);
    }

    void FixedUpdate()
    {
        // 地面の傾斜を調べるため、下方向にRaycast
        float slopeBonus = 0f;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance, groundLayer))
        {
            // 地面の法線のX成分が負（右下の傾斜）なら、傾斜に応じたボーナスを計算
            if (hit.normal.x < 0)
            {
                slopeBonus = -hit.normal.x * slopeMultiplier;
            }
        }
        
        // 実際の移動速度 = 現在の速度 + 地形による加速（坂ボーナス）
        float effectiveSpeed = currentSpeed + slopeBonus;
        
        // 常に正のX方向へ移動。Yはジャンプ等の影響、Zは固定（2.5D想定）
        rb.linearVelocity = new Vector3(effectiveSpeed, rb.linearVelocity.y, 0);
    }

    void OnCollisionEnter(Collision collision)
    {
        // 地面と接触した場合
        if(collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;
            animator.SetBool("isGrounded", true);
        }
        
        // 障害物との接触検出
        if(collision.gameObject.CompareTag("Obstacle"))
        {
            if(currentSpeed >= obstacleSpeedThreshold)
            {
                TriggerRagdoll();
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            animator.SetBool("isGrounded", false);
        }
    }
    
    /// <summary>
    /// 障害物衝突時にラグドールへ切り替える処理
    /// </summary>
    void TriggerRagdoll()
    {
        // 現在の位置・回転でラグドールPrefabを生成
        GameObject ragdoll = Instantiate(ragdollPrefab, transform.position, transform.rotation);
        
        // プレイヤーの現在の速度をラグドールの各パーツにコピー（自然な物理挙動を維持）
        Rigidbody[] ragdollRigidbodies = ragdoll.GetComponentsInChildren<Rigidbody>();
        foreach(Rigidbody rbRagdoll in ragdollRigidbodies)
        {
            rbRagdoll.linearVelocity = rb.linearVelocity;
        }
        
        // プレイヤーオブジェクトは破棄する
        Destroy(gameObject);
    }
}