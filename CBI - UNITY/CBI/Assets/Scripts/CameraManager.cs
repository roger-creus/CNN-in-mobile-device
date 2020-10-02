using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;

public class CameraManager : MonoBehaviour
{


    private bool camAvailable;
    private WebCamTexture webCamTexture;
    private Texture defaultBackground;

    public ModelManager mm;

    public RawImage background;
    public AspectRatioFitter fit;
    public bool frontFacing;

    public Text camText;

    float ratio, scaleY;
    int orient;
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
                webCamTexture = new WebCamTexture();
            }
        }

        if (webCamTexture == null)
            camText.gameObject.SetActive(true);

        Application.RequestUserAuthorization(UserAuthorization.WebCam);
        Application.targetFrameRate = 300;
        webCamTexture.requestedFPS = 600;

        webCamTexture.Play(); // Start the camera
        background.texture = webCamTexture; // Set the texture

        camAvailable = true; // Set the camAvailable for future purposes.
    }

    // Update is called once per frame
    void Update()
    {
       

        ratio = (float)webCamTexture.width / (float)webCamTexture.height;
        fit.aspectRatio = ratio; // Set the aspect ratio

        scaleY = webCamTexture.videoVerticallyMirrored ? -1f : 1f; // Find if the camera is mirrored or not
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f); // Swap the mirrored camera

        orient = -webCamTexture.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
    }

    public void Pict()
    {
        StartCoroutine("TakePicture");
    }

    public IEnumerator TakePicture()
    {

        yield return new WaitForEndOfFrame();

        Texture2D PhotoTaken = new Texture2D(webCamTexture.width, webCamTexture.height);
        PhotoTaken.SetPixels(webCamTexture.GetPixels());
        PhotoTaken.Apply();

        background.texture = PhotoTaken;


        Tuple<double[], double[]> result = mm.FeedModel(PhotoTaken);
        mm.SetPredictionTexts(result.Item1, result.Item2);

        camAvailable = false;

    }

    public void BackToCam()
    {
        background.texture = webCamTexture;
        mm.predictionPanel.SetActive(false);
        camAvailable = true;
    }

}