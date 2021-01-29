using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetItem
{
    [SerializeField] public string item;
    [SerializeField] public string room;
    [SerializeField] public string guid;

    public NetItem(string item, string room, string guid = "") {
        this.item = item;
        this.room = room;
        this.guid = guid;
    }
}

[System.Serializable]
public class NetItemList
{
    [SerializeField] public List<NetItem> items;

    public NetItemList(List<NetItem> items) {
        this.items = items;
    }
}