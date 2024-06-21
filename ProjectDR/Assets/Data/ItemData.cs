using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScrObj/ItemData")]

public class ItemData : MonoBehaviour
{
    public enum ItemType { Weapon, Head, Body, Leg, SubWp, Amulet, Ring }

    public enum ArmorMaterial { No, Cloth, Leather, Metal }

    [Header("# Main Info")]
    [SerializeField]
    private Sprite _icon;    //아이템 아이콘
    public Sprite Icon { get { return _icon; } }

    [SerializeField]
    private string _name;   //아이템 이름
    public string Name { get { return _name; } }

    [SerializeField]
    private ItemType _type; //아이템 타입
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
        public BtlActData[] AtkAct_Arr; //공격 행동 목록

        public BtlActData[] DefAct_Arr; //방어 행동 목록

        public BtlActData[] DgeAct_Arr; //회피 행동 목록

        public BtlActData[] TacAct_Arr; //전술 행동 목록
    }

    [Serializable]
    public class ArmorClass
    {
        public ArmorMaterial Material;  //방어구 재질
    }
}
