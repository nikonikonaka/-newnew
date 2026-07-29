
using UnityEngine;
using System.Collections;

public class FireBall : MonoBehaviour
{
    [Header("火球の移動速度")]
    public float speed = 14f;

    [Header("発射してから着火するまでの時間")]
    public float flyTime = 3f;

    [Header("地面の火")]
    public GameObject groundFirePrefab;

    public GameObject owner;

    private Vector2 direction;
    private Rigidbody2D rb;

    private bool isStopped = false;
    private bool hasIgnited = false;

    // 火が燃える時間
    private const float FIRE_DURATION = 10f;

    // 火が縮んで消える時間
    private const float SHRINK_TIME = 3f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 dir)
    {
        direction = dir.normalized;

        // 発射した瞬間から前進
        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }

        // 発射してから3秒後に着火
        StartCoroutine(FireBallSequence());
    }

    private void Update()
    {
        // 停止中は動かさない
        if (isStopped)
        {
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    private IEnumerator FireBallSequence()
    {
        // 3秒間前進
        yield return new WaitForSeconds(flyTime);

        // 3秒後に停止
        isStopped = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // 二重着火防止
        if (hasIgnited)
            yield break;

        hasIgnited = true;

        // 地面に火を出す
        if (groundFirePrefab != null)
        {
            GameObject fire = Instantiate(
                groundFirePrefab,
                transform.position,
                Quaternion.identity
            );

            // 10秒燃えた後、3秒かけて縮んで消える
            StartCoroutine(ShrinkAndDestroyFire(fire));
        }

        // 火球自身を消す
        Destroy(gameObject);
    }

    private IEnumerator ShrinkAndDestroyFire(GameObject fire)
    {
        // 10秒間燃える
        yield return new WaitForSeconds(FIRE_DURATION);

        if (fire == null)
            yield break;

        // 縮み始める前のサイズを保存
        Vector3 startScale = fire.transform.localScale;

        float timer = 0f;

        // 3秒かけてゆっくり縮む
        while (timer < SHRINK_TIME)
        {
            if (fire == null)
                yield break;

            timer += Time.deltaTime;

            float t = timer / SHRINK_TIME;

            // 徐々に小さくする
            fire.transform.localScale = Vector3.Lerp(
                startScale,
                Vector3.zero,
                t
            );

            yield return null;
        }

        // 完全に消す
        Destroy(fire);
    }
}
