using UnityEngine;
using System.Collections;

public class FishBounce : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip catchSound;
    [SerializeField][Range(0f, 1f)] private float catchVolume = 1.0f;
    [SerializeField] private GameObject effectPrefab;

    [Header("Behavior Settings")]
    [SerializeField] private float changeDirInterval = 3f;
    [SerializeField][Range(0, 100)] private int changeDirChance = 30;

    private Rigidbody2D rb;
    private AudioSource audioSource;
    private float timer;
    private bool isGameOverTriggered = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        LaunchFish();
    }

    void LaunchFish()
    {
        float randomAngle = Random.Range(0f, 360f);
        Vector2 direction = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
        rb.velocity = direction * speed;
    }

    void Update()
    {
        if (isGameOverTriggered) return;

        timer += Time.deltaTime;
        if (timer >= changeDirInterval)
        {
            timer = 0;
            if (Random.Range(0, 100) < changeDirChance)
            {
                ChangeDirectionRandomly();
            }
        }
    }

    void FixedUpdate()
    {
        if (isGameOverTriggered) return;

        if (rb.velocity != Vector2.zero)
        {
            rb.velocity = rb.velocity.normalized * speed;
        }
    }

    void ChangeDirectionRandomly()
    {
        float currentAngle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        float newAngle = currentAngle + Random.Range(-45f, 45f);
        Vector2 newDir = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
        rb.velocity = newDir * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isGameOverTriggered) return;

        Vector2 normal = collision.contacts[0].normal;
        Vector2 reflectDir = Vector2.Reflect(rb.velocity.normalized, normal);
        float noise = Random.Range(-20f, 20f);
        Quaternion noiseRotation = Quaternion.Euler(0, 0, noise);
        rb.velocity = (noiseRotation * reflectDir) * speed;

        if (GameDirector.instance != null && !GameDirector.instance.isGameActive) return;
        if (hitSound != null) audioSource.PlayOneShot(hitSound);
    }

    private void OnMouseDown()
    {
        if (GameDirector.instance != null && !GameDirector.instance.isGameActive) return;
        if (isGameOverTriggered) return;

        if (gameObject.CompareTag("BlackFish"))
        {
            StartCoroutine(BlackFishTapRoutine());
        }
        else
        {
            if (catchSound != null) AudioSource.PlayClipAtPoint(catchSound, Camera.main.transform.position, catchVolume);
            if (effectPrefab != null) Instantiate(effectPrefab, transform.position, Quaternion.identity);

            Destroy(gameObject);
            GameDirector.instance.Invoke("CheckLevelClear", 0.05f);
        }
    }

    // ★追加：クリア時に呼び出されて動きを止める
    public void StopFish()
    {
        isGameOverTriggered = true;
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }
    }

    IEnumerator BlackFishTapRoutine()
    {
        StopFish(); // 自身の停止処理を呼ぶ

        if (GameDirector.instance != null)
        {
            GameDirector.instance.TriggerBlackFishGameOver();
        }

        float duration = 1.5f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float angle = Mathf.Sin(Time.unscaledTime * 20f) * 15f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
    }
}
