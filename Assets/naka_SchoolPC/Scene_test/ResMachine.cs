using UnityEngine;

public class RescueUnlockMachine : MonoBehaviour
{
    // ==========================================
    // Inspectorで設定するもの
    // ==========================================

    [Header("① 最初に表示する画像")]
    public GameObject onObject;

    [Header("② Player2が触れた後に表示する画像")]
    public GameObject offObject;

    [Header("③ Player1に付いているRESCON_p")]
    public RESCON_p rescon_p;

    [Header("④ Main Cameraに付いているSearchRes")]
    public SearchRes searchRes;


    // ==========================================
    // 内部処理
    // ==========================================

    private bool isActivated = false;


    // ==========================================
    // ゲーム開始時
    // ==========================================

    private void Awake()
    {
        // --------------------------------------
        // 最初の画像をON
        // --------------------------------------

        if (onObject != null)
        {
            onObject.SetActive(true);
        }

        // --------------------------------------
        // 解放後の画像をOFF
        // --------------------------------------

        if (offObject != null)
        {
            offObject.SetActive(false);
        }

        // --------------------------------------
        // RESCON_pをOFF
        // --------------------------------------

        if (rescon_p != null)
        {
            rescon_p.enabled = false;
        }

        // --------------------------------------
        // SearchResをOFF
        // --------------------------------------

        if (searchRes != null)
        {
            searchRes.enabled = false;
        }
    }


    // ==========================================
    // ゲーム開始
    // ==========================================

    private void Start()
    {
        // 画像の位置を合わせる
        MatchOffPosition();

        Debug.Log("RescueUnlockMachine 起動");

        // --------------------------------------
        // RESCON_p設定確認
        // --------------------------------------


        // --------------------------------------
        // SearchRes設定確認
        // --------------------------------------

        if (searchRes != null)
        {
            Debug.Log(
                "SearchResが設定されています"
            );
        }
        else
        {
            Debug.LogError(
                "SearchResが設定されていません！"
            );
        }
    }


    // ==========================================
    // Player2がTriggerに触れた場合
    // ==========================================

    private void OnTriggerEnter2D(
        Collider2D collision)
    {
        CheckPlayer2(collision);
    }


    // ==========================================
    // Player2が通常Colliderに触れた場合
    // ==========================================

    private void OnCollisionEnter2D(
        Collision2D collision)
    {
        CheckPlayer2(
            collision.collider
        );
    }


    // ==========================================
    // Player2か確認
    // ==========================================

    private void CheckPlayer2(
        Collider2D collision)
    {
        if (collision == null)
        {
            return;
        }

        // --------------------------------------
        // Player2自身
        // --------------------------------------

        if (collision.CompareTag("Player2"))
        {
            ActivateRescueSystem();
            return;
        }

        // --------------------------------------
        // Player2の子Collider
        // --------------------------------------

        if (collision.transform.root.CompareTag("Player2"))
        {
            ActivateRescueSystem();
        }
    }


    // ==========================================
    // 救助システムを解放
    // ==========================================

    private void ActivateRescueSystem()
    {
        // すでに解放済みなら何もしない
        if (isActivated)
        {
            return;
        }

        isActivated = true;


        // ==========================================
        // ① 最初の画像をOFF
        // ==========================================

        if (onObject != null)
        {
            onObject.SetActive(false);
        }


        // ==========================================
        // ② 解放後の画像位置を合わせる
        // ==========================================

        MatchOffPosition();


        // ==========================================
        // ③ 解放後の画像をON
        // ==========================================

        if (offObject != null)
        {
            offObject.SetActive(true);
        }


        // ==========================================
        // ④ RESCON_pをON
        // ==========================================

        if (rescon_p != null)
        {
            rescon_p.enabled = true;

            Debug.Log(
                "RESCON_p ON！"
            );
        }
        else
        {
            Debug.LogError(
                "RESCON_pがInspectorに設定されていません！"
            );
        }


        // ==========================================
        // ⑤ SearchResをON
        // ==========================================

        if (searchRes != null)
        {
            searchRes.enabled = true;

            Debug.Log(
                "SearchRes ON！"
            );
        }
        else
        {
            Debug.LogError(
                "SearchResがInspectorに設定されていません！"
            );
        }


        // ==========================================
        // 完了
        // ==========================================

        Debug.Log(
            "Player2が救助解放機械に触れました！"
        );

        Debug.Log(
            "Player1の救助機能を解放しました！"
        );
    }


    // ==========================================
    // offObjectの位置をonObjectに合わせる
    // ==========================================

    private void MatchOffPosition()
    {
        if (onObject == null ||
            offObject == null)
        {
            return;
        }

        // 位置
        offObject.transform.position =
            onObject.transform.position;

        // 回転
        offObject.transform.rotation =
            onObject.transform.rotation;

        // 大きさ
        offObject.transform.localScale =
            onObject.transform.localScale;
    }
}