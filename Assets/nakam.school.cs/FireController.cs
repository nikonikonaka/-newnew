using UnityEngine;
using System.Collections;

public class FireController : MonoBehaviour
{
    public float extinguishTime = 2f;      // Hoseで消える時間
    public float shrinkTime = 0.5f;        // 小さくなる時間
    public float holdExtinguishTime = 6f;  // Enter長押しで消える時間

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    float timer = 0f;       // Hose用タイマー
    float holdTimer = 0f;   // Enter長押しタイマー

    bool isExtinguishing = false; // Hoseが当たっている
    bool isInside = false;        // Player2が範囲内
    bool isFinished = false;      // 完全に消えたか？

    Collider2D col;
    Vector3 originalScale;

    // Player2 消火アニメ用
    public bool isPlayer2Extinguishing = false;
    public float player2Timer = 0f;
    public float player2ExtinguishTime = 5f;
    Vector3 basePos;

    private Color normalColor;
    private Color extinguishColor = new Color(0.6f, 0.6f, 1f);

    void Start()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        normalColor = sr.color;
        originalScale = transform.localScale;

        basePos = transform.position;
    }

    void Update()
    {
        if (isFinished) return;

        float progress = 0f;

        // ----------------------------------------------------
        // ★ Player2 揺れ消火（最優先）
        // ----------------------------------------------------
        if (isPlayer2Extinguishing)
        {
            player2Timer += Time.deltaTime;

            sr.color = extinguishColor;

            float shake = Mathf.Sin(Time.time * 40f) * 0.05f;
            transform.position = basePos + new Vector3(shake, 0f, 0f);

            if (player2Timer >= player2ExtinguishTime)
            {
                ExtinguishImmediately();
                return;
            }

            return; // Player2消火中は他の処理を無視
        }
        else
        {
            transform.position = basePos;
            sr.color = normalColor;
            player2Timer = 0f;
        }

        // ----------------------------------------------------
        // ★ Hose による消火（StartExtinguish/StopExtinguish 使用）
        // ----------------------------------------------------
        if (isExtinguishing)
        {
            timer += Time.deltaTime;
            progress = Mathf.Max(progress, Mathf.Clamp01(timer / extinguishTime));

            if (timer >= extinguishTime)
            {
                FinishFire();
                return;
            }
        }
        else
        {
            timer = 0f;
        }

        // ----------------------------------------------------
        // ★ Player2 + Enter 長押し消火（6秒）
        // ----------------------------------------------------
        if (isInside && Input.GetKey(KeyCode.Return))
        {
            holdTimer += Time.deltaTime;
            progress = Mathf.Max(progress, Mathf.Clamp01(holdTimer / holdExtinguishTime));

            if (holdTimer >= holdExtinguishTime)
            {
                FinishFire();
                return;
            }
        }
        else
        {
            holdTimer = 0f;
        }

        // ----------------------------------------------------
        // ★ 縮小処理（Hose と Enter の進行度の大きい方）
        // ----------------------------------------------------
        transform.localScale = originalScale * (1f - progress * 0.3f);
    }

    // ----------------------------------------------------
    // ★ 火が完全に消える処理
    // ----------------------------------------------------
    void FinishFire()
    {
        isFinished = true;
        col.enabled = false;
        StartCoroutine(ShrinkAndDestroy());
    }

    public void ExtinguishImmediately()
    {
        if (isFinished) return;

        isFinished = true;
        isPlayer2Extinguishing = false;

        if (col != null)
            col.enabled = false;

        StartCoroutine(ShrinkAndDestroy());
    }

    IEnumerator ShrinkAndDestroy()
    {
        float t = 0f;

        while (t < shrinkTime)
        {
            t += Time.deltaTime;

            float rate = 1f - (t / shrinkTime);
            transform.localScale = originalScale * rate;

            yield return null;
        }

        Destroy(gameObject);
    }

    // ----------------------------------------------------
    // ★ Trigger（StartExtinguish / StopExtinguish を使用）
    // ----------------------------------------------------
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hose"))
            StartExtinguish();

        if (other.CompareTag("Player2"))
            isInside = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Hose"))
            StopExtinguish();

        if (other.CompareTag("Player2"))
            isInside = false;
    }

    // ----------------------------------------------------
    // ★ Start / Stop を残す
    // ----------------------------------------------------
    public void StartExtinguish()
    {
        isExtinguishing = true;
    }

    public void StopExtinguish()
    {
        isExtinguishing = false;
    }
}
