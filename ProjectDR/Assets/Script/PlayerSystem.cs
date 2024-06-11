using System.Collections.Generic;
using UnityEngine;
using static ICreature;

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
        get { return _actList; }
    }

    [Header("# UI Reference")]
    //�÷��̾� �⺻ ���� �г�
    [SerializeField]
    private PlayerInfoPannel _infoScr;

    [Header("# Sprite Reference")]
    [SerializeField]
    private SpriteSystem _p_spr;
    [SerializeField]
    private SpriteSystem _p_spr_btl;

    [SerializeField]
    private GameObject _btn_fullRest;

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

    public void Change_Name(string name) => _name = name;   //�̸� ����

    public void Change_Exp(bool plus, int value)    //����ġ ȹ��, ����
    {
        if (plus)
        {
            if (_expMax <= _exp + value)    //ȹ���� ����ġ�� �������� �߻��� ���
            {
                while (_expMax <= _exp + value) //������ ������� ����ġ üũ
                {
                    _exp = (_exp + value) - _expMax;    //����ġ�� ������ ���� �� ����ġ�� ����
                    _lv += 1;   //���� 1 ����
                    _expMax += (int)(_expMax * 1.75);   //�ִ� ����ġ�� ����
                }
            }
            else
                _exp += value;
        }
        else
        {
            if (_exp <= value)
                _exp = 0;
            else
                _exp -= value;
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

        _infoScr.Change_TextHp(_hp);

        if (_hp > old_hp)   //HP ȸ�� ��
            _infoScr.Change_HpMask(_hp / (float)_hpMax);    //HP ����ũ ����
        else                //HP ���ع޾��� ��
            _infoScr.Change_HpMeter(_hp / (float)_hpMax);   //HP ���� ����
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
        _infoScr.Change_TextHp(_hp);
        _infoScr.Change_TextHpMax(_hpMax);

        if (_hpMax > old_hpMax)
            _infoScr.Change_HpMeter(_hp / (float)_hpMax);
        else
            _infoScr.Change_HpMask(_hp / (float)_hpMax);
    }

    public void Change_AC(bool plus, int value) //�� ����
    {
        if (plus)
            _ac += value;
        else
            _ac -= value;

        //�� �����ܰ� ui��ġ����
        _infoScr.Change_Ac(_ac);
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
        _infoScr.Change_ApMeter(_ap);
        _infoScr.Change_Ap_UsePreview(_apUse);
    }

    public void Change_Ap_UsePreview(int use)
    {
        _apUse = use;
        _infoScr.Change_Ap_UsePreview(_apUse);
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
        _infoScr.Change_ApMeter(_ap);
        _infoScr.Change_Ap_UsePreview(_apUse);
        _infoScr.Change_ApMeterMax(_apMax);
    }

    public void Change_ActionStat(bool plus, Stats stat, int[] stat_arr)
    {
        int[] temp_stat = { };

        switch (stat)
        {
            case Stats.STR:
                temp_stat = _stat_STR;
                break;
            case Stats.INT:
                temp_stat = _stat_INT;
                break;
            case Stats.DEX:
                temp_stat = _stat_DEX;
                break;
            case Stats.AGI:
                temp_stat = _stat_AGI;
                break;
            case Stats.CON:
                temp_stat = _stat_CON;
                break;
            case Stats.WIL:
                temp_stat = _stat_WIL;
                break;
        }

        for (int i = 0; i < temp_stat.Length; i++)
        {
            if (plus)
                temp_stat[i] += stat_arr[i];
            else
                temp_stat[i] -= stat_arr[i];
        }
    }

    public void TakeDamage(int dmg, BtlActData.DamageType dmgType)
    {
        Change_Hp(false, dmg);
        //������ Ÿ�Կ� ���� �ǰ���
    }

    public void FullRest_OnOff(bool isOn)
    {
        _btn_fullRest.SetActive(isOn);
    }
}
