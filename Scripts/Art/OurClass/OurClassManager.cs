using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Video;

public class OurClassManager : MonoBehaviour
{
    [Header("Scripts")]
    public GalleryManager galleryManager;
    public FileReader fileReader;
    public FadeController fadeController;
    public CopyWebcam copyWebcam;
    public ScreenshotCapture screenshotCapture;

    [Header("Start_UI")]
    public GameObject classUI;

    [Header("Picture_UI")]
    public GameObject pictureUI;
    public GameObject confirmWinUI;
    public Image confirmImg;
    public Image confirmImg_X;
    public Image confirmImg_Y;

    public GameObject frame_X;
    public GameObject frame_Y;
    public GameObject widthBtn;
    public GameObject heightBtn;
    public bool mode_X = true;

    [Header("End_UI")]
    public GameObject endUI;
    public GameObject fixBtn;
    public GameObject homeBtn;
    public VideoPlayer fanfareVideo;
    public GameObject arrowGroup;
    private Coroutine coDisableBtn;

    public GameObject wallSet;
    public GameObject[] paintings;
    public Material targetMaterial;

    public TextMeshProUGUI[] countTxt;

    private void ResetPainting()
    {
        for (int i = 0; i < paintings.Length; i++)
        {
            paintings[i].SetActive(false);
        }
    }

    [ContextMenu("painting")]
    public void SetPainting()
    {
        fileReader.ReadAllFiles();
        UpdateCountTxt();

        ResetPainting();
        int count = (fileReader.sprites != null) ? fileReader.sprites.Length : 0;

        for (int i = 0; i < count; i++)
        {
            paintings[i].SetActive(true);
          
            Material materialCopy = new Material(targetMaterial); // 복사본 생성
            materialCopy.mainTexture = fileReader.sprites[i].texture;

            int width = materialCopy.mainTexture.width;
            int height = materialCopy.mainTexture.height;

            Renderer renderer = null;
          
            if (width > height)
            {
                var painting_X = paintings[i].gameObject.transform.GetChild(0).gameObject;
                painting_X.SetActive(true);
                var painting_Y = paintings[i].gameObject.transform.GetChild(1).gameObject;
                painting_Y.SetActive(false);

                renderer = painting_X.GetComponent<Renderer>();
            }
            else
            {
                var painting_X = paintings[i].gameObject.transform.GetChild(0).gameObject;
                painting_X.SetActive(false);
                var painting_Y = paintings[i].gameObject.transform.GetChild(1).gameObject;
                painting_Y.SetActive(true);

                renderer = painting_Y.GetComponent<Renderer>();
            }
           
            if (renderer == null)
            {
                Debug.LogWarning($"Renderer가 없음: paintings[{i}]");
                continue;
            }
            renderer.material = materialCopy;
        }


        wallSet.transform.GetChild(0).gameObject.SetActive(true);
        if (count == 0) return;
        if (count % 3 == 0)
        {
            int wallCnt = count / 3;
            for (int i = 1; i < wallCnt; i++)
            {
                var wall = wallSet.transform.GetChild(i).gameObject;
                wall.SetActive(true);
            }

            for (int i = wallCnt; i < 34; i++)
            {
                var wall = wallSet.transform.GetChild(i).gameObject;
                wall.SetActive(false);
            }
        }
        else
        {
            int wallCnt = (count / 3) + 1;
            for (int i = 1; i < wallCnt; i++)
            {
                var wall = wallSet.transform.GetChild(i).gameObject;
                wall.SetActive(true);
            }

            for (int i = wallCnt; i < 34; i++)
            {
                var wall = wallSet.transform.GetChild(i).gameObject;
                wall.SetActive(false);
            }
        }
    }

