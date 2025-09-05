using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class IntroManager : MonoBehaviour
{
    public GameManager gameManager;
    public MenuManager menuManager;
    public FadeController fadeController;

    public GameObject blackCanvas;
    public VideoPlayer bgVideo;
    public GameObject introObj;
    public GameObject blockTouch;
    public GameObject clickAlarm;

    public GameObject[] clickObj;
    public GameObject[] clickArea;

    private bool isSelected = false;
    private float time = 0f;
  
    private void Start()
    {
        Init();
    }

    public void Init()
    {
        // 메뉴.
        menuManager.backBtn.onClick.RemoveListener(Init);
        menuManager.backBtnList.Remove("Init");

        gameManager.soundManager.PlayBGM(GameEnums.eScene.Intro);
        //introObj.SetActive(false);
        for (int i = 0; i < clickObj.Length; i++)
            clickObj[i].SetActive(false);
        StartCoroutine(coShowIntro());
    }

    private IEnumerator coShowIntro()
    {
        yield return StartCoroutine(fadeController.FadeOut(0.5f));

        blackCanvas.SetActive(false);
        bgVideo.Play();
        introObj.SetActive(true);
        blockTouch.SetActive(false);
        isSelected = false;
        time = 0f;

        yield return StartCoroutine(fadeController.FadeIn(0.5f));
    }

    private void Update()
    {
        if (isSelected) return;
        
        time += Time.deltaTime;
        if (time >= 30f)
        {
            isSelected = true;
            clickAlarm.SetActive(true);
        }
    }

    public void CloseAlarm()
    {
        clickAlarm.SetActive(false);
    }

    // 명화 버튼을 클릭했을때.
    public void ClickPaintingBtn()
    {
        isSelected = true;
        blockTouch.SetActive(true);
        StartCoroutine(coChangeScene());
    }

    private IEnumerator coChangeScene()
    {
        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(fadeController.FadeOut(0.5f));
        bgVideo.Stop();
        introObj.SetActive(false);
        gameManager.galleryManager.SetDoorMode(true);
        yield return StartCoroutine(fadeController.FadeIn(0.5f));
    }

    public void ClickBuilding(int index, bool isActive)
    {
        clickObj[index].SetActive(isActive);
    }
}
