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

    public bool isSculpture = false;

    void Start()
    {
        // ���� �� īƮ�� �ʱ� ������ ��������
        baseForward = cart.transform.forward;
        baseRight = cart.transform.right;
        cameraTransform.rotation = Quaternion.LookRotation(-baseRight, Vector3.up);
    }

    void LateUpdate()
    {
        if (!isMoving) return;

        float pos = cart.SplinePosition;
        Quaternion targetRot;

        if (isSculpture)
        {
            if (pos >= 0.5f && pos < 0.6f) targetRot = Quaternion.LookRotation(baseForward, Vector3.up);    
            else if (pos >= 0.6f) targetRot = Quaternion.LookRotation(baseRight, Vector3.up);
            else targetRot = Quaternion.LookRotation(-baseRight, Vector3.up);

            // ī�޶�� ��ġ�� īƮ�� ����
            cameraTransform.position = cart.transform.position;

            // ȸ���� ����������
            cameraTransform.rotation = Quaternion.Slerp(
                cameraTransform.rotation,
                targetRot,
                Time.deltaTime * rotationSpeed
            );
        }
        else
        {
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
       
    }

    public void SkipCamera()
    {
        cameraTransform.rotation = Quaternion.LookRotation(baseRight, Vector3.up);
    }
}
