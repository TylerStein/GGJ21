using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class BoolEvent : UnityEvent<bool> { };

public class GameManager : MonoBehaviour
{
    public bool isPaused = false;
    public bool isMuted = false;

    public BoolEvent pausedEvent = new BoolEvent();
    public BoolEvent mutedEvent = new BoolEvent();

    public NetItemManager netItemManager;
    public bool isOnlineMode = false;

    public int onlinePostGameSceneIndex = 3;
    public int offlinePostGameSceneIndex = 3;
    public int mainMenuSceneIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        netItemManager.serverHealthEvent.AddListener(OnServerHealthEvent);
        if (isOnlineMode) netItemManager.GetServerHealth();
    }

    private void OnDestroy() {
        netItemManager.serverHealthEvent.RemoveListener(OnServerHealthEvent);
    }

    // Update is called once per frame
    void OnServerHealthEvent(NetHealth health)
    {
        Debug.Log($"Server Health Change (status = {netItemManager.serverStatus})");
    }

    public void TogglePause() {
        isPaused = !isPaused;
        pausedEvent.Invoke(isPaused);
    }

    public void ToggleMute() {
        isMuted = !isMuted;
        mutedEvent.Invoke(isMuted);
    }

    public void QuitToMain() {
        SceneManager.LoadScene(mainMenuSceneIndex);
    }

    public void CompleteGame() {
        SceneManager.LoadScene(isOnlineMode ? onlinePostGameSceneIndex : offlinePostGameSceneIndex);
    }
}
