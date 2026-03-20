using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FishGameManager : MonoBehaviour
{
    public static FishGameManager Instance;

    [Header("References")]
    public Transform fishRoot;
    public SpriteRenderer targetDisplay; // 青い丸の見本表示

    [Header("Timing")]
    public float timeLimit = 60f;
    public float resultDelay = 1.5f;

    [Header("Level")]
    public int currentLevel = 1;

    private HashSet<GameObject> remainingFishes = new HashSet<GameObject>();
    private GameObject currentTarget = null;
    private bool gameEnded = false;

    void Awake()
    {
        Instance = this;
        currentLevel = Mathf.Clamp(PlayerPrefs.GetInt("LEVEL", 1), 1, 8);
    }

    void Start()
    {
        SetupLevel();
        StartCoroutine(TimeLimitRoutine());
    }

    // --------------------
    // レベル初期化
    // --------------------
    void SetupLevel()
    {
        remainingFishes.Clear();

        // 使う魚数 = レベル数
        for (int i = 0; i < fishRoot.childCount; i++)
        {
            bool active = i < currentLevel;
            fishRoot.GetChild(i).gameObject.SetActive(active);

            if (active)
                remainingFishes.Add(fishRoot.GetChild(i).gameObject);
        }

        AssignSameSprite();
        targetDisplay.enabled = false;
    }

    // 全部同じ魚にする
    void AssignSameSprite()
    {
        List<Sprite> pool = new List<Sprite>();

        foreach (Transform t in fishRoot)
        {
            SpriteRenderer sr = t.GetComponentInChildren<SpriteRenderer>();
            if (sr != null && !pool.Contains(sr.sprite))
                pool.Add(sr.sprite);
        }

        if (pool.Count == 0) return;

        Sprite chosen = pool[Random.Range(0, pool.Count)];

        foreach (GameObject fish in remainingFishes)
        {
            fish.GetComponentInChildren<SpriteRenderer>().sprite = chosen;
        }
    }

    // --------------------
    // GoalArea から呼ばれる
    // --------------------
    public void SetCurrentTarget(GameObject fish)
    {
        if (gameEnded) return;

        currentTarget = fish;

        if (targetDisplay != null)
        {
            SpriteRenderer sr = fish.GetComponentInChildren<SpriteRenderer>();
            targetDisplay.sprite = sr.sprite;
            targetDisplay.enabled = true;
        }
    }

    public void ClearCurrentTarget(GameObject fish)
    {
        if (currentTarget == fish)
        {
            currentTarget = null;
            targetDisplay.enabled = false;
        }
    }

    // --------------------
    // タップ判定
    // --------------------
    public void OnFishTapped(GameObject fish)
    {
        if (gameEnded) return;
        if (fish != currentTarget) return;

        remainingFishes.Remove(fish);
        Destroy(fish);
        currentTarget = null;
        targetDisplay.enabled = false;

        if (remainingFishes.Count == 0)
        {
            gameEnded = true;
            StartCoroutine(ResultDelay(true));
        }
    }

    IEnumerator TimeLimitRoutine()
    {
        yield return new WaitForSeconds(timeLimit);

        if (!gameEnded)
        {
            gameEnded = true;
            StartCoroutine(ResultDelay(false));
        }
    }

    IEnumerator ResultDelay(bool clear)
    {
        yield return new WaitForSeconds(resultDelay);

        if (clear)
            GameStateManager.Instance.GameClear();
        else
            GameStateManager.Instance.GameOver();
    }
}
