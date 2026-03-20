using UnityEngine;
using System.Collections;

public class FishMove : MonoBehaviour
{
    public float minSpeed = 7f;

    [Header("Bounce")]
    public float angleRandomRange = 20f;
    public float pushOutDistance = 0.05f;

    [Header("Sound")]
    public AudioClip hitSE;
    public float hitInterval = 0.1f;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Transform spriteTf;
    AudioSource audioSource;

    bool stopped;
    bool resultShake;
    bool justReflected;
    float lastHitTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr) spriteTf = sr.transform;

        audioSource = GetComponent<AudioSource>();
        if (!audioSource)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;

        Vector2 dir = Random.insideUnitCircle.normalized;
        rb.velocity = dir * minSpeed;
    }

    void Update()
    {
        if (resultShake && spriteTf)
        {
            float angle = Mathf.Sin(Time.time * 5f) * 10f;
            spriteTf.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void FixedUpdate()
    {
        if (stopped) return;

        // 反射直後は触らない
        if (justReflected) return;

        rb.velocity = rb.velocity.normalized * minSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (stopped || justReflected) return;

        justReflected = true;
        StartCoroutine(ResetReflectFlag());

        // --- 効果音 ---
        if (Time.time - lastHitTime >= hitInterval && hitSE)
        {
            audioSource.PlayOneShot(hitSE);
            lastHitTime = Time.time;
        }

        // --- 反射処理 ---
        Vector2 normal = collision.contacts[0].normal;
        Vector2 dir = Vector2.Reflect(rb.velocity.normalized, normal);

        float randomAngle = Random.Range(-angleRandomRange, angleRandomRange);
        dir = Quaternion.Euler(0, 0, randomAngle) * dir;

        rb.velocity = dir.normalized * minSpeed;

        // --- 壁から物理的に押し出す（超重要） ---
        rb.position += normal * pushOutDistance;
    }

    IEnumerator ResetReflectFlag()
    {
        yield return new WaitForFixedUpdate();
        justReflected = false;
    }

    // ===== 共通 =====
    public void StopFish()
    {
        stopped = true;
        rb.velocity = Vector2.zero;
        rb.simulated = false;
    }

    // ===== ミス =====
    public void PlayWrongEffect()
    {
        StopFish();
        StartCoroutine(RedBlink());
    }

    IEnumerator RedBlink()
    {
        if (!sr) yield break;

        for (int i = 0; i < 6; i++)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
    }

    // ===== クリア =====
    public void PlayClearEffect()
    {
        StopFish();
        resultShake = true;
    }
}
