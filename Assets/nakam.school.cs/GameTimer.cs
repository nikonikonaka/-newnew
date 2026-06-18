using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public static int lastTimeScore = 0; // ★クリア時に使う残り時間

    public float timeLimit = 120f;
    public TMP_Text timerText;

    void Update()
    {
        timeLimit -= Time.deltaTime;

        int rest = Mathf.CeilToInt(timeLimit);
        timerText.text = "Time : " + rest;

        // ★残り時間を保存（常に更新）
        lastTimeScore = rest;

        if (timeLimit <= 0)
        {
            SceneManager.LoadScene("GameOver");
        }
    }
}
