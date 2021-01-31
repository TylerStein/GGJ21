using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    public Animator animator;
    public bool enableVelocity = true;
    public bool enableDie = true;
    public bool enableAttack = true;

    public bool enableMoveDirection = false;
    public bool enableBlocking = false;
    public bool enableDodge = false;
    public bool enableHit = false;

    public void PauseAll() {
        animator.speed = 0;
    }

    public void ResumeAll() {
        animator.speed = 1;
    }

    public void SetMoveDirection(Vector3 direction) {
        // TODO: Communicate with animator
    }

    public void SetVelocity(float velocity) {
        if (enableVelocity) animator.SetFloat("velocity", velocity);
    }

    public void SetIsBlocking(bool isBlocking) {
        if (enableBlocking) animator.SetBool("isBlocking", isBlocking);
    }

    public void SetIsDodging(bool isDodging) {
        if (enableDodge) animator.SetBool("isDodging", isDodging);
    }

    public void TriggerAttack() {
        if (enableAttack) animator.SetTrigger("attack");
    }

    public void TriggerHit() {
        if (enableHit) animator.SetTrigger("hit");
    }

    public void SetIsDead(bool isDead) {
        if (enableDie) animator.SetBool("isDead", isDead);
    }
}
