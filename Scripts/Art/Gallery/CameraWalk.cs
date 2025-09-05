using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;

public class CameraWalk : MonoBehaviour
{
    [Header("�� �κ�")]
    public CinemachineSplineCart doorCart;
    public SplineContainer[] doorSpline;
    public int doorIndex = -1;

    private bool doorSwitched = false;
    public float doorSpeed = 0.01f;
    public bool isDoorMoving = false;
    public bool isDoorOver = false;

    [Header("������ �κ�")]
    public CinemachineSplineCart cart;
    public SplineContainer[] gallerySpline;
    private bool switched = false;
    public float speed = 0.01f;
    public bool isMoving = false;
    public bool isOver = false;

    [Header("����")]
    public float sideOffset = 2f;
    public bool isCrafts = false;

    void Update()
    {
        if(isDoorMoving) DoorMovingCamera();
        if (isMoving) MovingCamera();
    }

    // ����.
    void LateUpdate()
    {
        if (!isCrafts) return;
        
        // cart ��ġ ��������
        Vector3 basePos = cart.transform.position;
        Vector3 sideDir = cart.transform.right; // ������
        transform.position = basePos + sideDir * sideOffset;
        transform.rotation = cart.transform.rotation;
    }

    // �� ī�޶� ��ŷ.
    public void DoorMovingCamera()
    {
        if (doorCart != null && !doorSwitched)
        {
            doorCart.SplinePosition += Time.deltaTime * doorSpeed;

            if (doorCart.SplinePosition >= 1)
            {
                isDoorMoving = false;
                isDoorOver = true;
                //cart.Spline = nextSpline;    // ���ö��� �ٲٱ�
                //cart.SplinePosition = 0f;          // �� ���ö��ο��� ó������ �̵�
                //switched = true;
                //Debug.Log("���ö��� ��ȯ �Ϸ�!");
            }
        }
        else if (doorSwitched && doorCart.SplinePosition < 1)
        {
            doorCart.SplinePosition += Time.deltaTime;
        }
    }

    public void SetDoorCamera(int index)
    {
        doorIndex = index;
        doorCart.Spline = doorSpline[index];
        doorCart.SplinePosition = 0;
    }

    // ������ ī�޶� ��ŷ.
    public void MovingCamera()
    {
        if (cart != null && !switched)
        {
            cart.SplinePosition += Time.deltaTime * speed;

            if (cart.SplinePosition >= 1)
            {
                isMoving = false;
                isOver = true;
                //cart.Spline = nextSpline;    // ���ö��� �ٲٱ�
                //cart.SplinePosition = 0f;          // �� ���ö��ο��� ó������ �̵�
                //switched = true;
                //Debug.Log("���ö��� ��ȯ �Ϸ�!");
            }
        }
        else if (switched && cart.SplinePosition < 1)
        {
            cart.SplinePosition += Time.deltaTime;
        }
    }

    public void SetCart(int index)
    {
        cart.Spline = gallerySpline[index];
        cart.SplinePosition = 0;
    }
}
