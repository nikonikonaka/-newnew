using UnityEngine;
using System.Collections;

public class FireController : MonoBehaviour
{
    public float extinguishTime = 2f; 
    public float shrinkTime = 0.5f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    float timer = 0f;
    bool isExtinguishing = false;
    bool isFinished = false;

    Collider2D col;
    Vector3 originalScale;

    // ★ Player2 消火アニメ用
    public bool isPlayer2Extinguishing = false; // ← P2Handle からON/OFFされる
    public float player2Timer = 0f;             // ← Player2専用の消火タイマー
    public float player2ExtinguishTime = 5f;    // ← Player2の消火時間
    Vector3 basePos;                            // ← 揺れの基準位置

    private Color normalColor;
    private Color extinguishColor = new Color(0.6f, 0.6f, 1f);

    void Start()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        normalColor = sr.color;
        originalScale = transform.localScale;

        basePos = transform.position; // ← 揺れの基準位置を保存
    }

    void Update()
    {
        if (isFinished) return;

        // ★ Player2 消火アニメ（Enter押してる間）
        if (isPlayer2Extinguishing)
        {
            player2Timer += Time.deltaTime;

            // 色を青くする
            sr.color = extinguishColor;

            // ★ 揺れ（位置がズレないように basePos を基準にする）
            float shake = Mathf.Sin(Time.time * 40f) * 0.05f;
            transform.position = basePos + new Vector3(shake, 0f, 0f);

            // ★ 一定時間で消火
            if (player2Timer >= player2ExtinguishTime)
            {
                ExtinguishImmediately();
            }

            return; // ← Player2消火中はホース処理を無視
        }
        else
        {
            // Player2が離したら元の位置に戻す
            transform.position = basePos;
            sr.color = normalColor;
            player2Timer = 0f;
        }

        // ★ ホースによる消火（元の処理）
        if (isExtinguishing)
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(timer / extinguishTime);
            transform.localScale = originalScale * (1f - progress * 0.3f);

            if (timer >= extinguishTime)
            {
                ExtinguishImmediately();
            }
        }
        else
        {
            timer = 0f;
            transform.localScale = originalScale;
        }
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hose"))
        {
            StartExtinguish();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Hose"))
        {
            StopExtinguish();
        }
    }

    public void StartExtinguish()
    {
        isExtinguishing = true;
    }

    public void StopExtinguish()
    {
        isExtinguishing = false;
    }
}
