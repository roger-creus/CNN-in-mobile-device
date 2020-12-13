##Integration of Convolutional Neural Networks in MobileApplications: Platforms and Challenges



The project is structured following the Cookie Cutter Data Science framework and shown in detail as follows.

```
├── README.md          <- The top-level README for developers using this project.
├── data
│   ├── interim        <- Manually Collected data for updating models
│   └── raw
│   	├── train
│		└── test
|	
├── unity project  	   <- Application Project source code
|	├── project
│		├── assets
│		├── library
│		├── logs
│		├── packages
│		├── project settings
│		└── temp
|	├── builds		   <- Application Builds
|
├── models             <- Trained and serialized models
│	├── trained 
│	└── updated 
│
├── notebooks          <- Python notebooks executed in KaggleGPU
│
├── reports            <- Generated article of the report
│   └── figures        
│
├── requirements.txt   <- The requirements file for reproducing the analysis environment
```



#####notebooks

The source  code is found in the **notebookes** folder. The folder contains the notebooks that train and save each of the experimented models. Furthermore, the pre-processing steps are also performed before the training phase in the same notebooks. 

The notebooks have been executed in the Kaggle GPU and saved from the same platform. (e.g The path for loading the data corresponds to the Kaggle directories)



#####data

The train and test datasets are provided in the raw and PPM image format. 

The update data contains the images collected in the system operations phase of the application that integrates the DL-base component.



#####**models**

The Small CNN and the ResNet34 are the deployed models to the applications built. These are saved in both the ".pt" file format from Pytorch and the ".onnx" file format. The first one allows loading the model in the same PyTorch framework and perform updates on it. The later allows exporting the model to the Unity Project.



#####unity project

The folder contains in the *./project* subfolder the Unity 3d project that builds the two applications that integrate a DL-based component. It also contains all the built applications in the ".apk" format in the *./builds* subfolder. 

