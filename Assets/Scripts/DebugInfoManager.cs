using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.VR.WSA;
using Academy.HoloToolkit.Unity;
using Academy.HoloToolkit.Sharing;

public class DebugInfoManager : Singleton<DebugInfoManager>
{
    string gestureMode = "";
    string savePointsStatus = "";
    string navigationStatus = "";
    string loadStatus = "UnLoad\n";
   
    void Start()
    {
    }

    // Update is called once per frame
    void Update ()
    {
        Update3DTextPosition();
        Update3DTextContent();
    }

    // Set 3DText Position
    private void Update3DTextPosition()
    {
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;
        var rightDirection = Camera.main.transform.right;
        var upDirection = Camera.main.transform.up;
        this.transform.position = headPosition + gazeDirection * 10 - rightDirection * 2f + upDirection;
        var headRotation = Camera.main.transform.rotation;
        this.transform.rotation = headRotation;
    }

    // Update 3DText Content
    private void Update3DTextContent()
    {
        // Gestrue command type 
  
        if (GestureManager.Instance.ActiveRecognizer == GestureManager.Instance.ClickRecognizer)
            gestureMode = "Tap to Select\n";
        else if (GestureManager.Instance.ActiveRecognizer == GestureManager.Instance.MoveRecognizer)
            gestureMode = "Hold to Move\n";
        else if (GestureManager.Instance.ActiveRecognizer == GestureManager.Instance.RotateRecognizer)
            gestureMode = "Hold to Rotate\n";
        else if (GestureManager.Instance.ActiveRecognizer == GestureManager.Instance.ZoomRecognizer)
            gestureMode = "Holo to Resize\n";
        else
            gestureMode = "";

        // Update content of 3DText
        this.GetComponent<TextMesh>().text = gestureMode + savePointsStatus + loadStatus + navigationStatus;
    }

    public void UpdateSavePointsStatus(bool saving)
    {
        if (saving)
            savePointsStatus = "Saving Points\n";
        else
            savePointsStatus = "";
    }

    public void UpdateNavigationStatus(bool arrived, bool ifshow)
    {
        if(!ifshow)
        {
            navigationStatus = "";
            return;
        }
        if (arrived)
            navigationStatus = "Arrive Destination\n";
        else
            navigationStatus = "Navigating\n";
    }

    public void UpdateLoadStatus(bool loaded)
    {
        if (loaded)
            loadStatus = "Load\n";
        else
            loadStatus = "UnLoad\n";
    }
}
