using System.Collections;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CraftsSceneUIManager : MonoBehaviour
{
    public CraftsManager manager;

    [Header("3D창")]
    public GameObject _3DWin;
    public GameObject _3DWinBG;
    public GameObject[] _3DObjs;
    public GameObject pointerGuide;
    public Animator guideSoundAni;
    public Sprite muteSprite;
    public Sprite endSprite;
    public GameObject[] ccImgs;
    public GameObject basicWin;
    public GameObject detailWin;

    [Header("상세창")]
    public GameObject explainBG;
    public GameObject explainWin;
    public GameObject[] explainImgs;
    public Image explainTxt;
    public Sprite[] explainTxts;
    public GameObject arrowGroup;
    public GameObject leftArrow;
    public GameObject rightArrow;
    public GameObject[] ccTxts;
    public GameObject[] ccImgs2;

    [Header("Wall")]
    public GameObject areaObj;
    public GameObject[] craftArea;

    [Header("갤러리")]
    public Button skipBtn;

    private int maxArtCount = -1;

    private void Start()
    {
        if (manager == null)
            manager = FindAnyObjectByType<CraftsManager>();

        var fadeController = manager.fadeController;
        Init();
    }

    private void Init()
    {
        var menuManager = manager.menuManager;
        menuManager.backBtn.onClick.AddListener(manager.QuitCraftsScene);
        menuManager.backBtnList.Add("manager.QuitCraftsScene");

        skipBtn.gameObject.SetActive(false);

        maxArtCount = explainImgs.Length;
        if (manager == null)
            manager = FindAnyObjectByType<CraftsManager>();

        // 초기화.
        var rotateScript = manager.mouseDragRotate;
        rotateScript._3Dobjs = new GameObject[_3DObjs.Length];
        for (int i = 0; i < _3DObjs.Length; i++)
        {
            rotateScript._3Dobjs[i] = _3DObjs[i];
            _3DObjs[i].SetActive(false);
        }
        rotateScript.Init();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && pointerGuide.activeSelf)
        {
            pointerGuide.SetActive(false);
        }
    }

    public void ShowExplainWin(bool isActive)
    {
        explainWin.SetActive(isActive);

        if (isActive)
        {
            // 메뉴.
            var menuManager = manager.menuManager;
            menuManager.backBtn.onClick.RemoveListener(manager.QuitCraftsScene);
            menuManager.backBtnList.Remove("manager.QuitCraftsScene");
            menuManager.backBtn.onClick.RemoveListener(Close3DWin);
            menuManager.backBtnList.Remove("Close3DWin");
            menuManager.backBtn.onClick.AddListener(CloseExplainWin);
            menuManager.backBtnList.Add("CloseExplainWin");

            ChangeUI();
            ShowArrowAndArea(false);
        }
        else
        {
            // 메뉴.
            var menuManager = manager.menuManager;
            menuManager.backBtn.onClick.RemoveListener(CloseExplainWin);
            menuManager.backBtnList.Remove("CloseExplainWin");
            menuManager.backBtn.onClick.AddListener(Close3DWin);
            menuManager.backBtnList.Add("Close3DWin");
        }
    }

    public void CloseExplainWin()
    {
        // 메뉴.
        var menuManager = manager.menuManager;
        menuManager.backBtn.onClick.RemoveListener(CloseExplainWin);
        menuManager.backBtnList.Remove("CloseExplainWin");
        menuManager.backBtn.onClick.AddListener(manager.QuitCraftsScene);
        menuManager.backBtnList.Add("manager.QuitCraftsScene");

        explainWin.SetActive(false);
        ShowArrowAndArea(true);
    }

    public void ShowArrowAndArea(bool isActive)
    {
        arrowGroup.SetActive(isActive);
        areaObj.SetActive(isActive);
    }

    public void SetCurrentIndex(int index)
    {
        manager.currentIndex = index;
    }

    public void ChangeUI()
    {
        // 초기화.
        for (int i = 0; i < explainImgs.Length; i++)
        {
            explainImgs[i].SetActive(false);
        }

        int index = manager.currentIndex;
        explainImgs[index].SetActive(true);
        explainTxt.sprite = explainTxts[index];

        if (manager.isSculpture)
        {
            // 텍스트
            for (int i = 0; i < ccTxts.Length; i++)
            {
                ccTxts[i].SetActive(false);
            }

            if(index == 6) // 해태상
                ccTxts[0].SetActive(true);
            else if (index == 7) // 마동 삼층석탑
                ccTxts[1].SetActive(true);

            // 마크
            for (int i = 0; i < ccImgs2.Length; i++)
            {
                ccImgs2[i].SetActive(false);
            }

            if(index == 6 || index == 7)
                ccImgs2[1].SetActive(true);
            else
                ccImgs2[0].SetActive(true);
        }
    }

    public void ChangeArrow()
    {
        if (manager.isSculpture)
        {
            int index = manager.currentSet;
            switch (index)
            {
                case 0:
                    leftArrow.SetActive(false);
                    rightArrow.SetActive(true);
                    break;
                case 1:
                    leftArrow.SetActive(true);
                    rightArrow.SetActive(true);
                    break;
                case 2:
                    leftArrow.SetActive(true);
                    rightArrow.SetActive(false);
                    break;
            }

            // 초기화.
            for (int i = 1; i < craftArea.Length; i++)
            {
                craftArea[i].SetActive(false);
            }

            for (int i = 3 * index; i < 3 * index + 3; i++)
            {
                craftArea[i].SetActive(true);
            }
        }
        else
        {
            int index = manager.currentSet;
            switch (index)
            {
                case 0:
                    leftArrow.SetActive(false);
                    rightArrow.SetActive(true);
                    break;
                case 1:
                case 2:
                    leftArrow.SetActive(true);
                    rightArrow.SetActive(true);
                    break;
                case 3:
                    leftArrow.SetActive(true);
                    rightArrow.SetActive(false);
                    break;
            }

            for (int i = 3 * index; i < 3 * index + 3; i++)
            {
                craftArea[i].SetActive(true);
            }
        }
    }

    public void ClickArrow(bool isNext)
    {
        var wallMoving = manager.wallMoving;
        if (wallMoving.isMoving) return;

        if (isNext)
        {
            if (manager.currentSet + 1 > manager.setCount) return;
            manager.currentSet += 1;
        }
        else
        {
            if (manager.currentSet - 1 < 0) return;
            manager.currentSet -= 1;
        }

        wallMoving.MovingWall(isNext);

        for(int i=0; i< craftArea.Length; i++)
            craftArea[i].SetActive(false);
    }

    public void Close3DWin()
    {
        Show3DWin(false);
        var soundManager = manager.soundManager;
        soundManager.EndGuideSound();
    }

    public void Show3DWin(bool isActive)
    {
        StartCoroutine(coShow3DWin(isActive));
    }

    private IEnumerator coShow3DWin(bool isActive)
    {
        var fadeController = manager.fadeController;
        yield return StartCoroutine(fadeController.FadeOut(0.5f));

        // 사운드.
        var soundManager = manager.soundManager;
        int index = manager.currentIndex;
        var rotateScript = manager.mouseDragRotate;

        if (!isActive)
        {
            soundManager.bgmSource.volume = 1;
            rotateScript.Stop3DMode();
            rotateScript.ResetValue();
        }
        else
        {
            soundManager.bgmSource.volume = 0.2f;
            if (index == 0)
                soundManager.PlayGuideSound(index);

            rotateScript.Set3DMode(index);
            
            basicWin.SetActive(true);
            detailWin.SetActive(false);
        }

        manager.wallMap.SetActive(!isActive);
        manager._3DLight.SetActive(isActive);
        if (manager.isSculpture)
        {
            manager._3DLight2.SetActive(isActive);

            // 초기화.
            for (int i = 0; i < ccImgs.Length; i++)
            {
                ccImgs[i].SetActive(false);
            }

            if (index == 6 || index == 7) // 해태상, 마동 삼층 석탑
                ccImgs[1].SetActive(true);
            else 
                ccImgs[0].SetActive(true);
        }

        ShowExplainWin(!isActive);
        _3DWin.SetActive(isActive);
        _3DWinBG.SetActive(isActive);
        _3DObjs[index].SetActive(isActive);
        pointerGuide.SetActive(isActive);

        yield return StartCoroutine(fadeController.FadeIn(0.5f));
    }

    // 사운드 버튼 관련.
    public void ActiveSoundAni(bool isActive)
    {
        guideSoundAni.enabled = isActive;
    }

    public void MuteSoundImg()
    {
        ActiveSoundAni(false);

        var lmg = guideSoundAni.gameObject.GetComponent<Image>();
        lmg.sprite = muteSprite;
    }

    public void EndSoundImg()
    {
        ActiveSoundAni(false);

        var lmg = guideSoundAni.gameObject.GetComponent<Image>();
        lmg.sprite = muteSprite;
    }

    public void Active3DWin(bool isActive)
    {
        StartCoroutine(coActive3DWin(isActive));
    }

    private IEnumerator coActive3DWin(bool isActive)
    {
        var fadeController = manager.fadeController;
        yield return StartCoroutine(fadeController.FadeOut(0.5f));

        var menuManager = manager.menuManager;
        if (isActive)
        {
            menuManager.backBtn.onClick.RemoveListener(CloseDetailWin);
            menuManager.backBtnList.Remove("CloseDetailWin");
            menuManager.backBtn.onClick.AddListener(Close3DWin);
            menuManager.backBtnList.Add("Close3DWin");
        }
        else // talk with AI teacher.
        {
            menuManager.backBtn.onClick.RemoveListener(Close3DWin);
            menuManager.backBtnList.Remove("Close3DWin");
            menuManager.backBtn.onClick.AddListener(CloseDetailWin);
            menuManager.backBtnList.Add("CloseDetailWin");
        }

        var rotateScript = manager.mouseDragRotate;
        rotateScript.ResetValue();

        basicWin.SetActive(isActive);
        detailWin.SetActive(!isActive);

        yield return StartCoroutine(fadeController.FadeIn(0.5f));
    }

    private void CloseDetailWin()
    { 
        Active3DWin(true);
    }
}