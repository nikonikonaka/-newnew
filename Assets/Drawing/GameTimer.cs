using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public float timeLimit = 120f; // 2•ª
    public TMP_Text timerText;

    void Update()
    {
        timeLimit -= Time.deltaTime;

        // c‚èŠÔ‚ğ®”‚Å•\¦
        timerText.text = "Time : " + Mathf.CeilToInt(timeLimit);

        if (timeLimit <= 0)
        {
            SceneManager.LoadScene("GameOver");
        }
    }
}