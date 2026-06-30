using UnityEngine;

public class BGM : MonoBehaviour
{
    public AudioSource bgmSource;   // BGM—p‚ÌAudioSource
    public AudioClip bgmClip;       // Ä¶‚µ‚½‚¢BGM

    void Start()
    {
        bgmSource.clip = bgmClip;
        bgmSource.loop = true;      // ƒ‹[ƒvÄ¶
        bgmSource.Play();
    }
}