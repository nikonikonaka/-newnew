using UnityEngine;
using UnityEngine.SceneManagement;
using static GoalManager;

public class ClearSceneManager : MonoBehaviour
{
    public void OnNextButton()
    {
        string next = "st" + StageData.currentStage;
        SceneManager.LoadScene(next);
    }
}
