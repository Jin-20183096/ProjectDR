using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Evnt", menuName = "ScrObj/EventData")]

public class EventData : ScriptableObject
{
    public enum EventType   //Battle 타입 등의 전투 이벤트는 enum에서 항상 맨 뒤에 배치시켜야 함
    { No, Event, Battle, Boss }

    public enum CheckRule
    {
        No, Total_Up, Total_Down, Total_Between, Total_Odd, Total_Even, 
        Each_Up, Each_Down, Each_Between, Each_Odd, Each_Even
    }

    public enum ResultType
    {
        No,
        ActRemove, ActAdd,
        Exp, Item, Buff, Debuff,
        Btl,
        EvntEnd
    }

    public EventModule Event;

    [Serializable]
    public class EventModule
    {
        public string Name;     //이벤트 이름
        public EventType Type;  //이벤트 타입

        public RuntimeAnimatorController EventObj_Anima;    //이벤트 오브젝트의 애니메이션 컨트롤러

        [TextArea(3, 5)]
        public string StartLog; //이벤트 시작 로그

        public List<EventAction> ActList;   //이벤트 행동 목록

        public EventResult[] Result;  //이벤트 결과 목록

        public ItemData[] Item;     //이벤트에서 등장하는 아이템 목록(해당 이벤트 결과에 아이템 목록이 배정되어 있지 않을 경우 사용)
    }

    [Serializable]
    public class EventAction
    {
        [TextArea(3, 5)]
        public string Name; //이벤트 행동명

        public bool IsDiceCheck;    //주사위 체크 행동 여부
        public ICreature.Stats CheckStat;     //(주사위 체크 행동이면) 주사위 체크 방식

        public int[] Result_Success;    //행동 성공 시 결과 인덱스 목록
        public int[] Result_Fail;       //행동 실패 시 결과 인덱스 목록
    }

    [Serializable]
    public class EventResult
    {
        [TextArea(3, 5)]
        public string Log;    //결과 로그

        public ResultType[] Type;   //이벤트 결과 종류 목록

        [Header("# Type: ActAdd")]
        public EventAction[] NewAct;    //추가 행동 목록

        [Header("# Type: Exp")]
        public int Exp;                 //이벤트에서 획득하는 최소 경험치

        [Header("# Type: Item")]
        public ItemData[] Item;         //등장 아이템 목록

        //버프 목록
        //디버프 목록

        [Header("# Type: Btl")]
        public EnemyData[] Enemy;       //등장 적 목록
    }
}
