using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Evnt", menuName = "ScrObj/EventData")]

public class EventData : ScriptableObject
{
    public enum EventType
    { No, Battle }

    public enum CheckStat { STR, INT, DEX, AGI, CON, WIL, LUC } //��, ����, ������, ��ø, �ǰ�, ����, ���(D6)


    public EventModule Event;

    [Serializable]
    public class EventModule
    {
        public string Name;     //�̺�Ʈ �̸�
        public EventType Type;  //�̺�Ʈ Ÿ��

        [TextArea(3, 5)]
        public string StairLog; //�̺�Ʈ ���� �α�

        public List<EventAction> ActList;   //�̺�Ʈ �ൿ ���

        public RuntimeAnimatorController EventObj_Anima;    //�̺�Ʈ ������Ʈ�� �ִϸ��̼� ��Ʈ�ѷ�

        public ItemData[] Item; //���� ������ ���
    }

    [Serializable]
    public class EventAction
    {
        [TextArea(3, 5)]
        public string Name; //�̺�Ʈ �ൿ��
    }
}
