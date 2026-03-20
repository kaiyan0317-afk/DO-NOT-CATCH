using UnityEngine;

public class Fish : MonoBehaviour
{
    public float speed = 5f;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip bounceSE;

    Vector2 dir;
    bool isTarget = false;

    Camera cam;
    float halfWidth;
    float halfHeight;

    void Start()
    {
        cam = Camera.main;

        // 画面サイズ取得
        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * cam.aspect;

        // 初期方向（必ず斜め）
        dir = new Vector2(
            Random.value < 0.5f ? -1 : 1,
            Random.value < 0.5f ? -1 : 1
        ).normalized;
    }

    void Update()
    {
        // 移動（物理なし）
        transform.Translate(dir * speed * Time.deltaTime);

        Vector3 pos = transform.position;
        bool bounced = false;

        // 左右反射
        if (pos.x <= -halfWidth || pos.x >= halfWidth)
        {
            dir.x *= -1;
            bounced = true;
        }

        // 上下反射
        if (pos.y <= -halfHeight || pos.y >= halfHeight)
        {
            dir.y *= -1;
            bounced = true;
        }

        // 効果音
        if (bounced && audioSource && bounceSE)
        {
            audioSource.PlayOneShot(bounceSE);
        }
    }

    public void SetTarget()
    {
        isTarget = true;
    }

    void OnMouseDown()
    {
        if (GameManager.Instance.State != GameManager.GameState.Selected)
            return;

        if (isTarget)
            GameManager.Instance.GameClear();
        else
            GameManager.Instance.GameOver();
    }
}
