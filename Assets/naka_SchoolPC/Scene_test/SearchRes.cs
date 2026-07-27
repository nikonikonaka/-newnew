using UnityEngine;
using System.Collections.Generic;

public class SearchRes : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Canvas arrowCanvas;
    public Camera targetCamera;

    public float arrowMargin = 40f;
    public float arrowScale = 1f;
    public float arrowRotationOffset = 0f;

    public bool debugLog = true;

    private Dictionary<GameObject, GameObject> arrows = new Dictionary<GameObject, GameObject>();
    private bool initialized = false;

    void LateUpdate() // ★ 方向ズレ完全防止
    {
        if (!initialized)
        {
            initialized = true;
            return; // ★ カメラ行列安定まで1フレーム待つ
        }

        UpdateArrows();
    }

    void UpdateArrows()
    {
        // ★ 毎フレーム RESPER を再取得（辞書破壊防止）
        GameObject[] people = GameObject.FindGameObjectsWithTag("RESPER");

        RectTransform canvasRect = arrowCanvas.GetComponent<RectTransform>();
        float scaleFactor = arrowCanvas.scaleFactor;

        // ★ 新しい救助者 → 矢印生成
        foreach (var p in people)
        {
            if (!arrows.ContainsKey(p))
            {
                GameObject arrow = Instantiate(arrowPrefab, arrowCanvas.transform);
                arrow.name = "SearchArrow_" + p.name;
                arrow.SetActive(false);

                RectTransform rt = arrow.GetComponent<RectTransform>();
                rt.localScale = Vector3.one * (arrowScale / scaleFactor);
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);

                arrows[p] = arrow;
            }
        }

        // ★ 消えた救助者 → 矢印削除
        List<GameObject> removeList = new List<GameObject>();
        foreach (var kv in arrows)
        {
            if (System.Array.IndexOf(people, kv.Key) == -1)
            {
                Destroy(kv.Value);
                removeList.Add(kv.Key);
            }
        }
        foreach (var r in removeList) arrows.Remove(r);

        // ★ 位置・方向更新
        foreach (var p in people)
        {
            GameObject arrow = arrows[p];
            RectTransform arrowRect = arrow.GetComponent<RectTransform>();

            // ★ 救助済みなら消す
            RescuePerson rp = p.GetComponent<RescuePerson>();
            if (rp != null && rp.rescued)
            {
                arrow.SetActive(false);
                continue;
            }

            // ★ ワールド → スクリーン座標（最も安定）
            Vector3 screenPos = targetCamera.WorldToScreenPoint(p.transform.position);

            bool isInside =
                screenPos.z > 0 &&
                screenPos.x >= 0 && screenPos.x <= Screen.width &&
                screenPos.y >= 0 && screenPos.y <= Screen.height;

            // ★ 画面内なら消す
            if (isInside)
            {
                arrow.SetActive(false);
                continue;
            }

            arrow.SetActive(true);

            // ★ 方向ベクトル（絶対ズレない）
            Vector2 dir = new Vector2(
                screenPos.x - Screen.width / 2f,
                screenPos.y - Screen.height / 2f
            );

            if (screenPos.z < 0f) dir = -dir;
            dir.Normalize();

            // ★ 画面端に正確に配置
            float halfW = Screen.width / 2f - arrowMargin;
            float halfH = Screen.height / 2f - arrowMargin;

            float tx = Mathf.Abs(dir.x) > 0.0001f ? halfW / Mathf.Abs(dir.x) : float.MaxValue;
            float ty = Mathf.Abs(dir.y) > 0.0001f ? halfH / Mathf.Abs(dir.y) : float.MaxValue;
            float t = Mathf.Min(tx, ty);

            Vector2 screenArrowPos = new Vector2(
                Screen.width / 2f + dir.x * t,
                Screen.height / 2f + dir.y * t
            );

            // ★ スクリーン → Canvas 座標
            Vector2 canvasPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenArrowPos,
                null,
                out canvasPos
            );

            arrowRect.anchoredPosition = canvasPos;

            // ★ 角度（Atan2で絶対ズレない）
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            arrowRect.localRotation = Quaternion.Euler(0, 0, angle + arrowRotationOffset);

            // ★ 大きさ補正
            arrowRect.localScale = Vector3.one * (arrowScale / scaleFactor);
        }
    }
}
