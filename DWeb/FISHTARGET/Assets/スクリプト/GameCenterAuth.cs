using UnityEngine;
using UnityEngine.SocialPlatforms; // これが必要

public class GameCenterAuth : MonoBehaviour
{
    // これを追加することで、GameStateManagerから呼び出せるようになります
    public static GameCenterAuth Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // アプリ起動時にログインを試みる
        SignIn();
    }

    public void SignIn()
    {
        // AppleのGame Center認証を呼び出す
        Social.localUser.Authenticate(success => {
            if (success)
            {
                Debug.Log("Game Center: ログイン成功！ ユーザー名: " + Social.localUser.userName);
            }
            else
            {
                Debug.Log("Game Center: ログイン失敗...");
            }
        });
    }

    // スコア（捕まえた数）を送信する機能
    public void ReportScore(long score, string leaderBoardID)
    {
        if (Social.localUser.authenticated)
        {
            Social.ReportScore(score, leaderBoardID, success => {
                if (success) Debug.Log("ランキング送信成功: " + score);
                else Debug.Log("ランキング送信失敗");
            });
        }
    }
}
