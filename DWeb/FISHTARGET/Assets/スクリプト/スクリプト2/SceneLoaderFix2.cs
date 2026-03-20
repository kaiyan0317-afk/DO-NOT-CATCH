using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderFix2 : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.LoadScene("DO NOT CATCH! Game");
    }
}