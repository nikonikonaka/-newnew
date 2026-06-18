using UnityEngine;
using UnityEngine.SceneManagement;

public class castamize : MonoBehaviour
{
    // インスペクターで設定できるようにする
    public string loadSceneName;

    public void StartGame()
    {
        SceneManager.LoadScene(loadSceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
