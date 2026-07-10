using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FSC_NO2 : MonoBehaviour
{
    [Header("Hitbox Settings")]
    public float hitLength = 2.0f;   // 前方の長さ
    public float hitWidth = 0.8f;    // 横の広さ
    public float forwardOffset = 0.6f;

    public ParticleSystem waterEffect;

    private MoveWASD player;
    private Collider2D col;

    [Header("Water Settings")]
    public float maxWater = 100f;
    public float currentWater;
    public float waterUseSpeed = 20f;
    public float waterRecoverSpeed = 10f;
    public Slider waterSlider;

    public float waterStartSpeed = 6f;
    public float waterStartLifetime = 0.5f;
    public float waterStartSize = 0.18f;
    public float waterEmissionRate = 800f;
    public float waterShapeAngle = 18f;
    public float waterShapeRadius = 0.12f;

    private Vector2 lastDir = Vector2.down;

    void Start()
    {
        player = GetComponent<MoveWASD>();
        col = GetComponent<Collider2D>();

        currentWater = maxWater;
        UpdateUI();

        InitWaterSettings();

        if (waterEffect != null)
        {
            waterEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            waterEffect.Clear();
        }

        ForceParticleFront(waterEffect, "Foreground", 1000);
    }

    void Update()
    {
        bool usingHose =
            Input.GetKey(KeyCode.LeftShift) ||
            Input.GetKey(KeyCode.RightShift);

        Vector2 dir = player.lookDirection;
        if (dir.sqrMagnitude < 0.0001f)
            dir = lastDir;
        else
            lastDir = dir;

        dir = dir.normalized;

        // ★ 水はプレイヤーのすぐ前から出す
        Vector2 waterOrigin = (Vector2)col.bounds.center + dir * forwardOffset;
        waterEffect.transform.position = waterOrigin;

        // ★ 当たり判定は前方だけに出す（中心を前へずらす）
        Vector2 hitOrigin =
            (Vector2)col.bounds.center +
            dir * (hitLength * 0.5f + forwardOffset);

        float angleDeg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        Vector2 boxSize = new Vector2(hitLength, hitWidth);

        Collider2D[] hits = Physics2D.OverlapBoxAll(hitOrigin, boxSize, angleDeg);

        bool touchingFire = false;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Fire"))
            {
                touchingFire = true;

                FireController fire = hit.GetComponent<FireController>();

                if (fire != null)
                {
                    if (usingHose && currentWater > 0)
                        fire.StartExtinguish();
                    else
                        fire.StopExtinguish();
                }
            }
        }

        RotateEffect(dir);

        bool shouldEmit = usingHose && currentWater > 0 && touchingFire;

        ControlWaterEmission(shouldEmit);

        if (shouldEmit)
            currentWater -= waterUseSpeed * Time.deltaTime;
        else
            currentWater += waterRecoverSpeed * Time.deltaTime;

        currentWater = Mathf.Clamp(currentWater, 0, maxWater);

        UpdateUI();
    }

    // ★★★ 消火範囲の可視化（Sceneビューで見える）
    void OnDrawGizmos()
    {
        if (col == null || player == null) return;

        Vector2 dir = lastDir.normalized;

        Vector2 hitOrigin =
            (Vector2)col.bounds.center +
            dir * (hitLength * 0.5f + forwardOffset);

        float angleDeg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        Vector2 boxSize = new Vector2(hitLength, hitWidth);

        Gizmos.color = new Color(0f, 0.5f, 1f, 0.4f);
        Matrix4x4 rot = Matrix4x4.TRS(hitOrigin, Quaternion.Euler(0, 0, angleDeg), Vector3.one);
        Gizmos.matrix = rot;
        Gizmos.DrawCube(Vector3.zero, boxSize);
    }

    void InitWaterSettings()
    {
        var main = waterEffect.main;
        main.startLifetime = waterStartLifetime;
        main.startSpeed = hitLength / waterStartLifetime;   // ★ 水の長さを判定に合わせる
        main.startSize = waterStartSize;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.playOnAwake = false;

        var emission = waterEffect.emission;
        emission.rateOverTime = waterEmissionRate;
        emission.enabled = false;

        var shape = waterEffect.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = waterShapeAngle;
        shape.radius = hitWidth * 0.5f;                    // ★ 水の太さを判定に合わせる
        shape.rotation = new Vector3(0, 0, 90);
    }

    void RotateEffect(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        waterEffect.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void ControlWaterEmission(bool emit)
    {
        var emission = waterEffect.emission;
        emission.enabled = emit;

        if (emit && !waterEffect.isPlaying)
            waterEffect.Play();
        else if (!emit && waterEffect.isPlaying)
            waterEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    void ForceParticleFront(ParticleSystem ps, string sortingLayer, int order)
    {
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.sortingLayerName = sortingLayer;
        renderer.sortingOrder = order;

        Vector3 p = ps.transform.position;
        ps.transform.position = new Vector3(p.x, p.y, -0.01f);
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
