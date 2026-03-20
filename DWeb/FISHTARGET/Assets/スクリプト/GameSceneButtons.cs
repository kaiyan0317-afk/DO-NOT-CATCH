using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // 追加

public class GameSceneButtons : MonoBehaviour
{
    [Header("シーン設定")]
    public string titleSceneName = "SampleScene";
    public string gameSceneName = "GameScene";

    private bool isLoading = false; // 二重読み込み防止

    public void ReloadGame()
    {
        // すでに読み込み中なら何もしない（鈍くなる原因を排除）
        if (isLoading) return;
        isLoading = true;

        // 1. 時間のスケールを強制的に正常化
        Time.timeScale = 1f;

        // 2. オーディオや物理演算の蓄積をクリアするために全停止
        AudioListener.pause = false;

        // 3. 即座にロード開始
        Debug.Log("爆速リトライ実行: " + gameSceneName);
        SceneManager.LoadScene(gameSceneName);
    }

    public void GoToTitle()
    {
        if (isLoading) return;
        isLoading = true;

        Time.timeScale = 1f;
        SimpleFishSpawner.ResetCheckpoint();
        SceneManager.LoadScene(titleSceneName);
    }
}
