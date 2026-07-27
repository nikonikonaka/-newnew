using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform[] players;       // プレイヤー複数
    public GameObject fireBallPrefab;
    public float shootInterval = 1.5f;
    public float moveSpeed = 2f;

    float timer;
    Vector2 fireDir;
    Transform target;

    Rigidbody2D rb;

    // ★上下左右の画像
    public Sprite spriteUp;
    public Sprite spriteDown;
    public Sprite spriteLeft;
    public Sprite spriteRight;

    SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (rb == null)
            Debug.LogError("EnemyAI に Rigidbody2D が必要です");

        if (sr == null)
            Debug.LogError("EnemyAI に SpriteRenderer が必要です");
    }

    void Update()
    {
        if (players == null || players.Length == 0) return;

        target = GetNearestPlayer();
        if (target == null) return;

        float dist = Vector2.Distance(transform.position, target.position);

        if (dist > 5f)
        {
            MoveToward(target.position);
        }
        else if (dist < 2f)
        {
            MoveAway(target.position);
        }
        else
        {
            AimDirection();
            ChangeSprite();   // ★画像切り替え
            Attack();
        }
    }

    Transform GetNearestPlayer()
    {
        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (Transform p in players)
        {
            if (p == null) continue;

            float d = Vector2.Distance(transform.position, p.position);
            if (d < minDist)
            {
                minDist = d;
                nearest = p;
            }
        }
        return nearest;
    }

    void AimDirection()
    {
        Vector2 dir = target.position - transform.position;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            fireDir = (dir.x > 0) ? Vector2.right : Vector2.left;
        else
            fireDir = (dir.y > 0) ? Vector2.up : Vector2.down;
    }

    // ★上下左右で画像を切り替える
    void ChangeSprite()
    {
        if (fireDir == Vector2.up)
            sr.sprite = spriteUp;
        else if (fireDir == Vector2.down)
            sr.sprite = spriteDown;
        else if (fireDir == Vector2.left)
            sr.sprite = spriteLeft;
        else if (fireDir == Vector2.right)
            sr.sprite = spriteRight;
    }

    void Attack()
    {
        timer += Time.deltaTime;
        if (timer > shootInterval)
        {
            Shoot();
            timer = 0;
        }
    }

    void Shoot()
    {
        GameObject fb = Instantiate(fireBallPrefab, transform.position, Quaternion.identity);
        FireBall fire = fb.GetComponent<FireBall>();
        if (fire != null)
            fire.Init(fireDir);
    }

    void MoveToward(Vector3 pos)
    {
        Vector2 dir = (pos - transform.position).normalized;
        rb.MovePosition(rb.position + dir * moveSpeed * Time.deltaTime);
    }

    void MoveAway(Vector3 pos)
    {
        Vector2 dir = (transform.position - pos).normalized;
        rb.MovePosition(rb.position + dir * moveSpeed * Time.deltaTime);
    }
}
