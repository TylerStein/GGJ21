using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractionHandlerEvent : UnityEvent<InteractionHandler> { }

public class PlayerInteractionController : MonoBehaviour
{
    public CollisionEventSource interactiveCollisionEventSource;
    public List<InteractionHandler> handlers;

    public InteractionHandler activeInterctionHandler;
    public InteractionHandlerEvent interactionHandlerEvent = new InteractionHandlerEvent();

    public InteractionHandler FirstHandler { get => handlers.Count > 0 ? handlers[0] : null; }
    public InteractionHandler LastHandler { get => handlers.Count > 0 ? handlers[handlers.Count - 1] : null; }

    private void Awake() {
        interactiveCollisionEventSource.enterEvent.AddListener(onEnter_Interactive);
        interactiveCollisionEventSource.exitEvent.AddListener(onExit_Interactive);
    }

    private void OnDestroy() {
        interactiveCollisionEventSource.enterEvent.RemoveListener(onEnter_Interactive);
        interactiveCollisionEventSource.exitEvent.RemoveListener(onExit_Interactive);
    }

    private void onEnter_Interactive(GameObject other) {
        InteractionHandler handler = other.GetComponent<InteractionHandler>();
        if (handler != null) {
             handler.GetDistableInteractionEvent().AddListener(onExit_Interactive);
            handlers.Add(handler);
            if (!activeInterctionHandler) {
                activeInterctionHandler = handler;
                 interactionHandlerEvent.Invoke(activeInterctionHandler);
            }
        }
    }

    private void onExit_Interactive(GameObject other) {
        InteractionHandler handler = other.GetComponent<InteractionHandler>();
        if (handler != null) {
            handler.GetDistableInteractionEvent().RemoveListener(onExit_Interactive);
            handlers.Remove(handler);
            if (activeInterctionHandler == handler) {
                activeInterctionHandler = FirstHandler;
                interactionHandlerEvent.Invoke(activeInterctionHandler);
            }
        }
    }
}
