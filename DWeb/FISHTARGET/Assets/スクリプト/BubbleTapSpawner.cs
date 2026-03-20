using UnityEngine;
using System.Collections;

public class BubbleTapSpawner : MonoBehaviour
{
    [Header("Bubble")]
    public GameObject bubblePrefab;
    public int minBubble = 3;
    public int maxBubble = 7;

    [Header("Movement")]
    public float riseSpeedMin = 1.0f;
    public float riseSpeedMax = 2.0f;
    public float lifeTime = 5f; // これも残しておきますが、画面外判定が優先されます

    [Header("Spawn Spread")]
    public float spawnRadius = 0.4f;
    public float spawnIntervalMin = 0.05f;
    public float spawnIntervalMax = 0.15f;

    [Header("Sound")]
    public AudioSource seSource;
    public AudioClip tapSE;

    void Update()
    {
        // GameStateManagerを介した入力制限
        if (GameStateManager.Instance != null && !GameStateManager.Instance.CanInput) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 tapPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            tapPos.z = 0f;

            StartCoroutine(SpawnBubbles(tapPos));

            if (tapSE != null && seSource != null)
                seSource.PlayOneShot(tapSE);
        }
    }

    IEnumerator SpawnBubbles(Vector3 center)
    {
        int count = Random.Range(minBubble, maxBubble + 1);

        for (int i = 0; i < count; i++)
        {
            Vector2 offset = Random.insideUnitCircle.normalized
                             * Random.Range(spawnRadius * 0.5f, spawnRadius);

            Vector3 spawnPos = center + new Vector3(offset.x, offset.y, 0f);

            GameObject bubble = Instantiate(bubblePrefab, spawnPos, Quaternion.identity);

            float speed = Random.Range(riseSpeedMin, riseSpeedMax);
            StartCoroutine(MoveUp(bubble.transform, speed));

            // 画面外に出なくても、念のため一定時間で消去する設定
            Destroy(bubble, lifeTime);

            yield return new WaitForSeconds(Random.Range(spawnIntervalMin, spawnIntervalMax));
        }
    }

    IEnumerator MoveUp(Transform bubble, float speed)
    {
        float time = 0f;
        float swayAmount = Random.Range(0.2f, 0.4f);
        float swaySpeed = Random.Range(1.5f, 3f);

        // カメラの上端位置を計算（orthographicSizeは中心から端までの距離）
        float camTop = Camera.main.transform.position.y + Camera.main.orthographicSize;

        while (bubble != null)
        {
            time += Time.deltaTime;

            float sway = Mathf.Sin(time * swaySpeed) * swayAmount;
            bubble.Translate(new Vector3(sway * Time.deltaTime, speed * Time.deltaTime, 0));

            // --- 【追加】画面外（カメラ上端 + 余裕分）に出たら削除 ---
            if (bubble.position.y > camTop + 1.0f)
            {
                Destroy(bubble.gameObject);
                yield break; // 泡が消えたので、このコルーチン（ループ）を終了する
            }

            yield return null;
        }
    }
}
