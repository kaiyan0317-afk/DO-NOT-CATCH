using UnityEngine;

public class FishMoverSimple : MonoBehaviour
{
    public float speed = 7f;
    public float angleRandomRange = 20f;

    Vector2 dir;

    void Start()
    {
        dir = Random.insideUnitCircle.normalized;
    }

    void Update()
    {
        transform.position += (Vector3)(dir * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Wall")) return;

        // •Ç‚Ì–@ü‚ğŠÈˆÕæ“¾
        Vector2 normal = GetWallNormal(other);
        if (normal == Vector2.zero) return;

        // ”½Ë
        dir = Vector2.Reflect(dir, normal);

        // Šp“xƒ‰ƒ“ƒ_ƒ€
        float rand = Random.Range(-angleRandomRange, angleRandomRange);
        dir = Quaternion.Euler(0, 0, rand) * dir;
        dir.Normalize();
    }

    Vector2 GetWallNormal(Collider2D col)
    {
        Vector2 pos = transform.position;
        Vector2 closest = col.ClosestPoint(pos);
        return (pos - closest).normalized;
    }
}
