using UnityEngine;
using TMPro;

public class Test : MonoBehaviour
{
    public TMP_Text myText;
    public GameObject obj;

    public TMP_Text myText2;
    public GameObject obj2;

    private void Update()
    {
       


        if(obj!= null)
        myText.text = "obj1 : " + obj.transform.position.z.ToString();
        
        if(obj2 != null)
        myText2.text = "obj2 : " + obj2.transform.position.z.ToString();
    }
}
