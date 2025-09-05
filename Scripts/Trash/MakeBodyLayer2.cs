using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class MakeBodyLayer2 : MonoBehaviour
{
    public XRInferRVM rvm;
    public GameObject resultColor;
    public GameObject resultIndex;
    public Transform jointPoolParent;
    public List<GameObject> bodyLayer = new List<GameObject>();

    //°üÀý.
    public Dictionary<int, List<GameObject>> jointLists = new Dictionary<int, List<GameObject>>();
    private Material sphereMat;

    private bool preDepth = false;
    public bool curDepth = false;

    private bool isLoad = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(coStart());
    }

    private IEnumerator coStart()
    {
        rvm.Init();
        rvm.WorkRVM = true;

        yield return null;
        Init();
    }

    //private void Update()
    //{
    //    Init();
    //}

    private void Init()
    {
        jointLists.Clear();

        for (int i = 0; i < bodyLayer.Count; i++)
            bodyLayer[i].SetActive(false);

        //resultColor.GetComponent<Renderer>().material.mainTexture = rvm.BodyColorRT;
        //resultIndex.GetComponent<Renderer>().material.mainTexture = rvm.BodyIndexRT;

        for (int i = 0; i < bodyLayer.Count; i++)
        {
            bodyLayer[i].GetComponent<Renderer>().material.SetTexture("_MainTex", rvm.BodyColorRT);
            bodyLayer[i].GetComponent<Renderer>().material.SetTexture("_MaskTex", rvm.BodyIndexRT);
            bodyLayer[i].SetActive(true);
            bodyLayer[i].GetComponent<Renderer>().material.SetInt("_BodyIndex", i);

            sphereMat = new Material(Shader.Find("Unlit/Color"));
            sphereMat.color = Color.red;
            sphereMat.renderQueue = (int)RenderQueue.Transparent;

            List<GameObject> joints = new List<GameObject>();

            string parentName = "JointParent_" + i;
            GameObject jointParent = new GameObject(parentName);
            jointParent.transform.SetParent(jointPoolParent, false);

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
        isLoad = true;
    }

    private void Update()
    {
        if (!isLoad) return;

        rvm.UseJoint(true);
        for (int i = 0; i < rvm.GetBodyCount; i++)
        {
            bodyLayer[i].GetComponent<Renderer>().material.SetTexture("_MainTex", rvm.BodyColorRT);
            bodyLayer[i].GetComponent<Renderer>().material.SetTexture("_MaskTex", rvm.BodyIndexRT);
            bodyLayer[i].GetComponent<Renderer>().material.SetInt("_BodyIndex", i);
            bodyLayer[i].SetActive(true);

            ShowJoints(true, i);
            UseBodydepth(true, i);
        }

        if (preDepth != curDepth)
        {
            for (int i = 0; i < rvm.GetBodyCount; i++)
            {
                UseBodydepth(curDepth, i);
            }
            preDepth = curDepth;
        }

        for (int i = rvm.GetBodyCount; i < bodyLayer.Count; i++)
            bodyLayer[i].SetActive(false);

        //rvm.UseJoint(true);
        //for (int i = 0; i < rvm.GetBodyCount; i++)
        //{
        //    ShowJoints(true, i);
        //    UseBodydepth(true, i);
        //}

        //for (int i = rvm.GetBodyCount; i < bodyLayer.Count; i++)
        //    bodyLayer[i].SetActive(false);

    }

    void ShowJoints(bool on, int bodyIndex)
    {
        if (on)
        {
            List<Vector3> copyList = rvm.JointsList(bodyIndex);
            if (copyList != null)
            {
                for (int i = 0; i < 17; i++)
                {
                    Vector3 pos = copyList[i];
                    jointLists[bodyIndex][i].transform.localPosition = CalculatePos(pos, bodyIndex);
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

    private Vector3 CalculatePos(Vector3 pos, int bodyIndex)
    {
        Vector3 bPos = bodyLayer[bodyIndex].transform.localPosition;
        Vector3 bScale = bodyLayer[bodyIndex].transform.localScale;

        float w = bScale.x;
        float h = bScale.z;
        float wh = bScale.x * 0.5f;
        float hh = bScale.z * 0.5f;

        float x = (pos.x * w - wh) * 10.0f;
        float y = (pos.y * h - hh) * 10.0f;

        Vector3 result = new Vector3(bPos.x + x, bPos.y + y, bPos.z);

        return result;
    }

    void UseBodydepth(bool on, int bodyIndex)
    {

        float depth = rvm.BodyDepth(bodyIndex);

        for (int i = 0; i < bodyLayer.Count; i++)
        {
            Vector3 pos = bodyLayer[i].transform.localPosition;
            if (on)
            {
                pos.z = depth;
            }
            else
            {
                pos.z = 0;
            }

            bodyLayer[i].transform.localPosition = pos;
        }
    }
}
