using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Video;

public class HandsUpDectector : MonoBehaviour
{
    public XRInferRVM rvm;
    public GameManager gameManager;
    public MakeBodyLayer makeBodyLayer;
    public RVMTester2 rvmTester;
    public GameObject bodyLayer;
    public SpineUIController spine;

    [Header("별 씬")]
    public GameObject starBg;
    public VideoPlayer[] windVideo;
    private bool isReadyStar = false;
    private bool isPlayStar = false;

    [Header("까마귀 씬")]
    public GameObject[] bgObjs;
    public VideoPlayer[] fieldVideo;
    public VideoPlayer crowVideo;

    private GameEnums.eScene prevScene = GameEnums.eScene.None;

    private bool isPlayCrow = false;

    void Update()
    {
        if (prevScene != gameManager.currentScene)
        {
            // 초기화.
            if (prevScene == GameEnums.eScene.Star)
            {
                for (int i = 0; i < windVideo.Length; i++)
                {
                    var renderer = windVideo[i].GetComponent<Renderer>();
                    renderer.enabled = false;
                }
            }
            else if (prevScene == GameEnums.eScene.Crow)
            {
                for (int i = 0; i < fieldVideo.Length; i++)
                {
                    var renderer = fieldVideo[i].GetComponent<Renderer>();
                    renderer.enabled = false;
                }
                for(int i=0; i< bgObjs.Length; i++)
                    bgObjs[i].SetActive(false);

                var crowRenderer = crowVideo.GetComponent<Renderer>();
                crowRenderer.enabled = false;
            }
            
            else if(gameManager.currentScene == GameEnums.eScene.Crow) StartCoroutine(coPlayFieldVideo());

            prevScene = gameManager.currentScene;
        }

        if (gameManager.currentScene == GameEnums.eScene.None) return;
        if (rvm == null || rvm.GetBodyCount == 0) return;

        // 동작.
        if (gameManager.currentScene == GameEnums.eScene.Star)
        {
            if (!isReadyStar) StartCoroutine(coReadyWind());
            if (isReadyStar)
            {
                CheckHandUp();
            }
        }
        else if (gameManager.currentScene == GameEnums.eScene.Crow && gameManager.isCrowScene)
        {
            if (!isPlayCrow && CheckHi())
            {
                StartCoroutine(coPlayCrowVideo());
            }
        }
    }

    private IEnumerator coPlayFieldVideo()
    {
        for (int i = 0; i < fieldVideo.Length; i++)
        {
            var renderer = fieldVideo[i].GetComponent<Renderer>();
            renderer.enabled = false;
        }
         for (int i = 0; i < bgObjs.Length; i++)
            bgObjs[i].SetActive(false);

        for (int i = 0; i < fieldVideo.Length; i++)
        {
            fieldVideo[i].Prepare();
        }

        foreach (var vp in fieldVideo)
        {
            while (!vp.isPrepared)
                yield return null;
        }

        for (int i = 0; i < fieldVideo.Length; i++)
        {
            fieldVideo[i].Play();
        }

        for (int i = 0; i < fieldVideo.Length; i++)
        {
            var renderer = fieldVideo[i].GetComponent<Renderer>();
            renderer.enabled = true;
        }
        for (int i = 0; i < bgObjs.Length; i++)
            bgObjs[i].SetActive(true);
    }


    private void CheckHandUp()
    {
        int totalBodies = rvm.GetBodyCount;
        int handsUpCount = 0;

        for (int i = 0; i < totalBodies; i++)
        {
            if (IsHandsUp(i)) handsUpCount++;
        }

        int threshold = (totalBodies % 2 == 0) ? totalBodies / 2 : totalBodies / 2 + 1;
        // 과반수 체크 (절반 초과일 경우 true)
        if (handsUpCount >= threshold)
        {
            gameManager.soundManager.PlayEffectSound(gameManager.currentScene, 1);

            spine.MoveTree();
            if (!isPlayStar) StartCoroutine(coPlayWind());
        }
        else
        {
            gameManager.soundManager.StopWindSound();

            spine.StopTree();
            for (int i = 0; i < windVideo.Length; i++)
            {
                if (windVideo[i].isPlaying)
                {
                    var renderer = windVideo[i].GetComponent<Renderer>();
                    renderer.enabled = false;

                    windVideo[i].Stop();
                }
            }
            spine.isTreeMoving = false;
        }
    }

