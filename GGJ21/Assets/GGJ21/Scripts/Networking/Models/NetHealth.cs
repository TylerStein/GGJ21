using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetHealth
{
    [SerializeField] public long status;

    public NetHealth(long status) {
        this.status = status;
    }
}
