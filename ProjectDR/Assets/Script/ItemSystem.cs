using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static PlayerSystem;
using static SingleToneCanvas;
using Random = UnityEngine.Random;

public class ItemSystem : MonoBehaviour
{
    public static ItemSystem ItemSys = null;

    public enum ItemSlotType { Equip, Inventory, Reward }
    public enum ItemOptionType { Stat, Action, Ability }

    [Serializable]
    private class ItemClass
    {
        public ItemData Data;

        //스탯
        public ICreature.Stats Stat1;
        public int Stat1_Value;
        public int[] Stat1_Arr;
        public ICreature.Stats Stat2;
        public int Stat2_Value;
        public int[] Stat2_Arr;

        //행동
        public ICreature.BtlActClass BtlAct1 = null;
        public ICreature.BtlActClass BtlAct2 = null;
    }

    [SerializeField]
    private ItemSlot[] _slot_equip;         //장비 슬롯 (0: 무기 / 1: 머리 / 2: 상의 / 3: 하의 / 4: 보조 / 5: 목걸이 / 6&7: 반지)
    [SerializeField]
    private ItemClass[] _itemClass_equip;   //장비 슬롯의 아이템 클래스들

    [SerializeField]
    private ItemSlot[] _slot_inventory;         //인벤토리 슬롯 (0 ~ 11)
    [SerializeField]
    private ItemClass[] _itemClass_inventory;    //인벤토리 슬롯의 아이템 클래스들

    [SerializeField]
    private ItemSlot[] _slot_reward;            //보상 슬롯
    [SerializeField]
    private ItemClass[] _itemClass_reward;      //보상 슬롯의 아이템 클래스들

    [SerializeField]
    private ItemTooltip _itemTooltip;       //아이템 툴팁
    [SerializeField]
    private ItemTooltip _itemTooltip_equip; //장착 중인 아이템 툴팁

    [SerializeField]
    private GameObject _dragIcon;      //드래그 아이콘
    private Image _img_dragIcon;       //드래그 아이콘 이미지
    private GameObject _dragCursor;    //드래그 중 커서

    private ItemClass _dragClass;       //드래그한 아이템의 클래스
    private ItemSlot _dragSlot;         //드래그한 아이템의 슬롯
    private ItemClass[] _dragGroup;     //드래그한 슬롯이 포함된 배열
    private int _dragIndex;             //드래그한 아이템의 배열 내 인덱스

    private ItemClass _dropClass;       //드롭한 슬롯의 클래스
    private ItemSlot _dropSlot;         //드롭한 슬롯
    private ItemClass[] _dropGroup;     //드롭한 슬롯이 포함된 배열
    private int _dropIndex;             //드롭한 슬롯의 배열 내 인덱스

    [SerializeField]
    private GameObject _dragScreen;     //드래그 중 출력되는 화면

    [SerializeField]
    private bool _isOn_inventory;       //인벤토리창 활성화 여부
    public bool ON_INVENTORY
    {
        set { _isOn_inventory = value; }
    }
    private bool _isOn_reward;          //보상창 활성화 여부
    public bool ON_REWARD
    {
        set { _isOn_reward = value; }
    }

    //아이템 생성시 활용되는 변수
    private int _cost = 10;     //아이템 생성에 사용하는 코스트

    private int _needCost_stat = 2;     //스탯 추가 시 필요 코스트
    private int _needCost_hp = 1;       //HP 스탯 1 상승의 필요 코스트 
    private int _needCost_ac = 3;       //방어도 스탯 1 상승의 필요 코스트
    private int _needCost_re = 2;       //재굴림 스탯 1 상승의 필요 코스트

    private int _needCost_btlAct = 4;   //행동 추가 시 필요 코스트

    [SerializeField]
    private Sprite _spr_noItem;    //아이템 없는 슬롯의 스프라이트
    [SerializeField]
    private Sprite[] _spr_equipItemType;    //아이템 없는 장비 슬롯의 스프라이트
    [SerializeField]
    private Sprite[] _spr_dragItemType;     //드래그 장비와 동일 타입의 장비 슬롯 강조 스프라이트

    private ItemClass _tooltip_itemClass;    //툴팁 표시할 아이템 클래스
    private ItemSlot _tooltip_itemSlot;    //툴팁 표시할 아이템 슬롯

    [SerializeField]
    private ItemData[] _test_item;

