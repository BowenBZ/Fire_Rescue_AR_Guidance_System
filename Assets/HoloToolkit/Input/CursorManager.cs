using UnityEngine;

namespace Academy.HoloToolkit.Unity
{
    /// <summary>
    /// CursorManager class takes Cursor GameObjects.
    /// One that is on Holograms and another off Holograms.
    /// 1. Shows the appropriate Cursor when a Hologram is hit.
    /// 2. Places the appropriate Cursor at the hit position.
    /// 3. Matches the Cursor normal to the hit surface.
    /// </summary>
    public class CursorManager : Singleton<CursorManager>
    {
        [Tooltip("Drag the Cursor object to show when it hits a hologram.")]
        public GameObject CursorOnHolograms;

        [Tooltip("Drag the Cursor object to show when it does not hit a hologram.")]
        public GameObject CursorOffHolograms;

        [Tooltip("Distance, in meters, to offset the cursor from the collision point.")]
        public float DistanceFromCollision = 0.01f;

        public GameObject CursorWhenHolding;    // Cursor will turn green when user is holding.
        public GameObject[] cursorFeedbackPrefab = new GameObject[(int)feedbackTpye.Max];   // Cursor will show more feedback apart from the text.
        GameObject[] cursorFeedbackObj = new GameObject[(int)feedbackTpye.Max];
        GameObject activeFeedbackObj = null;

        enum feedbackTpye
        {
            Move,
            Rotate,
            Zoom,
            Max
        }

        void Awake()
        {
            if (CursorOnHolograms == null || CursorOffHolograms == null)
            {
                return;
            }

            // Hide the Cursors to begin with.
            CursorOnHolograms.SetActive(false);
            CursorOffHolograms.SetActive(false);
            InitialCursorFeedback();
            CursorWhenHolding.SetActive(false);
        }

        void Start()
        {
        }

        void LateUpdate()
        {
            if (GazeManager.Instance == null || CursorOnHolograms == null || CursorOffHolograms == null)
            {
                return;
            }

            if (GazeManager.Instance.Hit)
            {
                CursorOnHolograms.SetActive(true);
                CursorOffHolograms.SetActive(false);
            }
            else
            {
                CursorOffHolograms.SetActive(true);
                CursorOnHolograms.SetActive(false);
            }

            // Place the cursor at the calculated position.
            this.gameObject.transform.position = GazeManager.Instance.Position + GazeManager.Instance.Normal * DistanceFromCollision;

            // Orient the cursor to match the surface being gazed at.
            gameObject.transform.up = GazeManager.Instance.Normal;

            // Update the cursor's feedback
            CheckActiveRecognizer();
        }

        void InitialCursorFeedback()
        {
            for (int i = 0; i < (int)feedbackTpye.Max; i++)
            {
                cursorFeedbackObj[i] = Instantiate(cursorFeedbackPrefab[i]);
                cursorFeedbackObj[i].SetActive(false);
            }
        }

        void CheckActiveRecognizer()
        {
            if(GestureManager.Instance.ActiveRecognizer == GestureManager.Instance.ClickRecognizer)
            {
                activeFeedbackObj = null;
            }
            else if (GestureManager.Instance.ActiveRecognizer == GestureManager.Instance.MoveRecognizer)
            {
                activeFeedbackObj = cursorFeedbackObj[(int)feedbackTpye.Move];
            }
            else if(GestureManager.Instance.ActiveRecognizer == GestureManager.Instance.RotateRecognizer)
            {
                activeFeedbackObj = cursorFeedbackObj[(int)feedbackTpye.Rotate];
            }
            else if(GestureManager.Instance.ActiveRecognizer == GestureManager.Instance.ZoomRecognizer)
            {
                activeFeedbackObj = cursorFeedbackObj[(int)feedbackTpye.Zoom];
            }

            for (int i = 0; i < (int)feedbackTpye.Max; i++)
                cursorFeedbackObj[i].SetActive(false);
            if (activeFeedbackObj == null)
                return;
            activeFeedbackObj.SetActive(true);
            activeFeedbackObj.transform.position = this.transform.position;
            activeFeedbackObj.transform.rotation = FollowUser();
        }

        Quaternion FollowUser()
        {
            Vector3 cameraDir = Camera.main.transform.eulerAngles;
            Vector3 objectDir = new Vector3(0, 0, 0);
            objectDir.x = cameraDir.x;
            objectDir.y = cameraDir.y;
            return Quaternion.Euler(objectDir);
        }

    }   
}