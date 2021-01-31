using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Player;

public enum EnemyState
{
    idle,
    aggro,
    attack,
    die
}

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterAnimator))]
public class MobController : AttackHandler
{
    public GameManager gameManager;
    public VFXController vfxController;
    public PlayerController playerController;
    public NavMeshAgent agent;
    public CharacterAnimator mobAnimator;
    public MobUIController uiController;
    public Collider mobCollider;
    public CharacterAudio characterAudio;

    public EnemyState state = EnemyState.idle;

    public float aggroDistance = 10f;
    public float giveUpDistance = 15f;
    public float attackDistance = 2f;

    public float attackCooldown = 0.5f;
    public float attackPreTime = 0.25f;
    public float attackPostTime = 0.25f;

    public float attackRangeRotateSpeed = 600;

    public int maxHitPoints = 10;
    public int hitPoints = 10;

    public bool canAttack = true;
    public bool didDie = false;

    public float distanceToPlayer = Mathf.Infinity;
    public RaycastHit[] attackCastHits;
    public LayerMask attackLayerMask;
    public Transform attackOrigin;

    public bool isPaused;
    public Vector3 lastAgentVelocity;

    public float sinkDelay = 2.5f;
    public float sinkYTarget = -5f;
    public float sinkYSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        vfxController = FindObjectOfType<VFXController>();
        playerController = FindObjectOfType<PlayerController>();
        gameManager = FindObjectOfType<GameManager>();
        gameManager.pausedEvent.AddListener(OnPauseChange);
        hitPoints = maxHitPoints;
    }

    // Update is called once per frame
    void Update() {
        if (isPaused) {
            if (agent.enabled) agent.isStopped = true;
            return;
        }

        distanceToPlayer = Vector3.Distance(transform.position, playerController.transform.position);

        switch (state) {
            case EnemyState.idle:
                updateState_Idle();
                break;
            case EnemyState.aggro:
                updateState_Aggro();
                break;
            case EnemyState.attack:
                updateState_Attack();
                break;
            case EnemyState.die:
                updateState_Die();
                break;
        }

        mobAnimator.SetMoveDirection(agent.desiredVelocity.normalized);
        mobAnimator.SetVelocity(agent.velocity.magnitude);
    }

    public void OnPauseChange(bool paused) {
        if (paused) {
            mobAnimator.PauseAll();
            if (agent.enabled) {
                lastAgentVelocity = agent.velocity;
                agent.velocity = Vector3.zero;
            }

        } else {
            if (agent.enabled) agent.velocity = lastAgentVelocity;
            mobAnimator.ResumeAll();
        }

        isPaused = paused;
    }

    public override void OnAttack(AttackSource source) {
        if (state != EnemyState.die) {
            // take attack
            hitPoints--;
            uiController.OnHit();
            vfxController.PlayBlood(source.hitLocation, Quaternion.LookRotation(source.hitNormal, Vector3.up));
            if (hitPoints > 0) characterAudio.OnPain();
        }
    }

    void updateState_Idle() {
        if (hitPoints <= 0) state = EnemyState.die;
        else if (distanceToPlayer < aggroDistance) state = EnemyState.aggro;
    }

    void updateState_Aggro() {
        if (hitPoints <= 0) state = EnemyState.die;
        else if (playerController.state != PlayerState.die) {
            if (distanceToPlayer > giveUpDistance) state = EnemyState.idle;
            else if (distanceToPlayer <= attackDistance) {
                if (canAttack) state = EnemyState.attack;
                else agent.isStopped = true;
            } else {
                agent.isStopped = false;
                agent.SetDestination(playerController.transform.position);
            }
        } else {
            agent.isStopped = true;
        }
    }

    void updateState_Attack() {
        if (canAttack) {
            canAttack = false;

            // run attack logic
            StartCoroutine(routine_Attack());
            mobAnimator.TriggerAttack();
        }


        if (agent.enabled) agent.isStopped = true;

        Vector3 targetLookRotation = new Vector3(playerController.transform.position.x, agent.transform.position.y, playerController.transform.position.z) - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetLookRotation, Vector3.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, attackRangeRotateSpeed * Time.deltaTime);

        if (hitPoints <= 0) state = EnemyState.die;
        else if (distanceToPlayer > attackDistance) state = EnemyState.aggro;
    }

    void updateState_Die() {

        if (!didDie) {
            didDie = true;
            agent.velocity = Vector3.zero;
            agent.enabled = false;
            mobCollider.enabled = false;
            uiController.HideUI();
            mobAnimator.SetIsDead(true);
            characterAudio.OnDeath();

            StartCoroutine(routine_SinkAndDestroy());
        }
    }

    IEnumerator routine_Attack() {
        yield return new WaitForSeconds(attackPreTime);

        // all but enemy
        Debug.DrawRay(attackOrigin.position, attackOrigin.forward * attackDistance, Color.red, 1.0f);
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
                vfxController.PlayMetalImpact(attackCastHits[0].point, Quaternion.LookRotation(attackCastHits[0].normal, Vector3.up));
                characterAudio.OnMetalImpact();
            }
        }

        yield return new WaitForSeconds(attackPostTime);

        if (state == EnemyState.attack) {
            state = EnemyState.aggro;
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    IEnumerator routine_SinkAndDestroy() {
        yield return new WaitForSeconds(sinkDelay);

        while (transform.position.y > sinkYTarget) {
            yield return new WaitForEndOfFrame();
            transform.Translate(Vector3.down * sinkYSpeed * Time.deltaTime);
        }

        Destroy(gameObject);
    }
}
