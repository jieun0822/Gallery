using System.Collections.Generic;
using UnityEngine;

public class MouseDragRotate : MonoBehaviour
{
    public GameObject[] _3Dobjs;

    // ���콺 ������.
    private float rotationSpeed = 5f;
    private float returnSpeed = 3f;

    private List<Quaternion> initialRotation = new List<Quaternion>();
    private List<Vector3> initialScale = new List<Vector3>();
    private bool isDragging = false;
    private Vector3 lastMousePosition;

    private float rotX = 0f;
    private float rotY = 0f;

    // Ȯ�� ���.
    private float zoomSpeed = 1f;     // Ȯ��/��� �ӵ�
    private float minScale = 0.5f;    // �ּ� ũ��
    private float maxScale = 4f;      // �ִ� ũ��

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

        // ��ü ������.
        // ���콺 �巡�� ���� ��
        if (isDragging)
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            float rotX = isSculpture? -delta.y * 90 * Time.deltaTime : delta.y * 90 * Time.deltaTime;
            float rotY = -delta.x * 100 * Time.deltaTime;

            // ���� ���� �������� Y�� ȸ��
            _3DObj.transform.GetChild(0).Rotate(0f, rotY, 0f, Space.World);

            // �� ����, ���� �������� X�� ȸ�� (����)
            _3DObj.transform.GetChild(0).Rotate(rotX, 0f, 0f, Space.Self);

            lastMousePosition = Input.mousePosition;
        }
        else
        {
            // �巡�� ���� �ƴ϶�� ���� ȸ������ �ε巴�� ����
            target.rotation 
                = Quaternion.Slerp(target.rotation, initialRotation[currentIndex], Time.deltaTime * returnSpeed);
        }

        // ���콺 �Է� ����
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // ��ü Ȯ�� ���.
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f && _3DObj != null)
        {
            Vector3 scale = _3DObj.transform.localScale;

            // scroll�� ����� Ȯ��, ������ ���
            float scaleFactor = 1 + scroll * zoomSpeed; // ��: scroll=0.1 �� 1.1��
            scale *= scaleFactor;

            // Clamp ũ�� ����
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
