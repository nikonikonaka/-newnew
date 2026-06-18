using UnityEngine;
using System.Collections;

public class FireController : MonoBehaviour
{
<<<<<<< HEAD
    public float extinguishTime = 2f; // Hoseで消える時間
    public float shrinkTime = 0.5f;   // 小さくなる時間
    public float holdExtinguishTime = 6f; // Enter長押しで消える時間
=======
    public float extinguishTime = 2f; 
    public float shrinkTime = 0.5f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
>>>>>>> af46d1ebaf370ed0642b28ca2bf482daf248d8ac

    float timer = 0f;          // Hose用タイマー
    float holdTimer = 0f;      // Enter長押しタイマー

    bool isExtinguishing = false; // Hoseが当たっている
    bool isInside = false;        // Player2が範囲内
    bool isFinished = false;      // 完全に消えたか？

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

<<<<<<< HEAD
        float progress = 0f; // 最終的に縮ませる割合

        // ----------------------------------------------------
        // ★Hose による消火（2秒）
        // ----------------------------------------------------
        if (isExtinguishing)
        {
            timer += Time.deltaTime;
            progress = Mathf.Max(progress, Mathf.Clamp01(timer / extinguishTime));

            if (timer >= extinguishTime)
            {
                FinishFire();
                return;
=======
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
>>>>>>> af46d1ebaf370ed0642b28ca2bf482daf248d8ac
            }
        }
        else
        {
            timer = 0f;
<<<<<<< HEAD
=======
            transform.localScale = originalScale;
>>>>>>> af46d1ebaf370ed0642b28ca2bf482daf248d8ac
        }

        // ----------------------------------------------------
        // ★Player2 + Enter 長押し消火（6秒）
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
        // ★縮小処理（Hose と Enter の進行度の大きい方を採用）
        // ----------------------------------------------------
        transform.localScale = originalScale * (1f - progress * 0.3f);
    }

<<<<<<< HEAD
    // ----------------------------------------------------
    // ★火が完全に消える処理
    // ----------------------------------------------------
    void FinishFire()
    {
        isFinished = true;
        col.enabled = false;
        StartCoroutine(ShrinkAndDestroy());
    }

    // Hose が触れた
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hose"))
            isExtinguishing = true;

        if (other.CompareTag("Player2"))
            isInside = true;
    }

    // Hose が離れた
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Hose"))
            isExtinguishing = false;

        if (other.CompareTag("Player2"))
            isInside = false;
=======
    public void ExtinguishImmediately()
    {
        if (isFinished) return;

        isFinished = true;
        isPlayer2Extinguishing = false;

        if (col != null)
            col.enabled = false;

        StartCoroutine(ShrinkAndDestroy());
>>>>>>> af46d1ebaf370ed0642b28ca2bf482daf248d8ac
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
