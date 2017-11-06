using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Academy.HoloToolkit.Unity;

public class ObjectPlacement : MonoBehaviour
{

    // Elements
    Vector3 manipulationPreviousPosition;
    public float ManipulationSensitivity = 3.0f;

    float rotationFactor;
    public float RotationSensitivity = 5.0f;

    public float ZoomSensitivity = 0.01f;
    public static float sizePremeter = 1.0f;

    #region MoveGesture
    public void PerformManipulationStart(Vector3 position)
    {
        manipulationPreviousPosition = position;
    }

    public void PerformManipulationUpdate(Vector3 position)
    {
        if (GestureManager.Instance.IsManipulating)
        {
            Vector3 moveVector = position - manipulationPreviousPosition;
            manipulationPreviousPosition = position;
            transform.position += moveVector * ManipulationSensitivity;
        }
    }
    #endregion MoveGesture

    #region RotateGesture
    public void PerformRotationUpdate(Vector3 NavigationPosition)
    {
        if (GestureManager.Instance.IsNavigating)
        {
            rotationFactor = NavigationPosition.x * RotationSensitivity;
            transform.Rotate(new Vector3(0, -1 * rotationFactor, 0));
        }
    }
    #endregion RotateGesture

    #region ZoomGesture
    public void PerformScale(Vector3 zoomPosition)
    {
        if (GestureManager.Instance.IsZooming)
        {
            sizePremeter = zoomPosition.y * ZoomSensitivity;
            this.transform.localScale += sizePremeter * new Vector3(1.0f, 1.0f, 1.0f);
            if (this.transform.localScale.x <= 0.01f)
                this.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        }
    }
    #endregion ZoomGesture
}
