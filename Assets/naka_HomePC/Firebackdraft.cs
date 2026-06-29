using UnityEngine;

public class Firebackdraft2D : MonoBehaviour
{
    [Header("‘‹‚Ì‰æ‘œ")]
    public SpriteRenderer windowSprite;
    public Sprite intactWindow;   // ”j‰ó‘O
    public Sprite brokenWindow;   // ”j‰óŒã

    [Header("”š”­İ’è")]
    public float explosionForce = 5f;
    public float explosionDuration = 0.2f;
    public float bigFireSize = 2f;
    public float windowExplosionForce = 8f;

    bool triggered = false;

    void Start()
    {
        windowSprite.sprite = intactWindow;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        // Player1 ‚Ü‚½‚Í Player2 ‚ª‘‹‚ÉG‚ê‚½‚ç”­“®
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            triggered = true;
            StartCoroutine(Backdraft());
        }
    }

    System.Collections.IEnumerator Backdraft()
    {
        // ‘‹”j‰ó
        windowSprite.sprite = brokenWindow;

        // Fire ƒ^ƒO‘S•”æ“¾
        GameObject[] fires = GameObject.FindGameObjectsWithTag("Fire");

        // ”š”­—h‚êi‘‹‚à—h‚ê‚éj
        float timer = 0f;
        while (timer < explosionDuration)
        {
            timer += Time.deltaTime;

            foreach (GameObject fire in fires)
            {
                var sr = fire.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.transform.position += (Vector3)Random.insideUnitCircle * explosionForce * Time.deltaTime;
                }
            }

            windowSprite.transform.position += (Vector3)Random.insideUnitCircle * windowExplosionForce * Time.deltaTime;

            yield return null;
        }

        // Fire ‘S•”‹‘å‰»{F•ÏX
        foreach (GameObject fire in fires)
        {
            var sr = fire.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.transform.localScale = Vector3.one * bigFireSize;
                sr.color = new Color(1f, 0.6f, 0.2f, 1f);
            }
        }

        // ‘‹‚à‹‘å‰»
        windowSprite.transform.localScale = Vector3.one * (bigFireSize * 0.7f);
    }
}
