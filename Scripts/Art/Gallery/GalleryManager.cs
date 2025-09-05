using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GalleryManager : MonoBehaviour
{
    [Header("Script")]
    public GameManager gameManager;
    public GalleryUIManager galleryUIManager;
    public IntroManager introManager;
    public MenuManager menuManager;
    public OurClassManager classManager;
    public CustomDropdownManager dropdownManager;
    public FileReader fileReader;
    public FadeController fadeController;

    [Header("갤러리")]
    public GameObject allGalllery;
    public GameObject galleryCanvas;
    public Button skipBtn;

    [Header("갤러리 문")]
    public GameObject doorCamera;
    public GameObject doorMovingCamera;
    public GameObject doorObj;
    public List<GameObject> doorArea = new List<GameObject>();
    public GameObject clickAlarm;
    public GameObject clickBtns;
    private bool isSelected = false;
    private bool isDoor = false;
    private float doorTime = 0f;
    public int artIndex; // 0 : 서양화, 1 : 우리 유치원 전시관, 2 : 동양화

    [Header("카메라워킹")]
    public CameraWalk cameraWalk;
    public GameObject galleryObj;
    public GameObject startCamera;
    public GameObject movingCamera;
    public GameObject introTxt;

    [Header("벽 부분")]
    public WallMoving wallMoving;
    public GameObject galleryObj2;
    public GameObject westernArt_galleryWall;
    public GameObject ourClass_galleryWall;
    public GameObject eastArt_galleryWall;
    public GameObject westernArt_Camera;

    [Header("서양화")]
    public List<GameObject> westernArtArea;
    public List<GameObject> westernArtImg;
    public List<Sprite> westernExplainTxt;

    [Header("동양화")]
    public List<GameObject> eastArtArea;
    public List<GameObject> eastArtImg;
    public List<Sprite> eastExplainTxt;

    [Header("상세창")]
    public Image explainImg;
    public GameObject activityBtn;
    public GameObject playerNumImg;
    public GameObject contentTxt;
    public GameObject[] leftArrow;
    public GameObject[] rightArrow;
    public GameObject explainWin;
    public GameObject[] copyrightTxts;
    private bool isExplainWinOpen = false;
    public Coroutine playSoundCoroutine;

    private string currentArt;
    public int currentSet = 0;
    public int setCount = -1;
    private bool isMovingWall = false;

    private void Start()
    {
        classManager.galleryManager = this;
        fileReader.galleryManager = this;
        skipBtn.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isSelected && isDoor)
        {
            doorTime += Time.deltaTime;
            if (doorTime >= 30f)
            {
                isSelected = true;
                clickAlarm.SetActive(true);
            }
        }

        // 문 카메라 워크 끝났을 때.
        if (cameraWalk.isDoorOver)
        {
            // 메뉴.
            menuManager.nextBtn.onClick.RemoveListener(SkipDoorCameraWalk);
            skipBtn.gameObject.SetActive(false);
            skipBtn.onClick.RemoveListener(SkipDoorCameraWalk);
            
            cameraWalk.isDoorOver = false;

            if (cameraWalk.doorIndex == 0)
            {
                wallMoving.ResetWall(0);
                MovingCamera(0);
            }
            else if (cameraWalk.doorIndex == 1)
            {
                // 초기화.
                dropdownManager.SelectItemByIndex(0);
                dropdownManager.scrollSync.isActive = true;
                dropdownManager.scrollSync.ShowContent();

                wallMoving.ResetWall(1);
                DisplayWall();
            }
            else if (cameraWalk.doorIndex == 2) // 동양화
            {
               
                wallMoving.ResetWall(2);
                MovingCamera(2);
            }
        }

        // 갤러리 카메라 워크 끝났을 때.
        if (cameraWalk.isOver)
        {
            // 메뉴.
            if (artIndex == 0 || artIndex == 2)
            {
                menuManager.nextBtn.onClick.RemoveListener(SkipCameraWalk);
                skipBtn.gameObject.SetActive(false);
                skipBtn.onClick.RemoveListener(SkipCameraWalk);
            }

            cameraWalk.isOver = false;
          
            DisplayWall();
        }

        if (gameManager.isContentEnd)
        {
            gameManager.isContentEnd = false;
            ShowGallery(true);
        }
    }

    // 문 부분.
    public void SetDoorMode(bool isEntering)
    {
        if (isEntering)
        {
            // 메뉴.
            menuManager.backBtn.onClick.AddListener(introManager.Init);
            menuManager.backBtnList.Add("Init");

            var soundManager = gameManager.soundManager;
            soundManager.PlayBGM(GameEnums.eScene.Intro);
        }

        doorObj.SetActive(isEntering);
        doorCamera.SetActive(isEntering);
        clickBtns.SetActive(isEntering);
        for (int i = 0; i < doorArea.Count; i++)
            doorArea[i].SetActive(isEntering);

        isSelected = false;
        isDoor = isEntering;
        doorTime = 0f;
    }

    public void DoorCameraWalk(int index)
    {
        // 메뉴.
        menuManager.backBtn.onClick.RemoveListener(introManager.Init);
        menuManager.backBtnList.Remove("Init");

        isSelected = true;
        clickBtns.SetActive(false);
        for (int i = 0; i < doorArea.Count; i++)
            doorArea[i].SetActive(false);
        StartCoroutine(coDoorCameraWalk(index));
    }

    private IEnumerator coDoorCameraWalk(int index)
    {
        yield return StartCoroutine(fadeController.FadeOut(0.5f));

        doorCamera.SetActive(false);
        doorMovingCamera.SetActive(true);
        cameraWalk.SetDoorCamera(index);

        yield return StartCoroutine(fadeController.FadeIn(0.5f));

        cameraWalk.isDoorMoving = true;

        // 메뉴.
        menuManager.nextBtn.onClick.AddListener(SkipDoorCameraWalk);
        skipBtn.gameObject.SetActive(true);
        skipBtn.onClick.AddListener(SkipDoorCameraWalk);
    }

    private void SkipDoorCameraWalk()
    {
        // 메뉴.
        menuManager.nextBtn.onClick.RemoveListener(SkipDoorCameraWalk);
        skipBtn.gameObject.SetActive(false);
        skipBtn.onClick.RemoveListener(SkipDoorCameraWalk);

        cameraWalk.isDoorMoving = false;
        cameraWalk.isDoorOver = false;

        if (cameraWalk.doorIndex == 0 || cameraWalk.doorIndex == 2)
        {
            int index = cameraWalk.doorIndex;
            if (index == 2)
            {
                var soundManager = gameManager.soundManager;
                soundManager.PlayBGM(GameEnums.eScene.EastArt);
            }

            doorObj.SetActive(false);
            galleryInit();
            wallMoving.ResetWall(index);

            cameraWalk.SetCart(index);
            cameraWalk.cart.SplinePosition = 1;

            doorMovingCamera.SetActive(false);
            startCamera.SetActive(false);
            movingCamera.SetActive(true);
        }
        else if (cameraWalk.doorIndex == 1)
        {
            // 초기화.
            dropdownManager.SelectItemByIndex(0);
            dropdownManager.scrollSync.isActive = true;
            dropdownManager.scrollSync.ShowContent();

            wallMoving.ResetWall(1);
        }
        DisplayWall();
    }

    public void SetArtMode(int index)
    {
        artIndex = index;
    }

    public void CloseClickAlarm()
    {
        clickAlarm.SetActive(false);
    }

    private void DisplayGellery(bool isActive)
    {
        startCamera.SetActive(isActive);
        galleryObj.SetActive(isActive);
    }

    // 서양화 부분.
    public void MovingCamera(int index)
    {
        cameraWalk.SetCart(index);
        StartCoroutine(coChangeCamera());
    }

    private IEnumerator coChangeCamera()
    {
        yield return StartCoroutine(fadeController.FadeOut(0.2f));

        doorMovingCamera.SetActive(false);
        doorObj.SetActive(false);
        galleryInit();
        movingCamera.SetActive(true);
        startCamera.SetActive(false);

        yield return null;
        yield return StartCoroutine(fadeController.FadeIn(0.2f));
        yield return null;

        if (artIndex == 2)
        {
            var soundManager = gameManager.soundManager;
            soundManager.PlayBGM(GameEnums.eScene.EastArt);
        }

        cameraWalk.isMoving = true;

        // 메뉴.
        if (artIndex == 0 || artIndex == 2)
        {
            menuManager.nextBtn.onClick.AddListener(SkipCameraWalk);
            skipBtn.gameObject.SetActive(true);
            skipBtn.onClick.AddListener(SkipCameraWalk);
        }
    }

    private void SkipCameraWalk()
    {
        cameraWalk.cart.SplinePosition = 1;
    }

    public void galleryInit()
    {
        allGalllery.SetActive(true);

        galleryObj.SetActive(true);
        startCamera.SetActive(true);
        movingCamera.SetActive(false);

        galleryObj2.SetActive(false);
        westernArt_galleryWall.SetActive(false);
        ourClass_galleryWall.SetActive(false);
        eastArt_galleryWall.SetActive(false);
        westernArt_Camera.SetActive(false);

        DisplayGellery(true);
    }

    // 벽면.
    public void DisplayWall()
    {
        StartCoroutine(coDisplayWall());
    }

    // 서양화 벽면, 우리 유치원 전시관 벽면.
    private IEnumerator coDisplayWall()
    {
        if (artIndex == 0)
        {
            introTxt.SetActive(true);
            yield return new WaitForSeconds(1f);
        }
        else if (artIndex == 2)
        {
            yield return new WaitForSeconds(1f);
        }
        yield return StartCoroutine(fadeController.FadeOut(0.5f));

        // 메뉴.
        menuManager.backBtn.onClick.AddListener(MoveFromGalleryToDoor);
        menuManager.backBtnList.Add("MoveFromGalleryToDoor");

        classManager.arrowGroup.SetActive(true);
        GameObject left = null;
        GameObject right = null;
        if (artIndex == 0 || artIndex == 2)
        {
            introTxt.SetActive(false);
            contentTxt.SetActive(true);
            setCount = 3;
            left = leftArrow[0];
            right = rightArrow[0];
        }
        else if (artIndex == 1)
        {
            doorMovingCamera.SetActive(false);
            doorObj.SetActive(false);

            classManager.classUI.SetActive(true);
            classManager.SetPainting();
            int length = (fileReader.sprites == null) ? 0 : fileReader.sprites.Length;
            setCount = (length % 3 == 0) ? length / 3 - 1 : length / 3;
            left = leftArrow[1];
            right = rightArrow[1];
        }

        int count = (fileReader.sprites == null) ? 0 : fileReader.sprites.Length;
        if (artIndex == 1 && count <= 3)
        {
            left.SetActive(false);
            right.SetActive(false);
        }
        else
        {
            left.SetActive(false);
            right.SetActive(true);
        }

        // 오브젝트.
        galleryObj.SetActive(false);
        galleryObj2.SetActive(true);

        if (artIndex == 0) westernArt_galleryWall.SetActive(true);
        else if (artIndex == 1) ourClass_galleryWall.SetActive(true);
        else if (artIndex == 2) eastArt_galleryWall.SetActive(true);

        westernArt_Camera.SetActive(true);

        movingCamera.SetActive(false);
        if (artIndex == 0)
        {
            for (int i = 0; i < 3; i++)
                westernArtArea[i].SetActive(true);
        }
        else if (artIndex == 2)
        {
            for (int i = 0; i < 3; i++)
                eastArtArea[i].SetActive(true);
        }
        yield return StartCoroutine(fadeController.FadeIn(0.5f));
    }

    public void MoveFromGalleryToDoor()
    {
        // 메뉴.
        menuManager.backBtn.onClick.RemoveListener(MoveFromGalleryToDoor);
        menuManager.backBtnList.Remove("MoveFromGalleryToDoor");

        if (artIndex == 0)
        {
            contentTxt.SetActive(false);
            westernArt_galleryWall.SetActive(false);

            for (int i = 0; i < westernArtArea.Count ; i++)
                westernArtArea[i].SetActive(false);

            leftArrow[0].SetActive(false);
            rightArrow[0].SetActive(false);
        }
        else if (artIndex == 1)
        {
            classManager.classUI.SetActive(false);
            ourClass_galleryWall.SetActive(false);

            leftArrow[1].SetActive(false);
            rightArrow[1].SetActive(false);
        }
        else if (artIndex == 2)
        {
            contentTxt.SetActive(false);
            eastArt_galleryWall.SetActive(false);

            for (int i = 0; i < eastArtArea.Count; i++)
                eastArtArea[i].SetActive(false);

            leftArrow[0].SetActive(false);
            rightArrow[0].SetActive(false);
        }

        setCount = -1;

        galleryObj2.SetActive(false);
        westernArt_Camera.SetActive(false);

        SetDoorMode(true);
    }

    public void UpdateWallUI()
    {
        int length = (fileReader.sprites == null) ? 0 : fileReader.sprites.Length;
        setCount = (length % 3 == 0) ? length / 3 - 1 : length / 3;
      
        if (length <= 3)
        {
            leftArrow[1].SetActive(false);
            rightArrow[1].SetActive(false);
            return;
        }

        if (currentSet == 0)
        {
            leftArrow[1].SetActive(false);
            rightArrow[1].SetActive(true);
        }
        else if (currentSet == setCount)
        {
            leftArrow[1].SetActive(true);
            rightArrow[1].SetActive(false);
        }
        else
        {
            leftArrow[1].SetActive(true);
            rightArrow[1].SetActive(true);
        }
    }

    // 벽면 움직임.
    public void MovingWall(bool isNext)
    {
        if (isMovingWall) return;
        isMovingWall = true;

        wallMoving.MovingWall(isNext);
        currentSet = isNext ? ++currentSet : --currentSet;
        StartCoroutine(coShowBtn());
    }

    private IEnumerator coShowBtn()
    {
        // 터치 막기.
        if (artIndex == 0)
        {
            for (int i = 0; i < westernArtArea.Count; i++)
                westernArtArea[i].SetActive(false);
        }
        else if (artIndex == 2)
        {
            for (int i = 0; i < eastArtArea.Count; i++)
                eastArtArea[i].SetActive(false);
        }

        yield return new WaitForSeconds(1f);

        isMovingWall = false;

        GameObject left = null;
        GameObject right = null;

        if (artIndex == 0 || artIndex == 2)
        {
            left = leftArrow[0];
            right = rightArrow[0];
        }
        else
        {
            left = leftArrow[1];
            right = rightArrow[1];
        }

        if (currentSet == 0)
        {
            left.SetActive(false);
            right.SetActive(true);
        }
        else if (currentSet == setCount)
        {
            left.SetActive(true);
            right.SetActive(false);
        }
        else
        {
            left.SetActive(true);
            right.SetActive(true);
        }

        if (artIndex == 0)
        {
            for (int i = 3 * currentSet; i < 3 + 3 * currentSet; i++)
                westernArtArea[i].SetActive(true);
        }
        else if (artIndex == 2)
        {
            for (int i = 3 * currentSet; i < 3 + 3 * currentSet; i++)
                eastArtArea[i].SetActive(true);
        }
    }

    public void SetCurrentArt(string artName)
    {
        currentArt = artName;
        // 초기화
        gameManager.targetScene = GameEnums.eScene.None;
        activityBtn.SetActive(false);
        playerNumImg.SetActive(false);
        galleryUIManager.StopAITeacherVideo(GameEnums.eScene.None, 0);

        if (artIndex != 0)
        {
            ChangeImg();
            return;
        }

        switch (currentArt)
        {
            case "Art_5":
                gameManager.targetScene = GameEnums.eScene.SunFlower; break;
            case "Art_7":
                gameManager.targetScene = GameEnums.eScene.Crow; break;
            case "Art_9":
                gameManager.targetScene = GameEnums.eScene.Star; break;
            case "Art_11":
                gameManager.targetScene = GameEnums.eScene.Room; break;
            default:
                ChangeImg(); return;
        }

        ChangeImg();
        activityBtn.SetActive(true);
        playerNumImg.SetActive(true);
        galleryUIManager.PlayAITeacherVideo(gameManager.targetScene, 0);

        // 서서히 나타나게
        VideoPlayer vp = null;
        switch (gameManager.targetScene)
        {
            case GameEnums.eScene.SunFlower:
                vp = galleryUIManager.sunflowerVideo[0];
                break;
            case GameEnums.eScene.Room:
                vp = galleryUIManager.roomVideo[0];
                break;
            case GameEnums.eScene.Star:
                vp = galleryUIManager.starVideo[0];
                break;
            case GameEnums.eScene.Crow:
                vp = galleryUIManager.crowVideo[0];
                break;
        }
        var rawImg = vp.gameObject.GetComponent<RawImage>();
        StartCoroutine(FadeInVideo(rawImg));
        playSoundCoroutine = StartCoroutine(PlaySound());
    }

    private IEnumerator FadeInVideo(RawImage videoRawImage)
    {
        // 시작 전 알파값 0으로
        Color color = videoRawImage.color;
        color.a = 0f;
        videoRawImage.color = color;

        float timer = 0f;
        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Clamp01(timer / 0.5f);
            videoRawImage.color = color;
            yield return null;
        }

        color.a = 1f;
        videoRawImage.color = color;
    }

    private IEnumerator PlaySound()
    {
        while (!galleryUIManager.videoReady)
        {
            yield return null;
        }
        // 사운드
        var soundManager = gameManager.soundManager;
        soundManager.PlayAITeacherSound(gameManager.targetScene, 0);
        playSoundCoroutine = null;
    }

    // 상세창.
    public void ShowExplainWin(bool isActive)
    {
        if (isActive)
        {
            isExplainWinOpen = true;
            // 메뉴.
            menuManager.backBtn.onClick.RemoveListener(MoveFromGalleryToDoor);
            menuManager.backBtnList.Remove("MoveFromGalleryToDoor");
            menuManager.backBtn.onClick.AddListener(CloseExplainWin);
            menuManager.backBtnList.Add("CloseExplainWin");

            explainWin.SetActive(true);
        }
        else
        {
            isExplainWinOpen = false;
            // 메뉴.
            menuManager.backBtn.onClick.RemoveListener(CloseExplainWin);
            menuManager.backBtnList.Remove("CloseExplainWin");
            menuManager.backBtn.onClick.AddListener(MoveFromGalleryToDoor);
            menuManager.backBtnList.Add("MoveFromGalleryToDoor");

            // 사운드
            var soundManager= gameManager.soundManager;
            soundManager.StopAITeacherSound();
            explainWin.SetActive(false);
        }
    }

    public void CloseExplainWin()
    {
        ShowExplainWin(false);
    }

    private void ChangeImg()
    {
        int index = currentArt switch
        {
            "Art_1" => 0,
            "Art_2" => 1,
            "Art_3" => 2,
            "Art_4" => 3,
            "Art_5" => 4,
            "Art_6" => 5,
            "Art_7" => 6,
            "Art_8" => 7,
            "Art_9" => 8,
            "Art_10" => 9,
            "Art_11" => 10,
            "Art_12" => 11,
            _ => -1  // 기본값 (해당하지 않을 때)
        };

        if (index == -1) return;

        for (int i = 0; i < copyrightTxts.Length; i++)
        {
            copyrightTxts[i].SetActive(false);
        }

        if (artIndex == 0)
        {
            for (int i = 0; i < westernArtImg.Count; i++)
                westernArtImg[i].SetActive(false);
            for (int i = 0; i < westernArtImg.Count; i++)
                eastArtImg[i].SetActive(false);

            westernArtImg[index].SetActive(true);
            explainImg.sprite = westernExplainTxt[index];
        }
        else if (artIndex == 2)
        {
            for (int i = 0; i < westernArtImg.Count; i++)
                westernArtImg[i].SetActive(false);
            for (int i = 0; i < westernArtImg.Count; i++)
                eastArtImg[i].SetActive(false);

            eastArtImg[index].SetActive(true);
            explainImg.sprite = eastExplainTxt[index];

            switch (index)
            {
                case 1:
                    copyrightTxts[0].SetActive(true);
                    break;
                case 4:
                    copyrightTxts[1].SetActive(true);
                    break;
                case 5:
                    copyrightTxts[2].SetActive(true);
                    break;
                case 8:
                    copyrightTxts[3].SetActive(true);
                    break;
                case 9:
                    copyrightTxts[4].SetActive(true);
                    break;
                case 10:
                    copyrightTxts[5].SetActive(true);
                    break;
                case 11:
                    copyrightTxts[6].SetActive(true);
                    break;
            }
        }
    }

    // 인터렉티브 창.
    public void ShowGallery(bool isActive)
    {
        galleryCanvas.SetActive(isActive);

        galleryObj2.SetActive(isActive);
        if (artIndex == 0) westernArt_galleryWall.SetActive(isActive);
        else if (artIndex == 1) ourClass_galleryWall.SetActive(isActive);
        else if (artIndex == 2) eastArt_galleryWall.SetActive(isActive);

        westernArt_Camera.SetActive(isActive);
    }

    public void ShowDoor()
    {
        StartCoroutine(coShowDoor());
    }

    private IEnumerator coShowDoor()
    {
        // 메뉴.
        menuManager.backBtn.onClick.RemoveListener(classManager.ClosePictureUI);
        menuManager.backBtnList.Remove("ClosePictureUI");
        
        yield return StartCoroutine(fadeController.FadeOut(0.5f));
        classManager.classUI.SetActive(false);
        classManager.endUI.SetActive(false);
        classManager.fanfareVideo.Stop();
        var soundManager = gameManager.soundManager;
        soundManager.StopFanfareSound();

        classManager.arrowGroup.SetActive(false);
        ShowGallery(false);
        galleryCanvas.SetActive(true);

        SetDoorMode(true);
        yield return StartCoroutine(fadeController.FadeIn(0.5f));
    }

    // 우리반 전시관 사진찍기 화면 보여줌.
    public void ShowPictureScene()
    {
        // 메뉴.
        menuManager.backBtn.onClick.RemoveListener(classManager.ClosePictureUI);
        menuManager.backBtnList.Remove("ClosePictureUI");

        StartCoroutine(coShowPictureScene());
    }

    private IEnumerator coShowPictureScene()
    {
        yield return StartCoroutine(fadeController.FadeOut(0.5f));
        ShowGallery(false);
        classManager.endUI.SetActive(false);
        classManager.fanfareVideo.Stop();
        var soundManager = gameManager.soundManager;
        soundManager.StopFanfareSound();

        classManager.ShowPictureUI(true);
        yield return StartCoroutine(fadeController.FadeIn(0.5f));
    }
}