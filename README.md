# Integration of Convolutional Neural Networks in MobileApplications: Platforms and Challenges

## Project Structure



├── README.md       <- The top-level README for developers using this project.
├── data
│       ├── interim        <- Manually Collected data for updating models
│       └── raw
│   	        ├── train
│		└── test
│	
├── unity project  	   <- Application Project source code
│	    ├── project
│		  ├── assets
│		  ├── library
│		  ├── logs
│		  ├── packages
│		  ├── project settings
│		  └── temp
│	   ├── builds		<- Application Builds
│
├── models             <- Trained and serialized models
│	    ├── trained 
│	    └── updated 
│
├── notebooks           <- Python notebooks executed in KaggleGPU
│
├── reports                  <- Generated article of the report
│        └── figures     

├── requirements.txt     <- The requirements file for reproducing the analysis environment

<p><small>Project based on a simplified version of the <a target="_blank" href="https://drivendata.github.io/cookiecutter-data-science/">cookiecutter data science project template</a>. #cookiecutterdatascience</small></p>

---


### Requirements

The python and ipython packages inside the `requirements.txt` file need to be installed using the following command.

```bash
pip install -r requirements.txt
```



###notebooks

The source code for data loading, model trainng and exporting for each of the experimented models is found in the **notebookes** folder. Additionally, it is also provided the notebook for loading a model and updating it with the collected data in the system operation phase of the applications.

The notebooks have been executed in the Kaggle GPU and saved from the same platform. (e.g The path for loading the data corresponds to the Kaggle directories)

### data

The `./data` directory holds different type of data:

- **raw**: The original, immutable data dump and with the original train/test splits for the GTSRB dataset. Images stored in the (.ppm) file format. 
- **interm**: The update data contains the images collected in the system operations phase of the application that integrates the DL-based component. Images stored in the (.jpg) file format

No **pre-processed** data is provided because the pre-processing steps are computed at run-tine during model training and the results are not stored.

### models

The Small CNN and the ResNet34 are the deployed models to the built applications. These are saved in both the ".pt" file format from Pytorch and the ".onnx" file format. The first one allows loading the model in the same PyTorch framework and perform updates on it. The later allows exporting the model to the Unity Project.


### unity project

The folder contains in the ./project subfolder the Unity 3d project that builds the two applications that integrate a DL-based component. It also contains all the built applications in the ".apk" format in the ./builds subfolder. 