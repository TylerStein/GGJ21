using UnityEngine.Events;

[System.Serializable]
public class NetItemsListEvent : UnityEvent<NetItemList> { }

[System.Serializable]
public class NetMetaUsersEvent : UnityEvent<NetMetaUsers> { }

[System.Serializable]
public class NetHealthEvent : UnityEvent<NetHealth> { }

[System.Serializable]
public class NetUserEvent : UnityEvent<NetUser> { }