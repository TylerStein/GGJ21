using UnityEngine;

namespace Player
{
    public class TwinStickCameraFollow : MonoBehaviour
    {
        public Vector3 cameraOffset = new Vector3(0, 8f, -2f);
        public float followSpeed = 1f;
        public float rotationSpeed = 1f;

        public Vector3 shakeForce = Vector3.zero;

        public Transform followTarget = null;

        // Start is called before the first frame update
        void Start() {
            if (!followTarget) throw new UnityException("CameraFollow does not have a target set!");
        }

        // Update is called once per frame
        void Update() {
            Vector3 desiredPosition = followTarget.position + cameraOffset;
            Vector3 targetDelta = followTarget.position - transform.position;
            Quaternion desiredRotation = Quaternion.LookRotation(targetDelta, Vector3.up);
            Vector3 desiredRotationEuler = desiredRotation.eulerAngles;
            desiredRotationEuler.y = 0;

            desiredRotation = Quaternion.Euler(desiredRotationEuler);

            Vector3 actualPosition = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
            Quaternion actualRotation = Quaternion.Lerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);

            shakeForce.Set(0, 0, 0);

            transform.SetPositionAndRotation(actualPosition, actualRotation);
        }

        public void Shake(float radius = 0.2f) {
            shakeForce += Random.insideUnitSphere * radius;
        }
    }
}