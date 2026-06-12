
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage2Change : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Stage2_shun");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

