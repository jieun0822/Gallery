using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class MakeBodyLayer3 : MonoBehaviour
{
    public XRInferRVM rvm;
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
        rvm.Init();
        rvm.WorkRVM = true;

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
        Debug.Log(bodyCount);

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
                pos.z = useDepth ? rvm.BodyDepth(i) : 0f;
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
