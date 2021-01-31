using UnityEngine;

namespace Player
{
    /// <summary>
    ///     Generic mosue input handler
    /// </summary>
    public class CursorCameraInput : MonoBehaviour
    {
        public Vector3 lastScreenMousePosition = Vector3.zero;
        public Vector3 lastWorldMouse = Vector3.zero;
        public Vector3 mouseForward = Vector3.zero;

        public float worldMouseDistance = 5.0f;
        public float maxHitDistance = 50;

        public Camera inputCamera;

        private Transform _cameraTransform;
        [SerializeField] private bool _debugMode = false;

        void Start() {
            lastScreenMousePosition = Vector3.zero;
            inputCamera = Camera.main;
            _cameraTransform = inputCamera.transform;
        }

        void Update() {
            lastScreenMousePosition = Input.mousePosition;

            lastWorldMouse = inputCamera.ScreenToWorldPoint(lastScreenMousePosition + new Vector3(0, 0, worldMouseDistance));
            mouseForward = (lastWorldMouse - _cameraTransform.position).normalized;
        }

        public bool Raycast(out RaycastHit hit) {
            return Physics.Raycast(_cameraTransform.position, mouseForward, out hit, maxHitDistance, 1 << 0);
        }

        public bool CastToPlane(Plane plane, out Vector3 point) {
            Ray ray = new Ray(inputCamera.transform.position, lastWorldMouse - inputCamera.transform.position);
            Debug.DrawRay(inputCamera.transform.position, lastWorldMouse - inputCamera.transform.position, Color.red);
            float enter = -1;
            plane.Raycast(ray, out enter);
            if (enter > 0) {
                point = ray.GetPoint(enter);
                return true;
            } else {
                point = Vector3.zero;
                return false;
            }
        }

        private void OnDrawGizmos() {
            if (_debugMode) {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(lastWorldMouse, 0.5f);
            }
        }
    }
}