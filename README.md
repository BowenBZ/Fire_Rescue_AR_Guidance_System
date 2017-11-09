# Frech
Fire Rescue Evacuation Command Helmet - Department of Automation, Tsinghua University

## Required Software
* Unity 2017.1
* Visual Studio 2015 (with Windows 10 SDK 10.0.10586)

## How to Run
Download or Clone this repository, use Unity open the project.  
In Unity editor,  
* Go to `File/Build Settings`  
* Select `Universal Windows Platform`, click `Switch Platform`  
* Set `Target device` to `HoloLens`, `Build Type` to `D3D`, `SDK` to `10.0.10586.0`
* Check `Unity C# Project`
* Click `Build`, create a new folder named `App` and select that folder  

Go to `...\App\Frech.sln`, use Visual Studio open the project  
In Visual Studio editor,
* Use USB connect HoloLens to your computer
* Change `Debug` to `Release`, `ARM` to `x86`, then choose `Device`
* Click `Debug\Run without Debug`

#### How to Run in Unity 2017.2
When you use Unity 2017.2 open this project, choose something like `Update Project`, Unity will automatically update those files to new Unity version. But when the project opens, you may still encounter some mistakes of scripts shown in the console. You just need to change all the `VR` regarded as mistakes to `XR`. For example,  
* Change `Using UnityEngine.VR.WSA;` to `Using UnityEngine.XR.WSA;`  
* Change `VRDevice` to `XRDevice`  

## Program Structure  
* **Main Camera**  
HoloTooklit Prefab. Indicates user's head. The view of Camera is the view of user.
* **Directional Light**  
HoloTookit Prefab. Supplies the light for the environment, enhancing the view of user.
* **Cursor**  
Indicates the focus of uesr. The cursor will show in the surface of object that user is gazing. It will provide feedbacks for uesr.   
  * CursorManager.cs  
  Manages the position and state of cursor object. If will change the shape when user gazes object or gazes environment. It will also show different extra tags when uesr is in different states.
* **Manager**  
Empty object. Hold many important scripts.
  * SpeechManager.cs  
  Manages the voice command.
  * GazeManager.cs  
  Manages the virtual raycast. This script can return what object the user is gazing, the CursorManager.cs just draws the cursor.
  * GestureManager.cs  
  Manages the gesture command.
  * PointsManager.cs  
  Save current position as a key point. Create an sphere in current position, add World Anchor to it and save it to disk.
  * Route.cs  
  Detect which two key points the user is in.  
* **DebugInformation**  
3DText object. It shows the information for user and developer.
  * DebugInfoManager.cs  
  Get the value for debugging from other scripts and shows them.
* **FITAll**  
FIT Building model. Presents the whole view of FIT.
  * ObjectPlacement.cs  
  Handles the event from GestureManager.cs
  * SingleFloorManager.cs  
  Handles the event from SpeechManager.cs
* **GuidedArrow**  
Arrow Object. Used for navigation for user.
  * GuidedArrow.cs  
  Manages the position and rotation of GuidedArrow object.
* **SpatialMapping**  
HoloToolkit prefab. Used for detect the boundary of environment.
  * SpatialMappingObserver.cs  
  Elements of the mesh drawn on the surface of environment.
  * SpatialMappingManager.cs  
  Manages the detection process.
