using UnityEngine;
using TMPro;

public class RESCON_p : MonoBehaviour
{
    [Header("救助人数表示")]
    public TextMeshProUGUI rescueText;

    private GameObject[] people;
    private Collider2D myCol;

    // ==========================================
    // 初期化
    // ==========================================

    private void Awake()
    {
        // 自分のColliderを取得
        myCol = GetComponent<Collider2D>();

        // RESPERを取得
        FindPeople();
    }

    // ==========================================
    // ゲーム開始
    // ==========================================

    private void Start()
    {
        UpdateUI();
    }

    // ==========================================
    // 毎フレーム
    // ==========================================

    private void Update()
    {
        // Player1タグのオブジェクトだけ
        // Shiftキーで救助
        if (CompareTag("Player1") &&
            (Input.GetKeyDown(KeyCode.LeftShift) ||
             Input.GetKeyDown(KeyCode.RightShift)))
        {
            TryRescue();
        }
    }

    // ==========================================
    // RESPERを取得
    // ==========================================

    private void FindPeople()
    {
        people =
            GameObject.FindGameObjectsWithTag("RESPER");
    }

    // ==========================================
    // 救助処理
    // ==========================================

    private void TryRescue()
    {
        // Colliderがない場合
        if (myCol == null)
        {
            Debug.LogWarning(
                "RESCON_p: Player1にCollider2Dがありません。"
            );

            return;
        }

        // peopleがnullの場合は再取得
        if (people == null)
        {
            FindPeople();
        }

        foreach (GameObject p in people)
        {
            // 救助対象が存在しない
            // または非表示ならスキップ
            if (p == null ||
                !p.activeSelf)
            {
                continue;
            }

            // 救助対象のCollider取得
            Collider2D pCol =
                p.GetComponent<Collider2D>();

            // Colliderがない場合
            if (pCol == null)
            {
                continue;
            }

            // ==========================================
            // Player1とRESPERが接触しているか
            // ==========================================

            if (myCol.IsTouching(pCol))
            {
                // 救助成功
                p.SetActive(false);

                // 救助人数更新
                UpdateUI();

                Debug.Log(
                    "Player1が " +
                    p.name +
                    " を救助しました！"
                );

                // 1回のShiftで1人だけ
                break;
            }
        }
    }

    // ==========================================
    // UI更新
    // ==========================================

    private void UpdateUI()
    {
        if (people == null)
        {
            FindPeople();
        }

        int remaining = 0;

        foreach (GameObject p in people)
        {
            if (p != null &&
                p.activeSelf)
            {
                remaining++;
            }
        }

        if (rescueText != null)
        {
            rescueText.text =
                "People: " +
                remaining;
        }
    }

    // ==========================================
    // 残り救助人数
    // GoalManagerから呼び出し可能
    // ==========================================

    public int GetRemainingPeople()
    {
        if (people == null)
        {
            FindPeople();
        }

        int remaining = 0;

        foreach (GameObject p in people)
        {
            if (p != null &&
                p.activeSelf)
            {
                remaining++;
            }
        }

        return remaining;
    }
}