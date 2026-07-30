using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RESCON : MonoBehaviour
{
    public TextMeshProUGUI rescueText;

    GameObject[] people;
    Collider2D myCol; // ★ 自分の当たり判定                

    // 近接している人を管理
    HashSet<GameObject> nearby = new HashSet<GameObject>();

    // 自動で触れたら救助するか（Inspectorで切替可）
    public bool autoRescueOnContact = true;

    // 新: Returnでの範囲救出を使うか（Inspectorで切替）
    public bool useRangeRescue = true;

    // 新: 範囲の半径（デフォルト1.2）
    public float rescueRadius = 1.2f;

    void Start()
    {
        people = GameObject.FindGameObjectsWithTag("RESPER");
        myCol = GetComponent<Collider2D>(); // ★ 自分のCollider取得
        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TryRescue();
        }
    }

    // Return押下時は近接しているオブジェクトのうち1体を救助
    void TryRescue()
    {
        // 1) 範囲救出が有効なら distance 判定を優先
        if (useRangeRescue && people != null && people.Length > 0)
        {
            Vector3 myPos = transform.position;
            foreach (GameObject p in people)
            {
                if (p == null || !p.activeSelf) continue;

                // Rigidbody や IsTrigger を要求しない距離チェック
                if (Vector2.Distance(myPos, p.transform.position) <= rescueRadius)
                {
                    Rescue(p);
                    return;
                }
            }
        }

        // 2) nearby（トリガー／衝突で検出）を優先して救助
        foreach (GameObject p in nearby)
        {
            if (p == null || !p.activeSelf) continue;
            Rescue(p);
            return;
        }

        // 3) フォールバック: 従来の Collider.IsTouching 判定
        foreach (GameObject p in people)
        {
            if (p == null || !p.activeSelf) continue;

            Collider2D pCol = p.GetComponent<Collider2D>();
            if (pCol == null) continue;

            // ★ 自分のColliderと相手のColliderが重なっているか判定
            if (myCol != null && myCol.IsTouching(pCol))
            {
                Rescue(p);
                return;
            }
        }
    }

    // 救助処理を共通化
    void Rescue(GameObject p)
    {
        p.SetActive(false);
        nearby.Remove(p);
        UpdateUI();
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

    // トリガーで接触（RESPERタグを想定）
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;
        if (!other.CompareTag("RESPER")) return;

        var go = other.gameObject;
        nearby.Add(go);

        if (autoRescueOnContact && go.activeSelf)
        {
            Rescue(go);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other == null) return;
        if (!other.CompareTag("RESPER")) return;

        nearby.Remove(other.gameObject);
    }

    // 衝突イベントにも対応（トリガーでない場合）
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null) return;
        var otherCol = collision.collider;              
        if (otherCol == null) return;
        if (!otherCol.CompareTag("RESPER")) return;

        var go = otherCol.gameObject;
        nearby.Add(go);

        if (autoRescueOnContact && go.activeSelf)
        {
            Rescue(go);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision == null) return;
        var otherCol = collision.collider;
        if (otherCol == null) return;
        if (!otherCol.CompareTag("RESPER")) return;

        nearby.Remove(otherCol.gameObject);
    }

    // シーンビューで範囲を可視化（選択時）
    void OnDrawGizmosSelected()
    {
        if (!useRangeRescue) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, rescueRadius);
    }
}
