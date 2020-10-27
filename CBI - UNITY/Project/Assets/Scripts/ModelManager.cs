using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;
using UnityEngine.UI;
using System;



public class ModelManager : MonoBehaviour
{
	public AppManager app;

    public NNModel modelSource;

    private IWorker worker;
    private Model model;

    public Texture2D trial_texture;

    // Start is called before the first frame update
    void Start()
    {
        /*
        foreach (var layer in model.layers)
            Debug.Log(layer.name + " does " + layer.type);

        */

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void StartModel()
    {

        model = ModelLoader.Load(modelSource,verbose: false);
        worker = WorkerFactory.CreateWorker(model, verbose: false);

    }


    Texture2D Resize(Texture2D texture2D, int targetX, int targetY)
    {

        RenderTexture rt = new RenderTexture(targetX, targetY, 24);
        RenderTexture.active = rt;
        Graphics.Blit(texture2D, rt);
        Texture2D result = new Texture2D(targetX, targetY);
        result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
        result.Apply();

        return result;
    }

    Tensor CropPytorch(Tensor source, int targetWidth, int targetHeight)
    {
        int sourceWidth = source.width;
        int sourceHeight = source.height;


        int crop_top = (int)((sourceHeight - targetHeight + 1) * 0.5);

        int crop_left = (int)((sourceWidth - targetWidth + 1) * 0.5);


        var cropped_img = new Tensor(1, targetHeight, targetWidth, 3);
        for (int channel = 0; channel < 3; channel++) {
            for (int i = crop_top; i < sourceHeight- crop_top; i++)
            {
                for (int j = crop_left; j < sourceWidth - crop_top; j++)
                {
                    cropped_img[0,i-crop_top, j-crop_left, channel] = source[0,i, j, channel];
                }
            }
        }
        Debug.Log("SHAPE: " + cropped_img.shape.ToString());
        return cropped_img;
    }


    Texture2D ResampleAndCrop(Texture2D source, int targetWidth, int targetHeight)
    {
        int sourceWidth = source.width;
        int sourceHeight = source.height;
        float sourceAspect = (float)sourceWidth / sourceHeight;
        float targetAspect = (float)targetWidth / targetHeight;
        int xOffset = 0;
        int yOffset = 0;
        float factor = 1;

        if (sourceAspect > targetAspect)
        { 
            factor = (float)targetHeight / sourceHeight;
            xOffset = (int)((sourceWidth - sourceHeight * targetAspect));
        }
        else
        { 
            factor = (float)targetWidth / sourceWidth;
            yOffset = (int)((sourceHeight - sourceWidth / targetAspect));
        }
        Color32[] data = source.GetPixels32();
        Color32[] data2 = new Color32[targetWidth * targetHeight];
        for (int y = 0; y < targetHeight; y++)
        {
            for (int x = 0; x < targetWidth; x++)
            {
                var p = new Vector2(Mathf.Clamp(xOffset + x / factor, 0, sourceWidth - 1), Mathf.Clamp(yOffset + y / factor, 0, sourceHeight - 1));
                // bilinear filtering
                var c11 = data[Mathf.FloorToInt(p.x) + sourceWidth * (Mathf.FloorToInt(p.y))];
                var c12 = data[Mathf.FloorToInt(p.x) + sourceWidth * (Mathf.CeilToInt(p.y))];
                var c21 = data[Mathf.CeilToInt(p.x) + sourceWidth * (Mathf.FloorToInt(p.y))];
                var c22 = data[Mathf.CeilToInt(p.x) + sourceWidth * (Mathf.CeilToInt(p.y))];
                var f = new Vector2(Mathf.Repeat(p.x, 1f), Mathf.Repeat(p.y, 1f));
                data2[x + y * targetWidth] = Color.Lerp(Color.Lerp(c11, c12, p.y), Color.Lerp(c21, c22, p.y), p.x);
            }
        }

        var tex = new Texture2D(targetWidth, targetHeight);
        tex.SetPixels32(data2);
        tex.Apply(true);
        return tex;
    }

    Tensor StandardizeTensor(Tensor img, double[] mean, double[] std)
    {
        Tensor img_copy = new Tensor(1, 256, 256, 3);
        for (int c = 0; c < 3; c++)
        {
            int channel = c;
            for (int i = 0; i < img.width; i++)
            {
                for (int j = 0; j < img.height; j++)
                {
                    img_copy[0, i, j, channel] = (img[0, i, j, channel] - (float)mean[c]) / (float)std[c];

                }
            }
        }


        return img_copy;
    }


    //transforms.Normalize(mean=[0.485, 0.456, 0.406], std=[0.229, 0.224, 0.225]) for IMAGENET
    Tensor ImageToTensor(Texture2D t)
    {
        var channelCount = 3; 

        //Custom Resize function
        Texture2D resized_img = Resize(trial_texture, 256, 256);
        //Custom CenterCrop function
        //Texture2D center_crop_img = ResampleAndCrop(resized_img, 224, 224);

        var img_tensor = new Tensor(resized_img, channelCount);

        var n = CropPytorch(img_tensor, 224, 224);

        //Custom Standarization
        var std_tensor = StandardizeTensor(n, new double[] { 0.485, 0.456, 0.406 }, new double[] { 0.229, 0.224, 0.225 });

        img_tensor.Dispose();

        return std_tensor;
    }

    double[] FindBestK(double[] output, int k)
    {
        Array.Sort(output);
        Array.Reverse(output);
        double[] best = new double[k];

        for (int i = 0; i < k; i++)
        {
            best[i] = output[i];
        }

        return best;
    }

    double[] Softmax(double[] oSums)

    {

        double max = oSums[0];

        for (int i = 0; i < oSums.Length; ++i)

            if (oSums[i] > max) max = oSums[i];

        // determine scaling factor -- sum of exp(each val - max)

        double scale = 0.0;

        for (int i = 0; i < oSums.Length; ++i)

            scale += Math.Exp(oSums[i] - max);

        double[] result = new double[oSums.Length];

        for (int i = 0; i < oSums.Length; ++i)

            result[i] = Math.Exp(oSums[i] - max) / scale;

        return result;

    }


    void FreeMemory(Tensor outputs)
    {
        outputs.Dispose();
        worker.Dispose();
    }

    public Tuple<double[], double[]> FeedModel(Texture2D t)
    {

        var img_t_ = ImageToTensor(t);

        worker.Execute(img_t_);

        var O =  worker.PeekOutput();

        img_t_.Dispose();

        //Store class probabilities 
        double[] outputs_probs = new double[O.length];
        
        for (int i = 0; i < O.length; i++)
        {
            outputs_probs[i] = O[0, 0, 0, i];
        }

        double[] sm_probs = Softmax(outputs_probs);

        Dictionary<double, int> pred_to_id = new Dictionary<double, int>();

        for(int i = 0; i < outputs_probs.Length; i++)
        {

            pred_to_id[sm_probs[i]] = i;
        }

        double[] best_probs = FindBestK(sm_probs, 5);
        double[] best_idxs = new double[5];
        for(int i = 0; i < 5; i++)
        {
            best_idxs[i] = pred_to_id[best_probs[i]];
        }


        return Tuple.Create(best_idxs, best_probs);
    }


}
