using UnityEngine;

public class FireKILLPLAYER
: MonoBehaviour
{
    public float range = 2f;
    public float power = 30f;

    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            anim.SetBool("Fire", true);

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);

            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("FIRE"))
                {
                    hit.GetComponent<Firefuck>()?.Extinguish(power);
                }
            }
        }
        else
        {
            anim.SetBool("Fire", false);
        }
    }
}
