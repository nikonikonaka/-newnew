using UnityEngine;

public class FireExplosion : MonoBehaviour
{
    public SpriteRenderer fireSprite;

    public float explosionForce = 5f;
    public float explosionDuration = 0.2f;
    public float bigFireSize = 2f;

    bool exploded = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (exploded) return;

        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            exploded = true;
            StartCoroutine(Explode());
        }
    }

    System.Collections.IEnumerator Explode()
    {
        float timer = 0f;
        while (timer < explosionDuration)
        {
            timer += Time.deltaTime;
            fireSprite.transform.position += (Vector3)Random.insideUnitCircle * explosionForce * Time.deltaTime;
            yield return null;
        }

        fireSprite.transform.localScale = Vector3.one * bigFireSize;
        fireSprite.color = new Color(1f, 0.6f, 0.2f, 1f);
    }
}
