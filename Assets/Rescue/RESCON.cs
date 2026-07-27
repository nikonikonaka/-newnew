using UnityEngine;
using TMPro;

public class RESCON : MonoBehaviour
{
    public TextMeshProUGUI rescueText;

    GameObject[] people;
    Collider2D myCol; // Åö é©ï™ÇÃìñÇΩÇËîªíË

    void Start()
    {
        people = GameObject.FindGameObjectsWithTag("RESPER");
        myCol = GetComponent<Collider2D>(); // Åö é©ï™ÇÃCollideréÊìæ
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

            Collider2D pCol = p.GetComponent<Collider2D>();
            if (pCol == null) continue;

            // Åö é©ï™ÇÃColliderÇ∆ëäéËÇÃColliderÇ™èdÇ»Ç¡ÇƒÇ¢ÇÈÇ©îªíË
            if (myCol.IsTouching(pCol))
            {
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

    private void FindPeople()
    {
        people = GameObject.FindGameObjectsWithTag("RESPER");
    }

    public int GetRemainingPeople()
    {
        if (people == null)
        {
            FindPeople();
        }

        int remaining = 0;

        foreach (GameObject p in people)
        {
            if (p != null && p.activeSelf)
            {
                remaining++;
            }
        }

        return remaining;
    }
}
