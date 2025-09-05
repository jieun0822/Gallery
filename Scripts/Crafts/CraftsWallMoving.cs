using System.Collections;
using UnityEngine;

public class CraftsWallMoving : MonoBehaviour
{
    public CraftsManager manager;
    public GameObject[] wallObj;
    //public GameObject flooroObj;

    public Vector3[] crafts_targetPos;
    private Vector3[] originWall; // 0 : 공예, 1 : 조각
    //private Vector3 originFloor;

    private float moveDuration = 1f;  // 이동 시간
    public bool isMoving = false;
    public int index = 0;

    private void Start()
    {
        originWall = new Vector3[1];
        originWall[0] = wallObj[0].transform.localPosition;
        //originFloor = flooroObj.transform.localPosition;

        crafts_targetPos = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            crafts_targetPos[i] = originWall[0] + new Vector3(7f * i, 0, 0);
        }
    }

    public void MovingWall(bool isNext)
    {
        if (isMoving) return;

        if (isNext)
        {
            if (index + 1 > crafts_targetPos.Length - 1) return;
            index += 1;
        }
        else
        {
            if (index - 1 < 0 ) return;
            index -= 1;
        }

        isMoving = true;
        StartCoroutine(coMovingWall(isNext));
    }

    private IEnumerator coMovingWall(bool isNext)
    {
        Transform wallTransform = wallObj[0].transform;
        Vector3 startPos = wallTransform.localPosition;
        //Vector3 startFloorPos = flooroObj.transform.localPosition;
        Vector3 endPos = GetTargetEndPosition(startPos);

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;
            t = Mathf.SmoothStep(0f, 1f, t);
            wallTransform.localPosition = Vector3.Lerp(startPos, endPos, t);
            //flooroObj.transform.localPosition = Vector3.Lerp(startFloorPos, endPos, t);
            yield return null;
        }

        // 이동 완료
        wallTransform.localPosition = endPos;
        //flooroObj.transform.localPosition = endPos;
        isMoving = false;
        var uiManager = manager.uiManager;
        uiManager.ChangeArrow();
    }

    private Transform GetCurrentWall()
    {
        //if (galleryManager.artIndex == 0)
        //    return westernArt_wallObj.transform;
        //else if (galleryManager.artIndex == 1)
        //    return galleryManager.ourClass_galleryWall.transform;
        //else if (galleryManager.artIndex == 2)
        //    return galleryManager.eastArt_galleryWall.transform;

        return null;
    }

    private Vector3 GetTargetEndPosition(Vector3 startPos)
    {
        if (index >= 0 && index < crafts_targetPos.Length)
            return crafts_targetPos[index];
        else
            return startPos;
    }
}
