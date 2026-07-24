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

    // 消火画像
    public Sprite extinguisherLeftSprite;
    public Sprite extinguisherLeftWalkSprite;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private Vector2 input;
    private bool isKnockback = false;

    // 向き記憶
    private Vector2 lookDirection = Vector2.up;

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
        // 向きを更新
        if (h > 0)
            lookDirection = Vector2.right;
        else if (h < 0)
            lookDirection = Vector2.left;
        else if (v > 0)
            lookDirection = Vector2.up;
        else if (v < 0)
            lookDirection = Vector2.down;

        // Enterを押している間は左右だけ消火画像
        bool usingExtinguisher =
            Input.GetKey(KeyCode.Return) ||
            Input.GetKey(KeyCode.KeypadEnter);

        if (usingExtinguisher)
        {
            if (lookDirection == Vector2.left)
            {
                sr.sprite = walkFrame ? extinguisherLeftWalkSprite : extinguisherLeftSprite;
                sr.flipX = false;
                return;
            }
            else if (lookDirection == Vector2.right)
            {
                sr.sprite = walkFrame ? extinguisherLeftWalkSprite : extinguisherLeftSprite;
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
<<<<<<< HEAD
<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
>>>>>>> 9c222a2a3217e6d8572d20de01310eab334a38a2

    public IEnumerator DoKnockback(Vector2 dir)
=======
    public IEnumerator DoKnockback(Vector2 dir, bool isLaser)
>>>>>>> d013112d02afb80063a459189eadaf976af5a1ff
    {
        if (isKnockback)
            yield break;
>>>>>>> 9c222a2a3217e6d8572d20de01310eab334a38a2

    public IEnumerator DoKnockback(Vector2 dir)
    {
        isKnockback = true;

        rb.linearVelocity = Vector2.zero;

        rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackTime);

        rb.linearVelocity = Vector2.zero;

        isKnockback = false;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hydrant"))
        {
            Debug.Log("消火栓接触");

            if (Input.GetKey(KeyCode.Return))
            {
                Debug.Log("開放");

                collision.gameObject.GetComponent<Hydrant>().isOpened = true;
            }
        }
    }
}