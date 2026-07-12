using UnityEngine;

public class Hose : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player1"))
        {
            other.GetComponent<PlayerController>().GetHose();
            Destroy(gameObject);
        }
    }
}
