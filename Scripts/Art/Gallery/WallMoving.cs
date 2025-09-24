using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class WallMoving : MonoBehaviour
{
    public GalleryManager galleryManager;
    public GameObject westernArt_wallObj;
    public GameEnums.eScene scene = GameEnums.eScene.SunFlower;

    private Vector3 startPos;
    public Vector3[] eastArt_targetPos;
    public Vector3[] westernArt_targetPos;
    private Vector3[] class_targetPos;
    private Vector3[] originWall; // 0 : ����ȭ, 1 : �츮�� ������, 2 : ����ȭ

    private float moveDuration = 1f;  // �̵� �ð�
    public bool isMoving = false;
    public int index = 0;

    private void Start()
    {
        originWall = new Vector3[3];
        originWall[0] = westernArt_wallObj.transform.localPosition;
        originWall[1] = galleryManager.ourClass_galleryWall.transform.localPosition;
        originWall[2] = galleryManager.eastArt_galleryWall.transform.localPosition;

       class_targetPos = new Vector3[34];
        for (int i = 0; i < 34; i++)
        {
            class_targetPos[i] = originWall[1] + new Vector3(0, 0, - 5f * i);
        }
    }

    public void ResetWall(int doorIndex)
    {
        index = 0;
        galleryManager.currentSetIndex = 0;
        if (doorIndex == 0)
            westernArt_wallObj.transform.localPosition = originWall[0];
        else if (doorIndex == 1)
            galleryManager.ourClass_galleryWall.transform.localPosition = originWall[1];
        else if (doorIndex == 2)
            galleryManager.eastArt_galleryWall.transform.localPosition = originWall[2];
    }

    public void UpdateWall(int size)
    {
        if (size == 0)
        {
            ResetWall(1);
        }
        else
        {
            int maxSetCount = (size % 3 == 0) ? size / 3 - 1 : size / 3;

            // ���� ���� ���� ���ٸ�
            if (maxSetCount < galleryManager.currentSetIndex)
            {
                if (maxSetCount == 0) // ù��° ���� ������.
                {
                    ResetWall(1);
                }
                else //���� ������ ���� ������.
                {
                    index = maxSetCount;
                    galleryManager.currentSetIndex = maxSetCount;
                    galleryManager.ourClass_galleryWall.transform.localPosition = class_targetPos[index];
                }
            }
            else
            {
                // ���� ���� �ִٸ�
                // ���� ���� ������                
            }
        }
    }

    public void MovingWall(bool isNext)
    {
        if (isMoving) return;

        isMoving = true;
        if(galleryManager.artIndex == 0)
            startPos = westernArt_wallObj.transform.localPosition;
        else if (galleryManager.artIndex == 1)
            startPos = galleryManager.ourClass_galleryWall.transform.localPosition;
        else if (galleryManager.artIndex == 2)
            startPos = galleryManager.eastArt_galleryWall.transform.localPosition;
        index = isNext ? ++index : --index;
        StartCoroutine(coMovingWall(isNext));
    }

    private IEnumerator coMovingWall(bool isNext)
    {
        Transform wallTransform = GetCurrentWall();
        Vector3 startPos = wallTransform.localPosition;
        Vector3 endPos = GetTargetEndPosition(startPos);

        float elapsed = 0f;

        //while (elapsed < moveDuration)
        //{
        //    elapsed += Time.deltaTime;
        //    float t = elapsed / moveDuration;
        //    t = Mathf.SmoothStep(0f, 1f, t);
        //    wallTransform.localPosition = Vector3.Lerp(startPos, endPos, t);
        //    yield return null;
        //}

        //while (elapsed < moveDuration)
        //{
        //    elapsed += Time.deltaTime;
        //    float t = elapsed / moveDuration; // ���� ���� (���)
        //    wallTransform.localPosition = Vector3.Lerp(startPos, endPos, t);
        //    yield return null;
        //}

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;

            // EaseInOut (�ε巴�� ����/��)
            //t = t * t * (3f - 2f * t);
            t = t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;

            wallTransform.localPosition = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        // �̵� �Ϸ�
        wallTransform.localPosition = endPos;
        isMoving = false;
    }

    private Transform GetCurrentWall()
    {
        if (galleryManager.artIndex == 0)
            return westernArt_wallObj.transform;
        else if (galleryManager.artIndex == 1)
            return galleryManager.ourClass_galleryWall.transform;
        else if(galleryManager.artIndex == 2)
            return galleryManager.eastArt_galleryWall.transform;

        return null;
    }

    private Vector3 GetTargetEndPosition(Vector3 startPos)
    {
        if (galleryManager.artIndex == 0)
        {
            if (index >= 0 && index < westernArt_targetPos.Length)
                return westernArt_targetPos[index];
            else
                return startPos;
        }
        else if (galleryManager.artIndex == 1)
        {
            if (index >= 0 && index < class_targetPos.Length)
                return class_targetPos[index];
            else
                return startPos;
        }
        else if (galleryManager.artIndex == 2)
        {
            if (index >= 0 && index < eastArt_targetPos.Length)
                return eastArt_targetPos[index];
            else
                return startPos;
        }

        return Vector3.zero;
    }
}
