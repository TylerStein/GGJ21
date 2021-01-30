using UnityEngine;

namespace Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        // public bool debugShoot = false;
        public bool debugMove = false;

        public CharacterController character;
        public CursorInput cursorInput;
        public Transform playerTransform;
        public Transform moveRelativeTo;
        public Transform aimCameraTarget;
        // public Transform shootSource;
        public TwinStickCameraFollow cameraFollower;

        // public float maxShootHeight = 2.0f;

        public float maxLookDistance = 4.0f;

        public float moveSpeed = 2.0f;

        public float velocity = 1.0f;

        [Range(0.1f, 20.0f)]
        public float maxSimTime = 10.0f;

        [Range(0.02f, 1.0f)]
        public float simStep = 0.1f;
        public float angle = 0f;
        public float gravity = 0.1f;

        private void Update() {
            Vector3 lookPoint = cursorInput.CursorWorldPosition;
            lookPoint.y = playerTransform.position.y;

            playerTransform.LookAt(lookPoint, Vector3.up);

            float relativeUpForwardRotation = Mathf.Deg2Rad * -moveRelativeTo.rotation.eulerAngles.y + (Mathf.PI * 0.5f);
            float relativeUpRightRotation = Mathf.Deg2Rad * -moveRelativeTo.rotation.eulerAngles.y;

            Vector3 relativeForward = new Vector3(Mathf.Cos(relativeUpForwardRotation), 0, Mathf.Sin(relativeUpForwardRotation)).normalized;
            Vector3 relativeRight = new Vector3(Mathf.Cos(relativeUpRightRotation), 0, Mathf.Sin(relativeUpRightRotation)).normalized;

            if (debugMove) {
                Debug.DrawLine(playerTransform.position, playerTransform.position + relativeForward * 5, Color.blue);
                Debug.DrawLine(playerTransform.position, playerTransform.position + relativeRight * 5, Color.red);
            }

            Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            Vector3 relativeMove = ((relativeForward * moveInput.z) + (relativeRight * moveInput.x)) * Time.deltaTime * moveSpeed;
            Vector3 gravity = Physics.gravity * Time.deltaTime;

            character.Move(relativeMove + gravity);
            // playerTransform.position += relativeMove * Time.deltaTime * moveSpeed;

            Vector3 aimTarget = Vector3.Lerp(playerTransform.position, lookPoint, 0.5f);
            Vector3 aimLineClamped = Vector3.ClampMagnitude(aimTarget - playerTransform.position, maxLookDistance);
            aimCameraTarget.position = playerTransform.position + aimLineClamped;

            //Vector3 shootTarget = cursorInput.CursorWorldPosition;
            //shootTarget.y = Mathf.Clamp(shootTarget.y, -maxShootHeight, maxShootHeight);
            //shootSource.LookAt(shootTarget);
        }

        private void OnDrawGizmos() {

            //for (float t = 0; t <= maxSimTime; t += simStep) {
            //    Vector2 end = projectileMotion.Calculate(Vector2.zero, velocity, angle, gravity, t);

            //    Vector3 worldPosition = transform.position + new Vector3(end.x, end.y);
            //    if (worldPosition.y <= 0) break;

            //    Gizmos.DrawWireSphere(worldPosition, 0.01f);
            //}

        }
    }
}