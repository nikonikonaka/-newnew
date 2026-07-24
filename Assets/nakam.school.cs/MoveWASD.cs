using UnityEngine;
using System.Collections;

public class MoveWASD : MonoBehaviour
{
    public float speed = 5f;

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

    // ホース使用画像
    public Sprite hoseLeftSprite;
    // ホース歩き画像
    public Sprite hoseLeftWalkSprite;
    // ホース
    public GameObject hoseUp;
    public GameObject hoseDown;
    public GameObject hoseLeft;
    public GameObject hoseRight;

    public Vector2 lookDirection = Vector2.up;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private Vector2 input;
    private bool isKnockback = false;
    private bool isUsingHose = false;

    // 歩きアニメ
    private float animTimer;
    private bool walkFrame;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        sr.flipX = false;

        hoseUp.SetActive(false);
        hoseDown.SetActive(false);
        hoseLeft.SetActive(false);
        hoseRight.SetActive(false);

        // ★起動時に上向き画像を強制
        lookDirection = Vector2.up;
        sr.sprite = upSprite;
    }

    void Update()
    {
        HandleMovementInput();

        AnimateWalk();

        HandleHose();

        ChangeSprite(input.x, input.y);
    }

    void FixedUpdate()
    {
        if (!isKnockback)
        {
            rb.MovePosition(
                rb.position +
                input * speed * Time.fixedDeltaTime
            );
        }
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

    void HandleHose()
    {
        isUsingHose =
            Input.GetKey(KeyCode.LeftShift) ||
            Input.GetKey(KeyCode.RightShift);

        hoseUp.SetActive(false);
        hoseDown.SetActive(false);
        hoseLeft.SetActive(false);
        hoseRight.SetActive(false);

        if (!isUsingHose)
            return;

        if (lookDirection == Vector2.up)
        {
            hoseUp.SetActive(true);
        }
        else if (lookDirection == Vector2.down)
        {
            hoseDown.SetActive(true);
        }
        else if (lookDirection == Vector2.left)
        {
            hoseLeft.SetActive(true);
        }
        else if (lookDirection == Vector2.right)
        {
            hoseRight.SetActive(true);
        }
    }

    void HandleMovementInput()
    {
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

    void ChangeSprite(float h, float v)
    {
        // 向きを更新
        if (h > 0)
            lookDirection = Vector2.right;
        else if (h < 0)
            lookDirection = Vector2.left;
        else if (v > 0)
            lookDirection = Vector2.up;
        else if (v < 0)
            lookDirection = Vector2.down;

        // Shift中で左右を向いている時だけホース画像
        if (isUsingHose)
        {
            if (lookDirection == Vector2.left)
            {
                sr.sprite = walkFrame ? hoseLeftWalkSprite : hoseLeftSprite;
                sr.flipX = false;
                return;
            }
            else if (lookDirection == Vector2.right)
            {
                sr.sprite = walkFrame ? hoseLeftWalkSprite : hoseLeftSprite;
                sr.flipX = true;
                return;
            }
        }
        // 通常画像
        sr.flipX = false;

        if (lookDirection == Vector2.right)
        {
            sr.sprite = (h > 0 && walkFrame) ? rightWalkSprite : rightSprite;
        }
        else if (lookDirection == Vector2.left)
        {
            sr.sprite = (h < 0 && walkFrame) ? leftWalkSprite : leftSprite;
        }
        else if (lookDirection == Vector2.up)
        {
            sr.sprite = (v > 0 && walkFrame) ? upWalkSprite : upSprite;
        }
        else if (lookDirection == Vector2.down)
        {
            sr.sprite = (v < 0 && walkFrame) ? downWalkSprite : downSprite;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isUsingHose)
            return;

        Vector2 dir = (transform.position - collision.transform.position).normalized;

        if (collision.gameObject.CompareTag("Fire"))
        {
            StartCoroutine(DoKnockback(dir, false)); // 通常
        }

        if (collision.gameObject.CompareTag("Laser"))
        {
            StartCoroutine(DoKnockback(dir, true)); // ★ Laserだけ強ノックバック
        }
    }

<<<<<<< HEAD
    public IEnumerator DoKnockback(Vector2 dir)
=======

    public IEnumerator DoKnockback(Vector2 dir, bool isLaser)
>>>>>>> d013112d02afb80063a459189eadaf976af5a1ff
    {
        isKnockback = true;

        rb.linearVelocity = Vector2.zero;

        if (isLaser)
        {
            // プレイヤーの進行方向の逆
            dir = -input;

            // 止まっている時は向いている方向の逆
            if (dir == Vector2.zero)
            {
                dir = -lookDirection;
            }
        }

        float force = isLaser ? knockbackForce * 1f : knockbackForce;
        float time = isLaser ? knockbackTime * 1.2f : knockbackTime;

        rb.AddForce(dir.normalized * force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(time);

        rb.linearVelocity = Vector2.zero;
        isKnockback = false;
    }
}