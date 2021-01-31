using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelEnd : InteractionHandler
{
    public GameObjectEvent disableInteractionEvent = new GameObjectEvent();

    public override GameObjectEvent GetDisableInteractionEvent() {
        return disableInteractionEvent;
    }

    public override string GetMessage() {
        return "leave the island";
    }

    public override bool CanInteract(PlayerController playerController) {
        return playerController.hasTreasure;
    }

    public override void OnInteract(PlayerController playerController) {
        disableInteractionEvent.Invoke(gameObject);
        playerController.gameManager.CompleteGame();
    }
}
