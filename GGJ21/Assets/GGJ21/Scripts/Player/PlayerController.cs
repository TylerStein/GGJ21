using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    public enum PlayerState
    {
        idle,
        moving,
        attacking,
        blocking,
        dodging,
        interact,
    }

    [RequireComponent(typeof(PlayerMovementController))]
    public class PlayerController : MonoBehaviour
    {
        public TwinStickCameraFollow cameraController;
        public CursorInput cursorInput;
        public PlayerMovementController playerMovementController;
        public PlayerAnimator playerAnimator;
        public VFXController vfxController;
        public PlayerColliderController colliderController;

        public PlayerState state = PlayerState.idle;

        public bool canAttack = false;
        public bool canBlock = false;
        public bool canDodge = false;
        public bool canInteract = false;

        public float moveSpeed = 2f;

        public float dodgeSpeed = 3f;
        public float dodgeDuration = 1.5f;
        public Vector3 dodgeDirection;

        public float blockSpeed = 1f;

        public float attackSpeed = 0.25f;
        public float attackDuration = 0.25f;

        public float shakeMagnitude = 0.5f;
        public float shakeDuration = 0.05f;

        public void Update() {
            switch (state) {
                case PlayerState.idle:
                    updateState_Idle();
                    break;
                case PlayerState.moving:
                    updateState_Moving();
                    break;
                case PlayerState.attacking:
                    updateState_Attacking();
                    break;
                case PlayerState.blocking:
                    updateState_Blocking();
                    break;
                case PlayerState.dodging:
                    updateState_Dodging();
                    break;
                case PlayerState.interact:
                    updateState_Interact();
                    break;
            }

            playerAnimator.SetMoveDirection(playerMovementController.lastInputMoveDirection);
            playerAnimator.SetVelocity(playerMovementController.lastInputVelocity);
        }

        public void updateState_Idle() {
            playerMovementController.Update_NormalMove(moveSpeed * Time.deltaTime);

            if (cursorInput.Primary && canAttack) state = PlayerState.attacking;
            else if (cursorInput.InteractDown && canInteract) state = PlayerState.interact;
            else if (cursorInput.SecondaryDown && canBlock) state = PlayerState.blocking;
            else if (cursorInput.ShiftDown && canDodge) state = PlayerState.dodging;
            else if (playerMovementController.lastInputVelocity > 0f) state = PlayerState.moving;
        }

        public void updateState_Moving() {
            playerMovementController.Update_NormalMove(moveSpeed * Time.deltaTime);

            if (cursorInput.Primary && canAttack) state = PlayerState.attacking;
            else if (cursorInput.InteractDown && canInteract) state = PlayerState.interact;
            else if (cursorInput.SecondaryDown && canBlock) state = PlayerState.blocking;
            else if (cursorInput.ShiftDown && canDodge) state = PlayerState.dodging;
            else if (playerMovementController.lastInputVelocity == 0f) state = PlayerState.idle;
        }

        public void updateState_Attacking() {
            if (canAttack) {
                StartCoroutine(routine_Attack());
                canAttack = false;
                canBlock = false;
                canDodge = false;
                canInteract = false;

                playerAnimator.SetIsAttacking(true);

                GameObject target = colliderController.FirstAttackObject;
                if (target != null) {
                    Vector3 dir = target.transform.position - playerMovementController.playerTransform.position;
                    Vector3 hitLocation = target.transform.position;
                    Vector3 hitNormal = -dir;

                    RaycastHit[] hits = Physics.RaycastAll(playerMovementController.playerTransform.position + Vector3.up * 1.5f, playerMovementController.transform.forward, 6f, 1 << 0);
                    foreach (RaycastHit hit in hits) {
                        if (hit.collider.gameObject == target) {
                            hitLocation = hit.point;
                            hitNormal = hit.normal;
                        }
                    }

                    AttackHandler handler = target.GetComponent<AttackHandler>();
                    if (handler != null) {
                        handler.OnAttack(new AttackSource() {
                            other = gameObject,
                            hitLocation = hitLocation,
                            hitNormal = hitNormal,
                        });

                        vfxController.PlayBlood(hitLocation, Quaternion.LookRotation(-hitNormal, Vector3.up));
                    } else {
                        vfxController.PlayMetalImpact(hitLocation, Quaternion.LookRotation(hitNormal, Vector3.up));
                    }

                    cameraController.AddShake(shakeMagnitude, shakeDuration);
                }

            }

            playerMovementController.Update_NormalMove(attackSpeed * Time.deltaTime);
        }

        public void updateState_Blocking() {
            if (canBlock) {
                canAttack = false;
                canBlock = false;
                canDodge = false;
                canInteract = false;

                playerAnimator.SetIsBlocking(true);
                vfxController.PlayBlockImpact(playerMovementController.playerTransform.position + playerMovementController.playerTransform.forward * 0.75f + (Vector3.up * 1.5f), playerMovementController.playerTransform.rotation);
                cameraController.AddShake(shakeMagnitude, shakeDuration);
            }

            if (!cursorInput.Secondary) {
                if (playerMovementController.lastInputVelocity == 0f) state = PlayerState.idle;
                if (playerMovementController.lastInputVelocity > 0f) state = PlayerState.moving;

                canAttack = true;
                canBlock = true;
                canDodge = true;
                canInteract = true;

                playerAnimator.SetIsBlocking(false);
            }

            playerMovementController.Update_NormalMove(blockSpeed * Time.deltaTime);
        }

        public void updateState_Dodging() {
            if (canDodge) {
                dodgeDirection = playerMovementController.moveInput;
                StartCoroutine(routine_Attack());
                canAttack = false;
                canBlock = false;
                canDodge = false;
                canInteract = false;

                playerAnimator.SetIsDodging(true);
                vfxController.PlayDash(playerMovementController.playerTransform.position, Quaternion.LookRotation(-dodgeDirection, Vector3.up));
            }

            playerMovementController.Update_LockedMove(dodgeDirection, dodgeSpeed * Time.deltaTime);
        }

        public void updateState_Interact() {
            if (canInteract) {
                canAttack = false;
                canBlock = false;
                canDodge = false;
                canInteract = false;

                GameObject interactiveObject = colliderController.FirstInteractionObject;
                if (interactiveObject != null) {
                    InteractionHandler handler = interactiveObject.GetComponent<InteractionHandler>();
                    if (handler != null) {
                        handler.OnInteract(this);
                    }
                }
            }


            canAttack = true;
            canBlock = true;
            canDodge = true;
            canInteract = true;

            if (playerMovementController.lastInputVelocity > 0f) state = PlayerState.moving;
            else if (playerMovementController.lastInputVelocity == 0f) state = PlayerState.idle;
        }

        public IEnumerator routine_Attack() {
            yield return new WaitForSeconds(attackDuration);

            canAttack = true;
            canBlock = true;
            canDodge = true;
            canInteract = true;

            if (playerMovementController.lastInputVelocity > 0f) state = PlayerState.moving;
            else if (playerMovementController.lastInputVelocity == 0f) state = PlayerState.idle;

            playerAnimator.SetIsAttacking(false);
        }

        public IEnumerator routine_Dodge() {
            yield return new WaitForSeconds(dodgeDuration);

            canAttack = true;
            canBlock = true;
            canDodge = true;
            canInteract = true;

            if (playerMovementController.lastInputVelocity > 0f) state = PlayerState.moving;
            else if (playerMovementController.lastInputVelocity == 0f) state = PlayerState.idle;

            playerAnimator.SetIsDodging(false);
        }
    }
}