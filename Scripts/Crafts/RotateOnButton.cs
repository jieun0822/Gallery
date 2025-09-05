using UnityEngine;

public class RotateOnButton : MonoBehaviour
{
    public float rotationSpeed = 90f; // �ʴ� ȸ�� �ӵ� (��)
    private bool isRotating = false;

    void Update()
    {
        if (isRotating)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
    }

    // ��ư���� �� �Լ��� ȣ���ϸ� ȸ�� ����
    public void Rotation()
    {
        isRotating = !isRotating;
    }
}
