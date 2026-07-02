using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FSC : MonoBehaviour
{
    public float maxDistance = 2.0f;
    public float angleRange = 60f;
    public float forwardOffset = 0.6f;

    public ParticleSystem waterEffect;
    public ParticleSystem steamEffect;

    private MoveWASD player;
    private Collider2D col;

    public float maxWater = 100f;
    public float currentWater;
    public float waterUseSpeed = 20f;
    public float waterRecoverSpeed = 10f;
    public Slider waterSlider;

    [Header("Water Settings")]
    public float waterStartSpeed = 4f;
    public float waterStartLifetime = 0.45f;
    public float waterStartSize = 0.12f;
    public float waterEmissionRate = 400f;
    public float waterShapeAngle = 12f;
    public float waterShapeRadius = 0.05f;

    [Header("Steam Settings")]
    public float steamStartSpeed = 0.8f;
    public float steamStartLifetime = 0.9f;
    public float steamStartSize = 0.35f;
    public float steamEmissionRate = 60f;
    public float steamShapeAngle = 35f;
    public float steamShapeRadius = 0.02f;

    void Start()
    {
        player = GetComponent<MoveWASD>();
        col = GetComponent<Collider2D>();

        currentWater = maxWater;
        UpdateUI();

        // 初期設定をコードで行う
        InitParticleSettings();

        // 初期状態は停止（残粒子も消す）
        if (waterEffect != null) { waterEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); waterEffect.Clear(); }
        if (steamEffect != null) { steamEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); steamEffect.Clear(); }

        // 描画順などを強制
        ForceParticleFrontAll();
    }

    void Update()
    {
        bool usingHose =
            Input.GetKey(KeyCode.LeftShift) ||
            Input.GetKey(KeyCode.RightShift);

        Vector2 dir = player.lookDirection.normalized;
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.down;

        Vector2 origin = (Vector2)col.bounds.center + dir * forwardOffset;

        if (waterEffect != null) waterEffect.transform.position = origin;
        if (steamEffect != null) steamEffect.transform.position = origin;

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
                        {
                            fire.StartExtinguish();
                        }
                        else
                        {
                            fire.StopExtinguish();
                        }
                    }
                }
            }
        }

        RotateEffects(dir);

        bool shouldEmit = usingHose && currentWater > 0 && touchingFire;
        ControlParticleEmission(waterEffect, shouldEmit);
        ControlParticleEmission(steamEffect, shouldEmit);

        if (shouldEmit)
        {
            currentWater -= waterUseSpeed * Time.deltaTime;
            if (currentWater < 0) currentWater = 0;
        }
        else
        {
            currentWater += waterRecoverSpeed * Time.deltaTime;
            if (currentWater > maxWater) currentWater = maxWater;
        }

        UpdateUI();
    }

    void InitParticleSettings()
    {
        if (waterEffect != null)
        {
            var main = waterEffect.main;
            main.startSpeed = waterStartSpeed;
            main.startLifetime = waterStartLifetime;
            main.startSize = waterStartSize;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
            main.playOnAwake = false;

            var emission = waterEffect.emission;
            emission.rateOverTime = waterEmissionRate;
            emission.enabled = false;

            var shape = waterEffect.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = waterShapeAngle;
            shape.radius = waterShapeRadius;
            shape.rotation = Vector3.zero;

            var renderer = waterEffect.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
            }
        }

        if (steamEffect != null)
        {
            var main = steamEffect.main;
            main.startSpeed = steamStartSpeed;
            main.startLifetime = steamStartLifetime;
            main.startSize = steamStartSize;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
            main.playOnAwake = false;

            var emission = steamEffect.emission;
            emission.rateOverTime = steamEmissionRate;
            emission.enabled = false;

            var shape = steamEffect.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = steamShapeAngle;
            shape.radius = steamShapeRadius;
            shape.rotation = Vector3.zero;

            var renderer = steamEffect.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
            }
        }
    }

    void RotateEffects(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (waterEffect != null) waterEffect.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        if (steamEffect != null) steamEffect.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void ControlParticleEmission(ParticleSystem ps, bool emit)
    {
        if (ps == null) return;

        var emission = ps.emission;
        if (emit)
        {
            emission.enabled = true;
            if (!ps.isPlaying) ps.Play();
        }
        else
        {
            emission.enabled = false;
            if (ps.isPlaying) { ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); ps.Clear(); }
        }
    }

    // 描画順を強制するヘルパー
    void ForceParticleFrontAll()
    {
        ForceParticleFront(waterEffect, "Foreground", 1000);
        ForceParticleFront(steamEffect, "Foreground", 1000);
    }

    void ForceParticleFront(ParticleSystem ps, string sortingLayer, int order)
    {
        if (ps == null) return;
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        if (renderer != null)
        {
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.sortingLayerName = sortingLayer;
            renderer.sortingOrder = order;
        }
        Vector3 p = ps.transform.position;
        ps.transform.position = new Vector3(p.x, p.y, -0.01f);
        var main = ps.main;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
    }

    public void SetEffectScale(ParticleSystem ps, float scale)
    {
        if (ps == null) return;
        ps.transform.localScale = new Vector3(scale, scale, 1f);
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
