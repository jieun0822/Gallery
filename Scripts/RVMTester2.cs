using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

public class RVMTester2 : MonoBehaviour
{
    [Header("[Editor setting]")]    
    public XRInferRVM rvm;
    public WebcamLoader webcamLoader;
    public GameObject resultColor;
    public GameObject resultIndex;
    public GameObject bodyLayer;

    [Header("[RVM setting]")]
    public XRInferRVM.ORIENTATIONMODE orientationMode;
    public float minDepth = 1.4f;

    [Header("[RVM OutputData]")]
    public int bodyCnt = 0;

    private bool showJoint = false;
    private bool useDepth = true;
    private List<GameObject> jointLists = new List<GameObject>();
    
    private Material sphereMat;
    //7 3.645835 3.9375
    public Vector3 inferScale;
    private Vector3 resultColorPos;
    private Vector3 resultIndexPos;
    private List<GameObject> bodyLayers = new List<GameObject>();
    private float shift = -12.5f;

    public Dictionary<int, List<GameObject>> jointGroup = new Dictionary<int, List<GameObject>>();

    public Transform bodyPool;
    public Transform jointPool;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rvm.OrientationMode = orientationMode;
        rvm.Init();
        rvm.WorkRVM = true;
        rvm.MinDepth = minDepth;
        switch(orientationMode)
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

        inferScale = new Vector3(16.84f, 8.770839f, 9.472501f);
        resultColor.GetComponent<Renderer>().material.mainTexture = rvm.BodyColorRT;
        resultColor.transform.localScale = inferScale;
        resultColor.transform.localPosition = resultColorPos;

        resultIndex.GetComponent<Renderer>().material.mainTexture = rvm.BodyIndexRT;
        resultIndex.transform.localScale = inferScale;
        resultIndex.transform.localPosition = resultIndexPos;

        bodyLayer.GetComponent<Renderer>().material.SetTexture("_MainTex", rvm.BodyColorRT);
        bodyLayer.GetComponent<Renderer>().material.SetTexture("_MaskTex", rvm.BodyIndexRT);
        bodyLayer.GetComponent<Renderer>().material.SetInt("_BodyIndex", 0);
        bodyLayer.transform.localScale = inferScale;

        sphereMat = new Material(Shader.Find("Unlit/Color"));
        sphereMat.color = Color.red;
        sphereMat.renderQueue = (int)RenderQueue.Transparent;

        for (int i = 0; i < 30; i++)
        {
            GameObject go = Instantiate(bodyLayer);
            go.name = "BodyLayer_" + i;
            go.transform.parent = this.transform;
            go.GetComponent<Renderer>().material.SetTexture("_MainTex", rvm.BodyColorRT);
            go.GetComponent<Renderer>().material.SetTexture("_MaskTex", rvm.BodyIndexRT);
            go.GetComponent<Renderer>().material.SetInt("_BodyIndex", i);
            go.transform.localScale = inferScale;
            Vector3 pos = bodyLayer.transform.localPosition;
            //pos.y += i * shift;
            go.transform.localPosition = pos;
            bodyLayers.Add(go);

            // Pool
            go.transform.SetParent(bodyPool, false);

            string parentName = "JointParent_" + i;
            GameObject jointParent = new GameObject(parentName);
            jointParent.transform.SetParent(jointPool, false);

            List<GameObject> list = new List<GameObject>();
            for (int j = 0; j < 17; j++)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.name = "Joint_" + i + "_" + j;
                sphere.GetComponent<Renderer>().material = sphereMat;
                sphere.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                sphere.transform.localPosition = Vector3.zero;
                sphere.transform.parent = this.transform;
                sphere.SetActive(false);
                jointLists.Add(sphere);
                list.Add(sphere);

                // Pool
                sphere.transform.SetParent(jointParent.transform, true);
            }
            jointGroup.Add(i, list);
        }       
        bodyLayer.SetActive(false);
    }
    

    void Update()
    {
        bodyCnt = rvm.GetBodyCount;
        
        UpdateTexture();

        //if (showJoint)
        //{
        //    ShowJoints(showJoint);
        //}

        if (useDepth)
        {
            UseBodydepth(useDepth);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        //if(Input.GetKeyDown(KeyCode.J))
        //{
            //showJoint = !showJoint;
            rvm.UseJoint(true);
            ShowJoints(true);
        //}

        if (Input.GetKeyDown(KeyCode.D))
        {
            useDepth = !useDepth;
            UseBodydepth(useDepth);
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            bool on = rvm.WorkRVM;
            on = !on;
            Debug.Log($"XRInferRVM Working : {on}");
            rvm.WorkRVM = on;
        }        
    }

    void UpdateTexture()
    {
        for(int i = 0; i < bodyCnt; i++)
        {
            int index = rvm.BodyIndex(i);
            bodyLayers[i].GetComponent<Renderer>().material.SetInt("_BodyIndex", index);
        }
        for (int i = bodyCnt; i < 12; i++)
        {            
            bodyLayers[i].GetComponent<Renderer>().material.SetInt("_BodyIndex", -1);
        }
    }

    void UseBodydepth(bool on)
    {
        for(int i = 0; i < bodyLayers.Count; i++)
        {
            Vector3 pos = bodyLayers[i].transform.localPosition;
            if (on)
            {
                float depth = rvm.BodyDepth(i);
                pos.z = depth;
            }
            else
            {
                pos.z = 0;
            }
            bodyLayers[i].transform.localPosition = pos;
        }
    }

    void ShowJoints(bool on)
    {
        if(on)
        {   
            for(int i = 0; i < bodyCnt; i++)
            {
                List<Vector3> copyList = rvm.JointsList(i);
                if (copyList != null)
                {
                    for (int j = 0; j < 17; j++)
                    {
                        int index = i * 17 + j;
                        Vector3 pos = copyList[j];                        
                        Vector3 cPos = CalculatePos(pos);
                        //cPos.y += i * shift;
                        if (useDepth) cPos.z = rvm.BodyDepth(i);
                        jointLists[index].transform.localPosition = cPos;
                        jointLists[index].SetActive(true);
                        var mesh = jointLists[index].GetComponent<MeshRenderer>();
                        mesh.enabled = false;
                    }
                }                
            }
            for(int i = bodyCnt * 17; i < jointLists.Count; i++)
            {
                jointLists[i].SetActive(false);
            }
            
        }
        else
        {
            foreach (GameObject g in jointLists) g.SetActive(false);
        }        
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
        if(yflip) y = ((1.0f - pos.y) * h - hh) * 10.0f;

        Vector3 result = new Vector3(bPos.x + x, bPos.y + y, bPos.z);

        return result;
    }
}
