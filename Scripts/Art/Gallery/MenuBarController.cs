using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuBarController : MonoBehaviour
{
    public GameObject arrow;
    public GameObject body;
    public RectTransform menuBar;    // 움직일 메뉴바 UI
    public Vector2 hiddenPos;         // 숨겨진 위치 (예: 화면 밖)
    public Vector2 shownPos;          // 보여질 위치
    public float moveDuration = 0.3f; // 움직이는 시간

    public bool isShown = false;    // 메뉴가 보여지고 있는지 상태

    private bool isMoving = false;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        Image img = arrow.GetComponent<Image>();
        img.enabled = true; 
        img.raycastTarget = false; 

        Image bodyImg = body.GetComponent<Image>();
        bodyImg.enabled = false;
        img.raycastTarget = true;
    }

    public void ToggleMenu()
    {
        if (isMoving) return;
        isMoving = true;

        if (isShown)
        {
            StartCoroutine(MoveMenu(menuBar.anchoredPosition, hiddenPos, isShown));
        }
        else
        {
            StartCoroutine(MoveMenu(menuBar.anchoredPosition, shownPos, isShown));
        }
    }

    private IEnumerator MoveMenu(Vector2 from, Vector2 to, bool isActive)
    {
        if (!isActive)
        {
            Image img = arrow.GetComponent<Image>();
            img.enabled = false; 
            img.raycastTarget = true; 

            Image bodyImg = body.GetComponent<Image>();
            bodyImg.enabled = true;
            img.raycastTarget = false;
        }

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            menuBar.anchoredPosition = Vector2.Lerp(from, to, elapsed / moveDuration);
            yield return null;
        }
        menuBar.anchoredPosition = to;

        if (isActive)
        {
            Init();
        }

        isShown = !isShown;
        isMoving = false; 
    }
}
