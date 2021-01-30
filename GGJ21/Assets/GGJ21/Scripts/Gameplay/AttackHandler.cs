using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class AttackHandler : MonoBehaviour
{
    public string attackableTag = "Attackable";

    // Start is called before the first frame update
    void Start() {
        if (tag != attackableTag) Debug.LogWarning($"AttackHandler is on an object without the {attackableTag} tag", this);
    }

    public void OnAttack(AttackSource source) {
        Debug.Log("Attacked by attack source " + source.other.name);
    }
}
