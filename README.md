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