    void Awake()
    {
        if (ItemSys)
        {
            DestroyImmediate(gameObject);
            return;
        }
        else
        {
            ItemSys = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        _img_dragIcon = _dragIcon.GetComponent<Image>();
        _dragCursor = _dragIcon.transform.GetChild(0).gameObject;

        Create_Weapon(_test_item[0], ItemSlotType.Inventory, 0);
    }

    public bool Get_PlayerArmed()   //플레이어 무기 장비 여부 반환
    {
        return _itemClass_equip[0].Data != null;
    }

    private void Update()
    {
        if (STCanvas.ITEM_DRAG)     //아이템 드래그 중일 때
        {
            Vector2 pos = Input.mousePosition;  //마우스 포인터 위치로 드래그 아이콘 이동

            _dragIcon.transform.position = pos + new Vector2(8, -8);
        }
    }

    public void Reward_Clear()      //보상창 아이템 초기화
    {
        for (int i = 0; i < _slot_reward.Length; i++)
        {
            ItemClear(ItemSlotType.Reward, i);
        }
    }

    public void ItemClear(ItemSlotType slot, int index)
    {
        switch (slot)
        {
            case ItemSlotType.Equip:
                _itemClass_equip[index] = new ItemClass() { };
                _slot_equip[index].EXIST = false;       //아이템 제거 후 그 슬롯의 아이템 존재 여부 FALSE
                break;
            case ItemSlotType.Inventory:
                _itemClass_inventory[index] = new ItemClass() { };
                _slot_inventory[index].EXIST = false;   //아이템 제거 후 그 슬롯의 아이템 존재 여부 FALSE
                break;
            case ItemSlotType.Reward:
                _itemClass_reward[index] = new ItemClass() { };
                _slot_reward[index].EXIST = false;      //아이템 제거 후 그 슬롯의 아이템 존재 여부 FALSE
                break;
        }
    }

    public void Reward_Item(ItemData data, int index)
    {
        //머리, 상의, 하의 방어구면
        if ((int)data.Type >= (int)ItemData.ItemType.Head && (int)data.Type <= (int)ItemData.ItemType.Leg)
        {
            //create 방어구 함수
        }
        else
        {
            switch (data.Type)
            {
                case ItemData.ItemType.Weapon:
                    //create 무기 함수
                    break;
            }
        }
    }

    //create 무기
    public void Create_Weapon(ItemData data, ItemSlotType slot, int index)
    {
        //스탯1
        var stat1 = ICreature.Stats.No; //무기 스탯1
        var stat1_value = 0;            //  일반 스탯: 정수 변수
        int[] stat1_arr = { };          //  행동 스탯: 정수 배열
        //스탯2
        var stat2 = ICreature.Stats.No; //무기 스탯2
        var stat2_value = 0;            //  일반 스탯: 정수 변수
        int[] stat2_arr = { };          //  행동 스탯: 정수 배열
        //행동1
        BtlActData btlAct1 = null;
        ICreature.Stats btlAct1_stat = ICreature.Stats.No;  //행동1 스탯
        //행동2
        BtlActData btlAct2 = null;
        ICreature.Stats btlAct2_stat = ICreature.Stats.No;  //행동2 스탯
        //능력
        //

        //각 옵션에 부여된 코스트량
        int cost_stat1 = 0;
        int cost_stat2 = 0;
        int cost_btlAct2 = 0;
        int cost_ability = 0;

        //생성 방식에 따라 옵션 값 생성
        //1. 옵션 생성에 필요한 코스트 설정
        //2. 데이터의 공격행동 목록에서 행동 하나를 무작위로 선택해 추가. 코스트 소모.
        //   그 행동의 보정스탯을 스탯1로 추가. 코스트 소모 
        //3. 보유한 코스트를 소모해서, 현재 무기에 생성될 스탯, 행동, 특성에 코스트를 부여함.
        //4) 모든 코스트를 소모한 뒤, 각 옵션의 세부정보를 부여받은 코스트에 따라 설정
        //5. 모든 옵션 설정이 끝나면, 아이템 생성

        //1. 옵션 생성에 필요한 코스트 설정
        int cost = _cost;   //임시 코스트

        //2. 데이터의 공격행동 목록에서 행동 하나를 무작위로 선택해 추가. 코스트 소모.
        btlAct1 = data.Action.AtkAct_Arr[Random.Range(0, data.Action.AtkAct_Arr.Length)];
        btlAct1_stat = btlAct1.Stats_Arr[Random.Range(0, btlAct1.Stats_Arr.Length)];
        cost -= _needCost_btlAct;
        //   그 행동의 보정스탯을 스탯1로 추가. 코스트 소모 
        stat1 = btlAct1_stat;
        stat1_arr = new int[] { 0, 0, 0, 0, 0, 0 };
        cost_stat1 = _needCost_stat;
        cost -= cost_stat1;

        //3. 보유한 코스트를 소모해서, 현재 무기에 생성될 스탯, 행동, 특성에 코스트를 부여함.
        List<ItemOptionType> option_list = new List<ItemOptionType>()
            { ItemOptionType.Stat, ItemOptionType.Action, ItemOptionType.Ability };

        while (cost > 0)
        {
            var option = option_list[Random.Range(0, option_list.Count)];
            var use_cost = 0;

            switch (option)
            {
                case ItemOptionType.Stat:   //스탯
                    if (stat2 == ICreature.Stats.No)    //스탯2가 없는 경우
                    {
                        //스탯2를 생성하고 코스트 소모
                        stat2 = (ICreature.Stats)Random.Range(1, (int)ICreature.Stats.RE_STR);  //재굴림을 제외한 모든 스탯

                        if (stat2 != stat1) //스탯2가 스탯1과 다를 때
                        {
                            if (stat2 < ICreature.Stats.HP) //행동 스탯인 경우
                                stat2_arr = new int[] { 0, 0, 0, 0, 0, 0 }; //스탯 배열 초기화

                            use_cost = _needCost_stat;  //스탯 생성을 위한 코스트 소모
                        }
                        else
                        {   //스탯2와 스탯1이 같을 경우
                            stat2 = ICreature.Stats.No; //스탯2를 만들지 않고, 스탯1과 통합

                            //스탯 생성을 위해 소모할 코스트의 1/3만큼 코스트 부여
                            use_cost = _needCost_stat / 2;
                            cost_stat1 += use_cost;
                        }
                    }
                    else    //스탯1과 스탯2가 둘 다 존재할 경우
                    {
                        use_cost = Random.Range(1, 4);  //무작위 코스트

                        if (Random.value < 0.5f)    //스탯1 또는 스탯2
                            cost_stat1 += use_cost;
                        else
                            cost_stat2 += use_cost;
                    }
                    break;
                case ItemOptionType.Action:
                    if (btlAct2 == null)    //행동2가 없는 이유
                    {
                        cost_btlAct2 += _needCost_btlAct;
                        use_cost = _needCost_btlAct;
                    }
                    else
                        use_cost = 1;
                    break;
                case ItemOptionType.Ability:
                    //특성 생성과 소모 코스트를 설정
                    break;
            }

            cost -= use_cost;
        }

        //4) 모든 코스트를 소모한 뒤, 각 옵션의 세부정보를 부여받은 코스트에 따라 설정
        if (stat1 != ICreature.Stats.No)
        {
            if (cost_stat1 > 0) //스탯 1 코스트가 1 이상이면
            {
                if (stat1 < ICreature.Stats.HP) //스탯 1이 행동 스탯일 때
                {
                    for (int i = cost_stat1; i > 0; i--)    //해당 스탯의 무작위 위치에 스탯 1씩 증가
                        stat1_arr[Random.Range(0, stat1_arr.Length)] += 1;
                }
                else
                {
                    stat1_value = Cost_To_StatValue(stat1, cost_stat1); //스탯에 따라 지정된 비율만큼 코스트를 수치로 변환

                    if (stat1_value == 0)   //수치가 0인 경우
                        stat1 = ICreature.Stats.No; //스탯 제거
                }
            }
            else    //스탯1 코스트가 0이면
            {
                //스탯 제거
                stat1 = ICreature.Stats.No;
                stat1_value = 0;
                stat1_arr = new int[] { };
            }
        }

        if (stat2 != ICreature.Stats.No)
        {
            if (cost_stat2 > 0) //스탯 2 코스트가 1 이상이면
            {
                if (stat2 < ICreature.Stats.HP) //스탯 2가 행동 스탯일 때
                {
                    for (int i = cost_stat2; i > 0; i--)    //해당 스탯의 무작위 위치에 스탯 1씩 증가
                        stat2_arr[Random.Range(0, stat2_arr.Length)] += 1;
                }
                else
                {
                    stat2_value = Cost_To_StatValue(stat2, cost_stat2); //스탯에 따라 지정된 비율만큼 코스트를 수치로 변환

                    if (stat2_value == 0)   //수치가 0인 경우
                        stat2 = ICreature.Stats.No; //스탯 제거
                }
            }
            else    //스탯1 코스트가 0이면
            {
                //스탯 제거
                stat2 = ICreature.Stats.No;
                stat2_value = 0;
                stat2_arr = new int[] { };
            }
        }

        if (cost_btlAct2 >= _needCost_btlAct)   //행동2 생성에 필요한 만큼 코스트가 있으면
        {
            //이 무기가 보유 중인 행동 타입 리스트 생성
            List<BtlActData.ActionType> actTypeList = new List<BtlActData.ActionType>();

            if (data.Action.AtkAct_Arr.Length > 0) actTypeList.Add(BtlActData.ActionType.Atk);
            if (data.Action.DefAct_Arr.Length > 0) actTypeList.Add(BtlActData.ActionType.Def);
            if (data.Action.DgeAct_Arr.Length > 0) actTypeList.Add(BtlActData.ActionType.Dge);
            if (data.Action.TacAct_Arr.Length > 0) actTypeList.Add(BtlActData.ActionType.Tac);

            if (actTypeList.Count > 0)  //보유한 행동 타입이 1개 이상일 때
            {
                //무작위 행동 타입을 하나 선택
                var makeActType = actTypeList[Random.Range(0, actTypeList.Count)];
                actTypeList.Remove(makeActType);    //선택한 타입은 리스트에서 제거

                BtlActData[] makeActArr = null;

                switch (makeActType)
                {
                    case BtlActData.ActionType.Atk:
                        makeActArr = data.Action.AtkAct_Arr;
                        break;
                    case BtlActData.ActionType.Def:
                        makeActArr = data.Action.DefAct_Arr;
                        break;
                    case BtlActData.ActionType.Dge:
                        makeActArr = data.Action.DgeAct_Arr;
                        break;
                    case BtlActData.ActionType.Tac:
                        makeActArr = data.Action.TacAct_Arr;
                        break;
                }

                BtlActData makeAct = makeActArr[Random.Range(0, makeActArr.Length)];    //그 타입의 무작위 행동 선택

                if (makeAct == btlAct1) //선택한 행동이 행동1과 동일한 경우
                {
                    Debug.Log("행동2가 행동1과 같음");

                    //다른 행동 타입을 선택하기
                    if (actTypeList.Count > 0)  //보유한 행동 타입이 1개 이상일 때
                    {
                        switch (actTypeList[Random.Range(0, actTypeList.Count)])
                        {
                            case BtlActData.ActionType.Atk:
                                makeActArr = data.Action.AtkAct_Arr;
                                break;
                            case BtlActData.ActionType.Def:
                                makeActArr = data.Action.DefAct_Arr;
                                break;
                            case BtlActData.ActionType.Dge:
                                makeActArr = data.Action.DgeAct_Arr;
                                break;
                            case BtlActData.ActionType.Tac:
                                makeActArr = data.Action.TacAct_Arr;
                                break;
                        }

                        makeAct = makeActArr[Random.Range(0, makeActArr.Length)];   //그 타입의 무작위 행동 선택

                        //행동2 생성
                        btlAct2 = makeAct;
                        btlAct2_stat = btlAct2.Stats_Arr[Random.Range(0, btlAct2.Stats_Arr.Length)];
                    }
                    else    //더 이상 선택할 수 있는 행동 타입이 없으면
                    {
                        Debug.Log("더 추가 할 수 있는 행동이 없음");
                        //행동을 추가하지 않고 종료
                        btlAct2 = null;
                        btlAct2_stat = ICreature.Stats.No;
                    }
                }
            }
        }
        else    //행동2 생성을 위한 코스트가 없으면
        {
            //행동 제거
            btlAct2 = null;
            btlAct2_stat = ICreature.Stats.No;
        }

        //5. 모든 옵션 설정이 끝나면, 아이템 생성
        ItemClass[] createGroup = null;
        ItemSlot[] createSlot = null;

        switch (slot)
        {
            case ItemSlotType.Equip:
                createGroup = _itemClass_equip;
                createSlot = _slot_equip;
                break;
            case ItemSlotType.Inventory:
                createGroup = _itemClass_inventory;
                createSlot = _slot_inventory;
                break;
            case ItemSlotType.Reward:
                createGroup = _itemClass_reward;
                createSlot = _slot_reward;
                break;
        }

        createGroup[index] = new ItemClass()
        {
            //아이템 데이터
            Data = data,
            //스탯1 & 스탯1 수치
            Stat1 = stat1,
            Stat1_Value = stat1_value,
            Stat1_Arr = stat1_arr.ToArray(),
            //스탯2 & 스탯2 수치
            Stat2 = stat2,
            Stat2_Value = stat2_value,
            Stat2_Arr = stat2_arr.ToArray(),
            //행동1
            BtlAct1 = new ICreature.BtlActClass()
            {
                Data = btlAct1,
                Stat = btlAct1_stat
            },
            //행동2
            BtlAct2 = new ICreature.BtlActClass()
            {
                Data = btlAct2,
                Stat = btlAct2_stat
            }
        };

        createSlot[index].EXIST = true; //아이템 생성 후 그 슬롯 아이템 존재 여부 true
    }


    //create 방어구

    int Cost_To_StatValue(ICreature.Stats stat, int cost)   //코스트를 소모해 비율에 맞는 스탯값으로 반환
    {
        switch (stat)
        {
            case ICreature.Stats.HP:
                return cost / _needCost_hp;
            case ICreature.Stats.AC:
                return cost / _needCost_ac;
            default:    //HP, 방어도가 아닌 스탯 => 재굴림
                return cost / _needCost_re;
        }
    }

    //장비창 아이템의 아이콘 표시
    public void Set_EquipIcon()
    {
        for (int i = 0; i < _slot_equip.Length; i++)
        {
            if (_itemClass_equip[i].Data != null)
                _slot_equip[i].Change_SlotIcon(_itemClass_equip[i].Data.Icon);
            else
            {
                //아이템 드래그 중인 아이템과 동일한 타입의 슬롯의 경우, 강조 표시
                if (STCanvas.ITEM_DRAG && (_dragClass.Data.Type == _slot_equip[i].EQUIP_TYPE))
                    _slot_equip[i].Change_SlotIcon(_spr_dragItemType[(int)_slot_equip[i].EQUIP_TYPE]);  //빈 장비 슬롯 (강조)
                else
                    _slot_equip[i].Change_SlotIcon(_spr_equipItemType[(int)_slot_equip[i].EQUIP_TYPE]); //빈 장비 슬롯 (일반)
            }
        }
    }

    //인벤토리창 아이템의 아이콘 표시
    public void Set_InventoryIcon()
    {
        for (int i = 0; i < _slot_inventory.Length; i++)
        {
            if (_itemClass_inventory[i].Data != null)
                _slot_inventory[i].Change_SlotIcon(_itemClass_inventory[i].Data.Icon);
            else
                _slot_inventory[i].Change_SlotIcon(_spr_noItem);
        }
    }

    //보상창 아이템의 아이콘 표시
    public void Set_RewardIcon()
    {
        for (int i = 0; i < _slot_reward.Length; i++)
        {
            if (_itemClass_reward[i].Data != null)
                _slot_reward[i].Change_SlotIcon(_itemClass_reward[i].Data.Icon);
            else
                _slot_reward[i].Change_SlotIcon(_spr_noItem);
        }
    }

    //아이템 툴팁 설정
    public void ItemTooltip_On(ItemSlotType slot, int index)
    {
        switch (slot)
        {
            case ItemSlotType.Equip:
                _tooltip_itemClass = _itemClass_equip[index];
                _tooltip_itemSlot = _slot_equip[index];
                break;
            case ItemSlotType.Inventory:
                _tooltip_itemClass = _itemClass_inventory[index];
                _tooltip_itemSlot = _slot_inventory[index];
                break;
            case ItemSlotType.Reward:
                _tooltip_itemClass = _itemClass_reward[index];
                _tooltip_itemSlot = _slot_reward[index];
                break;
        }

        var item = _tooltip_itemClass;

        if (item != null)
        {
            _itemTooltip.Set_TooltipOutScreen();            //출력할 툴팁 화면 밖 좌표로 이동
            _itemTooltip.ItemTooltip_On();                  //아이템 툴팁 활성화
            _itemTooltip.Change_Name(item.Data.Name);       //아이템 이름
            _itemTooltip.Change_ItemType(item.Data.Type);   //아이템 타입 아이콘

            if (item.Stat1 != ICreature.Stats.No)   //스탯1이 존재할 때
            {
                if (item.Stat1 <= ICreature.Stats.WIL)
                    _itemTooltip.Change_ActionStat1(item.Stat1, item.Stat1_Arr);    //행동 스탯
                else
                    _itemTooltip.Change_Stat1(item.Stat1, item.Stat1_Value);        //기본 스탯
            }

            if (item.Stat2 != ICreature.Stats.No)   //스탯2가 존재할 때
            {
                if (item.Stat2 <= ICreature.Stats.WIL)
                    _itemTooltip.Change_ActionStat2(item.Stat2, item.Stat2_Arr);    //행동 스탯
                else
                    _itemTooltip.Change_Stat2(item.Stat2, item.Stat2_Value);        //기본 스탯
            }

            if (item.BtlAct1 != null)
                _itemTooltip.Change_Action1(item.BtlAct1);  //행동1
            if (item.BtlAct2 != null)
                _itemTooltip.Change_Action2(item.BtlAct2);  //행동2

            //능력이 존재할 때
            //툴팁에 능력 표기

            //만약 해당 아이템이 장비 슬롯 밖의 장비 아이템인 경우
            //동일한 타입의 장비를 착용 중이라면, 장착 중 아이템의 툴팁 준비
            if (slot != ItemSlotType.Equip &&
                item.Data.Type <= ItemData.ItemType.Ring &&
                _itemClass_equip[(int)item.Data.Type].Data != null)
            {
                //장착 중 아이템 표시
            }
        }
    }

    //장착 중 아이템 툴팁 설정
    public void ItemTooltip_Equip_On(ItemData.ItemType type)
    {
        var item = _itemClass_equip[(int)type];

        if (item != null)
        {
            _itemTooltip_equip.Set_TooltipOutScreen();
            _itemTooltip_equip.ItemTooltip_On();        //장비 아이템 툴팁 활성화
            _itemTooltip_equip.Change_Name(item.Data.Name); //아이템 툴팁 활성화
            _itemTooltip_equip.Change_ItemType(item.Data.Type); //아이템 타입 아이콘

            if (item.Stat1 != ICreature.Stats.No)   //스탯1이 존재할 때
            {
                if (item.Stat1 <= ICreature.Stats.WIL)  //행동 스탯의 경우
                    _itemTooltip_equip.Change_ActionStat1(item.Stat1, item.Stat1_Arr);  //행동 스탯
                else
                    _itemTooltip_equip.Change_Stat1(item.Stat1, item.Stat1_Value);      //기본 스탯
            }

            if (item.Stat2 != ICreature.Stats.No)   //스탯2이 존재할 때
            {
                if (item.Stat2 <= ICreature.Stats.WIL)  //행동 스탯의 경우
                    _itemTooltip_equip.Change_ActionStat2(item.Stat2, item.Stat2_Arr);  //행동 스탯
                else
                    _itemTooltip_equip.Change_Stat2(item.Stat2, item.Stat2_Value);      //기본 스탯
            }

            if (item.BtlAct1 != null)
                _itemTooltip_equip.Change_Action1(item.BtlAct1);    //행동 1
            if (item.BtlAct2 != null)
                _itemTooltip_equip.Change_Action2(item.BtlAct2);    //행동 2
        }

        //능력이 존재할 때
        //툴팁에 능력 표기
    }

    //툴팁 출력 위치 설정
    public void Set_ItemTooltipPosition()
    {
        if (STCanvas.ITEM_DRAG == false)
        {
            var item = _tooltip_itemClass;

            if (item != null)
            {
                _itemTooltip.Set_TooltipPosition(_tooltip_itemSlot.transform.position);          //아이템 툴팁 위치 조정
                _itemTooltip_equip.Set_TooltipPosition(_tooltip_itemSlot.transform.position);    //장착중인 아이템 툴팁 위치 조정
            }

            _itemTooltip.Refresh_Layout();  //툴팁 레이아웃 새로고침
        }
    }

    //툴팁 Off
    public void ItemTooltip_Off()
    {
        if (STCanvas.ITEM_DRAG == false)
        {
            _itemTooltip.ItemTooltip_Off();
            _itemTooltip_equip.ItemTooltip_Off();
        }
    }

    //아이템 우클릭
    public void Item_RightClick(ItemSlotType slot, int index)
    {
        ItemClass[] tempGroup = null;
        ItemSlot[] tempSlot = null;
        int equip_index;

        switch (slot)
        {
            case ItemSlotType.Inventory:
                tempGroup = _itemClass_inventory;
                tempSlot = _slot_inventory;
                break;
            case ItemSlotType.Reward:
                tempGroup = _itemClass_reward;
                tempSlot = _slot_reward;
                break;
        }

        if ((int)tempGroup[index].Data.Type <= (int)ItemData.ItemType.Ring) //우클릭한 아이템이 장비 아이템이면
        {
            ItemTooltip_Off();  //아이템 툴팁 Off
            tempSlot[index].StopCoroutine("Print_ItemTooltip");

            //아이템이 장착될 슬롯 인덱스 지정
            if (tempGroup[index].Data.Type != ItemData.ItemType.Ring)   //우클릭한 장비가 반지가 아닌 장비인 경우
                equip_index = (int)tempGroup[index].Data.Type;  //장비 타입에 따른 슬롯 위치 지정
            else    //우클릭한 장비가 반지인 경우
            {
                if (_itemClass_equip[(int)ItemData.ItemType.Ring].Data = null)  //1번쨰 반지 슬롯이 비어있으면, 그 슬롯 위치로
                    equip_index = (int)ItemData.ItemType.Ring;
                else
                {   //1번째 반지 슬롯에 반지가 있을 경우
                    if (_itemClass_equip[(int)ItemData.ItemType.Ring + 1].Data = null)  //2번째 반지 슬롯이 비어있으면, 그 슬롯 위치로
                        equip_index = (int)ItemData.ItemType.Ring + 1;
                    else                                                                //1, 2번째 반지 슬롯 모두 반지가 있을 경우
                    {
                        //장착할 슬롯을 고르는 창을 등장시켜야 함.
                        //우선은 1번째 슬롯으로 고정
                        equip_index = (int)ItemData.ItemType.Ring;
                    }
                }
            }
            //기존 아이템 해제
            if (_itemClass_equip[equip_index].Data != null) //장착할 장비 슬롯에 이미 아이템이 있으면
            {
                //한손 무기와 양손 무기의 경우, 한손무기+보조무기도 같이 해제해야 함. 이러한 케이스 처리는 추후 추가 구현하기
                Change_EquipItem(false, _itemClass_equip[equip_index]); //
            }
            //아이템 장착
            Change_EquipItem(true, tempGroup[index]);

            //아이템 위치 스왑
            //(한손 무기 + 보조무기 <-> 양손 무기의 처리는 추후 추가 구현)
            Swap_Drag_To_Drop(tempGroup, tempSlot[index], index,
                            _itemClass_equip, _slot_equip[equip_index], equip_index);

            //장착 후 해당 슬롯에 아이템이 있으면, 툴팁 출력 코루틴 실행
            if (tempSlot[index].EXIST)
            {
                ItemTooltip_On(slot, index);  //툴팁이 아이템 정보를 미리 반영해서, 툴팁 크기 변화를 보이지 않게 하기
                tempSlot[index].StartCoroutine("Print_ItemTooltip");
            }
        }
    }

    //드래그 시작
    public void Drag_Start(ItemSlotType slot, int index)
    {
        STCanvas.Set_Drag(true);
        STCanvas.Set_ItemDrag(true);

        //드래그한 아이템의 슬롯 정보 기록
        switch (slot)
        {
            case ItemSlotType.Equip:
                _dragClass = _itemClass_equip[index];
                _dragSlot = _slot_equip[index];
                _dragGroup = _itemClass_equip;
                break;
            case ItemSlotType.Inventory:
                _dragClass = _itemClass_inventory[index];
                _dragSlot = _slot_inventory[index];
                _dragGroup = _itemClass_inventory;
                break;
            case ItemSlotType.Reward:
                _dragClass = _itemClass_reward[index];
                _dragSlot = _slot_reward[index];
                _dragGroup = _itemClass_reward;
                break;
        }
        _dragIndex = index;

        Sprite tempSpr = _dragClass.Data.Icon;  //드래그 아이콘에 사용될 스프라이트
        _dragSlot.Change_SlotIconAlpha(0.5f);   //드래그한 아이템 슬롯의 투명도 0.5
        _dragSlot.DRAGGING = true;

        //아이템 툴팁 Off
        _itemTooltip.ItemTooltip_Off();
        _itemTooltip_equip.ItemTooltip_Off();

        //드래그 아이콘을 표시
        _img_dragIcon.enabled = true;
        _img_dragIcon.sprite = tempSpr;
        _dragCursor.SetActive(true);

        //드래그 중 스크린 On

        if (_isOn_inventory)   //장비창 활성화 시
        {
            for (int i = 0; i < _slot_equip.Length; i++)
            {
                //현재 드래그 중인 아이템과 동일한 타입의 장비슬롯이 비어있으면
                if (_itemClass_equip[i].Data == null && _slot_equip[i].EQUIP_TYPE == _dragClass.Data.Type)
                    _slot_equip[i].Change_SlotIcon(_spr_dragItemType[(int)_slot_equip[i].EQUIP_TYPE]);     //장비 슬롯 강조
            }
        }
    }

    //드래그 드롭
    public void Drag_Drop(ItemSlotType slot, int index)
    {
        //드래그 드롭 스롯 정보 기록
        switch (slot)
        {
            case ItemSlotType.Equip:
                _dropSlot = _slot_equip[index];
                _dropGroup = _itemClass_equip;
                break;
            case ItemSlotType.Inventory:
                _dropSlot = _slot_inventory[index];
                _dropGroup = _itemClass_inventory;
                break;
            case ItemSlotType.Reward:
                _dropSlot = _slot_reward[index];
                _dropGroup = _itemClass_reward;
                break;
        }
        _dropIndex = index;
        _dropClass = _dropGroup[_dropIndex];

        //드래그 드롭 되는 슬롯 중 하나라도 장비 슬롯이 있을 때
        if (_dragSlot.SLOT_TYPE == ItemSlotType.Equip || _dropSlot.SLOT_TYPE == ItemSlotType.Equip)
        {
            //장비 슬롯의 아이템 드래그했고
            if (_dragSlot.SLOT_TYPE == ItemSlotType.Equip)
            {
                //드래그 드롭한 슬롯이 장비 슬롯인 경우
                if ((_dropSlot.SLOT_TYPE == ItemSlotType.Equip))
                {
                    //<드래그 아이템 타입>과 <드롭 아이템 타입>이 동일할 때, 장착 중인 아이템 간 위치 교체 (스탯 변화 X, 아이템 유무 여부 무시)
                    if (_dragClass.Data.Type == _dropClass.Data.Type)
                        Swap_Drag_To_Drop(_dragGroup, _dragSlot, _dragIndex,
                                        _dropGroup, _dropSlot, _dropIndex);
                }
                else    //드래그 드롭한 슬롯이 인벤토리/보상 슬롯일 때
                {
                    if (_dropClass.Data != null)    //드롭한 슬롯에 아이템이 있다면
                    {
                        //<드래그 아이템 타입>과 <드롭 아이템 타입>이 동일할 때, 장착 중인 장비를 인벤토리/전리품의 아이템과 교체
                        if (_dragSlot.EQUIP_TYPE == _dropClass.Data.Type)
                        {
                            Change_EquipItem(false, _dragClass);    //드래그한 장비해제
                            Change_EquipItem(true, _dropClass);     //드롭한 슬롯에 있는 장비 장착

                            Swap_Drag_To_Drop(_dragGroup, _dragSlot, _dragIndex,
                                            _dropGroup, _dropSlot, _dropIndex);
                        }
                    }
                    else    //인벤토리/보상 슬롯이 비어있다면, 장착 중인 장비를 그 슬롯에 해제
                    {
                        Change_EquipItem(false, _dragClass);    //드래그한 장비 해제

                        Swap_Drag_To_Drop(_dragGroup, _dragSlot, _dragIndex,
                                        _dropGroup, _dropSlot, _dropIndex);
                    }
                }
            }
            else    //인벤토리/보상을 드래그했으므로, 정황상 dropSlot은 장비 슬롯
            {
                //드래그한 장비의 타입과 드롭한 슬롯의 타입이 같을 때
                //인벤토리/보상 슬롯의 장비를 장착 및 교체
                if (_dragClass.Data.Type == _dropSlot.EQUIP_TYPE)
                {
                    if (_dropClass.Data != null)       //장비 슬롯에 아이템이 있을 경우
                        Change_EquipItem(false, _dropClass);

                    Change_EquipItem(true, _dragClass); //드래그한 장비 장착

                    Swap_Drag_To_Drop(_dragGroup, _dragSlot, _dragIndex,
                                    _dropGroup, _dropSlot, _dropIndex);
                }
            }
        }
        else    //아닐 경우, 아이템 Swap에 제약 없음 (스탯 변화 X)
        {
            Swap_Drag_To_Drop(_dragGroup, _dragSlot, _dragIndex,
                            _dropGroup, _dropSlot, _dropIndex);
        }

        //드롭 후 해당 슬롯에 아이템이 있으면, 툴팁 출력 코루틴 실행
        if (_dropSlot.EXIST)
        {
            ItemTooltip_On(slot, index);    //툴팁이 아이템 정보를 미리 반영해서, 툴팁 크기 변화를 보이지 않게 하기

            _dropSlot.StopCoroutine("Print_ItemTooltip");
            _dropSlot.StartCoroutine("Print_ItemTooltip");
        }

        Drag_End(); //드롭 했으므로, 드래그 종료 함수 출력
    }

    //드래그 종료
    void Drag_End()
    {
        if (STCanvas.ITEM_DRAG)    //아이템 드래그 중일 때
        {
            //드래그 중인 슬롯의 아이콘 기본 상태로 변경
            _dragSlot.DRAGGING = false;

            switch (_dragSlot.SLOT_TYPE)
            {
                case ItemSlotType.Equip:
                    if (_isOn_inventory)
                        _dragSlot.Change_SlotIconAlpha(1f);
                    break;
                case ItemSlotType.Inventory:
                    if (_isOn_inventory)
                        _dragSlot.Change_SlotIconAlpha(1f);
                    break;
                case ItemSlotType.Reward:
                    if (_isOn_reward)
                        _dragSlot.Change_SlotIconAlpha(1f);
                    break;
            }

            //드래그 커서 OFF
            _dragCursor.SetActive(false);
            //드래그 아이콘 이미지 OFF
            _img_dragIcon.enabled = false;

            if (_isOn_inventory)   //장비창 활성화 시
            {
                for (int i = 0; i < _slot_equip.Length; i++)
                {
                    //현재 드래그 중인 아이템과 동일한 타입의 장비슬롯이 비어있으면
                    if (_itemClass_equip[i].Data == null && _slot_equip[i].EQUIP_TYPE == _dragClass.Data.Type)
                        _slot_equip[i].Change_SlotIcon(_spr_equipItemType[(int)_slot_equip[i].EQUIP_TYPE]);    //장비 슬롯 강조 해제
                }
            }

            //드래그 슬롯, 드롭 슬롯 정보 초기화
            _dragClass = null;
            _dragSlot = null;
            _dropClass = null;
            _dropSlot = null;

            //드래그 스크린 off

            //드래그 상태 false
            STCanvas.Set_Drag(false);
            STCanvas.Set_ItemDrag(false);
        }
    }

    //<A 슬롯의 아이템 클래스, 슬롯 정보>와 <B 슬롯의 아이템 클래스, 슬롯정보>를 교체
    void Swap_Drag_To_Drop(ItemClass[] A_group, ItemSlot A_slot, int A_index,    //드래그한 아이템 슬롯의 정보
                            ItemClass[] B_group, ItemSlot B_slot, int B_index)   //드롭한 아이템 슬롯의 정보
    {
        //Debug.Log("" + A_slot.SLOT_TYPE + ": " + A_index + " => " + B_slot.SLOT_TYPE + ": " + B_index);

        //아이템 클래스 교체
        var temp_class = B_group[B_index];      //드롭될 슬롯의 정보를 temp에 저장

        B_group[B_index] = A_group[A_index];    //드롭한 슬롯에 드래그한 아이템의 클래스를 덮기

        A_group[A_index] = temp_class;          //드래그한 아이템의 슬롯에 temp에 저장된 클래스를 덮기

        //드래그한 슬롯, 드롭한 슬롯의 아이템 존재여부를 판단하고 갱신
        A_slot.EXIST = (A_group[A_index].Data != null);
        B_slot.EXIST = (B_group[B_index].Data != null);

        //드래그한 슬롯의 타입을 판단해, 해당 슬롯의 UI가 활성화 중이면, 아이콘 변경
        bool isOn_A_ui = false;     //드래그한 슬롯의 UI 활성화 여부
        bool isOn_B_ui = false;     //드롭한 슬롯의 UI 활성화 여부
        bool is_A_equip = (A_slot.SLOT_TYPE == ItemSlotType.Equip); //드래그한 슬롯이 장비 슬롯인지 여부 (빈 슬롯의 아이콘이 결정되므로, 필요)

        switch (A_slot.SLOT_TYPE)
        {
            case ItemSlotType.Equip:
                isOn_A_ui = _isOn_inventory;
                break;
            case ItemSlotType.Inventory:
                isOn_A_ui = _isOn_inventory;
                break;
            case ItemSlotType.Reward:
                isOn_A_ui = _isOn_reward;
                break;
        }

        switch (B_slot.SLOT_TYPE)
        {
            case ItemSlotType.Equip:
                isOn_B_ui = _isOn_inventory;
                break;
            case ItemSlotType.Inventory:
                isOn_B_ui = _isOn_inventory;
                break;
            case ItemSlotType.Reward:
                isOn_B_ui = _isOn_reward;
                break;
        }

        if (isOn_A_ui)  //드래그한 슬롯의 UI가 활성화중이면
        {
            if (A_group[A_index].Data != null)  //그 슬롯에 아이템이 있을 경우
                A_slot.Change_SlotIcon(A_group[A_index].Data.Icon);     //그 슬롯 아이템의 아이콘으로 변경
            else    //그 슬롯에 아이템이 없을 경우
            {
                if (is_A_equip) //빈 슬롯이 장비 슬롯이면, 
                    A_slot.Change_SlotIcon(_spr_equipItemType[(int)A_slot.EQUIP_TYPE]);    //인덱스에 맞는 빈 장비 슬롯 아이콘으로 변경
                else
                    A_slot.Change_SlotIcon(_spr_noItem);                                   //빈 슬롯 아이콘으로 변경
            }
        }

        if (isOn_B_ui)  //드롭한 슬롯의 UI가 활성화 중이면
        {
            //드롭 슬롯의 아이콘을 아이템 데이터를 참고해서 변경
            B_slot.Change_SlotIcon(B_group[B_index].Data.Icon);
        }
    }

    //장비 장착/해제
    void Change_EquipItem(bool isEquip, ItemClass itemClass)    //isEquip에 따라 장착 또는 해제
    {
        if (itemClass.Stat1 != ICreature.Stats.No)  //스탯 1이 존재할 경우
            Manage_EquipStat(isEquip, itemClass.Stat1, itemClass.Stat1_Arr, itemClass.Stat1_Value); //isEquip에 따라 스탯1 추가/제거
        if (itemClass.Stat2 != ICreature.Stats.No)  //스탯 2가 존재할 경우
            Manage_EquipStat(isEquip, itemClass.Stat2, itemClass.Stat2_Arr, itemClass.Stat2_Value); //isEquip에 따라 스탯2 추가/제거

        if (itemClass.BtlAct1 != null)   //전투행동1이 존재할 경우
            Manage_BtlAct(isEquip, itemClass.BtlAct1);   //전투행동1 추가/제거
        if (itemClass.BtlAct2 != null)   //전투행동2가 존재할 경우
            Manage_BtlAct(isEquip, itemClass.BtlAct2);   //전투행동2 추가/제거

        //특성 존재 시 추가/제거

        if (isEquip)
            PlayerSys.EquipItem(itemClass.Data);    //아이템 장착
        else
            PlayerSys.UnequipItem(itemClass.Data);  //아이템 제거
    }

    //아이템 교체시 증감되는 스탯 관리
    void Manage_EquipStat(bool plus, ICreature.Stats stat, int[] stat_arr, int stat_value)  //bool값에 따라 해당 스탯값을 증감
    {
        if (stat <= ICreature.Stats.WIL)    //행동스탯인 경우
            PlayerSys.Change_ActionStat(plus, stat, stat_arr);
        else    //일반스탯인 경우
        {
            switch (stat)
            {
                case ICreature.Stats.HP:
                    PlayerSys.Change_HpMax(plus, stat_value);
                    break;
                case ICreature.Stats.AC:
                    PlayerSys.Change_AC(plus, stat_value);
                    break;
                default:
                    PlayerSys.Change_Reroll(plus, stat, stat_value);
                    break;
            }
        }
    }

    //아이템 교체시 변경되는 행동 관리
    void Manage_BtlAct(bool plus, ICreature.BtlActClass btlAct)     //bool값에 따라 전투행동을 추가/제거
    {
        if (btlAct.Data != null)
            PlayerSys.Change_BtlAct(plus, btlAct);
    }
}
