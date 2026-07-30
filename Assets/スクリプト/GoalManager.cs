using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalManager : MonoBehaviour
{
    public RESCON rescueCon;

    // ★ インスペクターで設定できる初期ステージ番号
    public int startStage = 1;

    // ★ ClearScene から参照できるグローバルステージ番号
    public static int currentStage;

    private bool player1In = false;
    private bool player2In = false;

    void Start()
    {
        // インスペクター設定を static に反映
        currentStage = startStage;
    }

    void Update()
    {
        if (rescueCon.GetRemainingPeople() == 0)
        {
            if (player1In && player2In)
            {
                currentStage += 1;  // 次ステージへ
                SceneManager.LoadScene("ClearScene");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player1"))
            player1In = true;

        if (other.CompareTag("Player2"))
            player2In = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player1"))
            player1In = false;

        if (other.CompareTag("Player2"))
            player2In = false;
    }
}