    private IEnumerator coReadyWind()
    {
        for (int i = 0; i < windVideo.Length; i++)
        {
            var renderer = windVideo[i].GetComponent<Renderer>();
            renderer.enabled = false;
        }

        for (int i = 0; i < windVideo.Length; i++)
        {
            windVideo[i].Prepare();
        }

        foreach (var vp in windVideo)
        {
            while (!vp.isPrepared)
                yield return null;
        }

        for (int i = 0; i < windVideo.Length; i++)
        {
            var renderer = windVideo[i].GetComponent<Renderer>();
            renderer.enabled = true;
        }

        isReadyStar = true;
    }

    private IEnumerator coPlayWind()
    {
        isPlayStar = true;
        for (int i = 0; i < windVideo.Length; i++)
        {
            windVideo[i].Prepare();
        }

        foreach (var vp in windVideo)
        {
            while (!vp.isPrepared)
                yield return null;
        }

        for (int i = 0; i < windVideo.Length; i++)
        {
            windVideo[i].Play();
        }

        for (int i = 0; i < windVideo.Length; i++)
        {
            var renderer = windVideo[i].GetComponent<Renderer>();
            renderer.enabled = true;
        }

        isPlayStar = false;
    }

    private bool CheckHi()
    {
        int totalBodies = rvm.GetBodyCount;
        int handsUpCount = 0;

        for (int i = 0; i < totalBodies; i++)
        {
            if (IsHi(i))
            {
                handsUpCount++;
            }
        }

        int threshold = (totalBodies % 2 == 0) ? totalBodies / 2 : totalBodies / 2 + 1;
        // 과반수 체크 (절반 초과일 경우 true)
        if (handsUpCount >= threshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator coPlayCrowVideo()
    {
        isPlayCrow = true;
        
        var renderer = crowVideo.GetComponent<Renderer>();
        renderer.enabled = false;

        crowVideo.Prepare();
        
        while (!crowVideo.isPrepared)
            yield return null;

        gameManager.soundManager.PlayEffectSound(gameManager.currentScene);

        crowVideo.Play();
        renderer.enabled = true;

        yield return new WaitForSeconds(10f);
        isPlayCrow = false;
    }

    bool IsHandsUp(int bodyIndex)
    {
        int index = rvm.BodyIndex(bodyIndex);
        if (rvm == null || rvm.JointsList(index) == null) return false;

        List<Vector3> joints = rvm.JointsList(index);

        if (joints.Count < 11) return false;
        //if (!IsInsideBodyLayer(bodyIndex, new List<int> { 5, 6, 7, 8, 9, 10 })) return false;

        // 관절 좌표 추출
        //손목
        //Vector3 leftWrist = makeBodyLayer.jointLists[bodyIndex][9].transform.position;
        //Vector3 rightWrist = makeBodyLayer.jointLists[bodyIndex][10].transform.position;
        ////팔꿈치
        //Vector3 leftElbow = makeBodyLayer.jointLists[bodyIndex][7].transform.position;
        //Vector3 rightElbow = makeBodyLayer.jointLists[bodyIndex][8].transform.position;
        ////어깨
        //Vector3 leftShoulder = makeBodyLayer.jointLists[bodyIndex][5].transform.position;
        //Vector3 rightShoulder = makeBodyLayer.jointLists[bodyIndex][6].transform.position;
        List<Vector3> copyList = rvm.JointsList(index);
        
        
        //손목
        //GameObject leftWristObj = GameObject.Find("Joint_" + bodyIndex + "_" + 9);
        GameObject leftWristObj = rvmTester.jointGroup[bodyIndex][9];
        //Vector3 leftWrist = copyList[9];
        //GameObject rightWristObj = GameObject.Find("Joint_" + bodyIndex + "_" + 10);
        GameObject rightWristObj = rvmTester.jointGroup[bodyIndex][10];
        //Vector3 rightWrist = copyList[10];
        //팔꿈치
        //GameObject leftElbowObj = GameObject.Find("Joint_" + bodyIndex + "_" + 7);
        GameObject leftElbowObj = rvmTester.jointGroup[bodyIndex][7];
        //Vector3 leftElbow = copyList[7];
        //GameObject rightElbowObj = GameObject.Find("Joint_" + bodyIndex + "_" + 8);
        GameObject rightElbowObj = rvmTester.jointGroup[bodyIndex][8];
        //Vector3 rightElbow = copyList[8];
        //어깨
        //GameObject leftShoulderObj = GameObject.Find("Joint_" + bodyIndex + "_" + 5);
        GameObject leftShoulderObj = rvmTester.jointGroup[bodyIndex][5];
        //Vector3 leftShoulder = copyList[5];
        //GameObject rightShoulderObj = GameObject.Find("Joint_" + bodyIndex + "_" + 6);
        GameObject rightShoulderObj = rvmTester.jointGroup[bodyIndex][6];
        //Vector3 rightShoulder = copyList[6];

        Vector3 elbowToWrist_L = leftWristObj.transform.position - leftElbowObj.transform.position;
        Vector3 elbowToWrist_R = rightWristObj.transform.position - rightElbowObj.transform.position;

        // 손 위치를 손목에서 팔꿈치 방향으로 약간 더 연장
        float extendLength = 0.6f;  // 손 위치를 손목에서 이만큼 더 뻗음

        Vector3 normHand_L = leftWristObj.transform.position + elbowToWrist_L * extendLength;
        Vector3 normHand_R = rightWristObj.transform.position + elbowToWrist_R * extendLength;

        // 만세 조건: 손이 어깨보다 위
        bool leftArmUp = normHand_L.y > leftShoulderObj.transform.position.y + 0.1f;
        bool rightArmUp = normHand_R.y > rightShoulderObj.transform.position.y + 0.1f;

        return leftArmUp && rightArmUp;
    }

    bool IsHi(int bodyIndex)
    {
        if (rvm == null || rvm.JointsList(bodyIndex) == null) return false;

        List<Vector3> joints = rvm.JointsList(bodyIndex);

        if (joints.Count < 11) return false;
        //if (!IsInsideBodyLayer(bodyIndex, new List<int> { 5, 7, 9})) return false;
        // 관절 좌표 추출
        //손목
        //Vector3 rightWrist = makeBodyLayer.jointLists[bodyIndex][9].transform.position;
        //GameObject rightWristObj = GameObject.Find("Joint_" + bodyIndex + "_" + 9);
        GameObject rightWristObj = rvmTester.jointGroup[bodyIndex][9];
        //팔꿈치
        //Vector3 rightElbow = makeBodyLayer.jointLists[bodyIndex][7].transform.position;
        //GameObject rightElbowObj = GameObject.Find("Joint_" + bodyIndex + "_" + 7);
        GameObject rightElbowObj = rvmTester.jointGroup[bodyIndex][7];
        //어깨
        //Vector3 rightShoulder = makeBodyLayer.jointLists[bodyIndex][5].transform.position;
        //GameObject rightShoulderObj = GameObject.Find("Joint_" + bodyIndex + "_" + 5);
        GameObject rightShoulderObj = rvmTester.jointGroup[bodyIndex][5];

        Vector3 elbowToWrist_R = rightWristObj.transform.position - rightElbowObj.transform.position;

        // 손 위치를 손목에서 팔꿈치 방향으로 약간 더 연장
        float extendLength = 0.6f;  // 손 위치를 손목에서 이만큼 더 뻗음

        Vector3 normHand_R = rightWristObj.transform.position + elbowToWrist_R * extendLength;

        // 인사 조건: 손이 어깨보다 위
        bool rightArmUp = normHand_R.y > rightShoulderObj.transform.position.y + 0.1f;

        return rightArmUp;
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

    private bool IsInsideBodyLayer(int bodyIndex, List<int> jointIndex)
    {
        Renderer renderer = makeBodyLayer.bodyLayers[bodyIndex].GetComponent<Renderer>();
        Bounds bounds = renderer.bounds;

        Vector3 min = bounds.min;
        Vector3 max = bounds.max;  // 끝점 (x, y, z)

        bool isInside = true;
        for (int i = 0; i < jointIndex.Count; i++)
        {
            Vector3 joint = makeBodyLayer.jointLists[bodyIndex][jointIndex[i]].transform.position;

            bool inside =
                joint.x >= min.x && joint.x <= max.x &&
                joint.y >= min.y && joint.y <= max.y;

            isInside = isInside && inside;

            // 조기 탈출도 가능:
            if (!isInside) break;
        }

        return isInside;
    }

    private Vector3 CalculatePos(Vector3 pos, bool yflip = false)
    {
        Vector3 bPos = bodyLayer.transform.localPosition;
        Vector3 bScale = bodyLayer.transform.localScale;

        float w = bScale.x;
        float h = bScale.z;
        float wh = bScale.x * 0.5f;
        float hh = bScale.z * 0.5f;

        float x = (pos.x * w - wh) * 10.0f;

        float y = (pos.y * h - hh) * 10.0f;
        if (yflip) y = ((1.0f - pos.y) * h - hh) * 10.0f;

        Vector3 result = new Vector3(bPos.x + x, bPos.y + y, bPos.z);

        return result;
    }
}
