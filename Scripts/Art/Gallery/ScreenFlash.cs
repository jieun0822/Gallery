using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlash : MonoBehaviour
{
    public Image flashImage;
    public float flashDuration = 0.2f;

    private Coroutine flashCoroutine;

    public void PlayFlash()
    {
        // 이미 실행 중이면 중단 후 다시 실행
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(CoFlash());
    }

    private IEnumerator CoFlash()
    {
        flashImage.enabled = true;
        // 밝아짐
        float elapsed = 0f;
        while (elapsed < flashDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / (flashDuration * 0.5f));
            flashImage.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        // 어두워짐
        elapsed = 0f;
        while (elapsed < flashDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / (flashDuration * 0.5f));
            flashImage.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        flashImage.color = new Color(1f, 1f, 1f, 0f);
        flashCoroutine = null;
        flashImage.enabled = false;
    }
}
