using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float speed = 12f;
    public float gravityDelay = 0.25f;
    public float gravity = 12f;
    public float lifeTime = 5f;

    Vector2 direction;
    float timer = 0f;
    bool falling = false;
    bool ignited = false;

    SpriteRenderer sr;
    public Sprite flameSprite;

    Rigidbody2D rb;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        if (ignited) return;

        timer += Time.deltaTime;

        if (!falling)
        {
            // 前へ強く飛ぶ（火炎放射）
            transform.Translate(direction * speed * Time.deltaTime);

            if (timer > gravityDelay)
                falling = true;
        }
        else
        {
            // 落下
            transform.Translate(Vector2.down * gravity * Time.deltaTime);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (ignited) return;

        Ignite();
    }

    void Ignite()
    {
        ignited = true;
        falling = false;

        sr.sprite = flameSprite;

        // ★ここが重要（正しいプロパティ名）
        rb.linearVelocity = Vector2.zero;

        Destroy(gameObject, lifeTime);
    }
}
