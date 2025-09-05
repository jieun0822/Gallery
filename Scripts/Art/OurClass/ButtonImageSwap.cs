using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonImageSwap : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image targetImage;
    public Sprite normalSprite;
    public Sprite pressedSprite;
    public CustomBtn targetButton;

    // ���콺�� �÷��� �� (���̶���Ʈ)
    public void OnPointerEnter(PointerEventData eventData)
    {
        targetButton.SetVisualState("highlighted");
    }

    // ���콺�� ����� ��
    public void OnPointerExit(PointerEventData eventData)
    {
        targetButton.SetVisualState("normal");
    }

    // ��ư ������ ��
    public void OnPointerDown(PointerEventData eventData)
    {
        if (targetImage != null && pressedSprite != null)
            targetImage.sprite = pressedSprite;

        targetButton.SetVisualState("pressed");
    }

    // ��ư ���� ��
    public void OnPointerUp(PointerEventData eventData)
    {
        if (targetImage != null && normalSprite != null)
            targetImage.sprite = normalSprite;

        // ���콺�� ������ ��ư ���� ������ Highlight ����
        if (RectTransformUtility.RectangleContainsScreenPoint(
            targetButton.GetComponent<RectTransform>(),
            eventData.position,
            eventData.enterEventCamera))
        {
            targetButton.SetVisualState("highlighted");
        }
        else
        {
            targetButton.SetVisualState("normal");
        }
    }

    
}
