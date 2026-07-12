using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public EraserALL eraserALL;   // 足踏み消火のスクリプト
    public FSC_NO2 fscNO2;        // ホース消火のスクリプト

    public GameObject waterEffect;   // ← 追加（ホースの水エフェクト）

    void Start()
    {
        // 初期状態：足踏み消火オン、ホース消火オフ
        eraserALL.enabled = true;
        fscNO2.enabled = false;
        waterEffect.SetActive(false);

    }

    public void GetHose()
    {
        // ホース取得 → FSC_NO2オン、EraserALLオフ
        eraserALL.enabled = false;
        fscNO2.enabled = true;
        waterEffect.SetActive(true);

    }

    public void LoseHose()
    {
        // ホース無し → EraserALLオン、FSC_NO2オフ
        eraserALL.enabled = true;
        fscNO2.enabled = false;
        waterEffect.SetActive(false);

    }
}
