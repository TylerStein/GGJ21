using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public abstract class AttackHandler : MonoBehaviour
{
    public virtual void OnAttack(AttackSource source) {
        Debug.Log("Attacked by attack source " + source.other.name);
    }
}
