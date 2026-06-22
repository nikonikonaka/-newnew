using UnityEngine;
using UnityEngine.SceneManagement;
using static GoalManager;

public class ClearSceneManager : MonoBehaviour
{
    public void OnNextButton()
    {
        string next = "shun" + StageData.currentStage;
        SceneManager.LoadScene(next);
    }
}
