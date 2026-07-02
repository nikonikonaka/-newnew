
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage1Change : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("st1");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

