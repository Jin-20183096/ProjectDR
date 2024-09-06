using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScrObj/ItemData")]  //커스텀 메뉴 추가

public class ItemData : ScriptableObject
{
    public enum ItemType { Weapon, Head, Body, Leg, SubWp, Accessory }

    public enum ArmorMaterial { No, Cloth, Leather, Metal }

    [Header("# Main Info")]
    [SerializeField]
    private Sprite _icon;    //아이템 아이콘
    public Sprite Icon { get { return _icon; } }

    [SerializeField]
    [TextArea(1, 3)]
    private string _name;   //아이템 이름
    public string Name { get { return _name; } }

    [SerializeField]
    private ItemType _type; //아이템 타입
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
        public BtlActData[] NormalAtk_Arr;  //기본 공격 행동 목록

        public BtlActData[] AtkAct_Arr;     //공격 행동 목록

        public BtlActData[] DefAct_Arr;     //방어 행동 목록

        public BtlActData[] DgeAct_Arr;     //회피 행동 목록

        public BtlActData[] TacAct_Arr;     //전술 행동 목록
    }

    [Serializable]
    public class ArmorClass
    {
        public ArmorMaterial Material;  //방어구 재질
    }
}
