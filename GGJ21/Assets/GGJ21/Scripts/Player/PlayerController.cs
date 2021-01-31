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
        die,
    }

    [RequireComponent(typeof(PlayerMovementController))]
    public class PlayerController : AttackHandler
    {
        public GameManager gameManager;
        public TwinStickCameraFollow cameraController;
        public CursorInput cursorInput;
        public PlayerMovementController playerMovementController;
        public CharacterAnimator playerAnimator;
        public VFXController vfxController;
        public PlayerInteractionController interactionController;
        public PlayerUIController uiController;
        public CollisionEventSource interactionEventSource;
        public CharacterAudio characterAudio;
        public Transform playerTransform { get => playerMovementController.transform; }

        public float attackDistance = 1f;
        public Transform attackOrigin;
        public LayerMask attackLayerMask;

        public PlayerState state = PlayerState.idle;

        public bool didDie = false;

        public int maxHitPoints = 10;
        public int hitPoints = 10;

        public bool canAttack = true;
        public bool canBlock = true;
        public bool canDodge = true;
        public bool canInteract = true;

        public float moveSpeed = 2f;

        public float dodgeSpeed = 3f;
        public float dodgeDuration = 1.5f;
        public Vector3 dodgeDirection;
        public float dodgeCooldown = 0.5f;

        public float blockSpeed = 1f;

        public float attackCooldown = 0.5f;
        public float attackPreTime = 0.25f;
        public float attackPostTime = 0.25f;

        public float shakeMagnitude = 0.5f;
        public float shakeDuration = 0.05f;

        public float dieLevelChangeDelay = 2.0f;

        public RaycastHit[] attackCastHits;

        public bool isPaused;

        public bool hasTreasure;

        public void Start() {
            gameManager = FindObjectOfType<GameManager>();
            gameManager.pausedEvent.AddListener(OnPauseChange);
            hitPoints = maxHitPoints;
            interactionController.interactionHandlerEvent.AddListener(changeActiveInteraction);
        }

        public void OnDestroy() {
            interactionController.interactionHandlerEvent.RemoveListener(changeActiveInteraction);
            gameManager.pausedEvent.RemoveListener(OnPauseChange);
        }

        public void OnPauseChange(bool paused) {
            if (paused) {
                playerAnimator.PauseAll();

            } else {
                playerAnimator.ResumeAll();
            }

            isPaused = paused;
            uiController.SetPaused(isPaused);
        }

        public void Update() {
            if (cursorInput.PauseDown) {
                gameManager.TogglePause();
            }

            if (isPaused) return;

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
                case PlayerState.die:
                    updateState_Die();
                    break;
            }

            playerAnimator.SetMoveDirection(playerMovementController.lastInputMoveDirection);
            playerAnimator.SetVelocity(playerMovementController.lastInputVelocity * 100);
        }

        public override void OnAttack(AttackSource source) {
            if (state == PlayerState.blocking) {
                // block attack
                vfxController.PlayBlockImpact(source.hitLocation, Quaternion.LookRotation(source.hitNormal, Vector3.up));
                cameraController.AddShake(shakeMagnitude, shakeDuration);
                characterAudio.OnMetalImpact();
            } else if (state != PlayerState.die) {
                // take attack
                hitPoints--;
                uiController.OnHit();
                vfxController.PlayBlood(source.hitLocation, Quaternion.LookRotation(source.hitNormal, Vector3.up));
                cameraController.AddShake(shakeMagnitude, shakeDuration);
                if (hitPoints > 0) characterAudio.OnPain();
            }
        }

        public void updateState_Idle() {
            playerMovementController.Update_NormalMove(moveSpeed * Time.deltaTime);

            if (hitPoints <= 0) state = PlayerState.die;
            else if (cursorInput.Primary && canAttack) state = PlayerState.attacking;
            else if (cursorInput.InteractDown && canInteract) state = PlayerState.interact;
            else if (cursorInput.SecondaryDown && canBlock) state = PlayerState.blocking;
            else if (cursorInput.ShiftDown && playerMovementController.lastInputVelocity > 0f) state = PlayerState.dodging;
            else if (playerMovementController.lastInputVelocity > 0f) state = PlayerState.moving;
        }

        public void updateState_Moving() {
            playerMovementController.Update_NormalMove(moveSpeed * Time.deltaTime);

            if (hitPoints <= 0) state = PlayerState.die;
            else if (cursorInput.Primary && canAttack) state = PlayerState.attacking;
            else if (cursorInput.InteractDown && canInteract) state = PlayerState.interact;
            else if (cursorInput.SecondaryDown && canBlock) state = PlayerState.blocking;
            else if (cursorInput.ShiftDown && playerMovementController.lastInputVelocity > 0f) state = PlayerState.dodging;
            else if (playerMovementController.lastInputVelocity == 0f) state = PlayerState.idle;
        }

        public void updateState_Attacking() {
            if (canAttack) {
                StartCoroutine(routine_Attack());
                canAttack = false;
                canBlock = false;
                canDodge = false;
                canInteract = false;
                playerAnimator.TriggerAttack();
            }

            playerMovementController.Update_NormalMove(blockSpeed * Time.deltaTime);
            if (hitPoints <= 0) state = PlayerState.die;
        }

        public void updateState_Blocking() {
            if (canBlock) {
                canAttack = false;
                canBlock = false;
                canDodge = false;
                canInteract = false;

                playerAnimator.SetIsBlocking(true);
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
            if (hitPoints <= 0) state = PlayerState.die;
        }

        public void updateState_Dodging() {
            if (canDodge) {
                dodgeDirection = playerMovementController.moveInput;
                canAttack = false;
                canBlock = false;
                canDodge = false;
                canInteract = false;

                StartCoroutine(routine_Dodge());
                playerAnimator.SetIsDodging(true);
                vfxController.PlayDash(playerMovementController.playerTransform.position, Quaternion.LookRotation(-dodgeDirection, Vector3.up));
            }

            playerMovementController.Update_LockedMove(dodgeDirection, dodgeSpeed * Time.deltaTime);
            if (hitPoints <= 0) state = PlayerState.die;
        }

        public void changeActiveInteraction(InteractionHandler handler) {
            if (didDie) return;
            
            if (handler != null) {
                uiController.ShowInteractMessage(handler.GetMessage());
            } else {
                uiController.HideInteractMessage();
            }
        }

        public void updateState_Interact() {
            if (canInteract) {
                canInteract = false;

                InteractionHandler handler = interactionController.activeInterctionHandler;
                if (handler != null) {
                     handler.OnInteract(this);
                }
            }

            canInteract = true;

            if (hitPoints <= 0) state = PlayerState.die;
            else if (playerMovementController.lastInputVelocity > 0f) state = PlayerState.moving;
            else if (playerMovementController.lastInputVelocity == 0f) state = PlayerState.idle;
        }

        public void updateState_Die() {
            if (!didDie) {
                didDie = true;
                uiController.HideInteractMessage();
                playerAnimator.SetIsDead(true);
                characterAudio.OnDeath();
                StartCoroutine(routine_Die());
            }
        }

        public IEnumerator routine_Attack() {
            yield return new WaitForSeconds(attackPreTime);

            // all but player
            Debug.DrawRay(attackOrigin.position, attackOrigin.forward * attackDistance, Color.green, 1.0f);
            attackCastHits = Physics.RaycastAll(attackOrigin.position, attackOrigin.forward, attackDistance, attackLayerMask);
            if (attackCastHits.Length > 0) {
                GameObject target = attackCastHits[0].collider.gameObject;
                AttackHandler handler = target.GetComponent<AttackHandler>();
                if (handler) {
                    handler.OnAttack(new AttackSource() {
                        other = gameObject,
                        hitLocation = attackCastHits[0].point,
                        hitNormal = attackCastHits[0].normal,
                    });
                } else {
                    characterAudio.OnMetalImpact();
                    vfxController.PlayMetalImpact(attackCastHits[0].point, Quaternion.LookRotation(attackCastHits[0].normal, Vector3.up));
                }

                cameraController.AddShake(shakeMagnitude, shakeDuration);
            }

            yield return new WaitForSeconds(attackPostTime);

            if (state == PlayerState.attacking) {
                canBlock = true;
                canDodge = true;
                canInteract = true;

                if (playerMovementController.lastInputVelocity > 0f) state = PlayerState.moving;
                else if (playerMovementController.lastInputVelocity == 0f) state = PlayerState.idle;
            }

            yield return new WaitForSeconds(attackCooldown);

            canAttack = true;
        }

        public IEnumerator routine_Dodge() {
            yield return new WaitForSeconds(dodgeDuration);
            if (state == PlayerState.dodging) {
                canAttack = true;
                canBlock = true;
                canInteract = true;

                playerAnimator.SetIsDodging(false);

                if (playerMovementController.lastInputVelocity > 0f) state = PlayerState.moving;
                else if (playerMovementController.lastInputVelocity == 0f) state = PlayerState.idle;
            }

            yield return new WaitForSeconds(dodgeCooldown);
            canDodge = true;
        }

        public IEnumerator routine_Die() {
            yield return new WaitForSeconds(dieLevelChangeDelay);

            gameManager.LoseGame();
        }
    }
}