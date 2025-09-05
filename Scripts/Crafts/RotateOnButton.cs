using UnityEngine;

public class RotateOnButton : MonoBehaviour
{
    public float rotationSpeed = 90f; // 초당 회전 속도 (도)
    private bool isRotating = false;

    void Update()
    {
        if (isRotating)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
    }

    // 버튼에서 이 함수를 호출하면 회전 시작
    public void Rotation()
    {
        isRotating = !isRotating;
    }
}
