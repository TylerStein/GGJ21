using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetUser
{
    [SerializeField] public string name;
    [SerializeField] public int score;
    [SerializeField] public string countryCode;
    [SerializeField] public string country;
    [SerializeField] public string ip;

    public NetUser(string name, int score = 0, string countryCode = "", string country = "", string ip = "") {
        this.name = name;
        this.score = score;
        this.countryCode = countryCode;
        this.country = country;
        this.ip = ip;
    }
}
