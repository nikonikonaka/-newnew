using UnityEngine;
using System.Collections;

public class MoveArrow : MonoBehaviour
{
    public float speed = 5f;

    // ノックバック
    public float knockbackForce = 10f;
    public float knockbackTime = 0.2f;

    // 待機画像
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite rightSprite;
    public Sprite leftSprite;

    // 歩き画像
    public Sprite upWalkSprite;
    public Sprite downWalkSprite;
    public Sprite rightWalkSprite;
    public Sprite leftWalkSprite;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private Vector2 input;
    private bool isKnockback = false;

    // 向き記憶
    private Vector2 lookDirection = Vector2.down;

    // 歩きアニメ
    private float animTimer;
    private bool walkFrame;

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

        AnimateWalk();
        ChangeSprite(h, v);
    }

    void FixedUpdate()
    {
        if (isKnockback) return;

        rb.MovePosition(
            rb.position + input * speed * Time.fixedDeltaTime
        );
    }

    void AnimateWalk()
    {
        if (input != Vector2.zero)
        {
            animTimer += Time.deltaTime;

            if (animTimer >= 0.2f)
            {
                animTimer = 0f;
                walkFrame = !walkFrame;
            }
        }
        else
        {
            animTimer = 0f;
            walkFrame = false;
        }
    }

    void ChangeSprite(float h, float v)
    {
        if (h > 0)
        {
            sr.sprite = walkFrame ? rightWalkSprite : rightSprite;
            lookDirection = Vector2.right;
        }
        else if (h < 0)
        {
            sr.sprite = walkFrame ? leftWalkSprite : leftSprite;
            lookDirection = Vector2.left;
        }
        else if (v > 0)
        {
            sr.sprite = walkFrame ? upWalkSprite : upSprite;
            lookDirection = Vector2.up;
        }
        else if (v < 0)
        {
            sr.sprite = walkFrame ? downWalkSprite : downSprite;
            lookDirection = Vector2.down;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Fire"))
        {
            // Enterキーを押している間はノックバックしない
            if (Input.GetKey(KeyCode.Return) ||
                Input.GetKey(KeyCode.KeypadEnter))
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

        rb.linearVelocity = Vector2.zero;

        isKnockback = false;
    }
}