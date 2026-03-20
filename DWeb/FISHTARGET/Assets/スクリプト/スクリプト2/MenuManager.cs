using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // シーン切り替えに必要

public class MenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject menuPanel;

    [Header("Buttons")]
    public Button menuOpenButton; // ヒトデのボタン
    public Button resumeButton;   // 再開（Resume）
    public Button quitButton;     // 終了してタイトルへ（Quit）

    void Start()
    {
        if (menuPanel) menuPanel.SetActive(false);

        if (menuOpenButton) menuOpenButton.onClick.AddListener(OpenMenu);
        if (resumeButton) resumeButton.onClick.AddListener(CloseMenu);
        if (quitButton) quitButton.onClick.AddListener(BackToTitle);
    }

    public void OpenMenu()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(true);
            Time.timeScale = 0f; // 物理反射（魚の動き）を停止
            if (GameDirector.instance != null) GameDirector.instance.isGameActive = false;
        }
    }

    public void CloseMenu()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
            Time.timeScale = 1f; // 物理反射を再開
            if (GameDirector.instance != null) GameDirector.instance.isGameActive = true;
        }
    }

    // ★タイトルシーンへ戻る処理
    public void BackToTitle()
    {
        Time.timeScale = 1f; // 時間を動かしてからシーンを換える（重要！）
        SceneManager.LoadScene("DO NOT CATCH!");
    }
}
