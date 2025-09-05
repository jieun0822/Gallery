using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using NUnit.Framework;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    public Button backBtn;
    public Button nextBtn;
    string folderPath;
    public List<string> backBtnList;

    private void Start()
    {
        backBtnList = new List<string>();
        CreateFolder();
    }
    
    // 스크린샷.
    public void CreateFolder()
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string newFolderName = "스크린샷";

        // 전체 경로
        folderPath = Path.Combine(projectRoot, newFolderName);

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }

    public void TakeScreenshot()
    {
        StartCoroutine(CaptureScreenshot());
    }

    private IEnumerator CaptureScreenshot()
    {
        yield return new WaitForEndOfFrame();

        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = "screenshot_" + timestamp + ".png";

        string path = Path.Combine(folderPath, fileName);
        ScreenCapture.CaptureScreenshot(path);
    }
}
