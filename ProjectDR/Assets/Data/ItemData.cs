using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScrObj/ItemData")]

public class ItemData : MonoBehaviour
{
    public enum ItemType { Weapon, Head, Body, Leg, SubWp, Amulet, Ring }

    public enum ArmorMaterial { No, Cloth, Leather, Metal }

    [Header("# Main Info")]
    [SerializeField]
    private Sprite _icon;    //������ ������
    public Sprite Icon { get { return _icon; } }

    [SerializeField]
    private string _name;   //������ �̸�
    public string Name { get { return _name; } }

    [SerializeField]
    private ItemType _type; //������ Ÿ��
    public ItemType Type { get { return _type; } }

    [Header("# Weapon -----")]
    public WeaponClass Weapon;

    [Header("# Armor -----")]
    public ArmorClass Armor;

    [Header("# SpriteAnimation -----")]
    public AnimationClip Clip_Idle;
    public AnimationClip[] Clip_Atk1;

    [Serializable]
    public class WeaponClass
    {
        public BtlActData[] AtkAct_Arr; //���� �ൿ ���

        public BtlActData[] DefAct_Arr; //��� �ൿ ���

        public BtlActData[] DgeAct_Arr; //ȸ�� �ൿ ���

        public BtlActData[] TacAct_Arr; //���� �ൿ ���
    }

    [Serializable]
    public class ArmorClass
    {
        public ArmorMaterial Material;  //�� ����
    }
}
