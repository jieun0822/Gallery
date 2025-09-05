using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class ScreenshotCapture : MonoBehaviour
{
    public CustomDropdownManager dropdownManager;
    public GalleryManager galleryManager;
    private Rect captureRect1 = new Rect(1248.55f, 596.05f, 1342.9f, 967.9f); // 가로
    private Rect captureRect2 = new Rect(1439f, 409f, 962, 1342); // 세로

    private Texture2D tex;
    public bool mode_X = true;

    [ContextMenu("Capture Screenshot Area")]
    public void Capture()
    {
        StartCoroutine(CaptureArea());
    }

    private IEnumerator CaptureArea()
    {
        // 렌더링 끝날 때까지 대기
        yield return new WaitForEndOfFrame();

        // 스크린 전체에서 일부 영역만 읽기
        if (mode_X)
        {
            tex = new Texture2D((int)captureRect1.width, (int)captureRect1.height, TextureFormat.RGB24, false);
            tex.ReadPixels(captureRect1, 0, 0);
            tex.Apply();
        }
        else
        {
            tex = new Texture2D((int)captureRect2.width, (int)captureRect2.height, TextureFormat.RGB24, false);
            tex.ReadPixels(captureRect2, 0, 0);
            tex.Apply();
        }
        
        ShowConfirmImg();

        galleryManager.classManager.mode_X = (mode_X) ? true : false;
        galleryManager.classManager.ShowConfirmWin(true);
    }

    public void ShowConfirmImg()
    {
        if (tex != null)
        {
            // Texture2D → Sprite 변환
            Sprite screenshotSprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f)
            );

            // 초기화
            galleryManager.classManager.confirmImg_X.gameObject.SetActive(false);
            galleryManager.classManager.confirmImg_Y.gameObject.SetActive(false);

            Image img =(mode_X) ? galleryManager.classManager.confirmImg_X : galleryManager.classManager.confirmImg_Y;
            img.sprite = screenshotSprite;
            img.gameObject.SetActive(true);
        }
    }

    public void SavePicture()
    {
        // PNG로 저장
        byte[] bytes = tex.EncodeToPNG();
        string folderPath = dropdownManager.GetCurrentPath(); // 이게 '폴더' 경로인 경우

        // 파일 이름 지정 (예: 캡처_20250729_150033.png)
        string fileName = $"캡처_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";

        // 경로 결합
        string fullPath = Path.Combine(folderPath, fileName);
        File.WriteAllBytes(fullPath, bytes);

        Debug.Log($"캡처 완료: {fullPath}");
        Destroy(tex);
        tex = null;
    }
}
