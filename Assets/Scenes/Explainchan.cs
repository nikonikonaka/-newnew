using UnityEngine;
using UnityEngine.UI;

public class Explainchan : MonoBehaviour
{
    public Image displayImage;   // UI Image（Canvas の Image）
    public Sprite[] sprites;     // 切り替える Sprite 達
    private int index = 0;

    void Start()
    {
        // 最初のスプライトを表示
        if (sprites.Length > 0 && displayImage != null)
        {
            displayImage.sprite = sprites[0];
        }
    }

    public void Next()
    {
        index = (index + 1) % sprites.Length;
        displayImage.sprite = sprites[index];
    }

    public void Prev()
    {
        index = (index - 1 + sprites.Length) % sprites.Length;
        displayImage.sprite = sprites[index];
    }
}
