using UnityEngine;
using TMPro;

public class RESCON : MonoBehaviour
{
    public float rescueDistance = 2f;
    public TextMeshProUGUI rescueText;

    GameObject[] people;

    void Start()
    {
        // RESPERタグの人を取得
        people = GameObject.FindGameObjectsWithTag("RESPER");
        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TryRescue();
        }
    }

    void TryRescue()
    {
        foreach (GameObject p in people)
        {
            if (p == null || !p.activeSelf) continue;

            float dist = Vector3.Distance(
                transform.position,
                p.transform.position
            );

            if (dist <= rescueDistance)
            {
                // 救助 → 非表示
                p.SetActive(false);
                UpdateUI();
                break;
            }
        }
    }

    void UpdateUI()
    {
        int remaining = 0;

        foreach (GameObject p in people)
        {
            if (p != null && p.activeSelf)
                remaining++;
        }

        rescueText.text = "People: " + remaining;
    }
    public int GetRemainingPeople()
    {
        int remaining = 0;

        foreach (GameObject p in people)
        {
            if (p != null && p.activeSelf)
                remaining++;
        }

        return remaining;
    }
}