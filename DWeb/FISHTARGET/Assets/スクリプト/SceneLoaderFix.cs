using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderFix : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
