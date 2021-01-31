using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public abstract class InteractionHandler : MonoBehaviour
{
    public abstract GameObjectEvent GetDisableInteractionEvent();
    public abstract void OnInteract(PlayerController playerController);
    public abstract string GetMessage();

    public virtual bool CanInteract(PlayerController playerController) {
        return true;
    }
}
