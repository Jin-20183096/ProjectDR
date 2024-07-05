using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static ICreature;
using static ItemSystem;
using static SingleToneCanvas;

public class PlayerSystem : MonoBehaviour, ICreature
{
    public static PlayerSystem PlayerSys = null;

    [SerializeField]
    private Vector2Int _grid;
    public int X
    {
        get { return _grid.x; }
        set { _grid.x = value; }
    }
    public int Y
    {
        get { return _grid.y; }
        set { _grid.y = value; }
    }
    [SerializeField]
    private int _sight; //�þ�
    public int SIGHT
    {
        get { return _sight; }
    }

    [Header("# Main Info")]
    [SerializeField]
    private string _name;   //�̸�
    public string NAME
    {
        get { return _name; }
    }
    [SerializeField]
    private int _lv;     //����
    public int LV
    {
        get { return _lv; }
    }
    [SerializeField]
    private int _exp;    //����ġ
    public int EXP
    {
        get { return _exp; }
    }
    [SerializeField]
    private int _expMax;    //�ִ� ����ġ
    public int EXP_MAX
    {
        get { return _expMax; }
    }
    [SerializeField]
    private int _hp;        //HP
    public int HP
    {
        get { return _hp; }
    }
    [SerializeField]
    private int _hpMax;     //�ִ� HP
    public int HP_MAX
    {
        get { return _hpMax; }
    }
    [SerializeField]
    private int _ac;        //��
    public int AC
    {
        get { return _ac; }
    }
    [SerializeField]
    private int _ap;        //�ൿ��
    public int AP
    {
        get { return _ap; }
    }
    [SerializeField]
    private int _apMax;     //�ִ� �ൿ��
    public int AP_MAX
    {
        get { return _apMax; }
    }
    private int _apUse;     //����� �ൿ��

    [Header("# Action Related")]
    [SerializeField]
    private int[] _stat_STR = { 0, 0, 0, 0, 0, 0 };     //�� ����
    public int[] STR
    {
        get { return _stat_STR; }
    }
    [SerializeField]
    private int[] _stat_INT = { 0, 0, 0, 0, 0, 0 };     //���� ����
    public int[] INT
    {
        get { return _stat_INT; }
    }
    [SerializeField]
    private int[] _stat_DEX = { 0, 0, 0, 0, 0, 0 };     //������ ����
    public int[] DEX
    {
        get { return _stat_DEX; }
    }
    [SerializeField]
    private int[] _stat_AGI = { 0, 0, 0, 0, 0, 0 };     //��ø ����
    public int[] AGI
    {
        get { return _stat_AGI; }
    }
    [SerializeField]
    private int[] _stat_CON = { 0, 0, 0, 0, 0, 0 };     //�ǰ� ����
    public int[] CON
    {
        get { return _stat_CON; }
    }
    [SerializeField]
    private int[] _stat_WIL = { 0, 0, 0, 0, 0, 0 };     //�ǰ� ����
    public int[] WIL
    {
        get { return _stat_WIL; }
    }

    [SerializeField]
    private int[] _reroll = new int[7];     //No, ��, ����, ������, ��ø, �ǰ�, ����

    public int GetReroll(Stats stat)  //�籼�� ��ȯ
    {
        return _reroll[(int)stat];
    }

    [SerializeField]
    private List<BtlActClass> _actList; //�ൿ ����Ʈ
    public List<BtlActClass> ActList
    {
        get
        {
            var actList = _actList.ToList();
            actList.Add(_waitAct);
            return actList;
        }
    }

    [SerializeField]
    private BtlActClass _waitAct;   //��� �ൿ

    [SerializeField]
    private List<BtlActClass> _actList_unarm;   //�Ǽ� �ൿ ����Ʈ

    [Header("# Menu Screen")]
    [Header("   > Info Pannel")]
    [SerializeField]
    private bool _isOn_infoPannel;
    [SerializeField]
    private PlayerInfoPannel _infoPannel;

