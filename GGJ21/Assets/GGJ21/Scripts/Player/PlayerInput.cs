using UnityEngine;

namespace Player
{
    /// <summary>
    ///     Resolves interaction for passing to an interaction consumer
    /// </summary>
    public class PlayerInput : MonoBehaviour
    {
        public CursorCameraInput cursorCameraInput;
        public Transform cursorPlaneSource;
        public Transform controllerCursorRelativeTo;

        private bool mkPrimaryDown = false;
        private bool mkPrimary = false;
        private bool mkSecondaryDown = false;
        private bool mkSecondary = false;
        private bool mkShiftDown = false;
        private bool mkShift = false;
        private bool mkInteractDown = false;
        private bool mkInteract = false;
        private bool mkPauseDown = false;

        private bool cPrimaryDown = false;
        private bool cPrimary = false;
        private bool cSecondaryDown = false;
        private bool cSecondary = false;
        private bool cShiftDown = false;
        private bool cShift = false;
        private bool cInteractDown = false;
        private bool cInteract = false;
        private bool cPauseDown = false;

        public bool PrimaryDown { get { return mkPrimaryDown || cPrimaryDown; } }

        public bool Primary { get { return mkPrimary || cPrimary; } }

        public bool SecondaryDown { get { return mkSecondaryDown || cSecondaryDown; } }

        public bool Secondary { get { return mkSecondary || cSecondary; } }

        public bool ShiftDown { get { return mkShiftDown || cShiftDown; } }

        public bool Shift { get { return mkShift || cShift; } }

        public bool InteractDown { get { return mkInteractDown || cInteractDown; } }

        public bool Interact { get { return mkInteract || cInteract; } }

        public bool PauseDown { get { return mkPauseDown || cPauseDown; } }

        private Vector3 cursorPosition = Vector3.zero;
        public Vector3 CursorPosition { get { return cursorPosition; } }

        private Vector3 cursorWorldPosition = Vector3.zero;
        public Vector3 CursorWorldPosition { get { return cursorWorldPosition; } }

        private bool lastCursorWorldPositionValid = false;
        public bool LastCursorWorldPositionValid {  get { return lastCursorWorldPositionValid;  } }

        private Vector3 lastMousePosition;

        [SerializeField] private bool _debugMode = false;
        [SerializeField] private string _primaryButtonName = "Primary";
        [SerializeField] private string _secondaryButtonName = "Secondary";
        [SerializeField] private string _shiftButtonName = "Shift";
        [SerializeField] private string _interactButtonName = "Jump";
        [SerializeField] private string _pauseButtonName = "Cancel";

        [SerializeField] private float _cPrimaryThreshold = 0.2f;
        [SerializeField] private float _cSecondaryThreshold = 0.2f;

        [SerializeField] private string _cPrimaryButtonName = "CPrimary";
        [SerializeField] private string _cSecondaryButtonName = "CSecondary";
        [SerializeField] private string _cShiftButtonName = "CShift";
        [SerializeField] private string _cInteractButtonName = "CJump";
        [SerializeField] private string _cPauseButtonName = "CCancel";
        [SerializeField] private string _cHorizontalName = "CLookHorizontal";
        [SerializeField] private string _cVerticalName = "CLookVertical";

        private void Update() {
            mkPrimaryDown = Input.GetButtonDown(_primaryButtonName);
            mkPrimary = Input.GetButton(_primaryButtonName);
            mkSecondaryDown = Input.GetButtonDown(_secondaryButtonName);
            mkSecondary = Input.GetButton(_secondaryButtonName);
            mkShiftDown = Input.GetButtonDown(_shiftButtonName);
            mkShift = Input.GetButton(_shiftButtonName);
            mkInteractDown = Input.GetButtonDown(_interactButtonName);
            mkInteract = Input.GetButton(_interactButtonName);
            mkPauseDown = Input.GetButtonDown(_pauseButtonName);

            cShiftDown = Input.GetButtonDown(_cShiftButtonName);
            cShift = Input.GetButton(_cShiftButtonName);
            cInteractDown = Input.GetButtonDown(_cInteractButtonName);
            cInteract = Input.GetButton(_cInteractButtonName);
            cPauseDown = Input.GetButtonDown(_cPauseButtonName);

            cPrimary = Input.GetAxis(_cPrimaryButtonName) > _cPrimaryThreshold;
            if (cPrimary && !cPrimaryDown) {
                cPrimaryDown = true;
            } else if (cPrimaryDown) {
                cPrimaryDown = false;
            }

            cSecondary = Input.GetAxis(_cSecondaryButtonName) > _cSecondaryThreshold;
            if (cSecondary && !cSecondaryDown) {
                cSecondaryDown = true;
            } else if (cSecondaryDown) {
                cSecondaryDown = false;
            }

            Vector2 cLook = new Vector2(Input.GetAxis(_cHorizontalName), Input.GetAxis(_cVerticalName));
            float mouseChange = Vector3.Distance(lastMousePosition, Input.mousePosition);
            lastMousePosition = Input.mousePosition;

            if (mouseChange > 0) {
                cursorPosition = lastMousePosition;

                Vector3 point = Vector3.zero;
                Plane plane = new Plane(cursorPlaneSource.up, cursorPlaneSource.transform.position + cursorPlaneSource.transform.forward);
                if (cursorCameraInput.CastToPlane(plane, out point)) {
                    cursorWorldPosition = point;
                }
            } else {
                Vector2 normalizedLook = cLook.normalized;
                cursorWorldPosition = controllerCursorRelativeTo.position + new Vector3(normalizedLook.x * 2f, 0f, normalizedLook.y * -2f);
            }
        }

        private void OnDrawGizmos() {
            if (_debugMode) {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(cursorWorldPosition, 0.5f);
            }
        }
    }
}