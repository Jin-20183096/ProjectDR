using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Evnt", menuName = "ScrObj/EventData")]

public class EventData : ScriptableObject
{
    public enum EventType
    { No, Battle }

    public enum CheckStat { STR, INT, DEX, AGI, CON, WIL, LUC } //힘, 지능, 손재주, 민첩, 건강, 의지, 행운(D6)


    public EventModule Event;

    [Serializable]
    public class EventModule
    {
        public string Name;     //이벤트 이름
        public EventType Type;  //이벤트 타입

        [TextArea(3, 5)]
        public string StairLog; //이벤트 시작 로그

        public List<EventAction> ActList;   //이벤트 행동 목록

        public RuntimeAnimatorController EventObj_Anima;    //이벤트 오브젝트의 애니메이션 컨트롤러

        public ItemData[] Item; //보상 아이템 목록
    }

    [Serializable]
    public class EventAction
    {
        [TextArea(3, 5)]
        public string Name; //이벤트 행동명
    }
}
