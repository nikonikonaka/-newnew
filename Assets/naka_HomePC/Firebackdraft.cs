using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Firebackdraft : MonoBehaviour
{
    [Header("窓の画像")]
    public SpriteRenderer windowSprite;
    public Sprite intactWindow;
    public Sprite brokenWindow;

    [Header("爆発設定")]
    public float explosionRadius = 5f;
    public float shakeDuration = 0.18f;    // 短めに
    public float shakeMagnitude = 0.06f;   // 小さめに
    public float shakeDamping = 2.5f;

    [Header("動作設定")]
    public bool debugLogs = false;
    public bool triggerOnce = true;

    bool triggered = false;
    Vector3 windowOriginalLocalPos;
    Vector3 windowOriginalLocalScale;

    void Start()
    {
        if (windowSprite != null)
        {
            windowSprite.sprite = intactWindow;
            windowOriginalLocalPos = windowSprite.transform.localPosition;
            windowOriginalLocalScale = windowSprite.transform.localScale;
        }
        else
        {
            windowOriginalLocalPos = Vector3.zero;
            windowOriginalLocalScale = Vector3.one;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (debugLogs) Debug.Log($"Backdraft Trigger Enter by {other.name} tag={other.tag}");
        if (triggered && triggerOnce) return;

        // プレイヤータグが違う可能性があるならここを調整
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            triggered = true;
            StartCoroutine(BackdraftRoutine());
        }
    }

    IEnumerator BackdraftRoutine()
    {
        if (debugLogs) Debug.Log("Backdraft started");

        // 見た目：壊れた窓に差し替え（位置やスケールはここで直接変えない）
        if (windowSprite != null && brokenWindow != null)
            windowSprite.sprite = brokenWindow;

        // 範囲内の Fire を取得して FireExplosion を呼ぶ（ここでは火の位置やスケールは触らない）
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        List<FireExplosion> fires = new List<FireExplosion>();
        foreach (var c in hits)
        {
            if (c != null && c.CompareTag("Fire"))
            {
                var fe = c.GetComponent<FireExplosion>();
                if (fe != null)
                {
                    fires.Add(fe);
                    if (debugLogs) Debug.Log("Found FireExplosion on " + c.name);
                }
                else
                {
                    if (debugLogs) Debug.LogWarning("Fire tag found but no FireExplosion on " + c.name);
                }
            }
        }

        // 窓の短い揺れ（localPosition を使い、必ず元に戻す）
        if (windowSprite != null)
        {
            float t = 0f;
            Vector3 basePos = windowOriginalLocalPos;
            while (t < shakeDuration)
            {
                t += Time.deltaTime;
                float damp = Mathf.Clamp01(t / shakeDuration);
                float currentMagnitude = Mathf.Lerp(shakeMagnitude, 0f, damp * shakeDamping);
                Vector2 offset = Random.insideUnitCircle * currentMagnitude;
                windowSprite.transform.localPosition = basePos + (Vector3)offset;
                yield return null;
            }
            // 確実に元に戻す
            windowSprite.transform.localPosition = basePos;
        }

        // 各火に爆発トリガーを送る（FireExplosion 側で見た目や物理を処理）
        foreach (var fe in fires)
        {
            if (fe != null)
            {
                if (debugLogs) Debug.Log("Triggering explosion on " + fe.gameObject.name);
                fe.TriggerExplosion();
            }
        }

        if (debugLogs) Debug.Log("Backdraft finished");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
