using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetHealth
{
    [SerializeField] public string status;
    [SerializeField] public long code;

    public NetHealth(string status, long code) {
        this.status = status;
        this.code = code;
    }
}
