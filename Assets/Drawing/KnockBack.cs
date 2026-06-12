using UnityEngine;
using System.Collections;

public class KnockBack : MonoBehaviour
{
    public float speed = 5f;

    // ノックバック
    public float knockbackForce = 10f;
    public float knockbackTime = 0.2f;

    Rigidbody2D rb;
    Vector2 input;

    bool isKnockback = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // ノックバック中は操作できない
        if (isKnockback)
        {
            input = Vector2.zero;
            return;
        }

        float h = 0f;
        float v = 0f;

        if (Input.GetKey(KeyCode.A)) h = -1f;
        if (Input.GetKey(KeyCode.D)) h = 1f;
        if (Input.GetKey(KeyCode.W)) v = 1f;
        if (Input.GetKey(KeyCode.S)) v = -1f;

        input = new Vector2(h, v).normalized;
    }

    void FixedUpdate()
    {
        // ノックバック中は移動しない
        if (isKnockback) return;

        rb.MovePosition(rb.position + input * speed * Time.fixedDeltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Fireに当たったら
        if (collision.gameObject.CompareTag("Fire"))
        {
            Vector2 dir =
                (transform.position - collision.transform.position).normalized;

            StartCoroutine(DoKnockback(dir));
        }
    }

    IEnumerator DoKnockback(Vector2 dir)
    {
        isKnockback = true;

        rb.linearVelocity = Vector2.zero;

        rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackTime);

        isKnockback = false;
    }
}