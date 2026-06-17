using UnityEngine;
using TMPro;

public class RESCON : MonoBehaviour
{
    public TextMeshProUGUI rescueText;

    GameObject[] people;
    Collider2D myCol; // š ©•ª‚Ì“–‚½‚è”»’è

    void Start()
    {
        people = GameObject.FindGameObjectsWithTag("RESPER");
        myCol = GetComponent<Collider2D>(); // š ©•ª‚ÌCollideræ“¾
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

            // š ©•ª‚ÌCollider‚Æ‘Šè‚ÌCollider‚ªd‚È‚Á‚Ä‚¢‚é‚©”»’è
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
