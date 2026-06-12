using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalManager : MonoBehaviour
{
    public RESCON rescueCon;

    private bool player1In = false;
    private bool player2In = false;

    void Update()
    {
        // 救助完了していて
        if (rescueCon.GetRemainingPeople() == 0)
        {
            // プレイヤー2人ともゴール内
            if (player1In && player2In)
            {
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