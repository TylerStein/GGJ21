using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class InteractionHandler : MonoBehaviour
{
    public string interactiveTag = "Interactive";

    // Start is called before the first frame update
    void Start()
    {
        if (tag != interactiveTag) Debug.LogWarning($"InteractiveController is on an object without the {interactiveTag} tag", this);
    }

    public void OnInteract(PlayerController playerController) {
        Debug.Log("Interacted with by player!");
    }
}
