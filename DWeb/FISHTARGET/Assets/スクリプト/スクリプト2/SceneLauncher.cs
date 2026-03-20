using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // コルーチンに必要

public class SceneLauncher : MonoBehaviour
{
    [Header("Sound Settings")]
    public AudioSource audioSource; // インスペクターでAudioSourceをドラッグ＆ドロップ
    public AudioClip buttonClickSE; // 鳴らしたい音のファイルをドラッグ＆ドロップ
    public float delayBeforeLoad = 0.3f; // 音を響かせるための短い待ち時間

    // 魚のゲーム（DO NOT CATCH!）を開始する
    public void LaunchFishGame()
    {
        StartCoroutine(PlaySoundAndLoad("DO NOT CATCH!"));
    }

    // もう一つのゲーム（SampleScene）を開始する
    public void LaunchSampleGame()
    {
        StartCoroutine(PlaySoundAndLoad("SampleScene"));
    }

    // 音を鳴らしてからシーンをロードするコルーチン
    IEnumerator PlaySoundAndLoad(string sceneName)
    {
        // 音を再生
        if (audioSource != null && buttonClickSE != null)
        {
            audioSource.PlayOneShot(buttonClickSE);
        }

        // 少し待つ（音がブツ切りになるのを防ぐ）
        yield return new WaitForSecondsRealtime(delayBeforeLoad);

        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }

    // アプリを終了する
    public void QuitGame()
    {
        Application.Quit();
    }
}
