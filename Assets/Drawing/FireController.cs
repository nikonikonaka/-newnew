using UnityEngine;
using System.Collections;

public class FireController : MonoBehaviour
{
    public float extinguishTime = 2f; // 消火に必要な時間
    public float shrinkTime = 0.5f;   // 小さくなる時間

    float timer = 0f;
    bool isExtinguishing = false;
    bool isFinished = false;

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

        if (isExtinguishing)
        {
            timer += Time.deltaTime;

            // 消火進行度 0～1
            float progress = timer / extinguishTime;
            progress = Mathf.Clamp01(progress);

            // 徐々に縮む
            transform.localScale =
                originalScale * (1f - progress * 0.3f);

            if (timer >= extinguishTime)
            {
                isFinished = true;
                col.enabled = false;

                StartCoroutine(ShrinkAndDestroy());
            }
        }
        else
        {
            timer = 0f;

            // 離れたら元の大きさに戻す
            transform.localScale = originalScale;
        }
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
