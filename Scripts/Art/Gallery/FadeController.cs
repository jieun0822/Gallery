using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    public Image fadeImg;            // 검정색 이미지
    public float fadeDuration = 0.5f;  // 페이드 시간

    public IEnumerator FadeOut(float fadeDuration) // 화면 어두워짐
    {
        fadeImg.gameObject.SetActive(true);

        float elapsed = 0f;
        Color color = fadeImg.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            fadeImg.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        fadeImg.color = new Color(color.r, color.g, color.b, 1f);
    }

    public IEnumerator FadeIn(float fadeDuration) // 화면 밝아짐
    {
        float elapsed = 0f;
        Color color = fadeImg.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            fadeImg.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        fadeImg.color = new Color(color.r, color.g, color.b, 0f);

        fadeImg.gameObject.SetActive(false);
    }
}
