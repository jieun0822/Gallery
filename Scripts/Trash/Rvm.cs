using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Rvm : MonoBehaviour
{
    public XRInferRVM rvm;
    public GameObject resultColor;
    public GameObject resultIndex;
    public GameObject bodyLayer;
    public GameObject bodyLayer2;
    public int bodyIndex = 0;

    private bool showJoint = false;
    private bool useDepth = false;
    private List<GameObject> jointLists = new List<GameObject>();
    private Material sphereMat;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rvm.Init();
        rvm.WorkRVM = true;
        resultColor.GetComponent<Renderer>().material.mainTexture = rvm.BodyColorRT;
        resultIndex.GetComponent<Renderer>().material.mainTexture = rvm.BodyIndexRT;
        bodyLayer.GetComponent<Renderer>().material.SetTexture("_MainTex", rvm.BodyColorRT);
        bodyLayer.GetComponent<Renderer>().material.SetTexture("_MaskTex", rvm.BodyIndexRT);
        bodyLayer.GetComponent<Renderer>().material.SetInt("_BodyIndex", bodyIndex);

        bodyLayer2.GetComponent<Renderer>().material.SetTexture("_MainTex", rvm.BodyColorRT);
        bodyLayer2.GetComponent<Renderer>().material.SetTexture("_MaskTex", rvm.BodyIndexRT);
        bodyLayer2.GetComponent<Renderer>().material.SetInt("_BodyIndex", bodyIndex+1);

        sphereMat = new Material(Shader.Find("Unlit/Color"));
        sphereMat.color = Color.red;
        sphereMat.renderQueue = (int)RenderQueue.Transparent;

        for (int i = 0; i < 17; i++)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = "Joint_" + i;
            sphere.GetComponent<Renderer>().material = sphereMat;
            sphere.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            sphere.transform.localPosition = Vector3.zero;
            sphere.transform.parent = this.transform;
            jointLists.Add(sphere);
            sphere.SetActive(false);
        }
    }

    void Update()
    {
        //if (showJoint)
        //{
        //    ShowJoints(showJoint);
        //}

        //if (useDepth)
        //{
        //    UseBodydepth(useDepth);
        //}

        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    Application.Quit();
        //}

        //if (Input.GetKeyDown(KeyCode.J))
        //{
        //    showJoint = !showJoint;
        //    rvm.UseJoint(showJoint);
        //    ShowJoints(showJoint);
        //}

        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    useDepth = !useDepth;
        //    UseBodydepth(useDepth);
        //}

        //if (Input.GetKeyDown(KeyCode.LeftArrow))
        //{
        //    BodyIndexChange(-1);
        //}

        //if (Input.GetKeyDown(KeyCode.RightArrow))
        //{
        //    BodyIndexChange(1);
        //}

        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    bool on = rvm.WorkRVM;
        //    on = !on;
        //    Debug.Log($"XRInferRVM Working : {on}");
        //    rvm.WorkRVM = on;
        //}
    }

    void BodyIndexChange(int n)
    {
        int cnt = rvm.GetBodyCount;
        int nextIndex = bodyIndex + n;
        if (cnt == 0)
        {
            bodyIndex = 0;
            bodyLayer.GetComponent<Renderer>().material.SetInt("_BodyIndex", bodyIndex);
            return;
        }
        if (nextIndex < 0)
        {
            nextIndex = cnt - 1;
        }
        else if (nextIndex >= cnt)
        {
            nextIndex = 0;
        }

        bodyIndex = nextIndex;
        bodyLayer.GetComponent<Renderer>().material.SetInt("_BodyIndex", bodyIndex);
        UseBodydepth(useDepth);
        ShowJoints(showJoint);
    }

    void UseBodydepth(bool on)
    {

        float depth = rvm.BodyDepth(bodyIndex);
        float depth2 = rvm.BodyDepth(bodyIndex+1);
        Vector3 pos = bodyLayer.transform.localPosition;
        Vector3 pos2 = bodyLayer2.transform.localPosition;
        if (on)
        {
            pos.z = depth;
            pos2.z = depth;
        }
        else
        {
            pos.z = 0;
            pos2.z = 0;
        }

        bodyLayer.transform.localPosition = pos;
        bodyLayer2.transform.localPosition = pos2;
    }

    void ShowJoints(bool on)
    {
        if (on)
        {
            List<Vector3> copyList = rvm.JointsList(bodyIndex);
            if (copyList != null)
            {
                for (int i = 0; i < 17; i++)
                {
                    Vector3 pos = copyList[i];
                    jointLists[i].transform.localPosition = CalculatePos(pos);
                    jointLists[i].SetActive(true);
                }
            }
            else
            {
                foreach (GameObject g in jointLists) g.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject g in jointLists) g.SetActive(false);
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
}
