using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;

public class CameraWalk : MonoBehaviour
{
    [Header("문 부분")]
    public CinemachineSplineCart doorCart;
    public SplineContainer[] doorSpline;
    public int doorIndex = -1;

    private bool doorSwitched = false;
    public float doorSpeed = 0.01f;
    public bool isDoorMoving = false;
    public bool isDoorOver = false;

    [Header("갤러리 부분")]
    public CinemachineSplineCart cart;
    public SplineContainer[] gallerySpline;
    private bool switched = false;
    public float speed = 0.01f;
    public bool isMoving = false;
    public bool isOver = false;

    [Header("공예")]
    public float sideOffset = 2f;
    public bool isCrafts = false;

    void Update()
    {
        if(isDoorMoving) DoorMovingCamera();
        if (isMoving) MovingCamera();
    }

    // 공예.
    void LateUpdate()
    {
        if (!isCrafts) return;
        
        // cart 위치 가져오기
        Vector3 basePos = cart.transform.position;
        Vector3 sideDir = cart.transform.right; // 옆방향
        transform.position = basePos + sideDir * sideOffset;
        transform.rotation = cart.transform.rotation;
    }

    // 문 카메라 워킹.
    public void DoorMovingCamera()
    {
        if (doorCart != null && !doorSwitched)
        {
            doorCart.SplinePosition += Time.deltaTime * doorSpeed;

            if (doorCart.SplinePosition >= 1)
            {
                isDoorMoving = false;
                isDoorOver = true;
                //cart.Spline = nextSpline;    // 스플라인 바꾸기
                //cart.SplinePosition = 0f;          // 새 스플라인에서 처음으로 이동
                //switched = true;
                //Debug.Log("스플라인 전환 완료!");
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

    // 갤러리 카메라 워킹.
    public void MovingCamera()
    {
        if (cart != null && !switched)
        {
            cart.SplinePosition += Time.deltaTime * speed;

            if (cart.SplinePosition >= 1)
            {
                isMoving = false;
                isOver = true;
                //cart.Spline = nextSpline;    // 스플라인 바꾸기
                //cart.SplinePosition = 0f;          // 새 스플라인에서 처음으로 이동
                //switched = true;
                //Debug.Log("스플라인 전환 완료!");
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
