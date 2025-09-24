using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CraftsManager : MonoBehaviour
{
    [Header("스크립트")]
    public CraftsSoundManager soundManager;
    public CraftsSceneUIManager uiManager;
    public MouseDragRotate mouseDragRotate;
    public MenuManager menuManager;
    public FadeController fadeController;
    public CameraWalk cameraWalk;
    public CartCameraLook cameraLook;
    public CraftsWallMoving wallMoving;

    [Header("전시관")]
    public GameObject walkMap; 
    public GameObject wallMap;
    public GameObject walkCamera;
    public GameObject wallCamera;
    public GameObject _3DLight;
    public GameObject _3DLight2;
   
    public int currentSet = 0;
    public int setCount = -1;
    public int currentIndex;
    public bool isSculpture = false;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        soundManager.PlayBGM();

        uiManager.manager = this;
        soundManager.manager = this;
        wallMoving.manager = this;

        currentSet = 0;
        if (isSculpture) setCount = 2;
        else setCount = 3;
        currentIndex = 0;

        StartCoroutine(coWalkMap());
    }

    private void Update()
    {
        if (cameraWalk.isOver)
        {
            cameraWalk.isOver = false;

            menuManager.nextBtn.onClick.RemoveListener(SkipWalkMap);
            var skipBtn = uiManager.skipBtn;
            skipBtn.gameObject.SetActive(false);
            skipBtn.onClick.RemoveListener(SkipWalkMap);

            StartCoroutine(coShowWall());
        }
    }

    private IEnumerator coWalkMap()
    {
        yield return StartCoroutine(fadeController.FadeOut(0f));
        
       
        walkMap.SetActive(true);
        walkCamera.SetActive(true);
        yield return StartCoroutine(fadeController.FadeIn(1f));
        yield return new WaitForSeconds(1f);

        menuManager.nextBtn.onClick.AddListener(SkipWalkMap);
        var skipBtn = uiManager.skipBtn;
        skipBtn.gameObject.SetActive(true);
        skipBtn.onClick.AddListener(SkipWalkMap);

        cameraLook.isMoving = true;
        cameraWalk.isMoving = true;
    }

    private void SkipWalkMap()
    {
        cameraLook.SkipCamera();
        cameraWalk.cart.SplinePosition =1;
    }

    private IEnumerator coShowWall()
    {
        yield return new WaitForSeconds(0.8f);
        
        cameraLook.isMoving = false;
        yield return new WaitForSeconds(0.2f);

        yield return StartCoroutine(fadeController.FadeOut(0.5f));
        walkMap.SetActive(false);
        walkCamera.SetActive(false);
        wallMap.SetActive(true);
        wallCamera.SetActive(true);
        uiManager.ChangeArrow();
      
        yield return StartCoroutine(fadeController.FadeIn(0.5f));
    }

    public void QuitCraftsScene()
    {
        string sceneName = "Sample_RVM 5";
        SceneManager.LoadScene(sceneName);
    }
}