    [Header("   > Status Screen")]
    [SerializeField]
    private bool _isOn_statusScr;   //�������ͽ�â Ȱ��ȭ ����
    [SerializeField]
    private PlayerMenuButton _btn_statusScr;    //�������ͽ�â ��ư
    [SerializeField]
    private StatusScreen _statusScr;    //�������ͽ�â ��ũ��Ʈ

    [Header("   > Inventory Screen")]
    [SerializeField]
    private bool _isOn_inventoryScr;    //�κ��丮â Ȱ��ȭ ����
    [SerializeField]
    private PlayerMenuButton _btn_inventoryScr; //�κ��丮â ��ư
    [SerializeField]
    private GameObject _inventoryScr;  //�κ��丮â ��ũ��Ʈ

    [Header("   > Action Screen")]
    [SerializeField]
    private bool _isOn_actScr;  //�ൿâ Ȱ��ȭ ����
    [SerializeField]
    private bool _isActChange;  //���� �ൿ�� ���� ���� (�������� �ִٸ�, �ൿ��� â�� Ȱ��ȭ�� �� ��� ������ ��������)
    [SerializeField]
    private PlayerMenuButton _btn_actScr;   //�ൿâ ��ư
    [SerializeField]
    private ActionScreen _actScr;   //�ൿâ ��ũ��Ʈ

    [Header("# Sprite Reference")]
    [SerializeField]
    private SpriteSystem _p_spr;
    [SerializeField]
    private SpriteSystem _p_spr_btl;

    [Header("# Effect")]
    [SerializeField]
    private Transform _effect_group;
    [SerializeField]
    private ParticleSystem _effect_blood;

    void Awake()
    {
        if (PlayerSys)
        {
            DestroyImmediate(gameObject);
            return;
        }
        else
        {
            PlayerSys = this;
            DontDestroyOnLoad(gameObject);
        }

        //�ʱ� �������ͽ� ����
        Change_Name("�̸� ���� ���谡");

        _lv = 1;
        _exp = 0;
        _expMax = 10;

        _hpMax = Random.Range(8, 15);
        _hp = _hpMax;
        Change_HpMax(true, 0);
        Change_Hp(true, 0);

        _ac = 0;
        Change_AC(true, 0);

        _apMax = 6;
        _ap = Random.Range(1, _apMax + 1);
        Change_ApMax(true, 0);
        Change_Ap(true, 0);
    }

    public void Move_PlayerPosition(Vector3 vec3, bool isLeft)
    {
        transform.position = vec3;
        _p_spr.Flip_X(isLeft);
    }

    public void Set_PlayerLocalPos(bool isCenter, bool isLeft)
    {
        Debug.Log("�÷��̾� �߾ӿ� �ִ°�" + isCenter + " /\n �÷��̾� ������ �ִ°�" + isLeft);
        if (isCenter)
            _p_spr.transform.localPosition = new Vector3(0, 0, -1.25f);
        else
        {
            if (isLeft)
                _p_spr.transform.localPosition = new Vector3(-1.25f, 0, -1.25f);
            else
                _p_spr.transform.localPosition = new Vector3(1.25f, 0, -1.25f);
        }
    }

    public bool IsPlayer() => true;

    public void Change_Name(string name)
    {
        _name = name;   //�̸� ����

        if (_isOn_statusScr)    //�������ͽ�â�� �� ����
            _statusScr.Change_Name(_name);
    }

