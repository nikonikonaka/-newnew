
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage1Change : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Stage1_shun");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

