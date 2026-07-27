using UnityEngine;

public class StopLaser : MonoBehaviour
{
    [Header("停止するレーザーGameObject（複数設定可能）")]
    public GameObject[] laserObjects;

    [Header("最初に表示する画像")]
    public GameObject onObject;

    [Header("Player2が触れた後に表示する停止装置画像")]
    public GameObject offObject;

    private bool isStopped = false;

    private void Awake()
    {
        // ==========================================
        // ゲーム開始時の表示を強制的に初期化
        // InspectorでoffがONでも必ずOFFにする
        // ==========================================

        if (onObject != null)
        {
            onObject.SetActive(true);
        }

        if (offObject != null)
        {
            offObject.SetActive(false);
        }
    }

    private void Start()
    {
        // ==========================================
        // レーザーをすべてONにする
        // ==========================================

        if (laserObjects != null)
        {
            foreach (GameObject laser in laserObjects)
            {
                if (laser != null)
                {
                    laser.SetActive(true);
                }
            }
        }

        // ==========================================
        // on / off の位置を合わせる
        // ==========================================

        MatchOffPosition();

        Debug.Log("StopLaser 起動");
        Debug.Log("通常画像 ON");
        Debug.Log("停止装置画像 OFF");
    }

    // ==========================================
    // Trigger Colliderの場合
    // ==========================================

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckPlayer2(collision);
    }

    // ==========================================
    // 通常 Colliderの場合
    // ==========================================

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckPlayer2(collision.collider);
    }

    // ==========================================
    // Player2判定
    // ==========================================

    private void CheckPlayer2(Collider2D collision)
    {
        if (collision == null)
        {
            return;
        }

        // Player2自身
        if (collision.CompareTag("Player2"))
        {
            StopLaserNow();
            return;
        }

        // Player2の子Collider
        if (collision.transform.root.CompareTag("Player2"))
        {
            StopLaserNow();
        }
    }

    // ==========================================
    // レーザー停止
    // ==========================================

    private void StopLaserNow()
    {
        if (isStopped)
        {
            return;
        }

        isStopped = true;

        // ==========================================
        // ① on画像を消す
        // ==========================================

        if (onObject != null)
        {
            onObject.SetActive(false);
        }

        // ==========================================
        // ② off画像の位置をon画像に合わせる
        // ==========================================

        MatchOffPosition();

        // ==========================================
        // ③ off画像を表示
        // ==========================================

        if (offObject != null)
        {
            offObject.SetActive(true);
        }

        // ==========================================
        // ④ 登録したレーザーを全部OFF
        // ==========================================

        if (laserObjects != null)
        {
            foreach (GameObject laser in laserObjects)
            {
                if (laser != null)
                {
                    laser.SetActive(false);
                }
            }
        }

        Debug.Log("Player2が停止装置に触れました！");
        Debug.Log("通常画像 → 停止装置画像");
        Debug.Log("登録されたレーザーをすべてOFF");
    }

    // ==========================================
    // off画像の位置をon画像に合わせる
    // ==========================================

    private void MatchOffPosition()
    {
        if (onObject == null || offObject == null)
        {
            return;
        }

        // ワールド座標を完全に一致
        offObject.transform.position = onObject.transform.position;

        // 回転も一致
        offObject.transform.rotation = onObject.transform.rotation;

        // 大きさも一致
        offObject.transform.localScale = onObject.transform.localScale;
    }
}