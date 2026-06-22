using UnityEngine;

public class Firefuck
: MonoBehaviour
{
    public float hp = 100f;

    Vector3 startScale;

    void Start()
    {
        startScale = transform.localScale;
    }

    public void Extinguish(float power)
    {
        hp -= power * Time.deltaTime;

        // HPÇ…âûÇ∂ÇƒèôÅXÇ…è¨Ç≥Ç≠Ç∑ÇÈ
        float size = hp / 100f;
        transform.localScale = startScale * Mathf.Clamp01(size);

        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
