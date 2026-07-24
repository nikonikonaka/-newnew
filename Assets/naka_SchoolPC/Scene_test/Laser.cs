using UnityEngine;

public class Laser : MonoBehaviour
{
    public Transform startPoint;
    public float maxDistance = 20f;
    public LayerMask hitMask;

    public bool isActive = true;

    void Update()
    {
        if (!isActive) return;

        RaycastHit2D hit = Physics2D.Raycast(
            startPoint.position,
            startPoint.right,
            maxDistance,
            hitMask
        );

        if (hit.collider != null)
        {
            // がれきで止まる
            if (hit.collider.CompareTag("Debris"))
                return;

            Vector2 dir = (hit.collider.transform.position - startPoint.position).normalized;

            // Player1（WASD）
            if (hit.collider.CompareTag("Player1"))
            {
                MoveWASD p1 = hit.collider.GetComponent<MoveWASD>();
                if (p1 != null)
                    p1.StartCoroutine(p1.DoKnockback(dir));
            }

            // Player2（矢印キー）
            if (hit.collider.CompareTag("Player2"))
            {
                MoveArrow p2 = hit.collider.GetComponent<MoveArrow>();
                if (p2 != null)
                    p2.StartCoroutine(p2.DoKnockback(dir));
            }
        }
    }
}
