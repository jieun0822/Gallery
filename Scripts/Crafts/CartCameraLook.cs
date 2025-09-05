using Unity.Cinemachine;
using UnityEngine;

public class CartCameraLook : MonoBehaviour
{
    public CinemachineSplineCart cart; // 위치 이동 담당
    public Transform cameraTransform;  // 시선 담당 카메라
    private float rotationSpeed = 1.5f;

    private Vector3 baseForward;
    private Vector3 baseRight;
    public bool isMoving = false;

    void Start()
    {
        // 시작 시 카트의 초기 방향을 기준으로
        baseForward = cart.transform.forward;
        baseRight = cart.transform.right;
    }

    void LateUpdate()
    {
        if (!isMoving) return;

        float pos = cart.SplinePosition;
        Quaternion targetRot;

        // 비율 구간별 시선 변경
        if (pos <= 0.0001f) targetRot = Quaternion.LookRotation(baseForward, Vector3.up);  // 시작
        else if (pos <= 0.4f) targetRot = Quaternion.LookRotation(-baseRight, Vector3.up);    // 왼쪽
        else if (pos <= 0.55f) targetRot = Quaternion.LookRotation(baseForward, Vector3.up);   // 앞
        else if (pos <= 0.97f) targetRot = Quaternion.LookRotation(baseRight, Vector3.up);
        else targetRot = Quaternion.LookRotation(-baseForward, Vector3.up);

        // 카메라는 위치만 카트를 따라감
        cameraTransform.position = cart.transform.position;

        // 회전은 독립적으로
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
