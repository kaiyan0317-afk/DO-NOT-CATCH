using UnityEngine;

public class FishControl : MonoBehaviour
{
    [Header("水中設定")]
    public float sinkSpeed = 1.5f;
    public float swayIntensity = 2f;
    public float swaySpeed = 3f;
    public float deadLineY = -5.5f;

    [Header("サウンド・エフェクト")]
    public AudioClip tapSound;
    public GameObject ExplosionEffect;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float startTime;
    private bool isStarted = false;

    // --- 追加：ランダムな動きのための変数 ---
    private float moveDirection = 1f; // 1なら右から、-1なら左から
    private float individualSwaySpeed;
    private float individualSwayIntensity;

    private bool hasNotifiedNextSpawn = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        // 生成時に動きをバラつかせる
        moveDirection = (Random.value > 0.5f) ? 1f : -1f; // 50%の確率で反転
        individualSwaySpeed = swaySpeed * Random.Range(0.8f, 1.2f); // 速度に20%のバラつき
        individualSwayIntensity = swayIntensity * Random.Range(0.8f, 1.2f); // 幅に20%のバラつき

        rb.gravityScale = 0;
        rb.simulated = false;
        if (sr != null) sr.enabled = false;

        if (GetComponent<Collider2D>() != null)
        {
            PhysicsMaterial2D mat = new PhysicsMaterial2D();
            mat.friction = 0f;
            mat.bounciness = 0.2f;
            GetComponent<Collider2D>().sharedMaterial = mat;
        }
    }

    void Update()
    {
        if (GameStateManager.Instance == null || GameStateManager.Instance.CurrentState != GameState.Playing)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (!isStarted)
        {
            float camTop = Camera.main.transform.position.y + Camera.main.orthographicSize;
            if (transform.position.y <= camTop + 0.5f)
            {
                isStarted = true;
                rb.simulated = true;
                startTime = Time.time;
                if (sr != null) sr.enabled = true;
            }
            else
            {
                transform.Translate(Vector3.down * sinkSpeed * Time.deltaTime);
                return;
            }
        }

        // --- 移動処理（moveDirection を掛けて左右を入れ替え） ---
        float sway = Mathf.Sin((Time.time - startTime) * individualSwaySpeed) * individualSwayIntensity * moveDirection;
        rb.velocity = new Vector2(sway, -sinkSpeed);

        // スプライトの向きを進行方向に合わせる（必要なら）
        if (sr != null)
        {
            sr.flipX = (sway > 0);
        }

        if (!hasNotifiedNextSpawn && transform.position.y <= 0f)
        {
            NotifySpawn();
        }

        if (transform.position.y < deadLineY)
        {
            if (GameStateManager.Instance != null && GameStateManager.Instance.CurrentState == GameState.Playing)
                GameStateManager.Instance.GameOver();
            Destroy(gameObject);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (GetComponent<Collider2D>().OverlapPoint(mousePos))
                OnTapped();
        }
    }

    void OnTapped()
    {
        SimpleFishSpawner spawner = FindObjectOfType<SimpleFishSpawner>();
        if (spawner != null)
        {
            spawner.OnFishTapped();
        }

        NotifySpawn();

        if (tapSound != null) AudioSource.PlayClipAtPoint(tapSound, Camera.main.transform.position);
        if (ExplosionEffect != null) Instantiate(ExplosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void NotifySpawn()
    {
        if (hasNotifiedNextSpawn) return;
        hasNotifiedNextSpawn = true;

        SimpleFishSpawner spawner = FindObjectOfType<SimpleFishSpawner>();
        if (spawner != null)
        {
            spawner.SpawnNextFish();
        }
    }
}
