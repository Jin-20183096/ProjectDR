using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Evnt", menuName = "ScrObj/EventData")]

public class EventData : ScriptableObject
{
    public enum EventType
    { No, Battle }

    public EventModule Event;

    [Serializable]
    public class EventModule
    {
        public string Name;     //이벤트 이름
        public EventType Type;  //이벤트 타입
    }
}
