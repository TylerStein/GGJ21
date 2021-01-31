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

    private void Start() {
        if (tag != "Interactive") Debug.LogWarning($"InteractionHandler is on an object without the Interactive tag", this);
    }

    public override void OnInteract(PlayerController playerController) {
        Debug.Log("TODO: Add item to player");
        interactionCollider.enabled = false;
        chestAnimator.SetBool("isOpen", true);
        disableInteractionEvent.Invoke(gameObject);
    }

    public override string GetMessage() {
        return "Press SPACE to pick up treasure";
    }

    public override GameObjectEvent GetDistableInteractionEvent() {
        return disableInteractionEvent;
    }
}
