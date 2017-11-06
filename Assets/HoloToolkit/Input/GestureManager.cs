using UnityEngine;
using System.Collections;
using System;
using UnityEngine.VR.WSA.Input;

namespace Academy.HoloToolkit.Unity
{
    /// <summary>
    /// GestureManager creates a gesture recognizer and signs up for a tap gesture.
    /// When a tap gesture is detected, GestureManager uses GazeManager to find the game object.
    /// GestureManager then sends a message to that game object.
    /// </summary>
    [RequireComponent(typeof(GazeManager))]
    public class GestureManager : Singleton<GestureManager>
    {
        /// <summary>
        /// To select even when a hologram is not being gazed at,
        /// set the override focused object.
        /// If its null, then the gazed at object will be selected.
        /// </summary>
        public GameObject OverrideFocusedObject
        {
            get; set;
        }
 
        private GameObject focusedObject;
        public GameObject FocusedObject
        {
            get { return focusedObject; }
        }

        // Gesture Recognizer stands different kind of gesture module.
        public GestureRecognizer ClickRecognizer { get; private set; }
        public GestureRecognizer MoveRecognizer { get; private set; }
        public GestureRecognizer RotateRecognizer { get; private set; }
        public GestureRecognizer ZoomRecognizer { get; private set; }
        public GestureRecognizer SaveRecognizer { get; private set; }
        public GestureRecognizer ActiveRecognizer { get; private set; }

        // Gesture elements
        public bool IsClicking { get; private set; }
        public bool IsManipulating { get; private set; }
        public bool IsNavigating { get; private set; }
        public bool IsZooming { get; private set; }

        Vector3 initialHeadPos;
        GameObject handleObj;
        ObjectPlacement handlePlacement;

        void Start()
        {
            // Create a new GestureRecognizer. Sign up for tapped events.
            ClickRecognizer = new GestureRecognizer();
            ClickRecognizer.SetRecognizableGestures(GestureSettings.Tap);
            ClickRecognizer.TappedEvent += ClickRecognizer_TappedEvent;

            // Move Gesture
            MoveRecognizer = new GestureRecognizer();
            MoveRecognizer.SetRecognizableGestures(GestureSettings.ManipulationTranslate);
            MoveRecognizer.ManipulationStartedEvent += MoveRecognizer_ManipulationStartedEvent;
            MoveRecognizer.ManipulationUpdatedEvent += MoveRecognizer_ManipulationUpdatedEvent;
            MoveRecognizer.ManipulationCompletedEvent += MoveRecognizer_ManipulationCompletedEvent;
            MoveRecognizer.ManipulationCanceledEvent += MoveRecognizer_ManipulationCanceledEvent;

            // Rotate Gestrue
            RotateRecognizer = new GestureRecognizer();
            RotateRecognizer.SetRecognizableGestures(GestureSettings.NavigationX);
            RotateRecognizer.NavigationStartedEvent += RotateRecognizer_NavigationStartedEvent;
            RotateRecognizer.NavigationUpdatedEvent += RotateRecognizer_NavigationUpdatedEvent;
            RotateRecognizer.NavigationCompletedEvent += RotateRecognizer_NavigationCompletedEvent;
            RotateRecognizer.NavigationCanceledEvent += RotateRecognizer_NavigationCanceledEvent;

            // Zoom Gesture
            ZoomRecognizer = new GestureRecognizer();
            ZoomRecognizer.SetRecognizableGestures(GestureSettings.NavigationY);
            ZoomRecognizer.NavigationStartedEvent += ZoomRecognizer_NavigationStartedEvent;
            ZoomRecognizer.NavigationUpdatedEvent += ZoomRecognizer_NavigationUpdatedEvent;
            ZoomRecognizer.NavigationCompletedEvent += ZoomRecognizer_NavigationCompletedEvent;
            ZoomRecognizer.NavigationCanceledEvent += ZoomRecognizer_NavigationCanceledEvent;

            // Save Gesture
            SaveRecognizer = new GestureRecognizer();
            SaveRecognizer.SetRecognizableGestures(GestureSettings.Tap);
            SaveRecognizer.TappedEvent += SaveRecognizer_TappedEvent;

            // Start looking for gestures.
            IsClicking = true;
            ActiveRecognizer = ClickRecognizer;
            ActiveRecognizer.StartCapturingGestures();
        }

        void LateUpdate()
        {
            GameObject oldFocusedObject = focusedObject;

            if (GazeManager.Instance.Hit &&
                OverrideFocusedObject == null &&
                GazeManager.Instance.HitInfo.collider != null)
            {
                // If gaze hits a hologram, set the focused object to that game object.
                // Also if the caller has not decided to override the focused object.
                focusedObject = GazeManager.Instance.HitInfo.collider.gameObject;
            }
            else
            {
                // If our gaze doesn't hit a hologram, set the focused object to null or override focused object.
                focusedObject = OverrideFocusedObject;
            }

            if (focusedObject != oldFocusedObject)
            {
                // If the currently focused object doesn't match the old focused object, cancel the current gesture.
                // Start looking for new gestures.  This is to prevent applying gestures from one hologram to another.
                ClickRecognizer.CancelGestures();
                ClickRecognizer.StartCapturingGestures();
            }
        }

