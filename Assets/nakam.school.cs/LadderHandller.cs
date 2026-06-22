using UnityEngine;

public class LadderHandler : MonoBehaviour
{
    public bool hasLadder = false;

    public GameObject[] pickupLadders;
    public GameObject ladderPrefab;

    public GameObject[] holes;

    public float pickupDistance = 2f;
    public float placeDistance = 2.5f;

    private Collider2D playerCol;

    void Start()
    {
        playerCol = GetComponent<Collider2D>();

        // 初期化ログ
        Debug.Log("LadderHandler Start");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("SHIFT");

            if (!hasLadder)
                TryPickupLadder();
            else
                TryPlaceLadder();
        }
    }

    void TryPickupLadder()
    {
        foreach (var ladder in pickupLadders)
        {
            if (ladder == null) continue;

            float d = Vector2.Distance(transform.position, ladder.transform.position);

            Debug.Log("pickup check: " + ladder.name + " dist=" + d);

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

    void PlaceLadder(GameObject hole)
    {
        Collider2D holeCol = hole.GetComponent<Collider2D>();
        if (holeCol == null)
        {
            Debug.LogError("hole has no collider");
            return;
        }

        Vector3 pos = holeCol.bounds.center;

        GameObject ladder = Instantiate(ladderPrefab, pos, Quaternion.identity);

        Debug.Log("LADDER SPAWN at " + pos);

        // 見た目最前面
        SpriteRenderer sr = ladder.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = 999;
        }

        // ★重要：その穴だけ通れるようにする
        if (playerCol != null)
        {
            Physics2D.IgnoreCollision(playerCol, holeCol, true);
            Debug.Log("UNLOCK HOLE: " + hole.name);
        }

        hasLadder = false;
    }
}