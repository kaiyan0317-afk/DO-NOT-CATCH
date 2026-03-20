using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuRoot;

    // メニューボタン
    public void OpenMenu()
    {
        Time.timeScale = 0f;              // ★ 停止
        pauseMenuRoot.SetActive(true);
    }

    // Resume
    public void CloseMenu()
    {
        Time.timeScale = 1f;              // ★ 再開
        pauseMenuRoot.SetActive(false);
    }

    // Quit
    public void QuitToTitle()
    {
        Time.timeScale = 1f;              // ★ 念のため戻す
        SceneManager.LoadScene("SampleScene");
    }
}
