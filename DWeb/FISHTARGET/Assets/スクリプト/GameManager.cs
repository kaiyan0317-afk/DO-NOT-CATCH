using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState
    {
        Ready,      // ‚Ü‚¾’Š‘I‚µ‚Ä‚È‚¢
        Selected,   // “–‚½‚è‹›‚ªŒˆ‚Ü‚Á‚½
        Result      // Ÿ”sŠm’è
    }

    public GameState State { get; private set; }

    void Awake()
    {
        Instance = this;
        State = GameState.Ready;
    }

    public void SetSelected()
    {
        State = GameState.Selected;
    }

    public void GameClear()
    {
        if (State == GameState.Result) return;
        State = GameState.Result;
        Debug.Log("CLEAR");
    }

    public void GameOver()
    {
        if (State == GameState.Result) return;
        State = GameState.Result;
        Debug.Log("GAME OVER");
    }
}
