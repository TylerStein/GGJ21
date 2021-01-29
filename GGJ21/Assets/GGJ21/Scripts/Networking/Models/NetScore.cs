using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetScore
{
    [SerializeField] public int score;
    [SerializeField] public string status;

    public NetScore(int score, string status) {
        this.score = score;
        this.status = status;
    }
}
