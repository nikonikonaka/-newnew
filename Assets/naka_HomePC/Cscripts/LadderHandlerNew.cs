using UnityEngine;

public class LadderHandlerNew : MonoBehaviour
{
    // ▼ このプレイヤーがハシゴを持っているかどうか
    public bool hasLadder = false;

    // ▼ 拾えるハシゴの一覧（シーン上に置く）
    public GameObject[] pickupLadders;

    // ▼ 設置するハシゴのプレハブ
    public GameObject ladderPrefab;

    // ▼ ハシゴを設置できる穴の一覧
    public GameObject[] holes;

    // ▼ 拾える距離
    public float pickupDistance = 2f;

    // ▼ 設置できる距離
    public float placeDistance = 2.5f;

    // ▼ プレイヤー自身の Collider2D（穴との衝突を無効化するために必要）
    private Collider2D playerCol;

    // ▼ 穴を通れるようにしたいプレイヤーのタグ
    //   Player1, Player2 など後から自由に増やせる
    public string[] passablePlayerTags;

    void Start()
    {
        // ▼ 自分自身の Collider2D を取得してキャッシュ
        //   GetComponent は重いので Start で一度だけ呼ぶのが正しい
        playerCol = GetComponent<Collider2D>();

        Debug.Log("LadderHandler Start");
    }

    void Update()
    {
        // ▼ Shift を押した瞬間に拾う or 設置を実行
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            Debug.Log("SHIFT");

            if (!hasLadder)
                TryPickupLadder();
            else
                TryPlaceLadder();
        }
    }

    // ▼ ハシゴを拾う処理
    void TryPickupLadder()
    {
        foreach (var ladder in pickupLadders)
        {
            if (ladder == null) continue;

            // ▼ プレイヤーとハシゴの距離を測る
            float d = Vector2.Distance(transform.position, ladder.transform.position);

            Debug.Log("pickup check: " + ladder.name + " dist=" + d);

            // ▼ 一定距離以内なら拾える
            if (d <= pickupDistance)
            {
                hasLadder = true;
                Destroy(ladder);

                Debug.Log("GET LADDER");
                return;
            }
        }

        Debug.Log("NO LADDER NEAR");
    }

    // ▼ ハシゴを穴に設置する処理
    void TryPlaceLadder()
    {
        foreach (var hole in holes)
        {
            if (hole == null) continue;

            float d = Vector2.Distance(transform.position, hole.transform.position);

            Debug.Log("hole check: " + hole.name + " dist=" + d);

            if (d > placeDistance) continue;

            PlaceLadder(hole);
            return;
        }

        Debug.Log("NO HOLE IN RANGE");
    }

    // ▼ 実際にハシゴを生成して穴を通れるようにする処理
    void PlaceLadder(GameObject hole)
    {
        // ▼ 穴の Collider2D を取得
        Collider2D holeCol = hole.GetComponent<Collider2D>();
        if (holeCol == null)
        {
            Debug.LogError("hole has no collider");
            return;
        }

        // ▼ 穴の中心位置にハシゴを生成
        Vector3 pos = holeCol.bounds.center;
        GameObject ladder = Instantiate(ladderPrefab, pos, Quaternion.identity);

        Debug.Log("LADDER SPAWN at " + pos);

        // ▼ ハシゴを最前面に表示（見た目のため）
        SpriteRenderer sr = ladder.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = 999;
        }

        // ▼ 穴を通れるようにする処理
        //   → passablePlayerTags に含まれるタグのプレイヤーだけ通過可能にする
        foreach (string tag in passablePlayerTags)
        {
            // ▼ 自分のタグが passablePlayerTags に含まれているかチェック
            if (CompareTag(tag))
            {
                Physics2D.IgnoreCollision(playerCol, holeCol, true);
                Debug.Log("UNLOCK HOLE for tag: " + tag);
            }
        }

        hasLadder = false;

        // ▼ Player2 に穴を解放する
        LadderPassOnly p2 = FindFirstObjectByType<LadderPassOnly>();
        if (p2 != null)
        {
            p2.UnlockHole(hole);
        }

    }


}