    public void LvUp(int value)     //���� ��
    {
        _exp = 0;   //����ġ �ʱ�ȭ
        var val = value;

        while (val > 0)
        {
            val--;  //����� ���� 1 ���
            _lv++;  //���� 1 ���

            //�����뿡 ���� �ִ� ����ġ�� ����
            if (_lv >= 1 && _lv <= 2)
            {
                Debug.Log(_expMax + "+" + (_expMax * 1.6f));
                _expMax = (int)(_expMax + (_expMax * 1.6f));
            }
            else if (_lv >= 3 && _lv <= 4)
            {
                Debug.Log(_expMax + "+" + (_expMax * 1.3f));
                _expMax = (int)(_expMax + (_expMax * 1.3f));
            }
            else if (_lv >= 5 && _lv <= 7)
            {
                Debug.Log(_expMax + "+" + (_expMax * 0.9f));
                _expMax = (int)(_expMax + (_expMax * 0.9f));
            }
            else if (_lv >= 8 && _lv <= 10)
            {
                Debug.Log(_expMax + "+" + (_expMax * 0.5f));
                _expMax = (int)(_expMax + (_expMax * 0.5f));
            }
            else
            {
                Debug.Log(_expMax + "+" + (_expMax * 0.2f));
                _expMax = (int)(_expMax + (_expMax * 0.2f));
            }
        }

        if (_isOn_statusScr)
        {
            _statusScr.Change_Lv(_lv);
            _statusScr.Change_Exp(_exp);
            _statusScr.Change_ExpMax(_expMax);
            _statusScr.Change_ExpMeter(_exp / (float)_expMax);
        }
    }

    public void Change_Exp(bool plus, int value)    //����ġ ȹ��, ����
    {
        if (plus)
            _exp += value;
        else
        {
            if (_exp <= value)
                _exp = 0;
            else
                _exp -= value;
        }

        if (_isOn_statusScr)    //�������ͽ�â�� �� ����
        {
            _statusScr.Change_Lv(_lv);
            _statusScr.Change_Exp(_exp);
            _statusScr.Change_ExpMax(_expMax);
            _statusScr.Change_ExpMeter(_exp / (float)_expMax);
        }
    }

    public void Change_Hp(bool plus, int value)     //HP ����
    {
        var old_hp = _hp;

        if (value >= 0) //���氪�� 0 �̻��϶��� ó��
        {
            if (plus)   //ȸ��
            {
                if (_hpMax <= _hp + value)  //ȸ������ �ִ� HP�� ���� ���
                    _hp = _hpMax;   //�ִ� HP�� ȸ��
                else
                    _hp += value;   //����ŭ ȸ��
            }
            else        //����
            {
                if (_hp <= value)   //���ط��� ���� HP�� ���� ���
                    _hp = 0;        //HP�� 0����
                else
                    _hp -= value;   //����ŭ ����
            }
        }

        _infoPannel.Change_TextHp(_hp);

        if (_hp > old_hp)   //HP ȸ�� ��
            _infoPannel.Change_HpMask(_hp / (float)_hpMax);    //HP ����ũ ����
        else                //HP ���ع޾��� ��
            _infoPannel.Change_HpMeter(_hp / (float)_hpMax);   //HP ���� ����

        if (_isOn_statusScr)    //�������ͽ�â�� �� ����
            _statusScr.Change_Hp(_hp);
    }

    public void Change_HpMax(bool plus, int value)  //�ִ� HP ����
    {
        var old_hpMax = _hpMax;

        //����ŭ ����
        if (plus)
            _hpMax += value;
        else
            _hpMax -= value;

        if (_hp >= _hpMax)      //�ִ� HP�� ���� HP���� �������� ���
            _hp = _hpMax;       //������ �ִ� HP��ŭ ���� HP�� ����

        //����� ������ŭ HP�� ����
        _infoPannel.Change_TextHp(_hp);
        _infoPannel.Change_TextHpMax(_hpMax);

        if (_hpMax > old_hpMax)
            _infoPannel.Change_HpMeter(_hp / (float)_hpMax);
        else
            _infoPannel.Change_HpMask(_hp / (float)_hpMax);

        if (_isOn_statusScr)    //�������ͽ�â�� �� ����
        {
            _statusScr.Change_Hp(_hp);
            _statusScr.Change_HpMax(_hpMax);
        }
    }

    public void Change_AC(bool plus, int value) //�� ����
    {
        if (plus)
            _ac += value;
        else
            _ac -= value;

        //�� �����ܰ� ui��ġ����
        _infoPannel.Change_Ac(_ac);

        if (_isOn_statusScr)    //�������ͽ�â�� �� ����
            _statusScr.Change_AC(_ac);
    }

