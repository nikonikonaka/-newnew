
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage1Change : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("shun1");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

