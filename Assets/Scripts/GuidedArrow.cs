using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Academy.HoloToolkit.Unity;

public class GuidedArrow : Singleton<GuidedArrow> {

    Vector3 headPosition;
    Vector3 gazeDirection;
    Vector3 upDirection;
    Vector3 downDirection;
    float distance = 5.0f;

    bool ifHandler = false;

    void Start()
    {  
    }

    // Update is called once per frame
    void Update() {

        UpdateArrowPosition();
        UpdateArrowDirection();
        CheckWhetherAtTerminal(); 
    }

    // Switch the status of guided arrow.
    public void GASwitch(bool status)
    {
        gameObject.SetActive(status);
        ifHandler = status;
    }

    // Let guided arrow locate in the center of user's view.
    void UpdateArrowPosition()
    {
        if(ifHandler)
        {
            headPosition = Camera.main.transform.position;
            gazeDirection = Camera.main.transform.forward;
            upDirection = Camera.main.transform.up;
            downDirection = -upDirection;

            transform.position = headPosition + gazeDirection * distance + downDirection * 0.3f;
        }
    }

    // Let guided arrow locate in the center of user's view.
    void UpdateArrowDirection()
    {
        if(ifHandler)
        {
            transform.forward = Route.Instance.CurrentDirection;
        }
    }

    // Check whether user arrive the terminal.
    void CheckWhetherAtTerminal()
    {
        if (ifHandler)
        {
            if (Route.Instance.Terminal)
            {
                ifHandler = false;
                transform.position = Route.Instance.EndPostion;
                transform.forward = Vector3.down;
                DebugInfoManager.Instance.UpdateNavigationStatus(true, true);
            }
        }
    }

    public void ShowArrow(bool flag)
    {
        if(!flag)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            headPosition = Camera.main.transform.position;
            gazeDirection = Camera.main.transform.forward;
            upDirection = Camera.main.transform.up;
            downDirection = -upDirection;

            transform.position = headPosition + gazeDirection * distance;// + downDirection;
        }
    }
}
