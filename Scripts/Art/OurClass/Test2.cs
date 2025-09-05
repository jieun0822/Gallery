using System.Data;
using TMPro;
using UnityEngine;

public class Test2 : MonoBehaviour
{
    public XRInferRVM rvm;
    public TMP_Text m_TextMeshPro;

    private void Update()
    {
        int index = rvm.BodyIndex(0);
        m_TextMeshPro.text = rvm.BodyDepth(index).ToString();
    }
}
