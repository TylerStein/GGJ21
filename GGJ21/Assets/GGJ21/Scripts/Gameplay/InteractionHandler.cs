using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public abstract class InteractionHandler : MonoBehaviour
{
    public abstract GameObjectEvent GetDistableInteractionEvent();
    public abstract void OnInteract(PlayerController playerController);
    public abstract string GetMessage();
}
