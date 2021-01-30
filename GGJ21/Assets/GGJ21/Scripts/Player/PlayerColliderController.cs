using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColliderController : MonoBehaviour
{
    public CollisionEventSource interactiveCollisionEventSource;
    public List<GameObject> interactionObjects;

    public GameObject FirstInteractionObject { get => interactionObjects.Count > 0 ? interactionObjects[0] : null; }
    public GameObject LastInteractionObject { get => interactionObjects.Count > 0 ? interactionObjects[interactionObjects.Count - 1] : null; }

    //public CollisionEventSource attackCollisionEventSource;
    //public List<GameObject> attackObjects;

    //public GameObject FirstAttackObject { get => attackObjects.Count > 0 ? attackObjects[0] : null; }
    //public GameObject LastAttackObject { get => attackObjects.Count > 0 ? attackObjects[attackObjects.Count - 1] : null; }

    private void Awake() {
        interactiveCollisionEventSource.enterEvent.AddListener(onEnter_Interactive);
        interactiveCollisionEventSource.exitEvent.AddListener(onExit_Interactive);

        //attackCollisionEventSource.enterEvent.AddListener(onEnter_Attack);
        //attackCollisionEventSource.exitEvent.AddListener(onExit_Attack);
    }

    private void OnDestroy() {
        interactiveCollisionEventSource.enterEvent.RemoveListener(onEnter_Interactive);
        interactiveCollisionEventSource.exitEvent.RemoveListener(onExit_Interactive);

        //attackCollisionEventSource.enterEvent.RemoveListener(onEnter_Attack);
        //attackCollisionEventSource.exitEvent.RemoveListener(onExit_Attack);
    }

    private void onEnter_Interactive(GameObject other) {
        interactionObjects.Add(other);
    }

    private void onExit_Interactive(GameObject other) {
        interactionObjects.Remove(other);
    }

    //private void onEnter_Attack(GameObject other) {
    //    attackObjects.Add(other);
    //}

    //private void onExit_Attack(GameObject other) {
    //    attackObjects.Remove(other);
    //}
}
