using UnityEngine;

public class UIScreenRectFinder : MonoBehaviour
{
    public RectTransform targetUI;

    [ContextMenu("Print Screen Rect")]
    public void GetScreenRect()
    {
        Vector3[] worldCorners = new Vector3[4];
        targetUI.GetWorldCorners(worldCorners);

        Vector2 screenPosMin = RectTransformUtility.WorldToScreenPoint(null, worldCorners[0]);
        Vector2 screenPosMax = RectTransformUtility.WorldToScreenPoint(null, worldCorners[2]);

        float x = screenPosMin.x;
        float y = screenPosMin.y;
        float width = screenPosMax.x - screenPosMin.x;
        float height = screenPosMax.y - screenPosMin.y;

        Rect screenRect = new Rect(x, y, width, height);
        Debug.Log($" UI의 화면 좌표 기준 Rect: {screenRect}");
    }
}
