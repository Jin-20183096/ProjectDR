using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Evnt", menuName = "ScrObj/EventData")]

public class EventData : ScriptableObject
{
    public enum EventType   //Battle Ÿ�� ���� ���� �̺�Ʈ�� enum���� �׻� �� �ڿ� ��ġ���Ѿ� ��
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
        public string Name;     //�̺�Ʈ �̸�
        public EventType Type;  //�̺�Ʈ Ÿ��

        public RuntimeAnimatorController EventObj_Anima;    //�̺�Ʈ ������Ʈ�� �ִϸ��̼� ��Ʈ�ѷ�

        [TextArea(3, 5)]
        public string StartLog; //�̺�Ʈ ���� �α�

        public List<EventAction> ActList;   //�̺�Ʈ �ൿ ���

        public EventResult[] Result;  //�̺�Ʈ ��� ���

        public ItemData[] Item;     //�̺�Ʈ���� �����ϴ� ������ ���(�ش� �̺�Ʈ ����� ������ ����� �����Ǿ� ���� ���� ��� ���)
    }

    [Serializable]
    public class EventAction
    {
        [TextArea(3, 5)]
        public string Name; //�̺�Ʈ �ൿ��

        public bool IsDiceCheck;    //�ֻ��� üũ �ൿ ����
        public ICreature.Stats CheckStat;     //(�ֻ��� üũ �ൿ�̸�) �ֻ��� üũ ���

        public int[] Result_Success;    //�ൿ ���� �� ��� �ε��� ���
        public int[] Result_Fail;       //�ൿ ���� �� ��� �ε��� ���
    }

    [Serializable]
    public class EventResult
    {
        [TextArea(3, 5)]
        public string Log;    //��� �α�

        public ResultType[] Type;   //�̺�Ʈ ��� ���� ���

        [Header("# Type: ActAdd")]
        public EventAction[] NewAct;    //�߰� �ൿ ���

        [Header("# Type: Exp")]
        public int Exp;                 //�̺�Ʈ���� ȹ���ϴ� �ּ� ����ġ

        [Header("# Type: Item")]
        public ItemData[] Item;         //���� ������ ���

        //���� ���
        //����� ���

        [Header("# Type: Btl")]
        public EnemyData[] Enemy;       //���� �� ���
    }
}
