using UnityEngine;


[System.Serializable]
public class NetMetaUsers
{
    [SerializeField] public int players;
    [SerializeField] public int lostItems;
    [SerializeField] public int foundItems;

    public NetMetaUsers(int players, int lostItems, int foundItems) {
        this.players = players;
        this.lostItems = lostItems;
        this.foundItems = foundItems;
    }
}
