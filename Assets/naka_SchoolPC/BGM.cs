using UnityEngine;

public class BGM : MonoBehaviour
{
    public AudioClip bgmClip;
    private AudioSource bgmSource;

    void Start()
    {
        Debug.Log("BGM Start");

        bgmSource = GetComponent<AudioSource>();

        Debug.Log("AudioSource = " + bgmSource);
        Debug.Log("AudioClip = " + bgmClip);

        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.Play();

        Debug.Log("IsPlaying = " + bgmSource.isPlaying);
    }
}