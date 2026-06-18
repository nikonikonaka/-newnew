
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage2Change : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("shun2");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

