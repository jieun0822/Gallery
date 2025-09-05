using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class MakeBodyLayer : MonoBehaviour
{
    [Header("[Editor setting]")]
    public XRInferRVM rvm;
    public WebcamLoader webcamLoader;
    public GameObject resultColor;
    public GameObject resultIndex;
    public Transform jointPoolParent;
    public GameObject bodyPool;
    public List<GameObject> bodyLayers = new List<GameObject>();

    [Header("[RVM setting]")]
    public XRInferRVM.ORIENTATIONMODE orientationMode;
    public float minDepth = 1.4f;

    private Vector3 inferScale;
    private Vector3 resultColorPos;
    private Vector3 resultIndexPos;

    [Header("[content setting]")]
    public List<Vector3> bodyDatas = new List<Vector3>();
    public List<Vector3> bodyScales = new List<Vector3>();
  
    //관절.
    public Dictionary<int, List<GameObject>> jointLists = new Dictionary<int, List<GameObject>>();
    private Material sphereMat;

    //private bool preDepth = false;
    public bool useDepth = false;
    private bool isInit = false;

    void Start()
    {
        rvm.OrientationMode = orientationMode;
        rvm.Init();
        rvm.WorkRVM = true;
        rvm.MinDepth = minDepth;

        switch (orientationMode)
        {
            case XRInferRVM.ORIENTATIONMODE.PORTRAIT:
                rvm.InputRT = webcamLoader.PortraitTex;
                inferScale = new Vector3(1.08f, 1, 1.92f);
                resultColorPos = new Vector3(0, 16, 0);
                resultIndexPos = new Vector3(15, 16, 0);
                break;

            case XRInferRVM.ORIENTATIONMODE.LANDSCAPE:
                rvm.InputRT = webcamLoader.LandscapeTex;
                inferScale = new Vector3(1.92f, 1, 1.08f);
                resultColorPos = new Vector3(7.5f, 20, 0);
                resultIndexPos = new Vector3(7.5f, 5, 0);
                break;
        }

        resultColor.GetComponent<Renderer>().material.mainTexture = rvm.BodyColorRT;
        resultColor.transform.localScale = inferScale;
        resultColor.transform.localPosition = resultColorPos;

        resultIndex.GetComponent<Renderer>().material.mainTexture = rvm.BodyIndexRT;
        resultIndex.transform.localScale = inferScale;
        resultIndex.transform.localPosition = resultIndexPos;

        StartCoroutine(coStart());
    }

    private IEnumerator coStart()
    { 
        Init();
        isInit = true;
        yield return null;
    }

    private void Init()
    {
        for (int i = 0; i < bodyLayers.Count; i++)
            bodyLayers[i].SetActive(false);

        for (int i = 0; i < bodyLayers.Count; i++)
        {
            bodyLayers[i].GetComponent<Renderer>().material.SetTexture("_MainTex", rvm.BodyColorRT);
            bodyLayers[i].GetComponent<Renderer>().material.SetTexture("_MaskTex", rvm.BodyIndexRT);
            int index = rvm.BodyIndex(i);
            bodyLayers[i].GetComponent<Renderer>().material.SetInt("_BodyIndex", index);
            //bodyLayer[i].transform.localScale = inferScale;
            bodyLayers[i].SetActive(true);

            //sphereMat = new Material(Shader.Find("Unlit/Color"));
            //sphereMat.color = Color.red;
            Material sphereMat = new Material(Shader.Find("Unlit/Transparent"));
            sphereMat.color = new Color(1f, 0f, 0f, 0f); // 빨강 + 알파 0 (완전 투명)
            sphereMat.renderQueue = (int)RenderQueue.Transparent;

            List<GameObject> joints = new List<GameObject>();

            string parentName = "JointParent_" + i;
            GameObject jointParent = new GameObject(parentName);
            jointParent.transform.SetParent(jointPoolParent, false);
            jointParent.gameObject.SetActive(false);

            for (int j = 0; j < 17; j++)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.name = "Joint_" + j;
                sphere.GetComponent<Renderer>().material = sphereMat;
                sphere.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                sphere.transform.localPosition = Vector3.zero;
                //sphere.transform.parent = bodyLayer[i].transform;
                sphere.transform.SetParent(jointParent.transform, true);
                sphere.SetActive(false);

                joints.Add(sphere);
            }
            jointLists.Add(i, joints);
        }
    }

    public void ChangeBody(GameEnums.eScene scene)
    {
        int positionIndex = -1;
        int scaleIndex = 0;
        switch (scene)
        {
            case GameEnums.eScene.SunFlower:
                positionIndex = 0;
                break;
            case GameEnums.eScene.Room:
                positionIndex = 1;
                break;
            case GameEnums.eScene.Star:
                positionIndex = 2;
                break;
            case GameEnums.eScene.Crow:
                positionIndex = 3;
                break;
            case GameEnums.eScene.None:
                positionIndex = 4;
                scaleIndex = 1;
                break;
        }

        if (positionIndex == -1) return;

        for (int i = 0; i < bodyLayers.Count; i++)
        {
            bodyLayers[i].transform.localPosition = bodyDatas[positionIndex];
            bodyLayers[i].transform.localScale = bodyScales[scaleIndex];
        }
    }

    private void Update()
    {
        if (!isInit) return;

        rvm.UseJoint(true);
        //ShowJoints(true);

        Debug.Log("count : "+rvm.GetBodyCount);
        for (int i = 0; i < rvm.GetBodyCount; i++)
        {
            bodyLayers[i].SetActive(true);

            bodyLayers[i].GetComponent<Renderer>().material.SetTexture("_MainTex", rvm.BodyColorRT);
            bodyLayers[i].GetComponent<Renderer>().material.SetTexture("_MaskTex", rvm.BodyIndexRT);
            int index = rvm.BodyIndex(i);
            bodyLayers[i].GetComponent<Renderer>().material.SetInt("_BodyIndex", index);
            //bodyLayer[i].GetComponent<Renderer>().material.SetInt("_BodyIndex", i);

            ShowJoints(true, i);

            Vector3 pos = bodyLayers[i].transform.localPosition;

            int depthIndex = rvm.BodyIndex(i);
            pos.z = useDepth ? rvm.BodyDepth(depthIndex) : 0f;
            bodyLayers[i].transform.localPosition = pos;
        }

        for (int i = rvm.GetBodyCount; i < bodyLayers.Count; i++)
        {
            bodyLayers[i].GetComponent<Renderer>().material.SetInt("_BodyIndex", -1);
            bodyLayers[i].SetActive(false);
        }
    }

    void UpdateTexture()
    {
        int bodyCnt = rvm.GetBodyCount;
        for (int i = 0; i < bodyCnt; i++)
        {
            int index = rvm.BodyIndex(i);
            bodyLayers[i].GetComponent<Renderer>().material.SetInt("_BodyIndex", index);
        }
        for (int i = bodyCnt; i < 12; i++)
        {
            bodyLayers[i].GetComponent<Renderer>().material.SetInt("_BodyIndex", -1);
        }
    }

    //void ShowJoints(bool on)
    //{
    //    if (on)
    //    {
    //        int bodyCnt = bodyLayers.Count;
    //        float shift = -12.5f;
    //        for (int i = 0; i < bodyCnt; i++)
    //        {
    //            List<Vector3> copyList = rvm.JointsList(i);
    //            if (copyList != null)
    //            {
    //                for (int j = 0; j < 17; j++)
    //                {
    //                    int index = i * 17 + j;
    //                    Vector3 pos = copyList[j];
    //                    Vector3 cPos = CalculatePos(pos);
    //                    cPos.y += i * shift;
    //                    if (useDepth) cPos.z = rvm.BodyDepth(i);
    //                    jointLists[index][j].transform.localPosition = cPos;
    //                    jointLists[index][j].SetActive(true);
    //                }
    //            }
    //        }
    //        for (int i = bodyCnt * 17; i < jointLists.Count; i++)
    //        {
    //            jointLists[i].SetActive(false);
    //        }

    //    }
    //    else
    //    {
    //        foreach (GameObject g in jointLists) g.SetActive(false);
    //    }
    //}

    private Vector3 CalculatePos(Vector3 pos, bool yflip = false)
    {
        Vector3 bPos = bodyLayers[0].transform.localPosition;
        Vector3 bScale = bodyLayers[0].transform.localScale;

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

    void ShowJoints(bool on, int bodyIndex)
    {
        if (on)
        {
            int index = rvm.BodyIndex(bodyIndex);
            List<Vector3> copyList = rvm.JointsList(index);
            
            if (copyList != null)
            {
                for (int i = 0; i < 17; i++)
                {
                    Vector3 pos = copyList[i];
                    Vector3 cPos = CalculatePos(pos, false);
                    if (useDepth) cPos.z = rvm.BodyDepth(i);
                    jointLists[bodyIndex][i].transform.localPosition = cPos;
                    jointLists[bodyIndex][i].SetActive(true);
                }
            }
            else
            {
                foreach (GameObject g in jointLists[bodyIndex]) g.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject g in jointLists[bodyIndex]) g.SetActive(false);
        }
    }

    //private Vector3 CalculatePos(Vector3 pos, int bodyIndex)
    //{
    //    Vector3 bPos = bodyLayer[bodyIndex].transform.localPosition;
    //    Vector3 bScale = bodyLayer[bodyIndex].transform.localScale;

    //    float w = bScale.x;
    //    float h = bScale.z;
    //    float wh = bScale.x * 0.5f;
    //    float hh = bScale.z * 0.5f;

    //    float x = (pos.x * w - wh) * 10.0f;
    //    float y = (pos.y * h - hh) * 10.0f;

    //    Vector3 result = new Vector3(bPos.x + x, bPos.y + y, bPos.z);

    //    return result;
    //}

    void UseBodydepth(bool on, int bodyIndex)
    {

        float depth = rvm.BodyDepth(bodyIndex);

        for (int i = 0; i < bodyLayers.Count; i++)
        {
            Vector3 pos = bodyLayers[i].transform.localPosition;
            if (on)
            {
                pos.z = depth;
            }
            else
            {
                pos.z = 0;
            }

            bodyLayers[i].transform.localPosition = pos;
        }
    }
}
