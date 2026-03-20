using UnityEngine;
using TMPro;

public class SimpleFishSpawner : MonoBehaviour
{
    public GameObject[] fishPrefabs;
    public float spawnWidth = 2.0f;

    [Header("スピード設定")]
    public float currentSinkSpeed = 2.0f;
    public float speedIncrement = 1.0f;
    public int countPerLevel = 20;

    [Header("UI設定")]
    public TextMeshProUGUI counterText;
    public GameObject checkpointUI;

    [Header("サウンド設定")]
    public AudioSource audioSource;
    public AudioClip checkpointSE;

    private static int checkpointScore = 0;
    private int tapCounter = 0;
    private bool firstSpawned = false;
    private bool isWaitingAtCheckpoint = false;

    void Start()
    {
        tapCounter = checkpointScore;
        int levels = tapCounter / countPerLevel;

        currentSinkSpeed = currentSinkSpeed + (levels * speedIncrement);

        UpdateCounterUI();

        if (checkpointUI != null) checkpointUI.SetActive(false);

        if (tapCounter > 0 && tapCounter % countPerLevel == 0)
        {
            EnterCheckpoint();
        }
    }

    public static void ResetCheckpoint()
    {
        checkpointScore = 0;
    }

    // 【追加】現在のタップ数を返す（ランキング送信で使用）
    public int GetTapCounter()
    {
        return tapCounter;
    }

    void Update()
    {
        if (GameStateManager.Instance == null || GameStateManager.Instance.CurrentState != GameState.Playing)
            return;

        if (!firstSpawned && !isWaitingAtCheckpoint)
        {
            SpawnNextFish();
            firstSpawned = true;
        }
    }

    public void OnFishTapped()
    {
        tapCounter++;
        UpdateCounterUI();

        if (tapCounter % countPerLevel == 0)
        {
            EnterCheckpoint();
        }
    }

    void EnterCheckpoint()
    {
        checkpointScore = tapCounter;
        currentSinkSpeed += speedIncrement;
        isWaitingAtCheckpoint = true;

        if (checkpointUI != null) checkpointUI.SetActive(true);
        PlayCheckpointSound();

        Invoke("ResumeGameFromCheckpoint", 1.0f);
    }

    void PlayCheckpointSound()
    {
        if (audioSource != null && checkpointSE != null)
        {
            audioSource.PlayOneShot(checkpointSE);
        }
    }

    public void ResumeGameFromCheckpoint()
    {
        isWaitingAtCheckpoint = false;
        if (checkpointUI != null) checkpointUI.SetActive(false);
        SpawnNextFish();
    }

    void UpdateCounterUI()
    {
        if (counterText != null) counterText.text = tapCounter.ToString();
    }

    public void SpawnNextFish()
    {
        if (isWaitingAtCheckpoint) return;
        CancelInvoke("Spawn");
        Invoke("Spawn", 0.2f);
    }

    void Spawn()
    {
        if (GameStateManager.Instance == null || GameStateManager.Instance.CurrentState != GameState.Playing)
            return;

        if (isWaitingAtCheckpoint) return;

        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;
        float randomX = Random.Range(-camWidth * 0.8f, camWidth * 0.8f);

        Vector3 spawnPos = new Vector3(randomX, Camera.main.transform.position.y + camHeight + 2.0f, 0);

        if (fishPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, fishPrefabs.Length);
            GameObject newFish = Instantiate(fishPrefabs[randomIndex], spawnPos, Quaternion.identity);

            FishControl fc = newFish.GetComponent<FishControl>();
            if (fc != null)
            {
                fc.sinkSpeed = currentSinkSpeed;
            }
        }
    }
}
