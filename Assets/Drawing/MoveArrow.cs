using UnityEngine;
using System.Collections;

public class MoveArrow : MonoBehaviour
{
    public float speed = 5f;

    // ノックバック
    public float knockbackForce = 10f;
    public float knockbackTime = 0.2f;

    // プレイヤー画像
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite sideSprite;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private Vector2 input;
    private bool isKnockback = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isKnockback)
        {
            input = Vector2.zero;
            return;
        }

        float h = 0f;
        float v = 0f;

        // 矢印キー操作
        if (Input.GetKey(KeyCode.LeftArrow)) h = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) h = 1f;
        if (Input.GetKey(KeyCode.UpArrow)) v = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) v = -1f;

        input = new Vector2(h, v).normalized;

        ChangeSprite(h, v);
    }

    void FixedUpdate()
    {
        if (isKnockback) return;

        rb.MovePosition(
            rb.position + input * speed * Time.fixedDeltaTime
        );
    }

    void ChangeSprite(float h, float v)
    {
        // 横
        if (h != 0)
        {
            sr.sprite = sideSprite;

            if (h > 0)
                sr.flipX = false;

            if (h < 0)
                sr.flipX = true;
        }

        // 上
        else if (v > 0)
        {
            sr.sprite = upSprite;
        }

        // 下
        else if (v < 0)
        {
            sr.sprite = downSprite;
        }
    }

void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Fire"))
    {
        // Enterキーを押している間はノックバックしない
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
            return;

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