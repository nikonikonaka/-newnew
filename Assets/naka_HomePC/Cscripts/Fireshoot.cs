
using UnityEngine;

public class Fireshoot : MonoBehaviour
{
    [Header("”­ËŒû")]
    public Transform gate;

    [Header("”­Ë‚·‚é’e‚ÌƒvƒŒƒnƒu")]
    public GameObject bulletPrefab;

    [Header("’e‚Ì‘¬“x")]
    public float speed = 14f;

    [Header("”­ËŠÔŠu")]
    public float fireInterval = 1f;

    // ’e‚ª”ò‚ÔŠÔ
    private const float BULLET_LIFE_TIME = 3f;

    private float fireTimer = 0f;

    private void Update()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireInterval)
        {
            fireTimer = 0f;
            Shoot();
        }
    }

    private void Shoot()
    {
        // ”­ËŒû‚ªİ’è‚³‚ê‚Ä‚¢‚È‚¢ê‡
        if (gate == null)
            return;

        // ’eƒvƒŒƒnƒu‚ªİ’è‚³‚ê‚Ä‚¢‚È‚¢ê‡
        if (bulletPrefab == null)
            return;

        // Gate‚ÌˆÊ’u‚ÆŒü‚«‚Å’e‚ğ¶¬
        GameObject bullet = Instantiate(
            bulletPrefab,
            gate.position,
            gate.rotation
        );

        // Gate‚Ìã•ûŒü‚Ö3•bŠÔˆÚ“®
        StartCoroutine(MoveBullet(bullet, gate.up));
    }

    private System.Collections.IEnumerator MoveBullet(
        GameObject bullet,
        Vector2 direction
    )
    {
        float timer = 0f;

        while (timer < BULLET_LIFE_TIME)
        {
            // ’e‚ª‘¶İ‚µ‚È‚­‚È‚Á‚½‚çI—¹
            if (bullet == null)
                yield break;

            // Gate‚©‚çæ“¾‚µ‚½•ûŒü‚Ö‘Oi
            bullet.transform.Translate(
                direction.normalized * speed * Time.deltaTime,
                Space.World
            );

            timer += Time.deltaTime;

            yield return null;
        }

        // 3•bŒã‚É’e‚ğÁ‚·
        if (bullet != null)
        {
            Destroy(bullet);
        }
    }
}