    public void Change_Ap(bool plus, int value) //�ൿ�� ����
    {
        var old_ap = _ap;

        if (value >= 0) //���氪�� 0 �̻��ϋ��� ó��
        {
            if (plus)   //�߰�
            {
                if (_apMax <= _ap + value)  //�߰����� �ִ� �ൿ���� ���� ���
                    _ap = _apMax;
                else
                    _ap += value;
            }
            else        //�Ҹ�
            {
                if (_ap <= value)           //�Ҹ��� ���� �ൿ���� ���� ���
                    _ap = 0;
                else
                    _ap -= value;
            }
        }

        if (old_ap == 0)    //�ൿ���� 0�̾��ٰ�
        {
            if (_ap > 0)    //0�� �ƴϰ� �Ǹ�, �� ȸ��
                Change_AC(true, 5);
        }
        else if (_ap == 0)  //�ൿ���� 0�� �ƴϾ��ٰ� 0�� �Ǹ�, �� ����
        {
            Change_AC(false, 5);
        }

        //���� ����
        _infoPannel.Change_ApMeter(_ap);
        _infoPannel.Change_Ap_UsePreview(_apUse);

        if (_isOn_statusScr)
            _statusScr.Change_Ap(_ap);
    }

    public void Change_Ap_UsePreview(int use)
    {
        _apUse = use;
        _infoPannel.Change_Ap_UsePreview(_apUse);
    }

    public void Change_ApMax(bool plus, int value)
    {
        if (plus)
            _apMax += value;
        else
            _apMax -= value;

        if (_ap >= _apMax)  //�ִ� �ൿ���� ���� �ൿ�º��� �������� ���
            _ap = _apMax;   //���� �ൿ���� ������ �ִ� �ൿ�¸�ŭ ����

        //���� ����
        _infoPannel.Change_ApMeter(_ap);
        _infoPannel.Change_Ap_UsePreview(_apUse);
        _infoPannel.Change_ApMeterMax(_apMax);

        if (_isOn_statusScr)
        {
            _statusScr.Change_Ap(_ap);
            _statusScr.Change_ApMax(_apMax);
        }
    }

    public void Change_ActionStat(bool plus, Stats stat, int[] stat_arr)
    {
        int[] changingStat = { };

        switch (stat)
        {
            case Stats.STR:
                changingStat = _stat_STR;
                break;
            case Stats.INT:
                changingStat = _stat_INT;
                break;
            case Stats.DEX:
                changingStat = _stat_DEX;
                break;
            case Stats.AGI:
                changingStat = _stat_AGI;
                break;
            case Stats.CON:
                changingStat = _stat_CON;
                break;
            case Stats.WIL:
                changingStat = _stat_WIL;
                break;
        }

        for (int i = 0; i < changingStat.Length; i++)
        {
            if (plus)
                changingStat[i] += stat_arr[i];
            else
                changingStat[i] -= stat_arr[i];
        }

        if (_isOn_statusScr)
            _statusScr.Change_ActionStat(stat, changingStat);
    }

    public void TakeDamage(int dmg, BtlActData.DamageType dmgType)
    {
        Change_Hp(false, dmg);

        var eff = Instantiate(_effect_blood, _effect_group);
        var pos = _p_spr_btl.transform.position;
        var sizeY = _p_spr_btl.GetComponent<SpriteRenderer>().bounds.size.y;
        eff.transform.position = new Vector3(pos.x, pos.y + sizeY / 2, pos.z);
        eff.transform.localScale = new Vector3(0.75f, 0.75f, 1f);

        //������ Ÿ�Կ� ���� �ǰ���
    }

    public void Change_Reroll(bool plus, Stats stat, int value)
    {
        var reroll_stat = Stats.No;

        switch (stat)
        {
            case Stats.RE_STR:
                reroll_stat = Stats.STR;
                break;
            case Stats.RE_INT:
                reroll_stat = Stats.INT;
                break;
            case Stats.RE_DEX:
                reroll_stat = Stats.DEX;
                break;
            case Stats.RE_AGI:
                reroll_stat = Stats.AGI;
                break;
            case Stats.RE_CON:
                reroll_stat = Stats.CON;
                break;
            case Stats.RE_WIL:
                reroll_stat = Stats.WIL;
                break;
        }

        if (plus)
            _reroll[(int)reroll_stat] += value;
        else
            _reroll[(int)reroll_stat] -= value;

        //�������ͽ� â�� ���� ����
        if (_isOn_statusScr)
            _statusScr.Change_Reroll(reroll_stat, _reroll[(int)reroll_stat]);
    }

