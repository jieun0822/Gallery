using System;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GalleryUIManager : MonoBehaviour
{
    public GalleryManager galleryManager;
    public ScreenFlash screenFlash;
    public GameObject bodyPool;
    public GameObject jointPool;

    public GameObject[] contentGroup; // 0 : �عٶ��, 1 : ������ ��, 2 : ���� ������ ��, 3 : ���
    public GameObject[] pictureGroup;
    public GameObject[] pictureBtn1;
    public GameObject[] pictureBtn2;
    public GameObject[] canvasGroup;
    public GameObject[] remindImgs;

    public VideoPlayer[] sunflowerVideo;
    public VideoPlayer[] starVideo;
    public VideoPlayer[] crowVideo;
    public VideoPlayer[] roomVideo;

    public bool videoReady = false;
    public Coroutine btnCoroutine = null;

    private void Start()
    {
        ResetVideo();
    }

    public void ResetVideo()
    {
        for (int i = 0; i < 3; i++)
        {
            sunflowerVideo[i].gameObject.SetActive(false);
            starVideo[i].gameObject.SetActive(false);
            crowVideo[i].gameObject.SetActive(false);
            roomVideo[i].gameObject.SetActive(false);
        }
    }

    public void ResetContent()
    {
        for (int i = 0; i < 4; i++)
        {
            contentGroup[i].SetActive(true);
            pictureGroup[i].SetActive(false);
            pictureBtn1[i].SetActive(false);
            pictureBtn2[i].SetActive(false);

            canvasGroup[i].SetActive(true);
        }
    }

    public void ShowContent(int index)
    {
        contentGroup[index].SetActive(true);
        pictureGroup[index].SetActive(false);
    }

    public void ShowPicture(int index)
    {
        StartCoroutine(coShowPicture(index));
    }

    private IEnumerator coShowPicture(int index)
    {
        var fadeController = galleryManager.fadeController;
        yield return StartCoroutine(fadeController.FadeOut(0.5f));

        pictureGroup[index].SetActive(true);
        remindImgs[index].SetActive(true);

        // AI ������
        var soundManager = galleryManager.gameManager.soundManager;
        var scene = galleryManager.gameManager.currentScene;
        PlayAITeacherVideo(scene, 2);

        // ����
        soundManager.StopBGM();
        soundManager.StopWindSound();
        soundManager.StopCrowSound();
        soundManager.StopEffectSound();

        soundManager.StopAITeacherSound();
     
        // ��ġ ����
        var check = galleryManager.gameManager.check;
        var coroutine = check.spine.coInitCoroutine;
        if (coroutine != null) StopCoroutine(coroutine);
        check.DisableGauge(scene);

        bodyPool.SetActive(false);
        jointPool.SetActive(false);

        contentGroup[index].SetActive(false);

        while (!videoReady)
        {
            yield return null;
        }
        soundManager.PlayAITeacherSound(scene, 2);

        // ������ ���� ���� �� ��ư�� �����ǵ���
        AudioClip clip = null;
        switch (scene)
        {
            case GameEnums.eScene.SunFlower:
                clip = soundManager.ai_sunflower.sound[2]; 
                break;
            case GameEnums.eScene.Room:
                clip = soundManager.ai_room.sound[2]; 
                break;
            case GameEnums.eScene.Star:
                clip = soundManager.ai_star.sound[2]; 
                break;
            case GameEnums.eScene.Crow:
                clip = soundManager.ai_crow.sound[2]; 
                break;
        }
        btnCoroutine = StartCoroutine(coShowBtn(index, clip.length));

        yield return StartCoroutine(fadeController.FadeIn(0.5f));
    }

    private IEnumerator coShowBtn(int index, float delay)
    {
        switch (index)
        {
            case 0:
                yield return new WaitForSeconds(delay - 7.5f);
                break;
            case 1:
                yield return new WaitForSeconds(delay - 8.5f);
                break;
            case 2:
                yield return new WaitForSeconds(delay - 9f);
                break;
            case 3:
                yield return new WaitForSeconds(delay - 6.5f);
                break;
        }
        pictureBtn1[index].SetActive(true);
        btnCoroutine = null;
    }

    public void ShowPerson(int index)
    {
        StartCoroutine(coShowPerson(index));
    }

    private IEnumerator coShowPerson(int index)
    {
        var fadeController = galleryManager.fadeController;
        yield return StartCoroutine(fadeController.FadeOut(0.5f));

        var soundManager = galleryManager.gameManager.soundManager;
        soundManager.StopAITeacherSound();
        var scene = galleryManager.gameManager.currentScene;
        StopAITeacherVideo(scene, 2);

        bodyPool.SetActive(true);
        jointPool.SetActive(true);
        pictureBtn1[index].SetActive(false);
        pictureBtn2[index].SetActive(true);
        remindImgs[index].SetActive(false);
       
        yield return StartCoroutine(fadeController.FadeIn(0.5f));
    }

    public void SetCanvasActive(bool isActive)
    {
        int index = -1;
        var curScene = galleryManager.gameManager.currentScene;
        switch (curScene)
        {
            case GameEnums.eScene.SunFlower: index = 0; break;
            case GameEnums.eScene.Room: index = 1; break;
            case GameEnums.eScene.Star: index = 2; break;
            case GameEnums.eScene.Crow: index = 3; break;
        }

        if (index == -1) return;
        canvasGroup[index].SetActive(isActive);
    }

    public void PlayAITeacherVideo(GameEnums.eScene scene, int index)
    {
        videoReady = false;
        VideoPlayer videoPlayer = null;
        switch (scene)
        {
            case GameEnums.eScene.SunFlower:
                sunflowerVideo[index].gameObject.SetActive(true);
                videoPlayer = sunflowerVideo[index];
                break;
            case GameEnums.eScene.Room:
                roomVideo[index].gameObject.SetActive(true);
                videoPlayer = roomVideo[index];
                break;
            case GameEnums.eScene.Star:
                starVideo[index].gameObject.SetActive(true);
                videoPlayer = starVideo[index];
                break;
            case GameEnums.eScene.Crow:
                crowVideo[index].gameObject.SetActive(true);
                videoPlayer = crowVideo[index];
                break;
        }

        var rawImg = videoPlayer.gameObject.GetComponent<RawImage>();
        Color color = rawImg.color;
        if (color.a == 0) 
        { 
            color.a = 1f; 
            rawImg.color = color;  
        }

        ClearRenderTexture(videoPlayer.targetTexture);
        // Ȥ�� ���� �̺�Ʈ�� ���� ���� �� ������ ����
        videoPlayer.prepareCompleted -= OnVideoPrepared;

        // �� �̺�Ʈ ���
        videoPlayer.prepareCompleted += OnVideoPrepared;

        // ��� �غ�
        videoPlayer.Prepare();
    }

    public void ClearRenderTexture(RenderTexture rt)
    {
        if (rt == null) return;

        RenderTexture active = RenderTexture.active;
        RenderTexture.active = rt;
        GL.Clear(true, true, Color.clear); // ���� ȭ������ Ŭ����
        RenderTexture.active = active;
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        videoReady = true;
        vp.Play();
        // �̺�Ʈ �ߺ� ������ ���� ����
        vp.prepareCompleted -= OnVideoPrepared;
    }

    public void StopAITeacherVideo(GameEnums.eScene scene, int index)
    {
        switch (scene)
        {
            case GameEnums.eScene.SunFlower:
                sunflowerVideo[index].Stop();
                sunflowerVideo[index].gameObject.SetActive(false);
                break;
            case GameEnums.eScene.Room:
                roomVideo[index].Stop();
                roomVideo[index].gameObject.SetActive(false);
                break;
            case GameEnums.eScene.Star:
                starVideo[index].Stop();
                starVideo[index].gameObject.SetActive(false);
                break;
            case GameEnums.eScene.Crow:
                crowVideo[index].Stop();
                crowVideo[index].gameObject.SetActive(false);
                break;
            case GameEnums.eScene.None:
                sunflowerVideo[index].gameObject.SetActive(false);
                roomVideo[index].gameObject.SetActive(false);
                starVideo[index].gameObject.SetActive(false);
                crowVideo[index].gameObject.SetActive(false);
                break;
        }
    }
}