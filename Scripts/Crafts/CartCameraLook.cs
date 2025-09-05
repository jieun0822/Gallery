using Unity.Cinemachine;
using UnityEngine;

public class CartCameraLook : MonoBehaviour
{
    public CinemachineSplineCart cart; // ��ġ �̵� ���
    public Transform cameraTransform;  // �ü� ��� ī�޶�
    private float rotationSpeed = 1.5f;

    private Vector3 baseForward;
    private Vector3 baseRight;
    public bool isMoving = false;

    void Start()
    {
        // ���� �� īƮ�� �ʱ� ������ ��������
        baseForward = cart.transform.forward;
        baseRight = cart.transform.right;
    }

    void LateUpdate()
    {
        if (!isMoving) return;

        float pos = cart.SplinePosition;
        Quaternion targetRot;

        // ���� ������ �ü� ����
        if (pos <= 0.0001f) targetRot = Quaternion.LookRotation(baseForward, Vector3.up);  // ����
        else if (pos <= 0.4f) targetRot = Quaternion.LookRotation(-baseRight, Vector3.up);    // ����
        else if (pos <= 0.55f) targetRot = Quaternion.LookRotation(baseForward, Vector3.up);   // ��
        else if (pos <= 0.97f) targetRot = Quaternion.LookRotation(baseRight, Vector3.up);
        else targetRot = Quaternion.LookRotation(-baseForward, Vector3.up);

        // ī�޶�� ��ġ�� īƮ�� ����
        cameraTransform.position = cart.transform.position;

        // ȸ���� ����������
        cameraTransform.rotation = Quaternion.Slerp(
            cameraTransform.rotation,
            targetRot,
            Time.deltaTime * rotationSpeed
        );
    }

    public void SkipCamera()
    {
        cameraTransform.rotation = Quaternion.LookRotation(baseRight, Vector3.up);
    }
}
