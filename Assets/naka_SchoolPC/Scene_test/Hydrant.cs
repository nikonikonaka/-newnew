using UnityEngine;

public class Hydrant : MonoBehaviour
{
    public bool isOpened = false;

    public float supplyRange = 1.5f;
    public float supplySpeed = 20f;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isOpened)
        {
            sr.color = Color.cyan;
        }

        if (!isOpened)
            return;

        GameObject player1 = GameObject.FindGameObjectWithTag("Player1");

        if (player1 == null)
            return;

        float dist = Vector2.Distance(
            transform.position,
            player1.transform.position
        );

        if (dist <= supplyRange)
        {
            FSC_NO2 hose = player1.GetComponent<FSC_NO2>();

            if (hose != null)
            {
                hose.currentWater += supplySpeed * Time.deltaTime;
                hose.currentWater =
                    Mathf.Clamp(hose.currentWater, 0, hose.maxWater);
            }

            EraserALL eraser = player1.GetComponent<EraserALL>();

            if (eraser != null)
            {
                eraser.currentWater += supplySpeed * Time.deltaTime;
                eraser.currentWater =
                    Mathf.Clamp(eraser.currentWater, 0, eraser.maxWater);
            }
        }
    }
}
