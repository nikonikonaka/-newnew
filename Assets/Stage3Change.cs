
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage3Change : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Stage3_shun");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}


