using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float speed = 14f;

    [Header("火")]
    public GameObject groundFirePrefab;
    [Range(0f, 1f)]
    public float igniteChance = 0.6f;

    public GameObject owner;

    private Vector2 direction;

    public void Init(Vector2 dir)
    {
        direction = dir.normalized;
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        // 自分には当たらない
        if (owner != null && col.gameObject == owner)
            return;

        // 壁・床・障害物に当たったら火を作る
        if (col.gameObject.CompareTag("Wall")
            )
        {
            if (groundFirePrefab != null && Random.value <= igniteChance)
            {
                Instantiate(groundFirePrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
            return;
        }

        // プレイヤーなどに当たっても消える
        Destroy(gameObject);
    }
}