    public void Change_BtlAct(bool plus, BtlActClass btlAct)
    {
        if (plus)   //�ൿ �߰�
            _actList.Add(btlAct);
        else
            _actList.Remove(btlAct);

        //�ൿ���� �ൿ Ÿ�Կ� �°� ����
        _actList.Sort((x, y) => string.Compare(x.Data.Type.ToString(), y.Data.Type.ToString()));

        //�ൿ��� â�� ���� ����
        if (_isOn_actScr)
            _actScr.Change_BtlActList();
        else
            _isActChange = true;
    }

    public void Armed_OnOff(bool b) //���� or �Ǽ�
    {
        if (b)  //�������� ���
        {
            foreach (BtlActClass act in _actList_unarm)
                Change_BtlAct(false, act);  //�Ǽ� �ൿ�� ���� ����
        }
        else
        {
            foreach (BtlActClass act in _actList_unarm)
                Change_BtlAct(true, act);   //�Ǽ� �ൿ�� ���� �߰�
        }
    }

    public void InfoPannel_OnOff(bool b)    //�÷��̾� �⺻ ����â OnOff
    {
        _isOn_infoPannel = b;
        _infoPannel.gameObject.SetActive(b);

        if (_isOn_infoPannel)
        {
            //�÷��̾� �⺻ ���� ����ȭ
            //HP��
            _infoPannel.Change_HpBar(_hp, _hpMax);
            //�ൿ��
            _infoPannel.Change_ApMeterMax(_apMax);
            _infoPannel.Change_ApMeter(_ap);
            //��
            _infoPannel.Change_Ac(_ac);
        }
    }

    public void StatusScreen_OnOff()    //�������ͽ�â OnOff
    {
        if (STCanvas.DRAG == false)
        {
            _isOn_statusScr = !_isOn_statusScr;
            _statusScr.gameObject.SetActive(_isOn_statusScr);

            if (_isOn_statusScr)
            {
                //��������â�� UI �켱������ �ֻ�����
                _statusScr.transform.SetAsLastSibling();

                //�������ͽ� �� ����ȭ
                _statusScr.Change_Name(_name);  //�̸�
                _statusScr.Change_Lv(_lv);      //����
                _statusScr.Change_Exp(_exp);    //����ġ
                _statusScr.Change_ExpMax(_expMax);  //�ִ� ����ġ
                _statusScr.Change_ExpMeter((float)_exp / _expMax);
                _statusScr.Change_Hp(_hp);          //HP
                _statusScr.Change_HpMax(_hpMax);    //�ִ� HP
                _statusScr.Change_Ap(_ap);          //�ൿ��
                _statusScr.Change_ApMax(_apMax);    //�ִ� �ൿ��
                _statusScr.Change_AC(_ac);          //��

                //�� ����
                _statusScr.Change_Reroll(Stats.STR, _reroll[(int)Stats.STR]);
                _statusScr.Change_ActionStat(Stats.STR, _stat_STR);
                //���� ����
                _statusScr.Change_Reroll(Stats.INT, _reroll[(int)Stats.INT]);
                _statusScr.Change_ActionStat(Stats.INT, _stat_INT);
                //������ ����
                _statusScr.Change_Reroll(Stats.DEX, _reroll[(int)Stats.DEX]);
                _statusScr.Change_ActionStat(Stats.DEX, _stat_DEX);
                //��ø ����
                _statusScr.Change_Reroll(Stats.AGI, _reroll[(int)Stats.AGI]);
                _statusScr.Change_ActionStat(Stats.AGI, _stat_AGI);
                //�ǰ� ����
                _statusScr.Change_Reroll(Stats.CON, _reroll[(int)Stats.CON]);
                _statusScr.Change_ActionStat(Stats.CON, _stat_CON);
                //���� ����
                _statusScr.Change_Reroll(Stats.WIL, _reroll[(int)Stats.WIL]);
                _statusScr.Change_ActionStat(Stats.WIL, _stat_WIL);
            }
        }
    }

