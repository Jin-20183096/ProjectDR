using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Panda;
using TMPro;
using static ICreature;
using static PlayerSystem;

public class EnemySystem : MonoBehaviour, ICreature
{
    [SerializeField]
    private BattleSystem _btlSys;

    [Header("# Enemy Sprite")]
    [SerializeField]
    private SpriteRenderer _e_spr;
    [SerializeField]
    private Animator _e_anima;

    private PandaBehaviour _bt;

    [Header("# Enemy UI")]
    [SerializeField]
    private GameObject _pannel_info;
    [SerializeField]
    private TextMeshProUGUI _txt_name;      //�� �̸� UI �ؽ�Ʈ
    [SerializeField]
    private Image _hpMask;                  //�� HP�� ����ũ
    [SerializeField]
    private Image _hpMeter;                 //�� UI HP�� ����
    [SerializeField]
    private GameObject _icon_ac;            //�� �� UI
    [SerializeField]
    private TextMeshProUGUI _txt_ac;        //�� �� UI �ؽ�Ʈ
    [SerializeField]
    private Transform[] _meter_ap;          //�ൿ�� ����(�����ܵ�)

    [Header("# Data Info")]
    [SerializeField]
    private EnemyData _data;    //�� ������
    public EnemyData Data
    {
        get { return _data; }
    }
    public string NAME
    {
        get { return _data.Name; }
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
    [SerializeField]
    private int[] _stat_STR = { 0, 0, 0, 0, 0, 0 }; //��
    public int[] STR
    {
        get { return _stat_STR; }
    }
    [SerializeField]
    private int[] _stat_INT = { 0, 0, 0, 0, 0, 0 }; //����
    public int[] INT
    {
        get { return _stat_INT; }
    }
    [SerializeField]
    private int[] _stat_DEX = { 0, 0, 0, 0, 0, 0 }; //������
    public int[] DEX
    {
        get { return _stat_DEX; }
    }
    [SerializeField]
    private int[] _stat_AGI = { 0, 0, 0, 0, 0, 0 }; //��ø
    public int[] AGI
    {
        get { return _stat_AGI; }
    }
    [SerializeField]
    private int[] _stat_CON = { 0, 0, 0, 0, 0, 0 }; //�ǰ�
    public int[] CON
    {
        get { return _stat_CON; }
    }
    [SerializeField]
    private int[] _stat_WIL = { 0, 0, 0, 0, 0, 0 }; //����
    public int[] WIL
    {
        get { return _stat_WIL; }
    }

    [Header("# Action Related")]
    [SerializeField]
    private List<BtlActData.ActionType> _actTypeList;    //�ൿŸ�� ���

    [SerializeField]
    private List<BtlActClass> _atkAct; //���� �ൿ ���
    [SerializeField]
    private List<BtlActClass> _defAct; //��� �ൿ ���
    [SerializeField]
    private List<BtlActClass> _dgeAct; //ȸ�� �ൿ ���
    [SerializeField]
    private List<BtlActClass> _tacAct; //���� �ൿ ���
    [SerializeField]
    private BtlActClass _waitAct;     //��� �ൿ

    [SerializeField]
    private int _nowDice;               //���� ���� �ֻ���
    [SerializeField]
    private int[] _result;              //���� ���� �ֻ��� ���

    [SerializeField]
    private bool[] _banActType = { false, false, false, false, false };
    public bool BAN_ATK
    {
        get { return _banActType[1]; }
    }
    public bool BAN_DEF
    {
        get { return _banActType[2]; }
    }
    public bool BAN_DGE
    {
        get { return _banActType[3]; }
    }
    public bool BAN_TAC
    {
        get { return _banActType[4]; }
    }

    Stack<BtlActClass> _act_stack; //�ൿ ����

    [Header("# Effect")]
    [SerializeField]
    private Transform _eff_group;
    [SerializeField]
    private ParticleSystem _eff_blood;

    void Awake()
    {
        _act_stack = new Stack<BtlActClass>();
    }

    public void Set_BattleEnemy(bool nowBattle, EnemyData new_enemy)
    {
        _pannel_info.SetActive(nowBattle);

        if (nowBattle)
        {
            _data = new_enemy;

            //�� ����
            Set_EnemyInfo(Random.Range(_data.HP_MIN, _data.HP_MAX + 1));

            //������, ���������� ���� ��������Ʈ ����
            _e_spr.gameObject.SetActive(true);
            _e_anima.runtimeAnimatorController = _data.Anima_Ctrl;
        }
        else    //���� ���� ��
        {
            //�� ������, ���� ���� �ʱ�ȭ
            _data = null;
            _hp = 0;
            _hpMax = 0;
            _ac = 0;
            _ap = 0;
            _apMax = 0;

            //�ൿ Ʈ�� �Ҵ� off
            Destroy(_bt);
            _bt = null;

            _e_spr.gameObject.SetActive(false);  //�� ��������Ʈ off
        }
    }

    public void Set_EnemyInfo(int hpMax)
    {
        //HP
        _hpMax = hpMax;
        _hp = _hpMax;
        //��
        _ac = _data.AC;
        //�ൿ��
        _apMax = _data.ApMax;
        _ap = Random.Range(1, _apMax + 1);

        //�ൿ��� ����
        _actTypeList.Clear();  //�ൿŸ�� ��� �ʱ�ȭ

        _atkAct.Clear();
        _atkAct = _data.Act_Atk.ToList();  //���� �ൿ ��Ͽ� �߰�
        if (_atkAct.Count > 0) _actTypeList.Add(BtlActData.ActionType.Atk);

        _defAct.Clear();
        _defAct = _data.Act_Def.ToList();  //��� �ൿ ��Ͽ� �߰�
        if (_defAct.Count > 0) _actTypeList.Add(BtlActData.ActionType.Def);

        _dgeAct.Clear();
        _dgeAct = _data.Act_Dge.ToList();  //ȸ�� �ൿ ��Ͽ� �߰�
        if (_dgeAct.Count > 0) _actTypeList.Add(BtlActData.ActionType.Dge);

        _tacAct.Clear();
        _tacAct = _data.Act_Tac.ToList();  //���� �ൿ ��Ͽ� �߰�
        if (_tacAct.Count > 0) _actTypeList.Add(BtlActData.ActionType.Tac);

        //���� ������ ���� ����
        _txt_name.text = NAME;  //�̸�
        Change_HpMax(true, 0);
        Change_Hp(true, 0);
        _hpMask.fillAmount = _hp / (float)_hpMax;
        _hpMeter.fillAmount = _hp / (float)_hpMax;
        Change_AC(true, 0);
        Change_ApMax(true, 0);
        Change_Ap(true, 0);

        //�ൿ ���� ���� ����
        _stat_STR = _data.STR.ToArray();
        _stat_INT = _data.INT.ToArray();
        _stat_DEX = _data.DEX.ToArray();
        _stat_AGI = _data.AGI.ToArray();
        _stat_CON = _data.CON.ToArray();
        _stat_WIL = _data.WIL.ToArray();

        //�ൿ Ʈ�� �Ҵ� On
        _bt = gameObject.AddComponent(typeof(PandaBehaviour)) as PandaBehaviour;

        _bt.scripts = new TextAsset[] { _data.BT };
        _bt.tickOn = BehaviourTree.UpdateOrder.Manual;
        _bt.Compile();
        _bt.Apply();
    }

    public bool IsPlayer() => false;

    public void Change_Hp(bool plus, int value)
    {
        var old_hp = _hp;

        if (value >= 0) //���氪�� 0 �̻��� ���� ó��
        {
            if (plus)   //ȸ��
            {
                if (_hpMax <= _hp + value)      //ȸ������ �ִ� HP�� ���� ���
                    _hp = _hpMax;               //�ִ� HP��ŭ ȸ��
                else
                    _hp += value;               //����ŭ ȸ��
            }
            else
            {
                if (_hp <= value)   //���ط��� ���� HP�� ���� ���
                    _hp = 0;        //HP�� 0����
                else
                    _hp -= value;   //����ŭ ����
            }
        }

        if (_hp > old_hp)   //HP ȸ�� ��
        {
            //HP����ũ ����
            _hpMask.fillAmount = _hp / (float)_hpMax;
            StartCoroutine("HpMask_Up");
        }
        else
        {
            //HP���� ����
            _hpMeter.fillAmount = _hp / (float)_hpMax;
            StartCoroutine("HpMask_Down");
        }
    }

    public void Change_HpMax(bool plus, int value)
    {
        int old_hpMax = _hpMax;

        if (plus)
            _hpMax += value;
        else
            _hpMax -= value;

        if (_hp > _hpMax)   //�ִ� HP�� ���� HP���� �������� ���
            _hp = _hpMax;   //������ �ִ� HP�� ���� HP�� ����

        //����� HP������ŭ HP�� ����
        if (_hpMax >= old_hpMax)
        {
            _hpMeter.fillAmount = _hp / (float)_hpMax;
            StartCoroutine("HpMask_Down");
        }
        else
        {
            _hpMask.fillAmount = _hp / (float)_hpMax;
            StartCoroutine("HpMask_Up");
        }
    }

    IEnumerator HpMask_Down()   //�پ�� ���͸�ŭ  HP�� ����ũ �����
    {
        while (true)
        {
            if (_hpMask.fillAmount > _hpMeter.fillAmount)
                _hpMask.fillAmount -= 0.01f;
            else
                StopCoroutine("HpMask_Down");

            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator HpMask_Up()     //�þ�� ����ũ��ŭ HP�� ���� �ø���
    {
        while (true)
        {
            if (_hpMask.fillAmount > _hpMeter.fillAmount)
                _hpMeter.fillAmount += 0.01f;
            else
                StopCoroutine("HpMask_Up");

            yield return new WaitForSeconds(0.01f);
        }
    }

    public void Change_AC(bool plus, int value)
    {
        if (plus)
            _ac += value;
        else
            _ac -= value;

        _icon_ac.SetActive(_ac != 0);

        if (_ac != 0)
            _txt_ac.text = _ac.ToString();
    }

    public void Change_Ap(bool plus, int value)
    {
        var old_ap = _ap;

        if (value >= 0) //���氪�� 0 �̻��� ���� ó��
        {
            if (plus)   //�߰�
            {
                if (_apMax <= _ap + value)  //�߰����� �ִ� �ൿ���� ���� ���
                    _ap = _apMax;
                else
                    _ap += value;
            }
            else
            {
                if (_ap <= value)           //�Ҹ��� ���� �ൿ���� ���� ���
                    _ap = 0;
                else
                    _ap -= value;
            }
        }

        /*
        if (old_ap == 0)    //�ൿ���� 0�̾��ٰ�
        {
            if (_ap > 0)    //0�� �ƴϰ� �Ǹ�, �� ȸ��
                Change_AC(true, 5);
        }
        else if (_ap == 0)  //�ൿ���� 0�� �ƴϾ��ٰ� 0�� �Ǹ�, �� ����
        {
            Change_AC(false, 5);
        }
        */

        //AP�� ����
        for (int i = 0; i < _meter_ap.Length; i++)
        {
            if (_meter_ap[i].gameObject.activeSelf)
            {
                //���� �ൿ�¿� �ش��ϴ� �����̸�, Ȱ��ȭ. �ƴϸ� ��Ȱ��ȭ
                _meter_ap[i].GetChild(0).gameObject.SetActive(i < _ap);     
            }
        }
    }

    public void Change_ApMax(bool plus, int value)
    {
        if (plus)
            _apMax += value;
        else
            _apMax -= value;

        if (_ap >= _apMax)  //�ִ� �ൿ���� ���� �ൿ�º��� �������� ���,
            _ap = _apMax;   //���� �ൿ���� ������ �ִ� �ൿ�¸�ŭ ����

        for (int i = 0; i < _meter_ap.Length; i++)
            _meter_ap[i].gameObject.SetActive(i < _apMax);
    }

    public void TakeDamage(int dmg, BtlActData.DamageType dmgType)
    {
        Change_Hp(false, dmg);

        var eff = Instantiate(_eff_blood, _eff_group);
        var pos = _e_spr.transform.position;
        var sizeY = _e_spr.bounds.size.y;
        eff.transform.position = new Vector3(pos.x + 0.5f, pos.y + sizeY / 2, pos.z);
        eff.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
        //���������� ���� ���� ��ƼŬ ���� ����
        var burst = eff.emission.GetBurst(0);
        burst.count = dmg * 5;
        eff.emission.SetBurst(0, burst);
        eff.Play();
    }

    public void Request_NextAction()    //���� �ý��ۿ��� ���� �� �ൿ�� ��û
    {
        _bt.scripts = new TextAsset[] { _data.BT };
        _bt.Compile();
        _bt.Apply();

        if (_act_stack.Count == 0)  //�ൿ ���ȿ� �ൿ�� ���� ���
        {
            //�ൿƮ���� ���� �ൿ ����
            _bt.Tick();
        }

        var peek = _act_stack.Peek().Data;    //�ൿ ���� �� ���� �ൿ
        Debug.Log("���� ���� �� �ൿ: " + peek.Name);

        if (peek.NoDice == false && _ap < peek.DiceMin)  //���� �� ���� �ൿ�� �ൿ���� �Ҹ��ϸ鼭, ���� �ൿ���� �ּ� �ൿ�º��� ������
        {
            _act_stack.Push(_waitAct);    //��� �ൿ PUSH
            Debug.Log("������ �ൿ���� ������ " + _waitAct.Data.Name + " �ϱ�� ��");
        }

        BtlActClass nowAct = _act_stack.Pop();
        BtlActData data = nowAct.Data;

        //�ֻ��� ������ ������ �������� �ʾ��� ���
        if (_nowDice == 0)  //�ּ� �ֻ��� ~ �ִ� �ֻ��� ���̿��� ���� (�ִ� �ֻ����� �ൿ���� ������ ������)
            _nowDice = Random.Range(data.DiceMin,
                                (_ap < data.DiceMax ? _ap : data.DiceMax));

        //�ֻ��� ������ ���� �ൿ ��� ����
        int[] statArr = null;

        switch (nowAct.Stat)
        {
            case Stats.STR:
                statArr = _stat_STR;
                break;
            case Stats.INT:
                statArr = _stat_INT;
                break;
            case Stats.DEX:
                statArr = _stat_DEX;
                break;
            case Stats.AGI:
                statArr = _stat_AGI;
                break;
            case Stats.CON:
                statArr = _stat_CON;
                break;
            case Stats.WIL:
                statArr = _stat_WIL;
                break;
        }

        _result = new int[5] { -1, -1, -1, -1, -1 };

        for (int i = 0; i < _nowDice; i++)
            _result[i] = statArr[Random.Range(0, statArr.Length)];

        //���� �ý��ۿ� �ൿ ���� ����
        _btlSys.Set_BtlAct_Enemy(data, _result);

        //���� �� �ൿ ���� �ʱ�ȭ
        _nowDice = 0;               //�ൿ�� �ֻ��� ���� �ʱ�ȭ

        _result = new int[5] { -1, -1, -1, -1, -1 };    //�ֻ��� �� ���� �ʱ�ȭ
    }

    ///////////////////////BT���� ���Ǵ� �Լ���///////////////////////
    [Task]
    public bool Is_TrueOrFalse() //��������
    {
        if (Random.value < 0.5f) return true;
        else return false;
    }

    [Task]
    public bool Is_Ap_2Under()   //���� �ൿ���� 2 ������ ��
    {
        if (_ap <= 2) return true;
        else return false;
    }

    [Task]
    public bool Is_AtkSuccess() //���� �� ���� ���� ���� ��ȯ
    {
        if (_btlSys.E_HIT_ATK) return true;
        else return false;
    }

    [Task]
    public bool Is_DefSuccess() //���� �� ��� ���� ���� ��ȯ
    {
        if (_btlSys.E_HIT_DEF) return true;
        else return false;
    }

    [Task]
    public bool Is_DgeSuccess() //���� �� ȸ�� ���� ���� ��ȯ
    {
        if (_btlSys.E_HIT_DGE) return true;
        else return false;
    }

    [Task]
    public bool Is_TacSuccess() //���� �� ���� ��� ���� ��ȯ
    {
        if (_btlSys.E_HIT_TAC) return true;
        else return false;
    }

    [Task]
    public bool Is_P_Ap_0Under()  //�÷��̾� �ൿ�� 0���� ���� ��ȯ
    {
        if (PlayerSys.AP <= 0) return true;
        else return false;
    }

    [Task]
    public bool Is_P_Ap_4Over() //�÷��̾� �ൿ�� 
    {
        if (PlayerSys.AP >= 4) return true;
        else return false;
    }

    [Task]
    public bool Is_P_FailDefDge()   //�÷��̾� ���/ȸ�� ���ǹ��ߴ��� ���� ��ȯ
    {
        if ((_btlSys.P_LAST == BtlActData.ActionType.Def && _btlSys.P_HIT_DEF == false) ||
            (_btlSys.P_LAST == BtlActData.ActionType.Dge && _btlSys.P_HIT_DGE == false))
        {
            return true;
        }
        else
            return false;
    }

    [Task]
    public bool Is_P_LastWait() //�÷��̾� ������ �ൿ�� ������� ���� ��ȯ
    {
        if (_btlSys.P_LAST == BtlActData.ActionType.Wait)
            return true;
        else
            return false;
    }

    [Task]
    public bool Push_RandomAction() //������ �ൿ PUSH
    {
        BtlActData.ActionType type = BtlActData.ActionType.No;

        Debug.Log("���� ������ �ൿ Ÿ�� ����: " + _actTypeList.Count);

        if (_actTypeList.Count > 0)
            type = _actTypeList[Random.Range(0, _actTypeList.Count)];

        switch (type)
        {
            case BtlActData.ActionType.No:
                Debug.Log("���� �ƹ� �ൿ�� �� �� ����");
                break;
            case BtlActData.ActionType.Atk:
                Push_RandomAtk();
                break;
            case BtlActData.ActionType.Def:
                Push_RandomDef();
                break;
            case BtlActData.ActionType.Dge:
                Push_RandomDge();
                break;
            case BtlActData.ActionType.Tac:
                Push_RandomTac();
                break;
        }

        return true;
    }

    [Task]
    public bool Push_RandomAtk()    //������ ���� �ൿ PUSH
    {
        BtlActClass act;

        if (_atkAct.Count != 0)    //���� �ൿ�� �������� ���
            act = _atkAct[Random.Range(0, _atkAct.Count)];    //������ ���� �ൿ ����
        else
            return false;

        _act_stack.Push(act);

        var actData = act.Data;

        if (actData.NoDice == false && _ap < actData.DiceMin)    //�ൿ���� �Ҹ��ϸ鼭, ���� �ൿ���� �ּ� �ൿ�º��� ������
        {
            Debug.Log("�ൿ�� �������� ���");
            _act_stack.Push(_waitAct);    //��� �ൿ PUSH
        }

        return true;
    }

    [Task]
    public bool Push_RandomDef()    //������ ��� �ൿ PUSH
    {
        BtlActClass act;

        if (_defAct.Count != 0)    //��� �ൿ�� �������� ���
            act = _defAct[Random.Range(0, _defAct.Count)];    //������ ��� �ൿ ����
        else
            return false;

        var actData = act.Data;

        if (actData.NoDice == false && _ap < actData.DiceMin)    //�ൿ���� �Ҹ��ϸ鼭, ���� �ൿ���� �ּ� �ൿ�º��� ������
        {
            Debug.Log("�ൿ�� �������� ���");
            _act_stack.Push(_waitAct);    //��� �ൿ PUSH
        }
        else
        {
            _act_stack.Push(act);
        }

        return true;
    }

    [Task]
    public bool Push_RandomDge()    //������ ȸ�� �ൿ push
    {
        BtlActClass act;

        if (_dgeAct.Count != 0)    //ȸ�� �ൿ�� �������� ���
            act = _dgeAct[Random.Range(0, _dgeAct.Count)];    //������ ȸ�� �ൿ ����
        else
            return false;

        var actData = act.Data;

        if (actData.NoDice == false && _ap < actData.DiceMin)    //�ൿ���� �Ҹ��ϸ鼭, ���� �ൿ���� �ּ� �ൿ�º��� ������
        {
            Debug.Log("�ൿ�� �������� ���");
            _act_stack.Push(_waitAct);    //��� �ൿ PUSH
        }
        else
        {
            _act_stack.Push(act);
        }

        return true;
    }

    [Task]
    public bool Push_RandomTac()    //������ ���� �ൿ push
    {
        BtlActClass act;

        if (_tacAct.Count != 0)    //���� �ൿ�� �������� ���
            act = _tacAct[Random.Range(0, _tacAct.Count)];    //������ ���� �ൿ ����
        else
            return false;

        _act_stack.Push(act);

        var actData = act.Data;

        if (actData.NoDice == false && _ap < actData.DiceMin)    //�ൿ���� �Ҹ��ϸ鼭, ���� �ൿ���� �ּ� �ൿ�º��� ������
        {
            Debug.Log("�ൿ�� �������� ���");
            _act_stack.Push(_waitAct);    //��� �ൿ PUSH
        }

        return true;
    }

    [Task]
    public bool Push_Wait() //��� �ൿ push
    {
        _act_stack.Push(_waitAct);

        return true;
    }
}
