using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;


public class ModelManager : MonoBehaviour
{

    public NNModel modelSource;

    private IWorker worker;
    private Model model;

    public Texture2D texture;

    // Start is called before the first frame update
    void Start()
    {
        model = ModelLoader.Load(modelSource, verbose:false);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model, verbose:false);
        FeedModel(null, texture);


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Tensor ImageToTensor(Texture2D t)
    {
        //var tensor = new Tensor(batchCount, height, width, channelCount);

        var channelCount = 3; // you can treat input pixels as 1 (grayscale), 3 (color) or 4 (color with alpha) channels
        var tensor = new Tensor(t, channelCount);

        return tensor;
    }

    void FeedModel(Dictionary<string, Tensor> inputs, Texture2D t)
    {

        worker.Execute(ImageToTensor(t));
        var O = worker.PeekOutput();
        FreeMemory(O);
    }

    void FreeMemory(Tensor outputs)
    {
        outputs.Dispose();
        worker.Dispose();
    }
}
