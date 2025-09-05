using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static XRInferRVM;

public class MakeBodyLayer4 : MonoBehaviour
{
    [Header("[Editor setting]")]
    public XRInferRVM rvm;
    public WebcamLoader webcamLoader;
    public GameObject resultColor;
    public GameObject resultIndex;

    [Header("[RVM setting]")]
    public XRInferRVM.ORIENTATIONMODE orientationMode;

    private Vector3 inferScale;
    private Vector3 resultColorPos;
    private Vector3 resultIndexPos;

    public List<GameObject> bodyLayerList;
    public GameObject bodyLayerPrefab;
    public Transform bodySetParent;
    private bool useDepth = true;

    //public Test test;

    private void Start()
    {

        StartCoroutine(coStart());

    }

    IEnumerator coStart()
    {
        rvm.OrientationMode = orientationMode;
        rvm.Init();
        rvm.WorkRVM = true;
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

        yield return new WaitForSeconds(1f);
        AllBodyShow();
    }

    private void Update()
    {
        AllBodyShow();
    }

    void AllBodyShow()
    {
        int bodyCount = rvm.GetBodyCount;
        //Debug.Log(bodyCount);
        rvm.UseJoint(true);
        while (bodyLayerList.Count < bodyCount)
        {
            GameObject newLayer = Instantiate(bodyLayerPrefab, transform);
            newLayer.transform.SetParent(bodySetParent, false);
            newLayer.SetActive(true);
            bodyLayerList.Add(newLayer);
        }

        // 모든 Body Layer 업데이트
        for (int i = 0; i < bodyLayerList.Count; i++)
        {
            GameObject layer = bodyLayerList[i];

            if (i < bodyCount)
            {
                layer.SetActive(true);

                Renderer renderer = layer.GetComponent<Renderer>();
                renderer.material.SetTexture("_MainTex", rvm.BodyColorRT);
                renderer.material.SetTexture("_MaskTex", rvm.BodyIndexRT);
                renderer.material.SetInt("_BodyIndex", i);

                //Debug.Log("rvm.BodyDepth[i] " + rvm.BodyDepth[i]);

                Vector3 pos = layer.transform.localPosition;

                int depthIndex = rvm.BodyIndex(i);

                pos.z = useDepth ? rvm.BodyDepth(depthIndex) : 0f;
                layer.transform.localPosition = pos;




            }
            else
            {
                // 사람 수보다 많은 객체는 비활성화
                layer.SetActive(false);

            }
        }
    }
}
