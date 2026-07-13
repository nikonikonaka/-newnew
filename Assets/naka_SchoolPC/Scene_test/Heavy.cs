using UnityEngine;

public class Heavy : MonoBehaviour
{
    public float pushForce = 50f; // d‚¢‚È‚ç‚±‚Ì‚­‚ç‚¢•K—v
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player1"))
        {
            // WASD“ü—Í‚ğæ“¾
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            Vector2 dir = new Vector2(x, y).normalized;

            // “ü—Í‚ª‚ ‚é‚¾‚¯‰Ÿ‚·
            if (dir.magnitude > 0.1f)
            {
                rb.AddForce(dir * pushForce);
            }

        }
        else if (collision.gameObject.CompareTag("Fire"))
        {
            Destroy(collision.gameObject); // Fire ‚ğÁ‚·
        }
      

    }

    // š Hose ‚ª IsTrigger ‚Ìê‡‚Í‚±‚Á‚¿‚ª“®‚­
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Hose"))
        {
            Destroy(other.gameObject); // Hose ‚ğÁ‚·
        }
    }
}
