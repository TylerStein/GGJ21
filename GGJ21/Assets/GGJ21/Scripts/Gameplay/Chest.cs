using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class Chest : InteractionHandler
{
    public Animator chestAnimator;
    public float chestOpenTime = 0.15f;
    public Collider interactionCollider;
    public bool isOpened = false;
    public GameObjectEvent disableInteractionEvent = new GameObjectEvent();
    public Transform coinEffectsSource;
    public float cointEffectsDelay = 0.15f;
    public VFXController vfxController;
    public AudioSource audioSource;

    public bool isMainTreasure = true;

    private void Start() {
        if (tag != "Interactive") Debug.LogWarning($"InteractionHandler is on an object without the Interactive tag", this);
    }

    public override void OnInteract(PlayerController playerController) {
        Debug.Log("TODO: Add item to player");
        if (!vfxController) vfxController = playerController.vfxController;
        interactionCollider.enabled = false;
        chestAnimator.SetBool("isOpen", true);
        StartCoroutine(routine_OpenFX());
        playerController.hasTreasure = true;
        disableInteractionEvent.Invoke(gameObject);
    }

    public override string GetMessage() {
        return "open treasure chest";
    }

    public override GameObjectEvent GetDisableInteractionEvent() {
        return disableInteractionEvent;
    }

    private IEnumerator routine_OpenFX() {
        yield return new WaitForSeconds(cointEffectsDelay);
        vfxController.PlayCoinExplosion(coinEffectsSource.position, coinEffectsSource.rotation);
        audioSource.Play();
    }
}
