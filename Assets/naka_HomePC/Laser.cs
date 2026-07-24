using UnityEngine;

public class LightningLaser : MonoBehaviour
{
    public Transform firePoint;
    public float maxDistance = 20f;
    public LineRenderer line;
    public string wallTag = "Wall";

    public int segments = 4;
    public float jaggedness = 0.1f;

    // ノックバックの連続発動防止
    public float knockbackInterval = 0.3f;
    private float nextKnockbackTime;

    void Update()
    {
        Vector2 start = firePoint.position;
        Vector2 dir = firePoint.up.normalized;

        RaycastHit2D hit = Physics2D.Raycast(start, dir, maxDistance);

        Vector2 endPos = start + dir * maxDistance;

        if (hit.collider != null)
        {
            endPos = hit.point;   // ★プレイヤーでも壁でもここで止める

            // Player1
            MoveWASD player1 = hit.collider.GetComponent<MoveWASD>();
            if (player1 != null && Time.time >= nextKnockbackTime)
            {
                nextKnockbackTime = Time.time + knockbackInterval;
                player1.StartCoroutine(player1.DoKnockback(Vector2.zero, true));
            }

            // Player2
            MoveArrow player2 = hit.collider.GetComponent<MoveArrow>();
            if (player2 != null && Time.time >= nextKnockbackTime)
            {
                nextKnockbackTime = Time.time + knockbackInterval;
                player2.StartCoroutine(player2.DoKnockback(Vector2.zero, true));
            }
        }

        line.positionCount = segments + 1;

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            Vector2 pos = Vector2.Lerp(start, endPos, t);

            if (i != 0 && i != segments)
            {
                Vector2 perpendicular = new Vector2(-dir.y, dir.x);
                pos += perpendicular * Random.Range(-jaggedness, jaggedness);
            }

            line.SetPosition(i, pos);
        }
    }
}