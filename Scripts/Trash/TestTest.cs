using UnityEngine;
using TMPro;
public class TestTest : MonoBehaviour
{
    public TMP_Text txt;
    public TMP_Text txt2;
    public GameObject obj1;
    public GameObject obj2;
    public GameObject parent;

    private void Update()
    {
        if (obj1 == null)
        {
            obj1 = parent.transform.GetChild(0).gameObject;
        }
        if (obj2 == null)
        {
            obj2 = parent.transform.GetChild(1).gameObject;
        }

        txt.text = obj1.transform.position.z.ToString();
        txt2.text = obj2.transform.position.z.ToString();
    }
}
