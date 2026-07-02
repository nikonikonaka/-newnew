using UnityEngine;
using System.Collections;

public class FireExplosion : MonoBehaviour
{
    public SpriteRenderer fireSprite;
    public float explosionForce = 5f;         // 見た目の揺れ強さ（Rigidbody があれば力としても使う）
    public float explosionDuration = 0.2f;    // 揺れの時間
    public float bigFireSize = 2f;            // 爆発後のスケール
    public bool useRigidbodyForce = true;     // Rigidbody2D に力を与えるか
    public float rigidbodyImpulse = 2f;       // Rigidbody に与えるインパルス量

    bool exploded = false;

    void Reset()
    {
        // 自動で参照を取る（Inspector で未設定なら）
        if (fireSprite == null) fireSprite = GetComponent<SpriteRenderer>();
    }

    public void TriggerExplosion()
    {
        if (exploded) return;
        StartCoroutine(ExplodeRoutine());
    }

    IEnumerator ExplodeRoutine()
    {
        exploded = true;

        // 揺れフェーズ
        float timer = 0f;
        Vector3 originalPos = transform.position;
        while (timer < explosionDuration)
        {
            timer += Time.deltaTime;
            Vector2 offset = Random.insideUnitCircle * explosionForce * Time.deltaTime;
            transform.position = originalPos + (Vector3)offset;
            yield return null;
        }
        // 位置を戻す（重要）
        transform.position = originalPos;

        // 見た目の変化
        if (fireSprite != null)
        {
            fireSprite.transform.localScale = Vector3.one * bigFireSize;
            fireSprite.color = new Color(1f, 0.6f, 0.2f, 1f);
        }

        // Rigidbody2D に短いインパルスを与える（オプション）
        if (useRigidbodyForce)
        {
            var rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // ランダム方向に少し飛ばす（プレイヤーに影響しないように火だけのレイヤーにしておく）
                Vector2 impulse = Random.insideUnitCircle.normalized * rigidbodyImpulse;
                rb.AddForce(impulse, ForceMode2D.Impulse);
            }
        }

        // 必要ならここで火の状態を「燃え盛る」フラグにする、タグを変える、エフェクトを切り替える等
        // 例: gameObject.tag = "BigFire";
    }
}
