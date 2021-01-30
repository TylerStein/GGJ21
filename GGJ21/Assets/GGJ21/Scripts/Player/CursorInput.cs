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

        private bool modifierDown = false;
        public bool ModifierDown { get { return modifierDown; } }

        private Vector3 cursorPosition = Vector3.zero;
        public Vector3 CursorPosition { get { return cursorPosition; } }

        private Vector3 cursorWorldPosition = Vector3.zero;
        public Vector3 CursorWorldPosition { get { return cursorWorldPosition; } }

        private bool lastCursorWorldPositionValid = false;
        public bool LastCursorWorldPositionValid {  get { return lastCursorWorldPositionValid;  } }

        [SerializeField] private bool _debugMode = false;
        [SerializeField] private string _primaryButtonName = "Primary";
        [SerializeField] private string _secondaryButtonName = "Secondary";
        [SerializeField] private string _modifierButtonName = "Shift";

        private void Update() {
            cursorPosition = UnityEngine.Input.mousePosition;
            primaryDown = UnityEngine.Input.GetButtonDown(_primaryButtonName);
            primary = UnityEngine.Input.GetButton(_primaryButtonName);
            secondaryDown = UnityEngine.Input.GetButtonDown(_secondaryButtonName);
            modifierDown = UnityEngine.Input.GetButtonDown(_modifierButtonName);

            //RaycastHit hit;
            //lastCursorWorldPositionValid = cursorCameraInput.Raycast(out hit);
            //cursorWorldPosition = hit.point;

            RaycastHit hit;
            bool didHit = cursorCameraInput.Raycast(out hit);
            if (didHit) {
                cursorWorldPosition = hit.point;
            } else {
                Vector3 point = Vector3.zero;
                Plane plane = new Plane(cursorPlaneSource.up, cursorPlaneSource.transform.position + cursorPlaneSource.transform.forward);
                if (cursorCameraInput.CastToPlane(plane, out point)) {
                    cursorWorldPosition = point;
                } else {
                    Debug.Log("Cursor Input Cast Hit Nothing");
                }
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