using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState { Ready, Playing, GameOver, Clear }

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    [Header("UI Panels")]
    public GameObject goUI;
    public GameObject gameOverUI;
    public GameObject gameClearUI;

    [Header("Independent Buttons")]
    public Button retryButton;
    public Button exitButton;

    [Header("Sound")]
    public AudioSource seSource;
    public AudioClip goSE;
    public AudioClip gameOverSE;
    public AudioClip gameClearSE;

    [Header("Timing")]
    public float waitBeforeGo = 2f;
    public float goDuration = 1f;

    private static int retryCount = 0;
    public GameState CurrentState { get; private set; }
    public bool CanInput => CurrentState == GameState.Playing;

    void Awake()
    {
        if (Instance == null) { Instance = this; Time.timeScale = 1; }
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        if (goUI != null) goUI.SetActive(false);
        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (gameClearUI != null) gameClearUI.SetActive(false);

        if (retryButton != null) retryButton.gameObject.SetActive(false);
        if (exitButton != null) exitButton.gameObject.SetActive(false);

        StartCoroutine(GameStartSequence());
    }

    IEnumerator GameStartSequence()
    {
        SetState(GameState.Ready);
        yield return new WaitForSeconds(waitBeforeGo);
        if (goUI != null)
        {
            goUI.SetActive(true);
            if (seSource != null && goSE != null) seSource.PlayOneShot(goSE);
        }
        yield return new WaitForSeconds(goDuration);
        if (goUI != null) goUI.SetActive(false);

        SetState(GameState.Playing);
    }

    public void GameOver()
    {
        if (CurrentState != GameState.Playing) return;
        SetState(GameState.GameOver);

        // タップ数を送信
        SendScoreToLeaderboard();

        if (gameOverUI != null) gameOverUI.SetActive(true);
        if (retryButton != null) retryButton.gameObject.SetActive(true);
        if (exitButton != null) exitButton.gameObject.SetActive(true);

        if (seSource != null && gameOverSE != null) seSource.PlayOneShot(gameOverSE);
    }

    public void GameClear()
    {
        if (CurrentState != GameState.Playing) return;
        SetState(GameState.Clear);

        SendScoreToLeaderboard();

        if (gameClearUI != null) gameClearUI.SetActive(true);
        if (seSource != null && gameClearSE != null) seSource.PlayOneShot(gameClearSE);
    }

    public void RetryGame()
    {
        retryCount++;
        if (retryCount >= 3)
        {
            retryCount = 0;
            if (AdMobService.Instance != null)
                AdMobService.Instance.ShowInterstitial(ExecuteRetry);
            else
                ExecuteRetry();
        }
        else
        {
            ExecuteRetry();
        }
    }

    public void BackToTitle()
    {
        if (CurrentState == GameState.Playing)
        {
            SendScoreToLeaderboard();
        }

        Time.timeScale = 1;
        SceneManager.LoadScene("TitleScene");
    }

    private void SendScoreToLeaderboard()
    {
        SimpleFishSpawner spawner = FindObjectOfType<SimpleFishSpawner>();
        if (spawner != null && GameCenterAuth.Instance != null)
        {
            // Appleで作ったIDを反映しました
            string lbID = "fish_target_ranking";
            GameCenterAuth.Instance.ReportScore(spawner.GetTapCounter(), lbID);
            Debug.Log("Ranking Sent Score: " + spawner.GetTapCounter());
        }
    }

    void ExecuteRetry()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void SetState(GameState state) { CurrentState = state; }
}
