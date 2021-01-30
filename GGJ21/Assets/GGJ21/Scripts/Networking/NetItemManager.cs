using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
public class NetItemManager : MonoBehaviour
{
    public NetHealthEvent serverHealthEvent = new NetHealthEvent();
    public NetHealth lastServerHealth = null;

    public NetItemsListEvent sessionNetItemsEvent = new NetItemsListEvent();
    public NetItemList lastSessionNetItems = null;

    public UnityEvent sessionNetItemsAddEvent = new UnityEvent();

    public NetUserEvent sessionNetUserEvent = new NetUserEvent();
    public NetUser lastNetUser = null;

    public string serverAddress = "http://localhost:3000";
    public long serverStatus = -1;

    public void GetServerHealth() {
        StartCoroutine(getServerHealthRoutine());
    }

    /// <summary>Finds a lit of lost items from the server for the player to find</summary>
    public void GetSessionNetItems() {
        StartCoroutine(getSessionLostNetItemsRoutine());
    }

    /// <summary>Updates or adds found items on the server</summary>
    public void AddSessionNetItems(NetItemList items) {
        // TODO: Send a list of net items for other players to find
        // Note: If an item already exists with the GUID, ignore (race)
        StartCoroutine(addSessionFoundNetItemsRoutine(items));
    }
    
    public void GetNetUser() {

    }

    public void AddNetUser(string name) {

    }

    private void setServerHealth(NetHealth value) {
        lastServerHealth = value;
        serverHealthEvent.Invoke(value);
    }

    private void setSessionNetItems(NetItemList items) {
        lastSessionNetItems = items;
        sessionNetItemsEvent.Invoke(items);
    }

    private IEnumerator getServerHealthRoutine() {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(serverAddress)) {
            yield return webRequest.SendWebRequest();
            NetHealth value = handleWebRequest<NetHealth>(webRequest);
            if (value != null) {
                value.code = webRequest.responseCode;
                setServerHealth(value);
            }
        }
    }

    private IEnumerator getSessionLostNetItemsRoutine() {
        using (UnityWebRequest webRequest = UnityWebRequest.Get($"{serverAddress}/items/lost")) {
            yield return webRequest.SendWebRequest();
            NetItemList value = handleWebRequest<NetItemList>(webRequest);
            if (value != null) setSessionNetItems(value);
        }
    }

    private IEnumerator addSessionFoundNetItemsRoutine(NetItemList items) {
        string postData = JsonUtility.ToJson(items);
        using (UnityWebRequest webRequest = UnityWebRequest.Post($"{serverAddress}/items/found", postData)) {
            yield return webRequest.SendWebRequest();
            handleEmptyWebRequest(webRequest);
            sessionNetItemsAddEvent.Invoke();
        }
    }

    private void handleEmptyWebRequest(UnityWebRequest req) {
        if (req.result == UnityWebRequest.Result.ConnectionError) {
            setServerHealth(new NetHealth(req.error, req.responseCode));
        }
    }

    private T handleWebRequest<T>(UnityWebRequest req) where T : class {
        if (req.result == UnityWebRequest.Result.ConnectionError) {
            setServerHealth(new NetHealth(req.error, req.responseCode));
            return null;
        } else {
            string bodyText = req.downloadHandler.text;
            T value = JsonUtility.FromJson<T>(bodyText);
            return value;
        }
    }

}
