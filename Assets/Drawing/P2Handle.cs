using UnityEngine;

public class P2Handle : MonoBehaviour
{
    public float extinguishRadius = 1.2f; // ★ 当たり判定を広げる（距離ではなく円判定）
    public float extinguishTime = 5f;

    private float timer = 0f;
    private GameObject targetFire;

    private SpriteRenderer sr;
    private Color normalColor;
    private Color extinguishColor = new Color(0.7f, 0.7f, 1f);

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            normalColor = sr.color;
    }

    void Update()
    {
        // ★ Enter を押していないときだけターゲット更新（火は1つだけ固定）
        if (!Input.GetKey(KeyCode.Return))
            targetFire = FindFireByCollider();

        if (targetFire == null)
        {
            StopExtinguish();
            return;
        }

        if (Input.GetKey(KeyCode.Return))
        {
            timer += Time.deltaTime;

            // ★ FireController に「Player2消火中」を通知
            FireController fire = targetFire.GetComponent<FireController>();
            if (fire != null)
                fire.isPlayer2Extinguishing = true;

            if (sr != null)
                sr.color = extinguishColor;

            // ★ 押しっぱなしで時間が進む
            if (timer >= extinguishTime)
            {
                if (fire != null)
                    fire.ExtinguishImmediately();

                timer = 0f;

                if (sr != null)
                    sr.color = normalColor;
            }
        }
        else
        {
            StopExtinguish();
        }
    }

    void StopExtinguish()
    {
        timer = 0f;

        if (sr != null)
            sr.color = normalColor;

        // ★ FireController に「消火してない」を通知
        if (targetFire != null)
        {
            FireController fire = targetFire.GetComponent<FireController>();
            if (fire != null)
                fire.isPlayer2Extinguishing = false;
        }
    }

    // ★ Collider を使った当たり判定（距離より正確）
    GameObject FindFireByCollider()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, extinguishRadius);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Fire"))
                return hit.gameObject;
        }

        return null;
    }

    // ★ Sceneビューで当たり判定の円を可視化
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, extinguishRadius);
    }
}
