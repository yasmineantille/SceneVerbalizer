# Scene Verbalizer

Unity3D and python implementation of the paper *ARSUS: An Augmented Reality Scene Understanding System that implements Auditory Scene Verbalization on a Microsoft HoloLens 2*


## Installation

Download SceneVerbalizer Repo with:
```git clone https://github.com/yasmineantille/sceneverbalizer.git```

For RelTR (CPU): 

1. (Optional, but recommended) Create conda environment with ```conda create -n reltr python=3.6``` and activate it
2. Install PyTorch and PyVision with ```pip install torch==1.6.0 torchvision==0.7.0 --extra-index-url https://download.pytorch.org/whl/cpu
pip3 install -r requirements.txt```
3. Download the RelTR submodel and place it into ckpt


## Usage

Run the RelTR server locally and make sure to add the correct IP address of the devices used to the server file and the unity application. Build and deploy the unity application on a HoloLens 2. Now you are ready to use the app on your environment. 
