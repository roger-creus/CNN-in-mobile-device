using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;
using UnityEngine.UI;
using System;



public class ModelManager : MonoBehaviour
{

    public NNModel modelSource;

    private IWorker worker;
    private Model model;

    public Texture2D texture;

    public GameObject predictionPanel;

    public Text prediction_text_1;
    public Text prediction_text_2;
    public Text prediction_text_3;
    public Text prediction_text_4;
    public Text prediction_text_5;

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



    //transforms.Normalize(mean=[0.485, 0.456, 0.406], std=[0.229, 0.224, 0.225]),
    Tensor ImageToTensor(Texture2D t)
    {
        //var tensor = new Tensor(batchCount, height, width, channelCount);

        var channelCount = 3; // you can treat input pixels as 1 (grayscale), 3 (color) or 4 (color with alpha) channels


        Texture2D resized_img = Resize(t, 256, 256);



        var img_tensor = new Tensor(resized_img, channelCount);

        var std_tensor = new Tensor(1, 3, 256, 256);

       
        std_tensor = StandardizeTensorChannel(img_tensor, new double[] {0.485, 0.456, 0.406}, new double[] { 0.229, 0.224, 0.225});

        //img_tensor.Dispose();

        return std_tensor;
    }

    public Tuple<double[], double[]> FeedModel(Texture2D t)
    {

        var img_t_ = ImageToTensor(t);

        worker.Execute(img_t_);

        var O =  worker.PeekOutput();

        //img_t_.Dispose();

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

        double[] best_probs = FindMaxInTensor(sm_probs, 5);
        double[] best_idxs = new double[5];
        for(int i = 0; i < 5; i++)
        {
            best_idxs[i] = pred_to_id[best_probs[i]];
        }

        return Tuple.Create(best_idxs, best_probs);
    }

    public void SetPredictionTexts(double[] best_idxs, double[] best_probs)
    {
        predictionPanel.SetActive(true);
        prediction_text_1.text = "1) Class " + best_idxs[0].ToString() + " with prob: " + best_probs[0].ToString();
        prediction_text_2.text = "2) Class " + best_idxs[1].ToString() + " with prob: " + best_probs[1].ToString();
        prediction_text_3.text = "3) Class " + best_idxs[2].ToString() + " with prob: " + best_probs[2].ToString();
        prediction_text_4.text = "4) Class " + best_idxs[3].ToString() + " with prob: " + best_probs[3].ToString();
        prediction_text_5.text = "5) Class " + best_idxs[4].ToString() + " with prob: " + best_probs[4].ToString();
    }

    private static double[] Softmax(double[] oSums)

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

    Tensor StandardizeTensorChannel(Tensor img, double[] mean, double[] std)
    {
        Tensor img_copy = new Tensor(1, 256, 256, 3);
        for(int c = 0; c < 3; c++)
        {
            int channel = c;
            for (int i = 0; i < img.width; i++)
            {
                for(int j = 0; j < img.height; j++)
                {
                    img_copy[0, i, j, channel] = (img[0, i, j, channel] - (float)mean[c]) / (float)std[c];

                }
            }
        }
        

        return img_copy;
    }


    double[] FindMaxInTensor(double[] output, int k)
    {
        /*
        double max = -1;
        int idx = 0;
        for(int i = 0; i < output.Length; i++)
        {
            //Debug.Log(output[i]);
            if(output[i] > max)
            {
                max = output[i];
                idx = i;
            }

        }

        */

        Array.Sort(output);
        Array.Reverse(output);
        double[] best = new double[k];

        for(int i = 0; i < k; i++)
        {
            best[i] = output[i];
        }

        return best;
    }

    void FreeMemory(Tensor outputs)
    {
        outputs.Dispose();
        worker.Dispose();
    }
}
