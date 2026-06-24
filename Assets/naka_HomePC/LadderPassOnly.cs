using UnityEngine;

public class LadderPassOnly : MonoBehaviour
{
    // ▼ Player2 が通過できる穴のタグ
    //   LadderHandler がハシゴを置く穴と同じタグにする
    public string holeTag = "Hole";

    // ▼ Player2 自身の Collider2D（穴との衝突を無効化するために必要）
    private Collider2D playerCol;

    void Start()
    {
        // ▼ 自分の Collider2D を取得してキャッシュ
        playerCol = GetComponent<Collider2D>();

        if (playerCol == null)
        {
            Debug.LogError($"{name}: Collider2D がありません。穴を通過できません。");
        }
    }

    // ▼ Player1 がハシゴを置いたときに LadderHandler から呼ばれる関数
    //   → この穴だけ Player2 が通れるようにする
    public void UnlockHole(GameObject hole)
    {
        // ▼ 穴の Collider2D を取得
        Collider2D holeCol = hole.GetComponent<Collider2D>();
        if (holeCol == null)
        {
            Debug.LogError($"{hole.name}: Collider2D がありません。");
            return;
        }

        // ▼ Player2 がこの穴だけ通過できるようにする
        Physics2D.IgnoreCollision(playerCol, holeCol, true);

        Debug.Log($"Player2 can pass hole: {hole.name}");
    }
}
