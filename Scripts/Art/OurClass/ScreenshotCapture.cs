using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class ScreenshotCapture : MonoBehaviour
{
    public CustomDropdownManager dropdownManager;
    public GalleryManager galleryManager;

    public Camera inputCamera;

    private Rect captureRect1 = new Rect(1248.55f, 596.05f, 1342.9f, 967.9f); // ����
    private Rect captureRect2 = new Rect(1439f, 409f, 962, 1342); // ����

    private Texture2D tex;
    public bool mode_X = true;

    [ContextMenu("Capture Screenshot Area")]
    public void Capture()
    {
        StartCoroutine(CaptureArea());
    }

    private IEnumerator CaptureArea()
    {
        // ������ ���� ������ ���
        yield return new WaitForEndOfFrame();

        //// ��ũ�� ��ü���� �Ϻ� ������ �б�
        //if (mode_X)
        //{
        //    tex = new Texture2D((int)captureRect1.width, (int)captureRect1.height, TextureFormat.RGB24, false);
        //    tex.ReadPixels(captureRect1, 0, 0);
        //    tex.Apply();
        //}
        //else
        //{
        //    tex = new Texture2D((int)captureRect2.width, (int)captureRect2.height, TextureFormat.RGB24, false);
        //    tex.ReadPixels(captureRect2, 0, 0);
        //    tex.Apply();
        //}

        Rect rect = mode_X ? captureRect1 : captureRect2;
        //rect = ClampRectToScreen(rect);
        //rect = GetScaledRect(rect);

        tex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        tex.ReadPixels(rect, 0, 0);
        tex.Apply();

        ShowConfirmImg();

        galleryManager.classManager.mode_X = (mode_X) ? true : false;
        galleryManager.classManager.ShowConfirmWin(true);
    }

    private float baseWidth = 3840f;
    private float baseHeight = 2160f;

    private Rect GetScaledRect(Rect original)
    {
        float scaleX = Screen.width / baseWidth;
        float scaleY = Screen.height / baseHeight;

        return new Rect(
            original.x * scaleX,
            original.y * scaleY,
            original.width * scaleX,
            original.height * scaleY
        );
    }

    private Rect ClampRectToScreen(Rect rect)
    {
        float x = Mathf.Clamp(rect.x, 0, Screen.width);
        float y = Mathf.Clamp(rect.y, 0, Screen.height);
        float width = Mathf.Clamp(rect.width, 0, Screen.width - x);
        float height = Mathf.Clamp(rect.height, 0, Screen.height - y);
        return new Rect(x, y, width, height);
    }

    public void ShowConfirmImg()
    {
        if (tex != null)
        {
            // Texture2D �� Sprite ��ȯ
            Sprite screenshotSprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f)
            );

            // �ʱ�ȭ
            galleryManager.classManager.confirmImg_X.gameObject.SetActive(false);
            galleryManager.classManager.confirmImg_Y.gameObject.SetActive(false);

            Image img =(mode_X) ? galleryManager.classManager.confirmImg_X : galleryManager.classManager.confirmImg_Y;
            img.sprite = screenshotSprite;
            img.gameObject.SetActive(true);
        }
    }

    public void SavePicture()
    {
        // PNG�� ����
        byte[] bytes = tex.EncodeToPNG();
        string folderPath = dropdownManager.GetCurrentPath(); // �̰� '����' ����� ���

        // ���� �̸� ���� (��: ĸó_20250729_150033.png)
        string fileName = $"ĸó_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";

        // ��� ����
        string fullPath = Path.Combine(folderPath, fileName);
        File.WriteAllBytes(fullPath, bytes);

        Debug.Log($"ĸó �Ϸ�: {fullPath}");
        Destroy(tex);
        tex = null;
    }
}
