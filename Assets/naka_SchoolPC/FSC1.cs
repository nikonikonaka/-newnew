using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FSC1 : MonoBehaviour
{
    public float maxDistance = 2.0f;     // 消火距離
    public float angleRange = 60f;       // 扇形の角度（左右30度）
    public float forwardOffset = 0.6f;   // ホースの先端の位置

    public ParticleSystem waterEffect;   // 水の噴射エフェクト
    public ParticleSystem steamEffect;   // 火に当たった時の蒸気

    private MoveWASD player;
    private Collider2D col;

    public float maxWater = 100f;
    public float currentWater;
    public float waterUseSpeed = 20f;
    public float waterRecoverSpeed = 10f;
    public Slider waterSlider;

    void Start()
    {
        player = GetComponent<MoveWASD>();
        col = GetComponent<Collider2D>();

        currentWater = maxWater;
        UpdateUI();
    }

    void Update()
    {
        bool usingHose =
            Input.GetKey(KeyCode.LeftShift) ||
            Input.GetKey(KeyCode.RightShift);

        Vector2 dir = player.lookDirection.normalized;

        // ★ ホースの先端位置（プレイヤーの前方向）
        Vector2 origin = (Vector2)col.bounds.center + dir * forwardOffset;

        // ★ 扇形判定：まず円形で候補を拾う
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, maxDistance);

        bool touchingFire = false;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Fire"))
            {
                Vector2 toFire = (Vector2)hit.transform.position - origin;

                // ★ 扇形の角度判定
                float angle = Vector2.Angle(dir, toFire);

                if (angle <= angleRange / 2f)
                {
                    touchingFire = true;

                    FireController fire = hit.GetComponent<FireController>();

                    if (fire != null)
                    {
                        if (usingHose && currentWater > 0)
                        {
                            fire.StartExtinguish();

                            // ★ 火に水が当たった時の蒸気エフェクト
                            if (steamEffect != null && !steamEffect.isPlaying)
                                steamEffect.Play();
                        }
                        else
                        {
                            fire.StopExtinguish();
                        }
                    }
                }
            }
        }

        // ★ 水の噴射エフェクト（ホース）
        if (usingHose && currentWater > 0)
        {
            if (waterEffect != null && !waterEffect.isPlaying)
                waterEffect.Play();
        }
        else
        {
            if (waterEffect != null && waterEffect.isPlaying)
                waterEffect.Stop();
        }

        // ★ 水の消費・回復
        if (usingHose && currentWater > 0 && touchingFire)
        {
            currentWater -= waterUseSpeed * Time.deltaTime;
            if (currentWater < 0) currentWater = 0;
        }
        else
        {
            currentWater += waterRecoverSpeed * Time.deltaTime;
            if (currentWater > maxWater) currentWater = maxWater;
        }

        // ★ エフェクトの向きをプレイヤーの向きに合わせる（最重要）
        RotateEffects(dir);

        UpdateUI();
    }

    void RotateEffects(Vector2 dir)
    {
        // 2Dで向きを合わせる最適解
        Quaternion rot = Quaternion.LookRotation(Vector3.forward, dir);

        if (waterEffect != null)
            waterEffect.transform.rotation = rot;

        if (steamEffect != null)
            steamEffect.transform.rotation = rot;
    }

    void UpdateUI()
    {
        if (waterSlider != null)
        {
            waterSlider.maxValue = maxWater;
            waterSlider.value = currentWater;
        }
    }
}
