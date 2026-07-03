using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FSC : MonoBehaviour
{
    public float maxDistance = 2.0f;
    public float angleRange = 60f;
    public float forwardOffset = 0.6f;

    public ParticleSystem waterEffect;

    private MoveWASD player;
    private Collider2D col;

    public float maxWater = 100f;
    public float currentWater;
    public float waterUseSpeed = 20f;
    public float waterRecoverSpeed = 10f;
    public Slider waterSlider;

    [Header("Water Settings")]
    public float waterStartSpeed = 6f;          // 行きすぎないように弱め
    public float waterStartLifetime = 0.5f;     // 距離短め
    public float waterStartSize = 0.18f;
    public float waterEmissionRate = 800f;      // 水柱の密度
    public float waterShapeAngle = 18f;         // 少し広げて水柱らしく
    public float waterShapeRadius = 0.12f;      // 水の出口を太く

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

        Vector2 dir = player.lookDirection.normalized;
        if (dir.sqrMagnitude < 0.0001f)
            dir = Vector2.down;

        Vector2 origin = (Vector2)col.bounds.center + dir * forwardOffset;
        waterEffect.transform.position = origin;

        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, maxDistance);
        bool touchingFire = false;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Fire"))
            {
                Vector2 toFire = (Vector2)hit.transform.position - origin;
                float angle = Vector2.Angle(dir, toFire);

                if (angle <= angleRange / 2f)
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

    void InitWaterSettings()
    {
        var main = waterEffect.main;
        main.startSpeed = waterStartSpeed;
        main.startLifetime = waterStartLifetime;
        main.startSize = waterStartSize;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;   // ★ 2Dで向きが正しくなる
        main.playOnAwake = false;

        var emission = waterEffect.emission;
        emission.rateOverTime = waterEmissionRate;
        emission.enabled = false;

        var shape = waterEffect.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = waterShapeAngle;
        shape.radius = waterShapeRadius;
        shape.rotation = new Vector3(0, 0, 90);   // ★ 2D前方向に向ける
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
