using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    bool camAvailable;
    WebCamTexture webCamTexture;
    Texture defaultBackground;

    public RawImage background;
    public AspectRatioFitter fit;


    void Start()
    {
        defaultBackground = background.texture;
        webCamTexture = new WebCamTexture();
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.Log("NO CAM");
            camAvailable = false;
            return;
        }
        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
                webCamTexture = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
            }
        }
        if (webCamTexture == null)
        {
            return;
        }

        webCamTexture.Play();
        background.texture = webCamTexture;

        camAvailable = true;
    }

    private void Update()
    {
        if (!camAvailable)
            return;

        float ratio = (float)webCamTexture.width / (float)webCamTexture.height;
        fit.aspectRatio = ratio;

        float scaleY = webCamTexture.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        int orient = -webCamTexture.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

    }
}