        void OnDestroy()
        {
            ClickRecognizer.StopCapturingGestures();
            ClickRecognizer.TappedEvent -= ClickRecognizer_TappedEvent;

            MoveRecognizer.ManipulationStartedEvent -= MoveRecognizer_ManipulationStartedEvent;
            MoveRecognizer.ManipulationUpdatedEvent -= MoveRecognizer_ManipulationUpdatedEvent;
            MoveRecognizer.ManipulationCompletedEvent -= MoveRecognizer_ManipulationCompletedEvent;
            MoveRecognizer.ManipulationCanceledEvent -= MoveRecognizer_ManipulationCanceledEvent;

            RotateRecognizer.NavigationStartedEvent -= RotateRecognizer_NavigationStartedEvent;
            RotateRecognizer.NavigationUpdatedEvent -= RotateRecognizer_NavigationUpdatedEvent;
            RotateRecognizer.NavigationCompletedEvent -= RotateRecognizer_NavigationCompletedEvent;
            RotateRecognizer.NavigationCanceledEvent -= RotateRecognizer_NavigationCanceledEvent;

            ZoomRecognizer.NavigationStartedEvent -= ZoomRecognizer_NavigationStartedEvent;
            ZoomRecognizer.NavigationUpdatedEvent -= ZoomRecognizer_NavigationUpdatedEvent;
            ZoomRecognizer.NavigationCompletedEvent -= ZoomRecognizer_NavigationCompletedEvent;
            ZoomRecognizer.NavigationCanceledEvent -= ZoomRecognizer_NavigationCanceledEvent;

            SaveRecognizer.TappedEvent -= SaveRecognizer_TappedEvent;
        }

        #region BasicFunction
        // Transfer from one gesture module to another one.
        public void Transition(GestureRecognizer newRecognizer)
        {
            if (newRecognizer == null)
            {
                return;
            }

            if (ActiveRecognizer != null)
            {
                if (ActiveRecognizer == newRecognizer)
                {
                    return;
                }
                ActiveRecognizer.CancelGestures();
                ActiveRecognizer.StopCapturingGestures();
            }
            newRecognizer.StartCapturingGestures();
            ActiveRecognizer = newRecognizer;

            if (ActiveRecognizer == ClickRecognizer)
                IsClicking = true;
            else
                IsClicking = false;

            if (ActiveRecognizer == SaveRecognizer)
                DebugInfoManager.Instance.UpdateSavePointsStatus(true);
            else
                DebugInfoManager.Instance.UpdateSavePointsStatus(false);
        }

        void ShowCursorHoldingFeedback(bool flag)
        {
            CursorManager.Instance.CursorWhenHolding.SetActive(flag);
            CursorManager.Instance.CursorOnHolograms.SetActive(!flag);
        }
        #endregion BasicFunction

        #region GestureEvent
        // Click Gesture
        private void ClickRecognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
        {
            focusedObject.SendMessage("OnSelect");
        }

        private void SaveRecognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
        {
            PointsManager.Instance.AddPoint();
        }
      
        void InitialHandleObject(GameObject focusedObject)
        {
            handleObj = focusedObject;
            handlePlacement = focusedObject.GetComponent<ObjectPlacement>();
            if (handlePlacement == null)
                handlePlacement = focusedObject.GetComponentInParent<ObjectPlacement>();
        }
    
        void FreeHandleObject()
        {
            handlePlacement = null;
            handleObj = null;
        }

        // Move Gesture
        private void MoveRecognizer_ManipulationStartedEvent(InteractionSourceKind source, Vector3 position, Ray ray)
        {
            IsManipulating = true;
            InitialHandleObject(focusedObject);
            initialHeadPos = Camera.main.transform.position;
            handlePlacement.PerformManipulationStart(position);
        }

        private void MoveRecognizer_ManipulationUpdatedEvent(InteractionSourceKind source, Vector3 position, Ray ray)
        {
            IsManipulating = true;
            handlePlacement.PerformManipulationUpdate(position - Camera.main.transform.position + initialHeadPos);
            ShowCursorHoldingFeedback(true);
        }

        private void MoveRecognizer_ManipulationCompletedEvent(InteractionSourceKind source, Vector3 position, Ray ray)
        {
            IsManipulating = false;
            FreeHandleObject();
            ShowCursorHoldingFeedback(false);
        }

        private void MoveRecognizer_ManipulationCanceledEvent(InteractionSourceKind source, Vector3 position, Ray ray)
        {
            IsManipulating = false;
            if (handleObj != null)
            {
                FreeHandleObject();
            }
            ShowCursorHoldingFeedback(false);
        }

        // Rotate Gesture
        private void RotateRecognizer_NavigationStartedEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            IsNavigating = true;
            InitialHandleObject(focusedObject);      
        }

        private void RotateRecognizer_NavigationUpdatedEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {        
            IsNavigating = true;
            handlePlacement.PerformRotationUpdate(relativePosition);
            ShowCursorHoldingFeedback(true);
        }

        private void RotateRecognizer_NavigationCompletedEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            IsNavigating = false;
            FreeHandleObject();
            ShowCursorHoldingFeedback(false);
        }

        private void RotateRecognizer_NavigationCanceledEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            IsNavigating = false;
            if (handleObj != null)
            {
                FreeHandleObject();
            }
            ShowCursorHoldingFeedback(false);
        }

        // Zoom Gesture
        private void ZoomRecognizer_NavigationStartedEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            IsZooming = true;
            InitialHandleObject(focusedObject);
        }

        private void ZoomRecognizer_NavigationUpdatedEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            IsZooming = true;
            handlePlacement.PerformScale(relativePosition);
            ShowCursorHoldingFeedback(true);
        }

        private void ZoomRecognizer_NavigationCompletedEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            IsZooming = false;
            FreeHandleObject();
            ShowCursorHoldingFeedback(false);
        }

        private void ZoomRecognizer_NavigationCanceledEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            IsZooming = false;
            if (handleObj != null)
            {
                FreeHandleObject();
            }
            ShowCursorHoldingFeedback(false);
        }
        #endregion GestureEvent
    }
}