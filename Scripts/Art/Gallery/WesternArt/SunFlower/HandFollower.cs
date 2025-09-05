using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class HandFollower : MonoBehaviour
{
    public GameManager manager;

    public XRInferRVM rvm;         // JointsList ������
    public MakeBodyLayer makeBodyLayer;
    public RVMTester2 rvmTester;
    public GameObject bodyLayer;         // bodyLayer ������Ʈ

    //public List<GameObject> hand;
    public List<RectTransform> handImage_R;  // ������ UI �̹��� (RectTransform)
    public List<RectTransform> handImage_L;
    public Canvas handCanvas; // �ش� UI�� ���Ե� ĵ����

    public List<RectTransform> visibleHandImg_R;
    public List<RectTransform> visibleHandImg_L;
    public Canvas visibleHandCanvas;
    public Canvas showAreaCanvas;

    public GameObject[] tailGroup;
    public GameObject tailPrefab;

    public bool isVisibleHand = false;
    public GameEnums.eScene targetScene;
    public bool isLeftHand = false;

    //private void Start()
    //{
    //    tailGroup = new GameObject[handImage.Count];
    //    for (int i = 0; i < handImage.Count; i++)
    //    {
    //        tailGroup[i] = Instantiate(tailPrefab);
    //        tailGroup[i].SetActive(false);
    //    }
    //}

    void Update()
    {
        if ((targetScene != manager.currentScene)) return;

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V))
        {
            isVisibleHand = !isVisibleHand;
            manager.isVisibleHand = isVisibleHand;
        }
            if (manager.currentScene != GameEnums.eScene.None)
        {
            ChangeTransImage();
        }

        if (manager.currentScene == GameEnums.eScene.SunFlower)
        {
           
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C))
            {
                showAreaCanvas.gameObject.SetActive(!showAreaCanvas.gameObject.activeSelf);
            }
        }
    }

    //���ӿ�����Ʈ.
    //private void ChangeTrans()
    //{
    //    List<Vector3> copyList = new List<Vector3>();

    //    for (int i = 0; i < rvm.GetBodyCount; i++)
    //    {
    //        copyList = rvm.JointsList(i);
    //        if (copyList == null || copyList.Count <= 10) return;

    //        if (!IsInsideBodyLayer(i))
    //        {
    //            hand[i].SetActive(false);
    //            return;
    //        }

    //        // �� ���� ��ġ ���
    //        Vector3 normWrist = copyList[9]; //10:������ �ո� 9:���� �ո�
    //        Vector3 normElbow = copyList[7]; //8:������ �Ȳ�ġ 7:���� �Ȳ�ġ
    //        Vector3 offset = (normWrist - normElbow).normalized * 0.3f;
    //        Vector3 normHand = normWrist + offset;

    //        // �� ũ�� ����: �ո�-�Ȳ�ġ �Ÿ�
    //        Vector3 worldWrist = ConvertToWorld(normWrist);
    //        Vector3 worldElbow = ConvertToWorld(normElbow);
    //        float handLength = Vector3.Distance(worldWrist, worldElbow);

    //        // ������ ����: baseScale + �� ���� ��� ����
    //        float baseScale = 0.1f;
    //        float scaleFactor = baseScale + handLength * 0.5f;
    //        hand[i].transform.localScale = Vector3.one * scaleFactor;

    //        // ��ġ ����
    //        hand[i].transform.localPosition = CalculatePos(normHand);
    //        hand[i].SetActive(true);
    //    }

    //    // ���� �� �����
    //    for (int i = rvm.GetBodyCount; i < hand.Count; i++)
    //    {
    //        hand[i].gameObject.SetActive(false);
    //    }
    //}
    private void ChangeTransImage()
    {
        List<Vector3> copyList = new List<Vector3>();
        for (int i = 0; i < rvm.GetBodyCount; i++)
        {
            int index = rvm.BodyIndex(i);
            copyList = rvm.JointsList(index);

            if (copyList == null || copyList.Count <= 10) continue;

            //�̸����� ã��
            GameObject wristPosObj = rvmTester.jointGroup[i][9];
            //GameObject wristPosObj = GameObject.Find("Joint_" + i + "_" + 9);
            var wristPos = CalculatePos( copyList[9]);
            Vector3 normWrist = wristPosObj.transform.position;

            GameObject elbowPosObj = rvmTester.jointGroup[i][7];
            //GameObject elbowPosObj = GameObject.Find("Joint_" + i + "_" + 7);
            var elbowPos = CalculatePos(copyList[7]);
            Vector3 normElbow = elbowPosObj.transform.position;

            // �Ȳ�ġ->�ո� ����
            Vector3 elbowToWrist_R = normWrist - normElbow;

            // �� ��ġ�� �ո񿡼� �Ȳ�ġ �������� �ణ �� ����
            float extendLength = 0.6f;  // �� ��ġ�� �ո񿡼� �̸�ŭ �� ����

            Vector3 pos = normWrist + elbowToWrist_R * extendLength;
            Vector3 normHand = normWrist + elbowToWrist_R * extendLength;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(normHand);

            // Tail Ȱ��ȭ.
            //tailGroup[i].transform.position = normHand;
            //tailGroup[i].SetActive(true);

            // Canvas ���� ��ǥ ���
            Vector2 anchoredPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                handCanvas.transform as RectTransform,
                screenPos,
                Camera.main,
                out anchoredPos
            );

            handImage_R[i].anchoredPosition = anchoredPos;
            handImage_R[i].gameObject.SetActive(true);

            if (isVisibleHand)
            {
                //�� ��ġ �����ִ� ��.
                Vector2 anchoredPos2;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    visibleHandCanvas.transform as RectTransform,
                    screenPos,
                    null,
                    out anchoredPos2
                );
                visibleHandImg_R[i].anchoredPosition = anchoredPos2;
                visibleHandImg_R[i].gameObject.SetActive(true);
            }
        }

        // ������ �� �����
        for (int i = rvm.GetBodyCount; i < handImage_R.Count; i++)
        {
            handImage_R[i].gameObject.SetActive(false);
            if(isVisibleHand) visibleHandImg_R[i].gameObject.SetActive(false);
        }

        // isVisual�� false�� visualHandImg �� ����
        if (!isVisibleHand)
        {
            for (int i = 0; i < visibleHandImg_R.Count; i++)
            {
                visibleHandImg_R[i].gameObject.SetActive(false);
            }
        }

        if (isLeftHand)
        {
            TrackLeftHand();
        }

        if (manager.inactiveHand == null)
            manager.inactiveHand = InActiveHand;

        // ������ tail �����.
        //for (int i = rvm.GetBodyCount; i < tailGroup.Length; i++)
        //{
        //    tailGroup[i].gameObject.SetActive(false);
        //}
    }

    private void TrackLeftHand()
    {
        List<Vector3> copyList = new List<Vector3>();
        for (int i = 0; i < rvm.GetBodyCount; i++)
        {
            int index = rvm.BodyIndex(i);
            copyList = rvm.JointsList(index);

            if (copyList == null || copyList.Count <= 11) continue;

            //�̸����� ã��
            GameObject wristPosObj = rvmTester.jointGroup[i][10];
            var wristPos = CalculatePos(copyList[10]);
            Vector3 normWrist = wristPosObj.transform.position;

            GameObject elbowPosObj = rvmTester.jointGroup[i][8];
            var elbowPos = CalculatePos(copyList[8]);
            Vector3 normElbow = elbowPosObj.transform.position;

            // �Ȳ�ġ->�ո� ����
            Vector3 elbowToWrist_L = normWrist - normElbow;

            // �� ��ġ�� �ո񿡼� �Ȳ�ġ �������� �ణ �� ����
            float extendLength = 0.6f;  // �� ��ġ�� �ո񿡼� �̸�ŭ �� ����

            Vector3 pos = normWrist + elbowToWrist_L * extendLength;
            Vector3 normHand = normWrist + elbowToWrist_L * extendLength;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(normHand);

            // Canvas ���� ��ǥ ���
            Vector2 anchoredPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                handCanvas.transform as RectTransform,
                screenPos,
                Camera.main,
                out anchoredPos
            );

            handImage_L[i].anchoredPosition = anchoredPos;
            handImage_L[i].gameObject.SetActive(true);

            if (isVisibleHand)
            {
                //�� ��ġ �����ִ� ��.
                Vector2 invisible_anchoredPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    visibleHandCanvas.transform as RectTransform,
                    screenPos,
                    null,
                    out invisible_anchoredPos
                );
                visibleHandImg_L[i].anchoredPosition = invisible_anchoredPos;
                visibleHandImg_L[i].gameObject.SetActive(true);
            }
        }

        // ������ �� �����
        for (int i = rvm.GetBodyCount; i < handImage_L.Count; i++)
        {
            handImage_L[i].gameObject.SetActive(false);
            if (isVisibleHand) visibleHandImg_L[i].gameObject.SetActive(false);
        }

        // isVisual�� false�� visualHandImg �� ����
        if (!isVisibleHand)
        {
            for (int i = 0; i < visibleHandImg_L.Count; i++)
            {
                visibleHandImg_L[i].gameObject.SetActive(false);
            }
        }
    }

    public void InActiveHand()
    {
        for (int i = 0; i < visibleHandImg_R.Count; i++)
        {
            visibleHandImg_R[i].gameObject.SetActive(false);
            visibleHandImg_L[i].gameObject.SetActive(false);
        }
    }

    private Vector3 CalculatePos(Vector3 pos)
    {
        Vector3 bPos = bodyLayer.transform.localPosition;
        Vector3 bScale = bodyLayer.transform.localScale;

        float w = bScale.x;
        float h = bScale.z;
        float wh = bScale.x * 0.5f;
        float hh = bScale.z * 0.5f;

        float x = (pos.x * w - wh) * 10.0f;
        float y = (pos.y * h - hh) * 10.0f;

        Vector3 result = new Vector3(bPos.x + x, bPos.y + y, bPos.z);

        return result;
    }

    private Vector3 ConvertToWorld(Vector3 norm)
    {
        Renderer renderer = bodyLayer.GetComponent<Renderer>();
        Vector3 size = renderer.bounds.size;
        Vector3 min = renderer.bounds.min;

        float x = Mathf.Lerp(min.x, min.x + size.x, norm.x);
        float y = Mathf.Lerp(min.y, min.y + size.y, norm.y);
        float z = bodyLayer.transform.position.z;

        return new Vector3(x, y, z);
    }

    private bool IsInsideBodyLayer(int bodyIndex)
    {
        //Renderer renderer = bodyLayer.GetComponent<Renderer>();
        Renderer renderer = makeBodyLayer.bodyLayers[bodyIndex].GetComponent<Renderer>();
        Bounds bounds = renderer.bounds;

        Vector3 min = bounds.min;
        Vector3 max = bounds.max;  // ���� (x, y, z)

        // ����� �ո� ��ǥ�� ������
        Vector3 joint1 = makeBodyLayer.jointLists[bodyIndex][9].transform.position;
        Vector3 joint2 = makeBodyLayer.jointLists[bodyIndex][7].transform.position;

        // X, Y�� ���ؼ� ���� �ȿ� �ִ��� Ȯ��
        bool isInside =
            joint1.x >= min.x && joint1.x <= max.x &&
            joint1.y >= min.y && joint1.y <= max.y;

        isInside = isInside &&
            joint2.x >= min.x && joint2.x <= max.x &&
            joint2.y >= min.y && joint2.y <= max.y;


        //Transform tf = makeBodyLayer.jointLists[bodyIndex][9].transform;
        //Vector3 worldPos = tf.position;
        //bool isInside = bounds.Contains(worldPos);

        //tf = makeBodyLayer.jointLists[bodyIndex][7].transform;
        //worldPos = tf.position;
        //isInside = isInside && bounds.Contains(worldPos);

        return isInside;
    }
}

