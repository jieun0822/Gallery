using Spine.Unity;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int index;
    public IntroManager introManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        introManager.ClickBuilding(index, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        introManager.ClickBuilding(index, false);
    }
}
