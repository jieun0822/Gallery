using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuBarController : MonoBehaviour
{
    public GameObject arrow;
    public GameObject body;
    public RectTransform menuBar;    // ������ �޴��� UI
    public Vector2 hiddenPos;         // ������ ��ġ (��: ȭ�� ��)
    public Vector2 shownPos;          // ������ ��ġ
    public float moveDuration = 0.3f; // �����̴� �ð�

    public bool isShown = false;    // �޴��� �������� �ִ��� ����

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
