using System.Collections.Generic;
using UnityEngine;

public class MouseDragRotate : MonoBehaviour
{
    public GameObject[] _3Dobjs;

    // 마우스 움직임.
    private float rotationSpeed = 5f;
    private float returnSpeed = 3f;

    private List<Quaternion> initialRotation = new List<Quaternion>();
    private List<Vector3> initialScale = new List<Vector3>();
    private bool isDragging = false;
    private Vector3 lastMousePosition;

    private float rotX = 0f;
    private float rotY = 0f;

    // 확대 축소.
    private float zoomSpeed = 1f;     // 확대/축소 속도
    private float minScale = 0.5f;    // 최소 크기
    private float maxScale = 4f;      // 최대 크기

    public bool is3DMode = false;
    private GameObject _3DObj;
    private int currentIndex = -1;

    public bool isSculpture = false;

    public void Init()
    {
        for (int i = 0; i < _3Dobjs.Length; i++)
        {
            var rotate = _3Dobjs[i].transform.GetChild(0).transform.rotation;
            initialRotation.Add(rotate);

            var scale = _3Dobjs[i].transform.transform.localScale;
            initialScale.Add(scale);
        }
    }

    public void ResetValue()
    {
        for (int i = 0; i < _3Dobjs.Length; i++)
        {
            _3Dobjs[i].transform.GetChild(0).transform.rotation = initialRotation[i];
            _3Dobjs[i].transform.transform.localScale = initialScale[i];
        }
    }


    void Update()
    {
        if (!is3DMode) return;

        var target = _3DObj.transform.GetChild(0);

        // 물체 움직임.
        // 마우스 드래그 중일 때
        if (isDragging)
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            float rotX = isSculpture? -delta.y * 90 * Time.deltaTime : delta.y * 90 * Time.deltaTime;
            float rotY = -delta.x * 100 * Time.deltaTime;

            // 먼저 월드 기준으로 Y축 회전
            _3DObj.transform.GetChild(0).Rotate(0f, rotY, 0f, Space.World);

            // 그 다음, 로컬 기준으로 X축 회전 (상하)
            _3DObj.transform.GetChild(0).Rotate(rotX, 0f, 0f, Space.Self);

            lastMousePosition = Input.mousePosition;
        }
        else
        {
            // 드래그 중이 아니라면 원래 회전으로 부드럽게 복귀
            target.rotation 
                = Quaternion.Slerp(target.rotation, initialRotation[currentIndex], Time.deltaTime * returnSpeed);
        }

        // 마우스 입력 감지
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // 물체 확대 축소.
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f && _3DObj != null)
        {
            Vector3 scale = _3DObj.transform.localScale;

            // scroll이 양수면 확대, 음수면 축소
            float scaleFactor = 1 + scroll * zoomSpeed; // 예: scroll=0.1 → 1.1배
            scale *= scaleFactor;

            // Clamp 크기 제한
            float clampedX = Mathf.Clamp(scale.x, minScale, maxScale);
            scale = new Vector3(clampedX, clampedX, clampedX);

            _3DObj.transform.localScale = scale;
        }
    }

    public void Set3DMode(int index)
    { 
        currentIndex = index;
        _3DObj = _3Dobjs[currentIndex];
        is3DMode = true;
    }

    public void Stop3DMode()
    {
        is3DMode = false;
    }
}