    public void ShowPictureUI(bool isActive)
    {
        if (isActive)
        {
            // 메뉴.
            var menuManager = galleryManager.menuManager;
            menuManager.backBtn.onClick.RemoveListener(galleryManager.MoveFromGalleryToDoor);
            menuManager.backBtnList.Remove("MoveFromGalleryToDoor");
            menuManager.backBtn.onClick.AddListener(ClosePictureUI);
            menuManager.backBtnList.Add("ClosePictureUI");

            ChangeXY(true);
        }
        else
        {
            // 메뉴.
            var menuManager = galleryManager.menuManager;
            menuManager.backBtn.onClick.RemoveListener(ClosePictureUI);
            menuManager.backBtnList.Remove("ClosePictureUI");
            menuManager.backBtn.onClick.AddListener(galleryManager.MoveFromGalleryToDoor);
            menuManager.backBtnList.Add("MoveFromGalleryToDoor");

            galleryManager.ShowGallery(true);
            endUI.SetActive(false);
            fanfareVideo.Stop();

            if (coDisableBtn != null)
            {
                StopCoroutine(coDisableBtn);
                coDisableBtn = null;
            }
            
            var soundManager = galleryManager.gameManager.soundManager;
            soundManager.StopFanfareSound();

            SetPainting();
            galleryManager.UpdateWallUI();
            confirmWinUI.SetActive(false);
        }

        classUI.SetActive(!isActive);
        pictureUI.SetActive(isActive);
        arrowGroup.SetActive(!isActive);

        copyWebcam.ActiveCanvas(isActive);
        if(isActive) copyWebcam.GetTexture();
    }

    // pictureUI, endUI -> classUI
    public void ClosePictureUI()
    {
        ShowPictureUI(false);
    }

    public void ShowEndUI()
    {
        StartCoroutine(coShowEndUI());
    }

    private IEnumerator coShowEndUI()
    {
        // 메뉴.
        var menuManager = galleryManager.menuManager;
        menuManager.backBtn.onClick.RemoveListener(galleryManager.MoveFromGalleryToDoor);
        menuManager.backBtnList.Remove("MoveFromGalleryToDoor");
        yield return StartCoroutine(fadeController.FadeOut(0.5f));

        classUI.SetActive(false);
        endUI.SetActive(true);
        fanfareVideo.Play();

        coDisableBtn = StartCoroutine(coDisableButtons());
        var soundManager = galleryManager.gameManager.soundManager;
        soundManager.PlayFanfareSound();

        yield return StartCoroutine(fadeController.FadeIn(0.5f));

        menuManager.backBtn.onClick.AddListener(ClosePictureUI);
        menuManager.backBtnList.Add("ClosePictureUI");
    }

    private IEnumerator coDisableButtons()
    { 
        fixBtn.SetActive(false);  
        homeBtn.SetActive(false);

        float durationInSeconds = (float)fanfareVideo.length; ; // 초 단위
        yield return new WaitForSeconds(durationInSeconds - 1);

        fixBtn.SetActive(true);
        homeBtn.SetActive(true);
    }

    public void ChangeXY(bool isX)
    {
        if (isX)
        {
            frame_X.SetActive(true);
            widthBtn.SetActive(false);

            frame_Y.SetActive(false);
            heightBtn.SetActive(true);

            mode_X = true;
            screenshotCapture.mode_X = true;
        }
        else
        {
            frame_X.SetActive(false);
            widthBtn.SetActive(true);

            frame_Y.SetActive(true);
            heightBtn.SetActive(false);

            mode_X = false;
            screenshotCapture.mode_X = false;
        }
    }

    public void ShowConfirmWin(bool isActive)
    {
        // 메뉴.
        var menuManager = galleryManager.menuManager;
        if (isActive)
        {
            menuManager.backBtn.onClick.RemoveListener(ClosePictureUI);
            menuManager.backBtnList.Remove("ClosePictureUI");
            menuManager.backBtn.onClick.AddListener(CloseConfirmWin);
            menuManager.backBtnList.Add("CloseConfirmWin");
        }
        else
        {
            menuManager.backBtn.onClick.RemoveListener(CloseConfirmWin);
            menuManager.backBtnList.Remove("CloseConfirmWin");
            menuManager.backBtn.onClick.AddListener(ClosePictureUI);
            menuManager.backBtnList.Add("ClosePictureUI");
        }

        // 초기화.
        confirmImg_X.gameObject.SetActive(false);
        confirmImg_Y.gameObject.SetActive(false);

        if (mode_X) confirmImg_X.gameObject.SetActive(true);
        else confirmImg_Y.gameObject.SetActive(true);

        confirmWinUI.SetActive(isActive);
    }

    private void CloseConfirmWin()
    {
        ShowConfirmWin(false);
    }

    public void UpdateCountTxt()
    {
        int count = (fileReader.sprites != null) ? fileReader.sprites.Length : 0;

        for (int i = 0; i < countTxt.Length; i++)
        {
            countTxt[i].text = $"{count} / 100";
        }
    }
}