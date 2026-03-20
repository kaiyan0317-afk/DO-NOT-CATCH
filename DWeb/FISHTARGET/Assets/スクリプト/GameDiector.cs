using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameDirector : MonoBehaviour
{
    public static GameDirector instance;
    public static int currentLevel = 1;
    public static int deathCount = 0;

    [Header("UI Panels")]
    public GameObject goUI;
    public GameObject gameOverUI;
    public GameObject gameClearUI;
    public GameObject allClearUI;

    [Header("Level Settings")]
    public int maxLevel = 8;
    public TMPro.TMP_Text levelDisplayText;

    [Header("Timer Settings")]
    public TMPro.TMP_Text timerText;
    public float timeLeft = 60f;

    [Header("Sound")]
    private AudioSource seSource;
    public AudioClip goSE;
    public AudioClip gameOverSE;
    public AudioClip gameClearSE;
    public AudioClip allClearSE;

    [Header("Timing")]
    public float waitBeforeGo = 2f;
    public float goDuration = 1f;
    public float clearDelay = 2f;

    [HideInInspector] public bool isGameActive = false;

    void Awake()
    {
        instance = this;

        if (SceneManager.GetActiveScene().name == "DO NOT CATCH!")
        {
            currentLevel = 1;
            deathCount = 0;
            Debug.Log("Level and DeathCount Reset!");
        }
    }

    void Start()
    {
        seSource = GetComponent<AudioSource>();
        UpdateTimerDisplay();
        UpdateLevelDisplay();
        HideAllUI();

        if (SceneManager.GetActiveScene().name != "DO NOT CATCH!")
        {
            StartCoroutine(StartGoRoutine());
        }
    }

    void Update()
    {
        if (isGameActive)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                timeLeft = 0;
                GameOver();
            }
            UpdateTimerDisplay();
        }
    }

    void UpdateTimerDisplay()
    {
        if (timerText != null)
            timerText.text = Mathf.CeilToInt(timeLeft).ToString();
    }

    void UpdateLevelDisplay()
    {
        if (levelDisplayText != null)
            levelDisplayText.text = "Lv." + currentLevel;
    }

    IEnumerator StartGoRoutine()
    {
        isGameActive = false;
        if (goUI != null)
        {
            yield return new WaitForSeconds(waitBeforeGo);
            goUI.SetActive(true);
            if (goSE) seSource.PlayOneShot(goSE);
            yield return new WaitForSeconds(goDuration);
            goUI.SetActive(false);
        }
        isGameActive = true;
    }

    public void GameOver()
    {
        isGameActive = false;
        Time.timeScale = 0;

        SendLevelToLeaderboard();

        HandleDeathCount();
        HideAllUI();
        if (gameOverUI) gameOverUI.SetActive(true);
        if (gameOverSE) seSource.PlayOneShot(gameOverSE);
    }

    public void TriggerBlackFishGameOver()
    {
        isGameActive = false;
        Time.timeScale = 0;
        StartCoroutine(ShowGameOverDelay());
    }

    IEnumerator ShowGameOverDelay()
    {
        yield return new WaitForSecondsRealtime(1.5f);

        SendLevelToLeaderboard();

        HandleDeathCount();
        HideAllUI();
        if (gameOverUI) gameOverUI.SetActive(true);
        if (gameOverSE) seSource.PlayOneShot(gameOverSE);
    }

    private void HandleDeathCount()
    {
        deathCount++;
        if (deathCount >= 3)
        {
            deathCount = 0;
            if (AdMobService.Instance != null)
            {
                AdMobService.Instance.ShowInterstitial();
            }
        }
    }

    public void CheckLevelClear()
    {
        if (!isGameActive) return;
        FishBounce[] allFish = FindObjectsOfType<FishBounce>();
        bool normalFishExists = false;
        foreach (FishBounce fish in allFish)
        {
            if (!fish.CompareTag("BlackFish")) { normalFishExists = true; break; }
        }
        if (!normalFishExists) StartCoroutine(ClearSequence());
    }

    IEnumerator ClearSequence()
    {
        isGameActive = false;
        FishBounce[] allFish = FindObjectsOfType<FishBounce>();
        foreach (FishBounce fish in allFish) fish.StopFish();
        yield return new WaitForSeconds(clearDelay);
        Time.timeScale = 0;
        HideAllUI();
        if (currentLevel >= maxLevel)
        {
            // 全クリア時のみ送信するように組んでいます
            SendLevelToLeaderboard();

            if (allClearUI) allClearUI.SetActive(true);
            if (allClearSE) seSource.PlayOneShot(allClearSE);
        }
        else
        {
            if (gameClearUI) gameClearUI.SetActive(true);
            if (gameClearSE) seSource.PlayOneShot(gameClearSE);
        }
    }

    public void OnRetryClicked() => ResetAndLoad();
    public void OnNextLevelClicked() { currentLevel++; ResetAndLoad(); }

    public void OnExitClicked()
    {
        SendLevelToLeaderboard();

        currentLevel = 1;
        deathCount = 0;
        Time.timeScale = 1;
        SceneManager.LoadScene("DO NOT CATCH!");
    }

    private void SendLevelToLeaderboard()
    {
        if (GameCenterAuth.Instance != null)
        {
            // Appleで作ったIDを反映しました
            string lbID = "do_not_catch_ranking";
            GameCenterAuth.Instance.ReportScore(currentLevel, lbID);
            Debug.Log("Ranking Sent Level: " + currentLevel);
        }
    }

    private void ResetAndLoad()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void HideAllUI()
    {
        if (goUI) goUI.SetActive(false);
        if (gameOverUI) gameOverUI.SetActive(false);
        if (gameClearUI) gameClearUI.SetActive(false);
        if (allClearUI) allClearUI.SetActive(false);
    }
}
