# Scene Verbalizer

Unity3D and python implementation of the paper *ARSUS: An Augmented Reality Scene Understanding System that implements Auditory Scene Verbalization on a Microsoft HoloLens 2*


## Installation

Download SceneVerbalizer Repo with:
```git clone https://github.com/yasmineantille/sceneverbalizer.git```

### RelTR (CPU): 

0. `cd` into `RelTR`
1. (Optional, but recommended) Create conda environment with ```conda create -n reltr python=3.6``` and activate it
2. Install PyTorch and PyVision with ```pip install torch==1.6.0 torchvision==0.7.0 --extra-index-url https://download.pytorch.org/whl/cpu```
3. pip3 install -r requirements.txt
4. Download the trained [RelTR weights](https://drive.google.com/open?id=1id6oD_iwiNDD6HyCn2ORgRTIKkPD3tUD) and place the file into `ckpt` (create directory if it does not exist)


## Usage

1. `cd` into `RelTR`
2. Run the server using `python3 server.py`

_Note_: Make sure to add the correct IP address of the devices used to the server file and the unity application.

3. Build and deploy the unity application on a HoloLens 2. Now you are ready to use the app on your environment. 
