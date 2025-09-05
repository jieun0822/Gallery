using NUnit.Framework;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    [Header("����")]
    public GameEnums.eScene currentScene = GameEnums.eScene.None;
    public GameEnums.eScene targetScene = GameEnums.eScene.None;
    public MakeBodyLayer bodyLayer;
    public SoundManager soundManager;
    public GalleryManager galleryManager;
    public GalleryUIManager galleryUIManager;
    public CheckObjectInUIImage check;

    public GameObject bodyPool;
    public GameObject jointPool;
    public FadeController fadeController;
    public bool isContentEnd = false;
    public bool isVisibleHand = false;

    [Header("���� UI")]
    public GameObject guideWin;
    public GameObject closeWin;
    public GameObject[] playbtn;
    public GameObject[] stopbtn;

    [Header("�ε� ��")]
    public GameObject loadingObj;
    //public GameObject curtainObj;
    public SkeletonAnimation curtain;
    public SkeletonAnimation spotlight;
    public GameObject startBtn;
    
    [Header("�عٶ�� ��")]
    public GameObject flowerObj;
    public GameObject[] flowerGaugeObjs;

    [Header("�� ��")]
    public bool isRoomGame;
    public GameObject roomObj;
    public GameObject roomGaugeObj;

    [Header("�� ��")]
    public GameObject starObj;
    public GameObject starGaugeObj;

    [Header("��� ��")]
    public GameObject crowObj;
    public bool isCrowScene = false;

    public delegate void InactiveHand();
    public InactiveHand inactiveHand;

    private void Start()
    {
        soundManager.gameManager = this;
        galleryUIManager.galleryManager = galleryManager;
        galleryUIManager.bodyPool = bodyPool;
        galleryUIManager.jointPool = jointPool;

        bodyPool.SetActive(false);
        jointPool.SetActive(false);
    }

    public void ToggleScene(GameEnums.eScene targetScene)
    {
        bool isSameScene = currentScene == targetScene;
        currentScene = isSameScene ? GameEnums.eScene.None : targetScene;
        bodyLayer.ChangeBody(currentScene);

        if (currentScene == GameEnums.eScene.None) soundManager.PlayBGM(GameEnums.eScene.Intro);
        else soundManager.PlayBGM(currentScene);

        //bodyLayer.useDepth = (currentScene == GameEnums.eScene.SunFlower ||
        //    currentScene == GameEnums.eScene.Crow ||
        //    currentScene == GameEnums.eScene.Star ||
        //    currentScene == GameEnums.eScene.Room)?  true : false;

        bodyLayer.useDepth = true;
        isCrowScene = (currentScene == GameEnums.eScene.Crow) ? true : false;
        galleryUIManager.ResetContent();

        SetSceneActive(GameEnums.eScene.SunFlower, !isSameScene && targetScene == GameEnums.eScene.SunFlower);
        SetSceneActive(GameEnums.eScene.Room, !isSameScene && targetScene == GameEnums.eScene.Room);
        SetSceneActive(GameEnums.eScene.Star, !isSameScene && targetScene == GameEnums.eScene.Star);
        SetSceneActive(GameEnums.eScene.Crow, !isSameScene && targetScene == GameEnums.eScene.Crow);
    }

    private void SetSceneActive(GameEnums.eScene scene, bool isActive)
    {
        switch (scene)
        {
            case GameEnums.eScene.SunFlower:
                flowerObj.SetActive(isActive);
                for (int i = 0; i <flowerGaugeObjs.Length; i++)
                    flowerGaugeObjs[i].SetActive(isActive);
                break;
            case GameEnums.eScene.Room:
                roomObj.SetActive(isActive);
                roomGaugeObj.SetActive(isActive);
                break;
            case GameEnums.eScene.Star:
                starObj.SetActive(isActive);
                starGaugeObj.SetActive(isActive);
                break;
            case GameEnums.eScene.Crow:
                crowObj.SetActive(isActive);
                break;
        }
    }

    public void ShowLoading()
    {
        // �޴�.
        var menuManager = galleryManager.menuManager;
        menuManager.backBtn.onClick.RemoveListener(galleryManager.CloseExplainWin);
        menuManager.backBtnList.Remove("CloseExplainWin");

        StartCoroutine(coShowLoading());
    }

    private IEnumerator coShowLoading()
    {
        yield return StartCoroutine(fadeController.FadeOut(0.5f));

        if (inactiveHand != null) inactiveHand();
        galleryManager.ShowGallery(false);
     
        // Ŀư
        loadingObj.SetActive(true);
        bodyPool.SetActive(false);
        jointPool.SetActive(false);
        startBtn.SetActive(true);
        
        curtain.AnimationState.ClearTracks();      // Ʈ�� �ʱ�ȭ
        curtain.Skeleton.SetToSetupPose();
        curtain.AnimationState.SetAnimation(0, "1_Curtain_idel", true);
        curtain.AnimationState.Apply(curtain.Skeleton); // ��� �ݿ�
        curtain.Update(0);

        // AI ������
        soundManager.StopAITeacherSound();
        galleryUIManager.StopAITeacherVideo(targetScene, 0);
        galleryUIManager.PlayAITeacherVideo(targetScene, 1);

        // ����
        soundManager.StopBGM();
        soundManager.PlayBGM(GameEnums.eScene.Stage);

        while (!galleryUIManager.videoReady)
        {
            yield return null;
        }
        soundManager.PlayAITeacherSound(targetScene, 1);

        yield return StartCoroutine(fadeController.FadeIn(0.5f));

    }

    public void OpenCurtain()
    {
        startBtn.SetActive(false);
        // AI ������
        soundManager.StopAITeacherSound();
     
        curtain.AnimationState.SetAnimation(0, "1_Curtain_open", false);
        spotlight.AnimationState.SetAnimation(0, "4_Spotlight_All-idle", true);

        // ����
        soundManager.PlaySound(soundManager.curtainSound);
        
        TrackEntry track = curtain.AnimationState.GetCurrent(0);
        float delay = (track != null) ? track.Animation.Duration : 0f;

        StartCoroutine(coShowPerson());
    }

    private IEnumerator coShowPerson()
    {
        bodyPool.SetActive(true);
        jointPool.SetActive(true);

        //������ �������
        VideoPlayer videoPlayer = null;
        switch (targetScene)
        {
            case GameEnums.eScene.SunFlower:
                videoPlayer = galleryUIManager.sunflowerVideo[1];
                break;
            case GameEnums.eScene.Room:
                videoPlayer = galleryUIManager.roomVideo[1];
                break;
            case GameEnums.eScene.Star:
                videoPlayer = galleryUIManager.starVideo[1];
                break;
            case GameEnums.eScene.Crow:
                videoPlayer = galleryUIManager.crowVideo[1];
                break;
        }

        var rawImg = videoPlayer.gameObject.GetComponent<RawImage>();
        StartCoroutine(FadeOutVideo(rawImg));
        yield return new WaitForSeconds(4f);

        ShowContent();
    }

    private IEnumerator FadeOutVideo(RawImage videoRawImage)
    {
        // ���� �� ���İ� 0����
        Color color = videoRawImage.color;
        color.a = 1f;
        videoRawImage.color = color;

        float timer = 0f;
        while (timer < 0.1f)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Clamp01(1f - (timer / 0.1f));
            videoRawImage.color = color;
            yield return null;
        }

        color.a = 0f;
        videoRawImage.color = color;

        var scene = galleryManager.gameManager.currentScene;
        galleryUIManager.StopAITeacherVideo(targetScene, 1);
    }

    public void ShowContent()
    {
        StartCoroutine(coShowContent());
    }

    private IEnumerator coShowContent()
    {
        yield return StartCoroutine(fadeController.FadeOut(0.5f));
      
        curtain.AnimationState.SetAnimation(0, "1_Curtain_idel", true);
        curtain.Update(0);
        loadingObj.SetActive(false);

        bodyLayer.bodyPool.transform.localPosition = Vector3.zero;
        ToggleScene(targetScene);

        if (currentScene == GameEnums.eScene.Crow)
        {
            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(fadeController.FadeIn(0.5f));
        }
        else
        {
            yield return StartCoroutine(fadeController.FadeIn(0.5f));
        }

        // �޴�.
        var menuManager = galleryManager.menuManager;
        menuManager.backBtn.onClick.AddListener(CloseContent);
        menuManager.backBtnList.Add("CloseContent");
    }

    public void CloseContent()
    {
        // �޴�.
        var menuManager = galleryManager.menuManager;
        menuManager.backBtn.onClick.RemoveListener(CloseContent);
        menuManager.backBtnList.Remove("CloseContent");

        var coroutine = galleryUIManager.btnCoroutine;
        if (coroutine != null) StopCoroutine(coroutine);

        soundManager.StopEffectSound();
        soundManager.StopCrowSound();
        ShowCloseWin(false);
        ShowGuideWin(false);

        bodyPool.SetActive(false);
        jointPool.SetActive(false);
        StartCoroutine(coCloseContent());
    }

    private IEnumerator coCloseContent()
    {
        yield return StartCoroutine(fadeController.FadeOut(0.5f));

        ToggleScene(GameEnums.eScene.None);
        isContentEnd = true;

        yield return StartCoroutine(fadeController.FadeIn(0.5f));

        // �޴�.
        var menuManager = galleryManager.menuManager;
        menuManager.backBtn.onClick.AddListener(galleryManager.CloseExplainWin);
        menuManager.backBtnList.Add("CloseExplainWin");
    }


    // ���� UI.
    public void ShowGuideWin(bool isActive)
    {
        guideWin.SetActive(isActive);
    }

    public void ShowCloseWin(bool isActive)
    {
        closeWin.SetActive(isActive);
    }

    public void SwitchBtn(int index)
    {
        if (playbtn[index].activeSelf)
        {
            playbtn[index].SetActive(false);
            stopbtn[index].SetActive(true);
        }
        else
        {
            playbtn[index].SetActive(true);
            stopbtn[index].SetActive(false);
        }
    }

    public void ChangeSceneByName(string sceneName)
    {
        StartCoroutine(coChangeSceneByName(sceneName));
    }

    private IEnumerator coChangeSceneByName(string sceneName)
    {
        yield return StartCoroutine(fadeController.FadeOut(0.5f));
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
