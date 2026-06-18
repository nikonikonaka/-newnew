using UnityEngine;

public class PlayerLadder : MonoBehaviour
{
    public bool hasLadder = false;          // はしごを持っているか
    public GameObject ladderPrefab;         // 設置するはしご
    public Transform placePoint;            // 設置位置（プレイヤーの前）

    void Update()
    {
        // はしごを持っている時だけ設置できる
        if (hasLadder && Input.GetKeyDown(KeyCode.E))
        {
            PlaceLadder();
        }
    }

    void PlaceLadder()
    {
        // はしごを設置（ここで壁が出現する）
        Instantiate(ladderPrefab, placePoint.position, placePoint.rotation);

        // 設置したので手持ちから消える
        hasLadder = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // はしごアイテムを拾う
        if (other.CompareTag("LadderItem"))
        {
            hasLadder = true;
            Destroy(other.gameObject);
        }
    }
}
