using UnityEngine;
using System.Collections;

namespace Player
{
    public class TwinStickCameraFollow : MonoBehaviour
    {
        public Vector3 cameraOffset = new Vector3(0, 8f, -2f);
        public float followSpeed = 1f;
        public float rotationSpeed = 1f;

        public Transform followTarget = null;

        public float shakeReturnSpeed = 1f;
        private float shakeForce = 0;
        private float shakeTimer = 0f;
        private Vector3 shakeOffset = Vector3.zero;

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

            if (shakeTimer > 0f) {
                actualPosition -= shakeOffset;
                shakeOffset = Random.insideUnitSphere * shakeForce;
                shakeTimer -= Time.deltaTime;
                if (shakeTimer < 0f) ClearShake();
            } else {
                shakeOffset = Vector3.Lerp(shakeOffset, Vector3.zero, Time.deltaTime * shakeReturnSpeed);
            }

            transform.SetPositionAndRotation(actualPosition, actualRotation);
        }

        public void ClearShake() {
            shakeForce = 0f;
            shakeTimer = 0f;
        }

        public void SetShate(float magnitude = 0.2f, float duration = 0.1f) {
            shakeTimer = duration;
            shakeForce = magnitude;
        }

        public void AddShake(float magnitude = 0.2f, float duration = 0.1f) {
            shakeTimer += duration;
            shakeForce += magnitude;

        }
    }
}