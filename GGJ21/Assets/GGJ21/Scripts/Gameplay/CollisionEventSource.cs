using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameObjectEvent : UnityEvent<GameObject> { }

public enum CollisionFilterMode
{
    disabled = 0,
    only = 1,
    ignore = 2,
}

public class CollisionEventSource : MonoBehaviour
{
    public CollisionFilterMode filterMode = CollisionFilterMode.disabled;
    public string tagFilter = "";

    public GameObject ignore;

    public GameObjectEvent enterEvent = new GameObjectEvent();
    public GameObjectEvent exitEvent = new GameObjectEvent();

    private void OnTriggerEnter(Collider other) {
        // Debug.Log($"{gameObject.name} OnTriggerEnter with {other.gameObject.name}");
        if (allowTag(other.tag)) enterEvent.Invoke(other.gameObject);
    }

    private void OnTriggerExit(Collider other) {
        // Debug.Log($"{gameObject.name} OnTriggerExit with {other.gameObject.name}");
        if (allowTag(other.tag)) exitEvent.Invoke(other.gameObject);
    }

    private bool allowTag(string tag) {
        if (filterMode == CollisionFilterMode.disabled) return true;
        else if (filterMode == CollisionFilterMode.ignore && tag != tagFilter) return true;
        else if (filterMode == CollisionFilterMode.only && tag == tagFilter) return true;
        else return false;
    }
}
