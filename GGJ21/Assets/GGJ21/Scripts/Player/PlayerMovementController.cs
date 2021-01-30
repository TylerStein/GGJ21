using UnityEngine;

namespace Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        public bool debugMove = false;

        public CharacterController character;
        public CursorInput cursorInput;
        public Transform playerTransform;
        public Transform moveRelativeTo;
        public Transform aimCameraTarget;
        public TwinStickCameraFollow cameraFollower;

        public float maxLookDistance = 4.0f;

        public Vector2 lastMoveDirection;
        public float lastVelocity = 1.0f;

        public Vector3 lookPoint;
        public Vector3 moveInput;

        public void Update() {
            moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        }

        public void Update_NormalMove(float speed) {
            lookPoint = cursorInput.CursorWorldPosition;
            lookPoint.y = playerTransform.position.y;

            playerTransform.LookAt(lookPoint, Vector3.up);

            ApplyRelativeMove(moveInput, speed);
        }

        public void Update_LockedMove(Vector3 direction, float speed) {
            ApplyRelativeMove(direction, speed);
        }

        public void ApplyRelativeMove(Vector3 move, float speed) {
            float relativeUpForwardRotation = Mathf.Deg2Rad * -moveRelativeTo.rotation.eulerAngles.y + (Mathf.PI * 0.5f);
            float relativeUpRightRotation = Mathf.Deg2Rad * -moveRelativeTo.rotation.eulerAngles.y;

            Vector3 relativeForward = new Vector3(Mathf.Cos(relativeUpForwardRotation), 0, Mathf.Sin(relativeUpForwardRotation)).normalized;
            Vector3 relativeRight = new Vector3(Mathf.Cos(relativeUpRightRotation), 0, Mathf.Sin(relativeUpRightRotation)).normalized;

            if (debugMove) {
                Debug.DrawLine(playerTransform.position, playerTransform.position + relativeForward * 5, Color.blue);
                Debug.DrawLine(playerTransform.position, playerTransform.position + relativeRight * 5, Color.red);
            }

            Vector3 relativeMove = ((relativeForward * move.z) + (relativeRight * move.x)) * speed;
            Vector3 gravity = Physics.gravity * Time.deltaTime;

            lastMoveDirection = relativeMove;
            lastVelocity = relativeMove.magnitude;

            character.Move(relativeMove + gravity);

            Vector3 aimTarget = Vector3.Lerp(playerTransform.position, lookPoint, 0.5f);
            Vector3 aimLineClamped = Vector3.ClampMagnitude(aimTarget - playerTransform.position, maxLookDistance);
            aimCameraTarget.position = playerTransform.position + aimLineClamped;
        }
    }
}