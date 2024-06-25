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

        //����
        public ICreature.Stats Stat1;
        public int Stat1_Value;
        public int[] Stat1_Arr;
        public ICreature.Stats Stat2;
        public int Stat2_Value;
        public int[] Stat2_Arr;

        //�ൿ
        public ICreature.BtlActClass BtlAct1 = null;
        public ICreature.BtlActClass BtlAct2 = null;
    }

    [SerializeField]
    private ItemSlot[] _slot_equip;         //��� ���� (0: ���� / 1: �Ӹ� / 2: ���� / 3: ���� / 4: ���� / 5: ����� / 6&7: ����)
    [SerializeField]
    private ItemClass[] _itemClass_equip;   //��� ������ ������ Ŭ������

    [SerializeField]
    private ItemSlot[] _slot_inventory;         //�κ��丮 ���� (0 ~ 11)
    [SerializeField]
    private ItemClass[] _itemClass_inventory;    //�κ��丮 ������ ������ Ŭ������

    [SerializeField]
    private ItemSlot[] _slot_reward;            //���� ����
    [SerializeField]
    private ItemClass[] _itemClass_reward;      //���� ������ ������ Ŭ������

    [SerializeField]
    private ItemTooltip _itemTooltip;       //������ ����
    [SerializeField]
    private ItemTooltip _itemTooltip_equip; //���� ���� ������ ����

    [SerializeField]
    private GameObject _dragIcon;      //�巡�� ������
    private Image _img_dragIcon;       //�巡�� ������ �̹���
    private GameObject _dragCursor;    //�巡�� �� Ŀ��

    private ItemClass _dragClass;       //�巡���� �������� Ŭ����
    private ItemSlot _dragSlot;         //�巡���� �������� ����
    private ItemClass[] _dragGroup;     //�巡���� ������ ���Ե� �迭
    private int _dragIndex;             //�巡���� �������� �迭 �� �ε���

    private ItemClass _dropClass;       //����� ������ Ŭ����
    private ItemSlot _dropSlot;         //����� ����
    private ItemClass[] _dropGroup;     //����� ������ ���Ե� �迭
    private int _dropIndex;             //����� ������ �迭 �� �ε���

    [SerializeField]
    private GameObject _dragScreen;     //�巡�� �� ��µǴ� ȭ��

    [SerializeField]
    private bool _isOn_inventory;       //�κ��丮â Ȱ��ȭ ����
    public bool ON_INVENTORY
    {
        set { _isOn_inventory = value; }
    }
    private bool _isOn_reward;          //����â Ȱ��ȭ ����
    public bool ON_REWARD
    {
        set { _isOn_reward = value; }
    }

    //������ ������ Ȱ��Ǵ� ����
    private int _cost = 10;     //������ ������ ����ϴ� �ڽ�Ʈ

    private int _needCost_stat = 2;     //���� �߰� �� �ʿ� �ڽ�Ʈ
    private int _needCost_hp = 1;       //HP ���� 1 ����� �ʿ� �ڽ�Ʈ 
    private int _needCost_ac = 3;       //�� ���� 1 ����� �ʿ� �ڽ�Ʈ
    private int _needCost_re = 2;       //�籼�� ���� 1 ����� �ʿ� �ڽ�Ʈ

    private int _needCost_btlAct = 4;   //�ൿ �߰� �� �ʿ� �ڽ�Ʈ

    [SerializeField]
    private Sprite _spr_noItem;    //������ ���� ������ ��������Ʈ
    [SerializeField]
    private Sprite[] _spr_equipItemType;    //������ ���� ��� ������ ��������Ʈ
    [SerializeField]
    private Sprite[] _spr_dragItemType;     //�巡�� ���� ���� Ÿ���� ��� ���� ���� ��������Ʈ

    private ItemClass _tooltip_itemClass;    //���� ǥ���� ������ Ŭ����
    private ItemSlot _tooltip_itemSlot;    //���� ǥ���� ������ ����

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

    public bool Get_PlayerArmed()   //�÷��̾� ���� ��� ���� ��ȯ
    {
        return _itemClass_equip[0].Data != null;
    }

    private void Update()
    {
        if (STCanvas.ITEM_DRAG)     //������ �巡�� ���� ��
        {
            Vector2 pos = Input.mousePosition;  //���콺 ������ ��ġ�� �巡�� ������ �̵�

            _dragIcon.transform.position = pos + new Vector2(8, -8);
        }
    }

    public void Reward_Clear()      //����â ������ �ʱ�ȭ
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
                _slot_equip[index].EXIST = false;       //������ ���� �� �� ������ ������ ���� ���� FALSE
                break;
            case ItemSlotType.Inventory:
                _itemClass_inventory[index] = new ItemClass() { };
                _slot_inventory[index].EXIST = false;   //������ ���� �� �� ������ ������ ���� ���� FALSE
                break;
            case ItemSlotType.Reward:
                _itemClass_reward[index] = new ItemClass() { };
                _slot_reward[index].EXIST = false;      //������ ���� �� �� ������ ������ ���� ���� FALSE
                break;
        }
    }

    public void Reward_Item(ItemData data, int index)
    {
        //�Ӹ�, ����, ���� ����
        if ((int)data.Type >= (int)ItemData.ItemType.Head && (int)data.Type <= (int)ItemData.ItemType.Leg)
        {
            //create �� �Լ�
        }
        else
        {
            switch (data.Type)
            {
                case ItemData.ItemType.Weapon:
                    //create ���� �Լ�
                    break;
            }
        }
    }

    //create ����
    public void Create_Weapon(ItemData data, ItemSlotType slot, int index)
    {
        //����1
        var stat1 = ICreature.Stats.No; //���� ����1
        var stat1_value = 0;            //  �Ϲ� ����: ���� ����
        int[] stat1_arr = { };          //  �ൿ ����: ���� �迭
        //����2
        var stat2 = ICreature.Stats.No; //���� ����2
        var stat2_value = 0;            //  �Ϲ� ����: ���� ����
        int[] stat2_arr = { };          //  �ൿ ����: ���� �迭
        //�ൿ1
        BtlActData btlAct1 = null;
        ICreature.Stats btlAct1_stat = ICreature.Stats.No;  //�ൿ1 ����
        //�ൿ2
        BtlActData btlAct2 = null;
        ICreature.Stats btlAct2_stat = ICreature.Stats.No;  //�ൿ2 ����
        //�ɷ�
        //

        //�� �ɼǿ� �ο��� �ڽ�Ʈ��
        int cost_stat1 = 0;
        int cost_stat2 = 0;
        int cost_btlAct2 = 0;
        int cost_ability = 0;

        //���� ��Ŀ� ���� �ɼ� �� ����
        //1. �ɼ� ������ �ʿ��� �ڽ�Ʈ ����
        //2. �������� �����ൿ ��Ͽ��� �ൿ �ϳ��� �������� ������ �߰�. �ڽ�Ʈ �Ҹ�.
        //   �� �ൿ�� ���������� ����1�� �߰�. �ڽ�Ʈ �Ҹ� 
        //3. ������ �ڽ�Ʈ�� �Ҹ��ؼ�, ���� ���⿡ ������ ����, �ൿ, Ư���� �ڽ�Ʈ�� �ο���.
        //4) ��� �ڽ�Ʈ�� �Ҹ��� ��, �� �ɼ��� ���������� �ο����� �ڽ�Ʈ�� ���� ����
        //5. ��� �ɼ� ������ ������, ������ ����

        //1. �ɼ� ������ �ʿ��� �ڽ�Ʈ ����
        int cost = _cost;   //�ӽ� �ڽ�Ʈ

        //2. �������� �����ൿ ��Ͽ��� �ൿ �ϳ��� �������� ������ �߰�. �ڽ�Ʈ �Ҹ�.
        btlAct1 = data.Action.AtkAct_Arr[Random.Range(0, data.Action.AtkAct_Arr.Length)];
        btlAct1_stat = btlAct1.Stats_Arr[Random.Range(0, btlAct1.Stats_Arr.Length)];
        cost -= _needCost_btlAct;
        //   �� �ൿ�� ���������� ����1�� �߰�. �ڽ�Ʈ �Ҹ� 
        stat1 = btlAct1_stat;
        stat1_arr = new int[] { 0, 0, 0, 0, 0, 0 };
        cost_stat1 = _needCost_stat;
        cost -= cost_stat1;

        //3. ������ �ڽ�Ʈ�� �Ҹ��ؼ�, ���� ���⿡ ������ ����, �ൿ, Ư���� �ڽ�Ʈ�� �ο���.
        List<ItemOptionType> option_list = new List<ItemOptionType>()
            { ItemOptionType.Stat, ItemOptionType.Action, ItemOptionType.Ability };

        while (cost > 0)
        {
            var option = option_list[Random.Range(0, option_list.Count)];
            var use_cost = 0;

            switch (option)
            {
                case ItemOptionType.Stat:   //����
                    if (stat2 == ICreature.Stats.No)    //����2�� ���� ���
                    {
                        //����2�� �����ϰ� �ڽ�Ʈ �Ҹ�
                        stat2 = (ICreature.Stats)Random.Range(1, (int)ICreature.Stats.RE_STR);  //�籼���� ������ ��� ����

                        if (stat2 != stat1) //����2�� ����1�� �ٸ� ��
                        {
                            if (stat2 < ICreature.Stats.HP) //�ൿ ������ ���
                                stat2_arr = new int[] { 0, 0, 0, 0, 0, 0 }; //���� �迭 �ʱ�ȭ

                            use_cost = _needCost_stat;  //���� ������ ���� �ڽ�Ʈ �Ҹ�
                        }
                        else
                        {   //����2�� ����1�� ���� ���
                            stat2 = ICreature.Stats.No; //����2�� ������ �ʰ�, ����1�� ����

                            //���� ������ ���� �Ҹ��� �ڽ�Ʈ�� 1/3��ŭ �ڽ�Ʈ �ο�
                            use_cost = _needCost_stat / 2;
                            cost_stat1 += use_cost;
                        }
                    }
                    else    //����1�� ����2�� �� �� ������ ���
                    {
                        use_cost = Random.Range(1, 4);  //������ �ڽ�Ʈ

                        if (Random.value < 0.5f)    //����1 �Ǵ� ����2
                            cost_stat1 += use_cost;
                        else
                            cost_stat2 += use_cost;
                    }
                    break;
                case ItemOptionType.Action:
                    if (btlAct2 == null)    //�ൿ2�� ���� ����
                    {
                        cost_btlAct2 += _needCost_btlAct;
                        use_cost = _needCost_btlAct;
                    }
                    else
                        use_cost = 1;
                    break;
                case ItemOptionType.Ability:
                    //Ư�� ������ �Ҹ� �ڽ�Ʈ�� ����
                    break;
            }

            cost -= use_cost;
        }

        //4) ��� �ڽ�Ʈ�� �Ҹ��� ��, �� �ɼ��� ���������� �ο����� �ڽ�Ʈ�� ���� ����
        if (stat1 != ICreature.Stats.No)
        {
            if (cost_stat1 > 0) //���� 1 �ڽ�Ʈ�� 1 �̻��̸�
            {
                if (stat1 < ICreature.Stats.HP) //���� 1�� �ൿ ������ ��
                {
                    for (int i = cost_stat1; i > 0; i--)    //�ش� ������ ������ ��ġ�� ���� 1�� ����
                        stat1_arr[Random.Range(0, stat1_arr.Length)] += 1;
                }
                else
                {
                    stat1_value = Cost_To_StatValue(stat1, cost_stat1); //���ȿ� ���� ������ ������ŭ �ڽ�Ʈ�� ��ġ�� ��ȯ

                    if (stat1_value == 0)   //��ġ�� 0�� ���
                        stat1 = ICreature.Stats.No; //���� ����
                }
            }
            else    //����1 �ڽ�Ʈ�� 0�̸�
            {
                //���� ����
                stat1 = ICreature.Stats.No;
                stat1_value = 0;
                stat1_arr = new int[] { };
            }
        }

        if (stat2 != ICreature.Stats.No)
        {
            if (cost_stat2 > 0) //���� 2 �ڽ�Ʈ�� 1 �̻��̸�
            {
                if (stat2 < ICreature.Stats.HP) //���� 2�� �ൿ ������ ��
                {
                    for (int i = cost_stat2; i > 0; i--)    //�ش� ������ ������ ��ġ�� ���� 1�� ����
                        stat2_arr[Random.Range(0, stat2_arr.Length)] += 1;
                }
                else
                {
                    stat2_value = Cost_To_StatValue(stat2, cost_stat2); //���ȿ� ���� ������ ������ŭ �ڽ�Ʈ�� ��ġ�� ��ȯ

                    if (stat2_value == 0)   //��ġ�� 0�� ���
                        stat2 = ICreature.Stats.No; //���� ����
                }
            }
            else    //����1 �ڽ�Ʈ�� 0�̸�
            {
                //���� ����
                stat2 = ICreature.Stats.No;
                stat2_value = 0;
                stat2_arr = new int[] { };
            }
        }

        if (cost_btlAct2 >= _needCost_btlAct)   //�ൿ2 ������ �ʿ��� ��ŭ �ڽ�Ʈ�� ������
        {
            //�� ���Ⱑ ���� ���� �ൿ Ÿ�� ����Ʈ ����
            List<BtlActData.ActionType> actTypeList = new List<BtlActData.ActionType>();

            if (data.Action.AtkAct_Arr.Length > 0) actTypeList.Add(BtlActData.ActionType.Atk);
            if (data.Action.DefAct_Arr.Length > 0) actTypeList.Add(BtlActData.ActionType.Def);
            if (data.Action.DgeAct_Arr.Length > 0) actTypeList.Add(BtlActData.ActionType.Dge);
            if (data.Action.TacAct_Arr.Length > 0) actTypeList.Add(BtlActData.ActionType.Tac);

            if (actTypeList.Count > 0)  //������ �ൿ Ÿ���� 1�� �̻��� ��
            {
                //������ �ൿ Ÿ���� �ϳ� ����
                var makeActType = actTypeList[Random.Range(0, actTypeList.Count)];
                actTypeList.Remove(makeActType);    //������ Ÿ���� ����Ʈ���� ����

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

                BtlActData makeAct = makeActArr[Random.Range(0, makeActArr.Length)];    //�� Ÿ���� ������ �ൿ ����

                if (makeAct == btlAct1) //������ �ൿ�� �ൿ1�� ������ ���
                {
                    Debug.Log("�ൿ2�� �ൿ1�� ����");

                    //�ٸ� �ൿ Ÿ���� �����ϱ�
                    if (actTypeList.Count > 0)  //������ �ൿ Ÿ���� 1�� �̻��� ��
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

                        makeAct = makeActArr[Random.Range(0, makeActArr.Length)];   //�� Ÿ���� ������ �ൿ ����

                        //�ൿ2 ����
                        btlAct2 = makeAct;
                        btlAct2_stat = btlAct2.Stats_Arr[Random.Range(0, btlAct2.Stats_Arr.Length)];
                    }
                    else    //�� �̻� ������ �� �ִ� �ൿ Ÿ���� ������
                    {
                        Debug.Log("�� �߰� �� �� �ִ� �ൿ�� ����");
                        //�ൿ�� �߰����� �ʰ� ����
                        btlAct2 = null;
                        btlAct2_stat = ICreature.Stats.No;
                    }
                }
            }
        }
        else    //�ൿ2 ������ ���� �ڽ�Ʈ�� ������
        {
            //�ൿ ����
            btlAct2 = null;
            btlAct2_stat = ICreature.Stats.No;
        }

        //5. ��� �ɼ� ������ ������, ������ ����
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
            //������ ������
            Data = data,
            //����1 & ����1 ��ġ
            Stat1 = stat1,
            Stat1_Value = stat1_value,
            Stat1_Arr = stat1_arr.ToArray(),
            //����2 & ����2 ��ġ
            Stat2 = stat2,
            Stat2_Value = stat2_value,
            Stat2_Arr = stat2_arr.ToArray(),
            //�ൿ1
            BtlAct1 = new ICreature.BtlActClass()
            {
                Data = btlAct1,
                Stat = btlAct1_stat
            },
            //�ൿ2
            BtlAct2 = new ICreature.BtlActClass()
            {
                Data = btlAct2,
                Stat = btlAct2_stat
            }
        };

        createSlot[index].EXIST = true; //������ ���� �� �� ���� ������ ���� ���� true
    }


    //create ��

    int Cost_To_StatValue(ICreature.Stats stat, int cost)   //�ڽ�Ʈ�� �Ҹ��� ������ �´� ���Ȱ����� ��ȯ
    {
        switch (stat)
        {
            case ICreature.Stats.HP:
                return cost / _needCost_hp;
            case ICreature.Stats.AC:
                return cost / _needCost_ac;
            default:    //HP, ���� �ƴ� ���� => �籼��
                return cost / _needCost_re;
        }
    }

    //���â �������� ������ ǥ��
    public void Set_EquipIcon()
    {
        for (int i = 0; i < _slot_equip.Length; i++)
        {
            if (_itemClass_equip[i].Data != null)
                _slot_equip[i].Change_SlotIcon(_itemClass_equip[i].Data.Icon);
            else
            {
                //������ �巡�� ���� �����۰� ������ Ÿ���� ������ ���, ���� ǥ��
                if (STCanvas.ITEM_DRAG && (_dragClass.Data.Type == _slot_equip[i].EQUIP_TYPE))
                    _slot_equip[i].Change_SlotIcon(_spr_dragItemType[(int)_slot_equip[i].EQUIP_TYPE]);  //�� ��� ���� (����)
                else
                    _slot_equip[i].Change_SlotIcon(_spr_equipItemType[(int)_slot_equip[i].EQUIP_TYPE]); //�� ��� ���� (�Ϲ�)
            }
        }
    }

    //�κ��丮â �������� ������ ǥ��
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

    //����â �������� ������ ǥ��
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

    //������ ���� ����
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
            _itemTooltip.Set_TooltipOutScreen();            //����� ���� ȭ�� �� ��ǥ�� �̵�
            _itemTooltip.ItemTooltip_On();                  //������ ���� Ȱ��ȭ
            _itemTooltip.Change_Name(item.Data.Name);       //������ �̸�
            _itemTooltip.Change_ItemType(item.Data.Type);   //������ Ÿ�� ������

            if (item.Stat1 != ICreature.Stats.No)   //����1�� ������ ��
            {
                if (item.Stat1 <= ICreature.Stats.WIL)
                    _itemTooltip.Change_ActionStat1(item.Stat1, item.Stat1_Arr);    //�ൿ ����
                else
                    _itemTooltip.Change_Stat1(item.Stat1, item.Stat1_Value);        //�⺻ ����
            }

            if (item.Stat2 != ICreature.Stats.No)   //����2�� ������ ��
            {
                if (item.Stat2 <= ICreature.Stats.WIL)
                    _itemTooltip.Change_ActionStat2(item.Stat2, item.Stat2_Arr);    //�ൿ ����
                else
                    _itemTooltip.Change_Stat2(item.Stat2, item.Stat2_Value);        //�⺻ ����
            }

            if (item.BtlAct1 != null)
                _itemTooltip.Change_Action1(item.BtlAct1);  //�ൿ1
            if (item.BtlAct2 != null)
                _itemTooltip.Change_Action2(item.BtlAct2);  //�ൿ2

            //�ɷ��� ������ ��
            //������ �ɷ� ǥ��

            //���� �ش� �������� ��� ���� ���� ��� �������� ���
            //������ Ÿ���� ��� ���� ���̶��, ���� �� �������� ���� �غ�
            if (slot != ItemSlotType.Equip &&
                item.Data.Type <= ItemData.ItemType.Ring &&
                _itemClass_equip[(int)item.Data.Type].Data != null)
            {
                //���� �� ������ ǥ��
            }
        }
    }

    //���� �� ������ ���� ����
    public void ItemTooltip_Equip_On(ItemData.ItemType type)
    {
        var item = _itemClass_equip[(int)type];

        if (item != null)
        {
            _itemTooltip_equip.Set_TooltipOutScreen();
            _itemTooltip_equip.ItemTooltip_On();        //��� ������ ���� Ȱ��ȭ
            _itemTooltip_equip.Change_Name(item.Data.Name); //������ ���� Ȱ��ȭ
            _itemTooltip_equip.Change_ItemType(item.Data.Type); //������ Ÿ�� ������

            if (item.Stat1 != ICreature.Stats.No)   //����1�� ������ ��
            {
                if (item.Stat1 <= ICreature.Stats.WIL)  //�ൿ ������ ���
                    _itemTooltip_equip.Change_ActionStat1(item.Stat1, item.Stat1_Arr);  //�ൿ ����
                else
                    _itemTooltip_equip.Change_Stat1(item.Stat1, item.Stat1_Value);      //�⺻ ����
            }

            if (item.Stat2 != ICreature.Stats.No)   //����2�� ������ ��
            {
                if (item.Stat2 <= ICreature.Stats.WIL)  //�ൿ ������ ���
                    _itemTooltip_equip.Change_ActionStat2(item.Stat2, item.Stat2_Arr);  //�ൿ ����
                else
                    _itemTooltip_equip.Change_Stat2(item.Stat2, item.Stat2_Value);      //�⺻ ����
            }

            if (item.BtlAct1 != null)
                _itemTooltip_equip.Change_Action1(item.BtlAct1);    //�ൿ 1
            if (item.BtlAct2 != null)
                _itemTooltip_equip.Change_Action2(item.BtlAct2);    //�ൿ 2
        }

        //�ɷ��� ������ ��
        //������ �ɷ� ǥ��
    }

    //���� ��� ��ġ ����
    public void Set_ItemTooltipPosition()
    {
        if (STCanvas.ITEM_DRAG == false)
        {
            var item = _tooltip_itemClass;

            if (item != null)
            {
                _itemTooltip.Set_TooltipPosition(_tooltip_itemSlot.transform.position);          //������ ���� ��ġ ����
                _itemTooltip_equip.Set_TooltipPosition(_tooltip_itemSlot.transform.position);    //�������� ������ ���� ��ġ ����
            }

            _itemTooltip.Refresh_Layout();  //���� ���̾ƿ� ���ΰ�ħ
        }
    }

    //���� Off
    public void ItemTooltip_Off()
    {
        if (STCanvas.ITEM_DRAG == false)
        {
            _itemTooltip.ItemTooltip_Off();
            _itemTooltip_equip.ItemTooltip_Off();
        }
    }

    //������ ��Ŭ��
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

        if ((int)tempGroup[index].Data.Type <= (int)ItemData.ItemType.Ring) //��Ŭ���� �������� ��� �������̸�
        {
            ItemTooltip_Off();  //������ ���� Off
            tempSlot[index].StopCoroutine("Print_ItemTooltip");

            //�������� ������ ���� �ε��� ����
            if (tempGroup[index].Data.Type != ItemData.ItemType.Ring)   //��Ŭ���� ��� ������ �ƴ� ����� ���
                equip_index = (int)tempGroup[index].Data.Type;  //��� Ÿ�Կ� ���� ���� ��ġ ����
            else    //��Ŭ���� ��� ������ ���
            {
                if (_itemClass_equip[(int)ItemData.ItemType.Ring].Data = null)  //1���� ���� ������ ���������, �� ���� ��ġ��
                    equip_index = (int)ItemData.ItemType.Ring;
                else
                {   //1��° ���� ���Կ� ������ ���� ���
                    if (_itemClass_equip[(int)ItemData.ItemType.Ring + 1].Data = null)  //2��° ���� ������ ���������, �� ���� ��ġ��
                        equip_index = (int)ItemData.ItemType.Ring + 1;
                    else                                                                //1, 2��° ���� ���� ��� ������ ���� ���
                    {
                        //������ ������ ���� â�� ������Ѿ� ��.
                        //�켱�� 1��° �������� ����
                        equip_index = (int)ItemData.ItemType.Ring;
                    }
                }
            }
            //���� ������ ����
            if (_itemClass_equip[equip_index].Data != null) //������ ��� ���Կ� �̹� �������� ������
            {
                //�Ѽ� ����� ��� ������ ���, �Ѽչ���+�������⵵ ���� �����ؾ� ��. �̷��� ���̽� ó���� ���� �߰� �����ϱ�
                Change_EquipItem(false, _itemClass_equip[equip_index]); //
            }
            //������ ����
            Change_EquipItem(true, tempGroup[index]);

            //������ ��ġ ����
            //(�Ѽ� ���� + �������� <-> ��� ������ ó���� ���� �߰� ����)
            Swap_Drag_To_Drop(tempGroup, tempSlot[index], index,
                            _itemClass_equip, _slot_equip[equip_index], equip_index);

            //���� �� �ش� ���Կ� �������� ������, ���� ��� �ڷ�ƾ ����
            if (tempSlot[index].EXIST)
            {
                ItemTooltip_On(slot, index);  //������ ������ ������ �̸� �ݿ��ؼ�, ���� ũ�� ��ȭ�� ������ �ʰ� �ϱ�
                tempSlot[index].StartCoroutine("Print_ItemTooltip");
            }
        }
    }

    //�巡�� ����
    public void Drag_Start(ItemSlotType slot, int index)
    {
        STCanvas.Set_Drag(true);
        STCanvas.Set_ItemDrag(true);

        //�巡���� �������� ���� ���� ���
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

        Sprite tempSpr = _dragClass.Data.Icon;  //�巡�� �����ܿ� ���� ��������Ʈ
        _dragSlot.Change_SlotIconAlpha(0.5f);   //�巡���� ������ ������ ���� 0.5
        _dragSlot.DRAGGING = true;

        //������ ���� Off
        _itemTooltip.ItemTooltip_Off();
        _itemTooltip_equip.ItemTooltip_Off();

        //�巡�� �������� ǥ��
        _img_dragIcon.enabled = true;
        _img_dragIcon.sprite = tempSpr;
        _dragCursor.SetActive(true);

        //�巡�� �� ��ũ�� On

        if (_isOn_inventory)   //���â Ȱ��ȭ ��
        {
            for (int i = 0; i < _slot_equip.Length; i++)
            {
                //���� �巡�� ���� �����۰� ������ Ÿ���� ��񽽷��� ���������
                if (_itemClass_equip[i].Data == null && _slot_equip[i].EQUIP_TYPE == _dragClass.Data.Type)
                    _slot_equip[i].Change_SlotIcon(_spr_dragItemType[(int)_slot_equip[i].EQUIP_TYPE]);     //��� ���� ����
            }
        }
    }

    //�巡�� ���
    public void Drag_Drop(ItemSlotType slot, int index)
    {
        //�巡�� ��� ���� ���� ���
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

        //�巡�� ��� �Ǵ� ���� �� �ϳ��� ��� ������ ���� ��
        if (_dragSlot.SLOT_TYPE == ItemSlotType.Equip || _dropSlot.SLOT_TYPE == ItemSlotType.Equip)
        {
            //��� ������ ������ �巡���߰�
            if (_dragSlot.SLOT_TYPE == ItemSlotType.Equip)
            {
                //�巡�� ����� ������ ��� ������ ���
                if ((_dropSlot.SLOT_TYPE == ItemSlotType.Equip))
                {
                    //<�巡�� ������ Ÿ��>�� <��� ������ Ÿ��>�� ������ ��, ���� ���� ������ �� ��ġ ��ü (���� ��ȭ X, ������ ���� ���� ����)
                    if (_dragClass.Data.Type == _dropClass.Data.Type)
                        Swap_Drag_To_Drop(_dragGroup, _dragSlot, _dragIndex,
                                        _dropGroup, _dropSlot, _dropIndex);
                }
                else    //�巡�� ����� ������ �κ��丮/���� ������ ��
                {
                    if (_dropClass.Data != null)    //����� ���Կ� �������� �ִٸ�
                    {
                        //<�巡�� ������ Ÿ��>�� <��� ������ Ÿ��>�� ������ ��, ���� ���� ��� �κ��丮/����ǰ�� �����۰� ��ü
                        if (_dragSlot.EQUIP_TYPE == _dropClass.Data.Type)
                        {
                            Change_EquipItem(false, _dragClass);    //�巡���� �������
                            Change_EquipItem(true, _dropClass);     //����� ���Կ� �ִ� ��� ����

                            Swap_Drag_To_Drop(_dragGroup, _dragSlot, _dragIndex,
                                            _dropGroup, _dropSlot, _dropIndex);
                        }
                    }
                    else    //�κ��丮/���� ������ ����ִٸ�, ���� ���� ��� �� ���Կ� ����
                    {
                        Change_EquipItem(false, _dragClass);    //�巡���� ��� ����

                        Swap_Drag_To_Drop(_dragGroup, _dragSlot, _dragIndex,
                                        _dropGroup, _dropSlot, _dropIndex);
                    }
                }
            }
            else    //�κ��丮/������ �巡�������Ƿ�, ��Ȳ�� dropSlot�� ��� ����
            {
                //�巡���� ����� Ÿ�԰� ����� ������ Ÿ���� ���� ��
                //�κ��丮/���� ������ ��� ���� �� ��ü
                if (_dragClass.Data.Type == _dropSlot.EQUIP_TYPE)
                {
                    if (_dropClass.Data != null)       //��� ���Կ� �������� ���� ���
                        Change_EquipItem(false, _dropClass);

                    Change_EquipItem(true, _dragClass); //�巡���� ��� ����

                    Swap_Drag_To_Drop(_dragGroup, _dragSlot, _dragIndex,
                                    _dropGroup, _dropSlot, _dropIndex);
                }
            }
        }
        else    //�ƴ� ���, ������ Swap�� ���� ���� (���� ��ȭ X)
        {
            Swap_Drag_To_Drop(_dragGroup, _dragSlot, _dragIndex,
                            _dropGroup, _dropSlot, _dropIndex);
        }

        //��� �� �ش� ���Կ� �������� ������, ���� ��� �ڷ�ƾ ����
        if (_dropSlot.EXIST)
        {
            ItemTooltip_On(slot, index);    //������ ������ ������ �̸� �ݿ��ؼ�, ���� ũ�� ��ȭ�� ������ �ʰ� �ϱ�

            _dropSlot.StopCoroutine("Print_ItemTooltip");
            _dropSlot.StartCoroutine("Print_ItemTooltip");
        }

        Drag_End(); //��� �����Ƿ�, �巡�� ���� �Լ� ���
    }

    //�巡�� ����
    void Drag_End()
    {
        if (STCanvas.ITEM_DRAG)    //������ �巡�� ���� ��
        {
            //�巡�� ���� ������ ������ �⺻ ���·� ����
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

            //�巡�� Ŀ�� OFF
            _dragCursor.SetActive(false);
            //�巡�� ������ �̹��� OFF
            _img_dragIcon.enabled = false;

            if (_isOn_inventory)   //���â Ȱ��ȭ ��
            {
                for (int i = 0; i < _slot_equip.Length; i++)
                {
                    //���� �巡�� ���� �����۰� ������ Ÿ���� ��񽽷��� ���������
                    if (_itemClass_equip[i].Data == null && _slot_equip[i].EQUIP_TYPE == _dragClass.Data.Type)
                        _slot_equip[i].Change_SlotIcon(_spr_equipItemType[(int)_slot_equip[i].EQUIP_TYPE]);    //��� ���� ���� ����
                }
            }

            //�巡�� ����, ��� ���� ���� �ʱ�ȭ
            _dragClass = null;
            _dragSlot = null;
            _dropClass = null;
            _dropSlot = null;

            //�巡�� ��ũ�� off

            //�巡�� ���� false
            STCanvas.Set_Drag(false);
            STCanvas.Set_ItemDrag(false);
        }
    }

    //<A ������ ������ Ŭ����, ���� ����>�� <B ������ ������ Ŭ����, ��������>�� ��ü
    void Swap_Drag_To_Drop(ItemClass[] A_group, ItemSlot A_slot, int A_index,    //�巡���� ������ ������ ����
                            ItemClass[] B_group, ItemSlot B_slot, int B_index)   //����� ������ ������ ����
    {
        //Debug.Log("" + A_slot.SLOT_TYPE + ": " + A_index + " => " + B_slot.SLOT_TYPE + ": " + B_index);

        //������ Ŭ���� ��ü
        var temp_class = B_group[B_index];      //��ӵ� ������ ������ temp�� ����

        B_group[B_index] = A_group[A_index];    //����� ���Կ� �巡���� �������� Ŭ������ ����

        A_group[A_index] = temp_class;          //�巡���� �������� ���Կ� temp�� ����� Ŭ������ ����

        //�巡���� ����, ����� ������ ������ ���翩�θ� �Ǵ��ϰ� ����
        A_slot.EXIST = (A_group[A_index].Data != null);
        B_slot.EXIST = (B_group[B_index].Data != null);

        //�巡���� ������ Ÿ���� �Ǵ���, �ش� ������ UI�� Ȱ��ȭ ���̸�, ������ ����
        bool isOn_A_ui = false;     //�巡���� ������ UI Ȱ��ȭ ����
        bool isOn_B_ui = false;     //����� ������ UI Ȱ��ȭ ����
        bool is_A_equip = (A_slot.SLOT_TYPE == ItemSlotType.Equip); //�巡���� ������ ��� �������� ���� (�� ������ �������� �����ǹǷ�, �ʿ�)

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

        if (isOn_A_ui)  //�巡���� ������ UI�� Ȱ��ȭ���̸�
        {
            if (A_group[A_index].Data != null)  //�� ���Կ� �������� ���� ���
                A_slot.Change_SlotIcon(A_group[A_index].Data.Icon);     //�� ���� �������� ���������� ����
            else    //�� ���Կ� �������� ���� ���
            {
                if (is_A_equip) //�� ������ ��� �����̸�, 
                    A_slot.Change_SlotIcon(_spr_equipItemType[(int)A_slot.EQUIP_TYPE]);    //�ε����� �´� �� ��� ���� ���������� ����
                else
                    A_slot.Change_SlotIcon(_spr_noItem);                                   //�� ���� ���������� ����
            }
        }

        if (isOn_B_ui)  //����� ������ UI�� Ȱ��ȭ ���̸�
        {
            //��� ������ �������� ������ �����͸� �����ؼ� ����
            B_slot.Change_SlotIcon(B_group[B_index].Data.Icon);
        }
    }

    //��� ����/����
    void Change_EquipItem(bool isEquip, ItemClass itemClass)    //isEquip�� ���� ���� �Ǵ� ����
    {
        if (itemClass.Stat1 != ICreature.Stats.No)  //���� 1�� ������ ���
            Manage_EquipStat(isEquip, itemClass.Stat1, itemClass.Stat1_Arr, itemClass.Stat1_Value); //isEquip�� ���� ����1 �߰�/����
        if (itemClass.Stat2 != ICreature.Stats.No)  //���� 2�� ������ ���
            Manage_EquipStat(isEquip, itemClass.Stat2, itemClass.Stat2_Arr, itemClass.Stat2_Value); //isEquip�� ���� ����2 �߰�/����

        if (itemClass.BtlAct1 != null)   //�����ൿ1�� ������ ���
            Manage_BtlAct(isEquip, itemClass.BtlAct1);   //�����ൿ1 �߰�/����
        if (itemClass.BtlAct2 != null)   //�����ൿ2�� ������ ���
            Manage_BtlAct(isEquip, itemClass.BtlAct2);   //�����ൿ2 �߰�/����

        //Ư�� ���� �� �߰�/����

        if (isEquip)
            PlayerSys.EquipItem(itemClass.Data);    //������ ����
        else
            PlayerSys.UnequipItem(itemClass.Data);  //������ ����
    }

    //������ ��ü�� �����Ǵ� ���� ����
    void Manage_EquipStat(bool plus, ICreature.Stats stat, int[] stat_arr, int stat_value)  //bool���� ���� �ش� ���Ȱ��� ����
    {
        if (stat <= ICreature.Stats.WIL)    //�ൿ������ ���
            PlayerSys.Change_ActionStat(plus, stat, stat_arr);
        else    //�Ϲݽ����� ���
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

    //������ ��ü�� ����Ǵ� �ൿ ����
    void Manage_BtlAct(bool plus, ICreature.BtlActClass btlAct)     //bool���� ���� �����ൿ�� �߰�/����
    {
        if (btlAct.Data != null)
            PlayerSys.Change_BtlAct(plus, btlAct);
    }
}
