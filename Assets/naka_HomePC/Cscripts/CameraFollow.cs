using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player1;
    public Transform player2;
    public float followSpeed = 3f;

    public Vector2 minLimit; // カメラの左下限界
    public Vector2 maxLimit; // カメラの右上限界

    void LateUpdate()
    {
        // 2人の中心点
        Vector3 center = (player1.position + player2.position) / 2f;

        // カメラ位置（Zは固定）
        Vector3 targetPos = new Vector3(center.x, center.y, -10f);

        // ステージ外に出ないようClamp
        targetPos.x = Mathf.Clamp(targetPos.x, minLimit.x, maxLimit.x);
        targetPos.y = Mathf.Clamp(targetPos.y, minLimit.y, maxLimit.y);

        // スムーズに追従
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
    }
}