    public void InventoryScreen_OnOff() //�κ��丮â OnOff
    {
        if (STCanvas.DRAG == false)
        {
            _isOn_inventoryScr = !_isOn_inventoryScr;
            _inventoryScr.SetActive(_isOn_inventoryScr);

            if (_isOn_inventoryScr)
            {
                _inventoryScr.transform.SetAsLastSibling(); //�κ��丮 UI �켱������ �ֻ�����

                ItemSys.Set_EquipIcon();        //������ ��� ������ �Ҵ�

                ItemSys.Set_InventoryIcon();    //�κ��丮 ������ �Ҵ�
            }

            ItemSys.ON_INVENTORY = _isOn_inventoryScr;
        }
    }

    public void ActionScreen_OnOff()    //�ൿâ OnOff
    {
        if (STCanvas.DRAG == false)
        {
            _isOn_actScr = !_isOn_actScr;
            _actScr.gameObject.SetActive(_isOn_actScr);

            if (_isOn_actScr)
            {
                //�κ��丮 UI �켱���� �ֻ���
                _actScr.transform.SetAsLastSibling();

                if (_isActChange)   //������ �ൿ�� ��ȭ�� ������ ���
                {
                    _actScr.Change_BtlActList();    //�ൿ ��� ����ȭ
                    _isActChange = false;           //�ൿ ��� ��ȭ �������� ó��
                }
            }
        }
    }

    public void MenuButton_OnOff_Status(bool b)  //�������ͽ�â ��ư OnOff
    {
        //�������ͽ� ��ư�� ��Ȱ��ȭ �ɶ�, �������ͽ�â�� Ȱ��ȭ ���̶��
        if (b == false && _isOn_statusScr)
        {
            _btn_statusScr.Button_OnOff();   //��ư Off ����
            StatusScreen_OnOff();               //�������ͽ�â off ����
        }

        _btn_statusScr.gameObject.SetActive(b);
    }

    public void MenuButton_OnOff_Inventory(bool b)  //�κ��丮â ��ư OnOff
    {
        //�κ��丮 ��ư�� ��Ȱ��ȭ�� ��, �κ��丮�� Ȱ��ȭ ���̶��
        if (b == false && _isOn_inventoryScr)
        {
            _btn_inventoryScr.Button_OnOff();    //��ư Off ����
            InventoryScreen_OnOff();                //�κ��丮â Off ����
        }

        _btn_inventoryScr.gameObject.SetActive(b);
    }

    public void MenuButton_OnOff_ActList(bool b)    //�ൿâ ��ư OnOff
    {
        //�ൿ��� ��ư�� ��Ȱ��ȭ�� ��, �ൿ����� Ȱ��ȭ ���̶��
        if (b == false && _isOn_actScr)
        {
            _btn_actScr.Button_OnOff();  //��ư Off ����
            ActionScreen_OnOff();          //�ൿ���â Off ����
        }

        _btn_actScr.gameObject.SetActive(b);
    }

    public void EquipItem(ItemData item)    //������ ���
    {
        if (item.Type == ItemData.ItemType.Weapon)
            Armed_OnOff(true);  //���⸦ �����

        //����, ��, �������� Ÿ���� ����� ��� ���� ����
        if (item.Type <= ItemData.ItemType.SubWp)
        {
            _p_spr.Change_Item(item, item.Type);
            _p_spr_btl.Change_Item(item, item.Type);
        }
    }

    public void UnequipItem(ItemData item)  //������ ����
    {
        if (item.Type == ItemData.ItemType.Weapon)
            Armed_OnOff(true);  //���⸦ ������

        //����, ��, �������� Ÿ���� ����� ��� ���� ����
        if (item.Type <= ItemData.ItemType.SubWp)
        {
            _p_spr.Change_Item(item, item.Type);
            _p_spr_btl.Change_Item(item, item.Type);
        }
    }
}
