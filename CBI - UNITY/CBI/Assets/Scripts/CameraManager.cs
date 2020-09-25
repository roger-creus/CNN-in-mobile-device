using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{

    private bool camAvailable;
    private WebCamTexture webCamTexture;
    private Texture defaultBackground;

    public RawImage background;
    public AspectRatioFitter fit;
    public bool frontFacing;

    public Text camText;
    // Use this for initialization
    void Start()
    {
        defaultBackground = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
            camText.gameObject.SetActive(true);

        for (int i = 0; i < devices.Length; i++)
        {
            var curr = devices[i];

            if (curr.isFrontFacing == frontFacing)
            {
                webCamTexture = new WebCamTexture(curr.name, Screen.width, Screen.height);
            }
        }

        if (webCamTexture == null)
            camText.gameObject.SetActive(true);


        webCamTexture.Play(); // Start the camera
        background.texture = webCamTexture; // Set the texture

        camAvailable = true; // Set the camAvailable for future purposes.
    }

    // Update is called once per frame
    void Update()
    {
       

        float ratio = (float)webCamTexture.width / (float)webCamTexture.height;
        fit.aspectRatio = ratio; // Set the aspect ratio

        float scaleY = webCamTexture.videoVerticallyMirrored ? -1f : 1f; // Find if the camera is mirrored or not
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f); // Swap the mirrored camera

        int orient = -webCamTexture.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
    }

    public void TakePicture()
    {

        Debug.Log("HI");
        Texture2D PhotoTaken = new Texture2D(webCamTexture.width, webCamTexture.height);
        PhotoTaken.SetPixels(webCamTexture.GetPixels());
        PhotoTaken.Apply();

        background.texture = PhotoTaken;
        

        camAvailable = false;
    }

    public void BackToCam()
    {
        background.texture = webCamTexture;
        camAvailable = true;
    }

}