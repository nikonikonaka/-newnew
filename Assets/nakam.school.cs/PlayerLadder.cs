/*using UnityEngine;

public class PlayerLadder : MonoBehaviour
{
    public bool hasLadder = false;
    public GameObject ladderPrefab;
    public Transform placePoint;

    void Update()
    {
        if (hasLadder && Input.GetKeyDown(KeyCode.LeftShift))
        {
            PlaceLadder();
        }
    }

    void PlaceLadder()
    {
        if (ladderPrefab == null || placePoint == null)
        {
            Debug.LogError("PrefabかPlacePointが未設定");
            return;
        }

        GameObject ladder = Instantiate(ladderPrefab, placePoint.position, Quaternion.identity);

        hasLadder = false;

        // ★ここが重要（通れるようにする）
        Collider2D col = ladder.GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        Debug.Log("はしご設置（通行可能）");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 梯子アイテム取得だけ
        if (other.CompareTag("LadderItem"))
        {
            hasLadder = true;
            Destroy(other.gameObject);

            Debug.Log("はしご取得");
        }
    }
}*/