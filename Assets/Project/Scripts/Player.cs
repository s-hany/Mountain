using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    // 【移動設定】
    public float baseSpeed = 5f;            // 基本状態での速度
    public float currentSpeed = 5f;         // 現在の移動速度
    public float accelerationRate = 3f;     // 右入力(Dキー)による加速率
    public float decelerationRate = 3f;     // 左入力(Aキー)による減速率
    public float maxSpeed = 15f;            // 最高速度
    public float minSpeed = 3f;             // 最低速度

    // 【坂道による補正】
    public float slopeMultiplier = 5f;      // 地形の傾斜が影響する加速係数
    public float raycastDistance = 1.0f;    // Raycast の距離
    public LayerMask groundLayer;           // 地面判定用レイヤー

    // 【ジャンプ設定】
    public float jumpForce = 7f;            // ジャンプ時のインパルス
    public int maxJumps = 2;              // 最大ジャンプ回数（二段ジャンプ）
    private int jumpCount = 0;             // 現在のジャンプ回数
    private bool jumpRequested = false;    // ジャンプ要求フラグ

    // 【障害物＆ラグドール】
    public float obstacleSpeedThreshold = 10f;  // この速度以上の衝突でラグドール切替
    public GameObject ragdollPrefab;            // ラグドール用のPrefab

    // 【コンポーネント参照】
    private Rigidbody rb;
    private Animator animator;

    // 【接地状態】
    private bool isGrounded = false;

    public float normalizedSpeed = 0f; // 0～1 の範囲で現在の移動速度を表す変数

    public AudioClip dieSE; // 死亡時のSE

    public AudioClip jumpSE; // ジャンプ時のSE

    public AudioClip explosionSE; // 爆発時のSE

    public AudioSource audioSource; // AudioSourceコンポーネント

    public SpeedMonitor speedMonitor; // SpeedMonitorコンポーネント

    public bool isStart = false; // ゲーム開始フラグ

    private void Awake()
    {
        // Rigidbodyコンポーネントを取得
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // 回転を固定

        rb.useGravity = false; // 重力を有効にする
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        currentSpeed = baseSpeed;
    }

    void Update()
    {
        // ジャンプ処理
        Jump(); 
        // アニメーション処理
        Anim();
    }

    void FixedUpdate()
    {
        // 移動および物理処理
        Move();
    }

    private void Jump()
    {
        if (!isStart) return; // ゲーム開始前はジャンプしない

        // ジャンプ入力の検出（Input は Update で処理）
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            if (jumpCount == 0) animator.SetTrigger("Jump");
            else if (jumpCount == 1) animator.SetTrigger("DoubleJump");
            jumpRequested = true;
            audioSource.PlayOneShot(jumpSE); // ジャンプSEを再生
        }
    }

    /// <summary>
    /// キャラクターの移動に関わる処理（入力、速度更新、坂道による補正、ジャンプ処理）
    /// </summary>
    private void Move()
    {
        if (!isStart) return; // ゲーム開始前は移動しない

        // Horizontal入力の取得（-1～+1）※Aキー→負、Dキー→正
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        // 入力に応じた速度更新
        if (horizontalInput > 0)
        {
            currentSpeed += accelerationRate * Time.deltaTime;
        }
        else if (horizontalInput < 0)
        {
            currentSpeed -= decelerationRate * Time.deltaTime;
        }
        else
        {
            // 入力が無ければ、基本速度に回帰する（自然減速）
            currentSpeed = Mathf.MoveTowards(currentSpeed, baseSpeed, decelerationRate * Time.deltaTime);
        }
        // 速度を下限・上限にクランプ
        currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);

        // 坂道による速度補正
        float slopeBonus = 0f;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance, groundLayer))
        {
            // 地面の法線の X 成分が負 → 右下の傾斜なら補正
            if (hit.normal.x < 0)
            {
                slopeBonus = -hit.normal.x * slopeMultiplier;
            }
        }

        float effectiveSpeed = currentSpeed + slopeBonus;

        // ジャンプ要求があれば処理
        if (jumpRequested)
        {
            // 現在の縦方向速度をリセットしてからジャンプ力を加える
            Vector3 tempVelocity = rb.linearVelocity;
            tempVelocity.y = 0;
            rb.linearVelocity = tempVelocity;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpCount++;
            jumpRequested = false;
        }

        // 常に正のX方向へ移動。Yはジャンプ/重力、Zは固定（2.5Dの場合）
        rb.linearVelocity = new Vector3(effectiveSpeed, rb.linearVelocity.y, 0);
    }

    /// <summary>
    /// アニメーションに関する処理。移動速度に応じた BlendTree 用の Speed 値の更新や、
    /// 入力により走っている／減速状態等のブール値を制御する。
    /// </summary>
    private void Anim()
    {
        if(!isStart) return; // ゲーム開始前はアニメーションしない

        // 現在の移動速度を下限・上限の範囲から 0～1 に正規化
        normalizedSpeed = (currentSpeed - minSpeed) / (maxSpeed - minSpeed);
        normalizedSpeed = Mathf.Clamp01(normalizedSpeed);
        animator.SetFloat("Speed", normalizedSpeed);

        // 入力に応じたアニメーション状態の切り替え
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        if (horizontalInput > 0)
        {
            animator.SetBool("isRunning", true);
            animator.SetBool("isSlowing", false);
        }
        else if (horizontalInput < 0)
        {
            animator.SetBool("isSlowing", true);
            animator.SetBool("isRunning", false);
        }
        else
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isSlowing", false);
        }

        // 接地状態（落下中の場合などの遷移に利用）
        animator.SetBool("isGrounded", isGrounded);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (speedMonitor != null && speedMonitor.CurrentSpeed >= obstacleSpeedThreshold)
            {
                TriggerRagdoll();
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    /// <summary>
    /// 障害物との衝突時に、ラグドールPrefabへキャラクターを切り替える処理
    /// </summary>
    private void TriggerRagdoll()
    {
        // 死亡SEを再生
        if (audioSource != null && dieSE != null)
        {
            audioSource.PlayOneShot(explosionSE);
            audioSource.PlayOneShot(dieSE);
        }
        GameObject ragdoll = Instantiate(ragdollPrefab, transform.position, transform.rotation);
        Rigidbody[] ragdollRigidbodies = ragdoll.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rbRagdoll in ragdollRigidbodies)
        {
            rbRagdoll.linearVelocity = rb.linearVelocity;
        }
        Destroy(gameObject);
    }

    public void GameStart()
    {
        animator.SetTrigger("isStart"); // ゲーム開始時にジャンプアニメーションを再生
        rb.useGravity = true; // 重力を有効にする
        isStart = true; // ゲーム開始フラグを立てる
        currentSpeed = baseSpeed; // 基本速度にリセット
        jumpCount = 0; // ジャンプ回数をリセット
        isGrounded = false; // 接地状態をリセット

    }
}