
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExplainScene : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Explain");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}


