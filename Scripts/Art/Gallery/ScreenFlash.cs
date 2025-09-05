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
        // �̹� ���� ���̸� �ߴ� �� �ٽ� ����
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(CoFlash());
    }

    private IEnumerator CoFlash()
    {
        flashImage.enabled = true;
        // �����
        float elapsed = 0f;
        while (elapsed < flashDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / (flashDuration * 0.5f));
            flashImage.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        // ��ο���
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
