using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EraserALL : MonoBehaviour
{
    // 消火中の炎
    List<FireController> targetFires =
        new List<FireController>();

    // 水
    public float maxWater = 100f;
    public float currentWater;

    // 水減少速度
    public float waterUseSpeed = 20f;

    // 水回復速度
    public float waterRecoverSpeed = 10f;

    // UI
    public Slider waterSlider;

    void Start()
    {
        currentWater = maxWater;

        UpdateUI();
    }

    void Update()
    {
        bool usingHose =
            Input.GetKey(KeyCode.LeftShift) ||
            Input.GetKey(KeyCode.RightShift);

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            GetComponent<Collider2D>().bounds.center,
            GetComponent<Collider2D>().bounds.size,
            0f);

        bool touchingFire = false;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Fire"))
            {
                touchingFire = true;

                FireController fire =
                    hit.GetComponent<FireController>();

                if (fire != null)
                {
                    if (usingHose && currentWater > 0)
                        fire.StartExtinguish();
                    else
                        fire.StopExtinguish();
                }
            }
        }

        if (usingHose && currentWater > 0 && touchingFire)
        {
            currentWater -= waterUseSpeed * Time.deltaTime;

            if (currentWater < 0)
                currentWater = 0;
        }
        else
        {
            currentWater += waterRecoverSpeed * Time.deltaTime;

            if (currentWater > maxWater)
                currentWater = maxWater;
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (waterSlider != null)
        {
            waterSlider.maxValue = maxWater;
            waterSlider.value = currentWater;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Fire"))
        {
            FireController fire =
                other.GetComponent<FireController>();

            if (fire != null &&
                !targetFires.Contains(fire))
            {
                targetFires.Add(fire);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Fire"))
        {
            FireController fire =
                other.GetComponent<FireController>();

            if (fire != null)
            {
                fire.StopExtinguish();

                targetFires.Remove(fire);
            }
        }
    }
}
