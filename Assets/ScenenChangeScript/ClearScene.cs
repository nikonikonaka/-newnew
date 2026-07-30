using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearSceneManager : MonoBehaviour
{
    public void OnNextButton()
    {
        string next = "st" + GoalManager.currentStage;  // © C³ƒ|ƒCƒ“ƒg
        SceneManager.LoadScene(next);
    }
}
