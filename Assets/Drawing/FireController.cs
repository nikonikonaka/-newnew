using UnityEngine;
using System.Collections;

public class FireController : MonoBehaviour
{
    public float extinguishTime = 2f; // Hoseで消える時間
    public float shrinkTime = 0.5f;   // 小さくなる時間
    public float holdExtinguishTime = 6f; // Enter長押しで消える時間

    float timer = 0f;          // Hose用タイマー
    float holdTimer = 0f;      // Enter長押しタイマー

    bool isExtinguishing = false; // Hoseが当たっている
    bool isInside = false;        // Player2が範囲内
    bool isFinished = false;      // 完全に消えたか？

    Collider2D col;
    Vector3 originalScale;

    void Start()
    {
        col = GetComponent<Collider2D>();
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (isFinished) return;

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
            }
        }
        else
        {
            timer = 0f;
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

    public void StartExtinguish()
    {
        isExtinguishing = true;
    }

    public void StopExtinguish()
    {
        isExtinguishing = false;
    }
}
