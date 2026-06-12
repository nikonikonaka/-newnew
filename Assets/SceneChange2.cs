
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("StageSelect");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
