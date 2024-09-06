using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScrObj/ItemData")]  //Ŀ���� �޴� �߰�

public class ItemData : ScriptableObject
{
    public enum ItemType { Weapon, Head, Body, Leg, SubWp, Accessory }

    public enum ArmorMaterial { No, Cloth, Leather, Metal }

    [Header("# Main Info")]
    [SerializeField]
    private Sprite _icon;    //������ ������
    public Sprite Icon { get { return _icon; } }

    [SerializeField]
    [TextArea(1, 3)]
    private string _name;   //������ �̸�
    public string Name { get { return _name; } }

    [SerializeField]
    private ItemType _type; //������ Ÿ��
    public ItemType Type { get { return _type; } }

    [Header("# BtlAct -----")]
    public BtlActSet BtlAct;

    [Header("# Trait -----")]
    public TraitData[] Trait_Arr;

    [Header("# Armor -----")]
    public ArmorClass Armor;

    [Header("# SpriteAnimation -----")]
    public AnimationClip Clip_Idle;
    public AnimationClip[] Clip_Atk1;
    public AnimationClip[] Clip_Atk2;
    public AnimationClip[] Clip_Atk3;

    [Serializable]
    public class BtlActSet
    {
        public BtlActData[] NormalAtk_Arr;  //�⺻ ���� �ൿ ���

        public BtlActData[] AtkAct_Arr;     //���� �ൿ ���

        public BtlActData[] DefAct_Arr;     //��� �ൿ ���

        public BtlActData[] DgeAct_Arr;     //ȸ�� �ൿ ���

        public BtlActData[] TacAct_Arr;     //���� �ൿ ���
    }

    [Serializable]
    public class ArmorClass
    {
        public ArmorMaterial Material;  //�� ����
    }
}
