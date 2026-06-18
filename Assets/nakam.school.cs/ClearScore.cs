using UnityEngine;
using TMPro;

public class ClearScore : MonoBehaviour
{
    public TMP_Text scoreText;

    void Start()
    {
        // ★残り時間 × 100 をスコアにする例
        int score = GameTimer.lastTimeScore * 100;

        scoreText.text = "SCORE : " + score;
    }
}
