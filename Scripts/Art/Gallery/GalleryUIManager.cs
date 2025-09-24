using MoreMountains.Tools;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Windows;
using static Unity.Burst.Intrinsics.X86.Avx;

public class GalleryUIManager : MonoBehaviour
{
    public GalleryManager galleryManager;
    public ScreenFlash screenFlash;
    public GameObject bodyPool;
    public GameObject jointPool;

    [Header("����ȭ")]
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

    [Header("�츮�� ���ð�")]
    // �̸� �κ�.
    public GameObject[] nameTags;
    public GameObject[] inputFields;

    // ���� �κ�.
    public GameObject[] toggle_width_ver;
    public GameObject[] toggle_height_ver;
    public GameObject deleteWin;
    public GameObject deleteAllWin;

    private void Start()
    {
        ResetVideo();
        InitToggle();

        for (int i = 0; i < inputFields.Length; i++)
        {
            int index = i;
            var inputField = inputFields[i].GetComponent<InputField>();
            inputField.onValueChanged.AddListener((value) => OnValueChanged(value, index));
            inputField.onEndEdit.AddListener((value) => OnValueChangedEndEdit(value, index));
        }
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

    // ���
    public void InitToggle()
    {
        for (int i = 0; i < toggle_width_ver.Length; i++)
        {
            int index = i; // Ŭ���� ���� �ذ�
            var toggle = toggle_width_ver[i].GetComponent<Toggle>();
            toggle.onValueChanged.AddListener((value) => OnToggleChanged(index, value, true));

            toggle = toggle_height_ver[i].GetComponent<Toggle>();
            toggle.onValueChanged.AddListener((value) => OnToggleChanged(index, value, false));
        }
    }

    void OnToggleChanged(int index, bool isOn, bool isWidth)
    {
        if (isWidth)
        {
            if (toggle_width_ver.Length > index && toggle_width_ver[index] != null)
            {
                var checkImg = toggle_width_ver[index].transform.GetChild(0).gameObject;
                checkImg.SetActive(isOn);
            }
        }
        else
        {
            if (toggle_height_ver.Length > index && toggle_height_ver[index] != null)
            {
                var checkImg = toggle_height_ver[index].transform.GetChild(0).gameObject;
                checkImg.SetActive(isOn);
            }
        }
    }

    public void ResetToggle()
    {
        for (int i = 0; i < toggle_width_ver.Length; i++)
        {
            var toggle = toggle_width_ver[i].GetComponent<Toggle>();
            toggle.isOn = false;
            toggle_width_ver[i].transform.GetChild(0).gameObject.SetActive(false);
            toggle_width_ver[i].SetActive(false);

            toggle = toggle_height_ver[i].GetComponent<Toggle>();
            toggle.isOn = false;
            toggle_height_ver[i].transform.GetChild(0).gameObject.SetActive(false);
            toggle_height_ver[i].SetActive(false);
        }
    }

    public void DisableToggle()
    {
        for (int i = 0; i < toggle_width_ver.Length; i++)
        {
            toggle_width_ver[i].SetActive(false);
            toggle_height_ver[i].SetActive(false);
        }
    }

    public void UpdateToggle()
    {
        DisableToggle();

        var currentSetIndex = galleryManager.currentSetIndex;
        var firstIndex = currentSetIndex * 3;

        int count = 0;
        for (int i = firstIndex; i < firstIndex + 3; i++)
        {
            var data = galleryManager.fileReader.spriteDatas;
            if (i < 0 || i >= data.Count) break;

            var toggleArray = data[i].isWidth ? toggle_width_ver[i] : toggle_height_ver[i];
            toggleArray.SetActive(true);
            count++;
        }
    }

    public void DeleteToggle()
    {
        var fileReader = galleryManager.fileReader;
        var count = fileReader.spriteDatas.Count;
        var afterSize = fileReader.spriteDatas.Count;
        for (int i = count - 1; i >= 0; i--)
        {
            bool isChecked = toggle_width_ver[i].GetComponent<Toggle>().isOn ||
                             toggle_height_ver[i].GetComponent<Toggle>().isOn;
            if (isChecked)
            {
                int spriteIndex = i; // spriteDatas index
                fileReader.DestroyFile(spriteIndex);
                afterSize--;
            }
        }

        var wallMoving = galleryManager.wallMoving;
        wallMoving.UpdateWall(afterSize);

        //UpdateNameTag();
    }

    // ���� �˾�
    public void ActiveDeleteWin(bool isActive)
    {
        deleteWin.SetActive(isActive);
    }

    public void ActiveDeleteAllWin(bool isActive)
    {
        deleteAllWin.SetActive(isActive);
    }

    // �̸� �Է�
    private void OnValueChanged(string value, int index)
    {
        int firstIndex = galleryManager.currentSetIndex * 3;
        var fileReader = galleryManager.fileReader;
        var data = fileReader.spriteDatas;
        data[firstIndex + index].personName = value;
    }

    private void OnValueChangedEndEdit(string value, int index)
    {
        int firstIndex = galleryManager.currentSetIndex * 3;
        var data = galleryManager.fileReader.spriteDatas;
        data[firstIndex + index].personName = value;

        // JSON�� ����
        string filePath = data[firstIndex + index].filePath; // ��ũ���� ��ζ�� ����
        var jsonManager = galleryManager.classManager.jsonManager;
        jsonManager.SaveCapture(value, filePath);

        Debug.Log(value + "����");
    }

    public void DisableNameTag()
    {
        for(int i =0; i< nameTags.Length; i++)
            nameTags[i].SetActive(false);
    }

    public void UpdateNameTag()
    {
        int firstIndex = galleryManager.currentSetIndex * 3;
        var data = galleryManager.fileReader.spriteDatas;
        
        for (int i = 0; i < 3; i++)
        {
            int index = firstIndex + i;
            var inputField = inputFields[i].GetComponent<InputField>();

            if (index < data.Count)
            {
                inputField.text = data[index].personName;
                nameTags[i].SetActive(true);
            }
            else
            {
                //inputField.text = string.Empty;
                nameTags[i].SetActive(false);
            }
        }
    }
}