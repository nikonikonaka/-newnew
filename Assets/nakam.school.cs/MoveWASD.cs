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

    // ホース
    public GameObject hoseUp;
    public GameObject hoseDown;
    public GameObject hoseLeft;
    public GameObject hoseRight;

    private Vector2 lookDirection = Vector2.down;

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

        hoseUp.SetActive(false);
        hoseDown.SetActive(false);
        hoseLeft.SetActive(false);
        hoseRight.SetActive(false);
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
        if (h > 0)
        {
            sr.sprite = walkFrame
                ? rightWalkSprite
                : rightSprite;

            lookDirection = Vector2.right;
        }
        else if (h < 0)
        {
            sr.sprite = walkFrame
                ? leftWalkSprite
                : leftSprite;

            lookDirection = Vector2.left;
        }
        else if (v > 0)
        {
            sr.sprite = walkFrame
                ? upWalkSprite
                : upSprite;

            lookDirection = Vector2.up;
        }
        else if (v < 0)
        {
            sr.sprite = walkFrame
                ? downWalkSprite
                : downSprite;

            lookDirection = Vector2.down;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isUsingHose)
            return;

        if (collision.gameObject.CompareTag("Fire"))
        {
            Vector2 dir =
                (transform.position -
                collision.transform.position).normalized;

            StartCoroutine(DoKnockback(dir));
        }
    }

    IEnumerator DoKnockback(Vector2 dir)
    {
        isKnockback = true;

        rb.linearVelocity = Vector2.zero;

        rb.AddForce(
            dir * knockbackForce,
            ForceMode2D.Impulse
        );

        yield return new WaitForSeconds(knockbackTime);

        rb.linearVelocity = Vector2.zero;

        isKnockback = false;
    }
}