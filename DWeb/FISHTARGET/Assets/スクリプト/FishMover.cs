using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class FishMover : MonoBehaviour
{
    public float speed = 5f;
    public float randomAngleRange = 20f;
    public AudioClip hitSE;

    Rigidbody2D rb;
    AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        Vector2 dir = Random.insideUnitCircle.normalized;
        rb.velocity = dir * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 normal = collision.contacts[0].normal;

        Vector2 reflected =
            Vector2.Reflect(rb.velocity.normalized, normal);

        float rand = Random.Range(-randomAngleRange, randomAngleRange);
        reflected = Quaternion.Euler(0, 0, rand) * reflected;

        rb.velocity = reflected.normalized * speed;

        if (hitSE != null)
            audioSource.PlayOneShot(hitSE);
    }

}
