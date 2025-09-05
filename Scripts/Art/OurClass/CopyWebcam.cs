using UnityEngine;
using UnityEngine.UI;

public class CopyWebcam : MonoBehaviour
{
    public WebcamLoader webcamLoader;
    public RawImage secondaryImg;
    public RawImage webcamImg;

    private void Start()
    {
        ActiveCanvas(false);
    }

    public void GetTexture()
    {
        if (webcamLoader.cameraTextureRot != null)
        {
            webcamImg.texture = webcamLoader.cameraTextureRot;
        }
        else
        {
            webcamImg.texture = secondaryImg.texture;
        }
    }

    public void ActiveCanvas(bool isActive)
    {
        webcamImg.gameObject.SetActive(isActive);
    }
}
