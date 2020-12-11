using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class CameraManager : MonoBehaviour
{
    public AppManager app;

    private bool camAvailable;
    private WebCamTexture webCamTexture;
    private Texture defaultBackground;

    public RawImage background;
    public AspectRatioFitter fit;
    public bool frontFacing;

    float ratio, scaleY;
    int orient;

    // Use this for initialization
    void Start()
    {
        InitCamera();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCamera();
        
    }


    void InitCamera()
    {
        defaultBackground = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
            Debug.Log("NO CAMERA");

        for (int i = 0; i < devices.Length; i++)
        {
            var curr = devices[i];

            if (curr.isFrontFacing == frontFacing)
            {
                webCamTexture = new WebCamTexture();
            }
        }

        if (webCamTexture == null)
            Debug.Log("NO CAMERA");

        Application.RequestUserAuthorization(UserAuthorization.WebCam);
        Application.targetFrameRate = 300;
        webCamTexture.requestedFPS = 600;

        webCamTexture.Play();
        background.texture = webCamTexture;

        camAvailable = true;
    }

    void UpdateCamera()
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

    IEnumerator TakePicture()
    {

        yield return new WaitForEndOfFrame();

        Texture2D PhotoTaken = new Texture2D(webCamTexture.width, webCamTexture.height);

        PhotoTaken.SetPixels(webCamTexture.GetPixels());
        PhotoTaken.Apply();

        

        // Encode texture into PNG

       
        //ystem.IO.File.WriteAllBytes(filename, bytes);

        // For testing purposes, also write to a file in the project folder
        //File.WriteAllBytes(Application.dataPath + "/../SavedScreen.jpg", bytes);

        background.texture = PhotoTaken;

        //Top k idxs with top k probabilities
        Tuple<double[], double[]> result = app.mm.FeedModel(PhotoTaken);

        app.SetPredictionTexts(result.Item1, result.Item2);

        camAvailable = false;

    }

    public void BackToCam()
    {
        background.texture = webCamTexture;
        app.predictionPanel.SetActive(false);
        camAvailable = true;
    }

}