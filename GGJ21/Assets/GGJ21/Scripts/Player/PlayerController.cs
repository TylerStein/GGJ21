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
        dodging
    }

    [RequireComponent(typeof(PlayerMovementController))]
    public class PlayerController : MonoBehaviour
    {
        public CursorInput cursorInput;
        public PlayerMovementController playerMovementController;
        public PlayerAnimator playerAnimator;

        public PlayerState state = PlayerState.idle;

        public bool canAttack = false;
        public bool canBlock = false;
        public bool canDodge = false;

        public float moveSpeed = 2f;

        public float dodgeSpeed = 3f;
        public float dodgeDuration = 1.5f;
        public Vector3 dodgeDirection;

        public float blockSpeed = 1f;

        public float attackSpeed = 0.25f;
        public float attackDuration = 0.25f;


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
            }

            playerAnimator.SetMoveDirection(playerMovementController.lastMoveDirection);
            playerAnimator.SetVelocity(playerMovementController.lastVelocity);
        }

        public void updateState_Idle() {
            playerMovementController.Update_NormalMove(moveSpeed * Time.deltaTime);

            if (cursorInput.Primary && canAttack) state = PlayerState.attacking;
            else if (cursorInput.SecondaryDown && canBlock) state = PlayerState.blocking;
            else if (cursorInput.ShiftDown && canDodge) state = PlayerState.dodging;
            else if (playerMovementController.lastVelocity > 0f) state = PlayerState.moving;
        }

        public void updateState_Moving() {
            playerMovementController.Update_NormalMove(moveSpeed * Time.deltaTime);

            if (cursorInput.Primary && canAttack) state = PlayerState.attacking;
            else if (cursorInput.SecondaryDown && canBlock) state = PlayerState.blocking;
            else if (cursorInput.ShiftDown && canDodge) state = PlayerState.dodging;
            else if (playerMovementController.lastVelocity == 0f) state = PlayerState.idle;
        }

        public void updateState_Attacking() {
            if (canAttack) {
                StartCoroutine(routine_Attack());
                canAttack = false;
                canBlock = false;
                canDodge = false;

                playerAnimator.SetIsAttacking(true);
            }

            playerMovementController.Update_NormalMove(attackSpeed * Time.deltaTime);
        }

        public void updateState_Blocking() {
            if (canBlock) {
                canAttack = false;
                canBlock = false;
                canDodge = false;

                playerAnimator.SetIsBlocking(true);
            }

            if (!cursorInput.Secondary) {
                if (playerMovementController.lastVelocity == 0f) state = PlayerState.idle;
                if (playerMovementController.lastVelocity > 0f) state = PlayerState.moving;

                canAttack = true;
                canBlock = true;
                canDodge = true;

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

                playerAnimator.SetIsDodging(true);
            }

            playerMovementController.Update_LockedMove(dodgeDirection, dodgeSpeed * Time.deltaTime);
        }

        public IEnumerator routine_Attack() {
            yield return new WaitForSeconds(attackDuration);

            canAttack = true;
            canBlock = true;
            canDodge = true;

            if (playerMovementController.lastVelocity > 0f) state = PlayerState.moving;
            else if (playerMovementController.lastVelocity == 0f) state = PlayerState.idle;

            playerAnimator.SetIsAttacking(false);
        }

        public IEnumerator routine_Dodge() {
            yield return new WaitForSeconds(dodgeDuration);

            canAttack = true;
            canBlock = true;
            canDodge = true;

            if (playerMovementController.lastVelocity > 0f) state = PlayerState.moving;
            else if (playerMovementController.lastVelocity == 0f) state = PlayerState.idle;

            playerAnimator.SetIsDodging(false);
        }
    }
}