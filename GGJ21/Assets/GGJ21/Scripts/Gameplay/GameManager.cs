using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public NetItemManager netItemManager;
    public bool connectOnStart = false;

    // Start is called before the first frame update
    void Start()
    {
        netItemManager.serverHealthEvent.AddListener(OnServerHealthEvent);
        if (connectOnStart) netItemManager.GetServerHealth();
    }

    private void OnDestroy() {
        netItemManager.serverHealthEvent.RemoveListener(OnServerHealthEvent);
    }

    // Update is called once per frame
    void OnServerHealthEvent(NetHealth health)
    {
        Debug.Log($"Server Health Change (status = {netItemManager.serverStatus})");
    }
}
