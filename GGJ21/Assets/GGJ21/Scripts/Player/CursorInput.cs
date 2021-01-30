using UnityEngine;

namespace Player
{
    /// <summary>
    ///     Resolves interaction for passing to an interaction consumer
    /// </summary>
    public class CursorInput : MonoBehaviour
    {
        public CursorCameraInput cursorCameraInput;
        public Transform cursorPlaneSource;

        private bool primaryDown = false;
        public bool PrimaryDown { get { return primaryDown; } }

        private bool primary = false;
        public bool Primary { get { return primary; } }

        private bool secondaryDown = false;
        public bool SecondaryDown { get { return secondaryDown; } }

        private bool secondary = false;
        public bool Secondary { get { return secondary; } }

        private bool shiftDown = false;
        public bool ShiftDown { get { return shiftDown; } }

        private bool shift = false;
        public bool Shift { get { return shift; } }

        private bool interactDown = false;
        public bool InteractDown { get { return interactDown; } }

        private bool interact = false;
        public bool Interact { get { return interact; } }

        private Vector3 cursorPosition = Vector3.zero;
        public Vector3 CursorPosition { get { return cursorPosition; } }

        private Vector3 cursorWorldPosition = Vector3.zero;
        public Vector3 CursorWorldPosition { get { return cursorWorldPosition; } }

        private bool lastCursorWorldPositionValid = false;
        public bool LastCursorWorldPositionValid {  get { return lastCursorWorldPositionValid;  } }

        [SerializeField] private bool _debugMode = false;
        [SerializeField] private string _primaryButtonName = "Primary";
        [SerializeField] private string _secondaryButtonName = "Secondary";
        [SerializeField] private string _shiftButtonName = "Shift";
        [SerializeField] private string _interactButtonName = "Jump";

        private void Update() {
            cursorPosition = Input.mousePosition;
            primaryDown = Input.GetButtonDown(_primaryButtonName);
            primary = Input.GetButton(_primaryButtonName);

            secondaryDown = Input.GetButtonDown(_secondaryButtonName);
            secondary = Input.GetButton(_secondaryButtonName);

            shiftDown = Input.GetButtonDown(_shiftButtonName);
            shift = Input.GetButton(_shiftButtonName);

            interactDown = Input.GetButtonDown(_interactButtonName);
            interact = Input.GetButton(_interactButtonName);


            //RaycastHit hit;
            //bool didHit = cursorCameraInput.Raycast(out hit);
            //if (didHit) {
            //    cursorWorldPosition = hit.point;
            //} else {
                Vector3 point = Vector3.zero;
                Plane plane = new Plane(cursorPlaneSource.up, cursorPlaneSource.transform.position + cursorPlaneSource.transform.forward);
                if (cursorCameraInput.CastToPlane(plane, out point)) {
                    cursorWorldPosition = point;
                } else {
                    Debug.Log("Cursor Input Cast Hit Nothing");
                }
            //}

        }

        private void OnDrawGizmos() {
            if (_debugMode) {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(cursorWorldPosition, 0.5f);
            }
        }
    }
}