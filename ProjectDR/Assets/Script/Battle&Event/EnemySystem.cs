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
    private SpriteSystem _e_spr;
    [SerializeField]
    private Animator _e_anima;

    private PandaBehaviour _bt;     //�ൿ Ʈ��

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
    private TextMeshProUGUI _txt_acMax;     //�� �ִ� �� UI �ؽ�Ʈ
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
    private int _acMax;     //�ִ� ��
    public int AC_MAX
    {
        get { return _acMax; }
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
    private List<BtlActData.ActType> _btlActTypeList;    //�����ൿ Ÿ�� ���

    [SerializeField]
    private List<BtlAct> _atkList; //���� �ൿ ���
    [SerializeField]
    private List<BtlAct> _defList; //��� �ൿ ���
    [SerializeField]
    private List<BtlAct> _dgeList; //ȸ�� �ൿ ���
    [SerializeField]
    private List<BtlAct> _tacList; //���� �ൿ ���
    [SerializeField]
    private BtlAct _wait;     //��� �ൿ

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

    Stack<BtlAct> _btlActStack;    //�����ൿ ����
    [SerializeField]
    private string _btlActClueLog;      //�����ൿ �ܼ� �α�
    public string BtlActClueLog
    {
        get { return _btlActClueLog; }
    }

    [Header("# Effect")]
    [SerializeField]
    private Transform _eff_group;
    [SerializeField]
    private ParticleSystem _eff_hit;
    [SerializeField]
    private ParticleSystem _eff_block;
    [SerializeField]
    private GameObject _dmgText_prefab;

    void Awake()
    {
        _btlActStack = new Stack<BtlAct>();
    }

    public void Set_BattleEnemy(bool isNowBattle, EnemyData new_enemy)
    {
        _pannel_info.SetActive(isNowBattle);    //�� �⺻����â on/off

        if (isNowBattle)
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
            _acMax = 0;
            _ap = 0;
            _apMax = 0;

            //�ൿ Ʈ�� �Ҵ� off
            Destroy(_bt);
            _bt = null;

            _btlActTypeList.Clear(); //������ �����ൿ Ÿ�� ��� �ʱ�ȭ

            _e_spr.gameObject.SetActive(false);  //�� ��������Ʈ off
        }
    }

    public void Set_EnemyInfo(int hpMax)
    {
        //HP
        _hpMax = hpMax;
        _hp = _hpMax;
        //��
        _acMax = _data.AC;
        _ac = _acMax;
        //�ൿ��
        _apMax = _data.ApMax;
        _ap = Random.Range(1, _apMax + 1);

        //�����ൿ Ÿ�� ��� ����
        _btlActTypeList.Clear(); //�����ൿ Ÿ�� ��� �ʱ�ȭ

        _atkList.Clear();
        _atkList = _data.Act_Atk.ToList();  //���� �ൿ ��Ͽ� �߰�
        if (_atkList.Count > 0) _btlActTypeList.Add(BtlActData.ActType.Atk);

        _defList.Clear();
        _defList = _data.Act_Def.ToList();  //��� �ൿ ��Ͽ� �߰�
        if (_defList.Count > 0) _btlActTypeList.Add(BtlActData.ActType.Def);

        _dgeList.Clear();
        _dgeList = _data.Act_Dge.ToList();  //ȸ�� �ൿ ��Ͽ� �߰�
        if (_dgeList.Count > 0) _btlActTypeList.Add(BtlActData.ActType.Dge);

        _tacList.Clear();
        _tacList = _data.Act_Tac.ToList();  //���� �ൿ ��Ͽ� �߰�
        if (_tacList.Count > 0) _btlActTypeList.Add(BtlActData.ActType.Tac);

        //���� ������ ���� ����
        _txt_name.text = NAME;  //�̸�
        Change_HpMax(true, 0);
        Change_Hp(true, 0);
        _hpMask.fillAmount = _hp / (float)_hpMax;
        _hpMeter.fillAmount = _hp / (float)_hpMax;
        Change_AC(true, 0);
        Change_ACMax(true, 0);
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

    public bool IsPlayer() => false;    //�÷��̾��ΰ�? => False

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

        _txt_ac.text = _ac.ToString();
    }

    public void Change_ACMax(bool plus, int value)
    {
        //����ŭ ����
        if (plus)
            _acMax += value;
        else
            _acMax -= value;

        /*
        if (_ac >= _acMax)      //�ִ� ���� ���� ������ �������� ���
            _ac = _acMax;       //������ �ִ� ����ŭ ���� ���� ����
        */

        //����� ����ŭ ��ġ ����
        _icon_ac.SetActive(_acMax != 0);
        if (_acMax != 0)
            _txt_acMax.text = "/ " + _acMax.ToString();
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

        //�ൿ�� ���� ����
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
        if (_ac >= 1)    //���� 1 �̻� ������ ��
        {
            int realDmg;    //���� ���ط�

            if (dmg > _ac)  //���ط��� ������ ���� ��
            {
                realDmg = dmg - _ac;    //���� ���ط��� ���� ������ ������ ��ġ

                Change_AC(false, _ac);  //���� ��� ����

                Change_Hp(false, realDmg);  //���� ���ط���ŭ hp �پ��
            }
            else            //���ط��� �� ������ ��
            {
                //���� ���ط��� 0 (���� ��� ���ظ� �氨)

                Change_AC(false, dmg);  //���ط���ŭ ���� ����
            }
        }
        else    //���� ���� ��
            Change_Hp(false, dmg);

        ParticleSystem eff = null;

        if (dmgType == BtlActData.DamageType.Defense)
            eff = Instantiate(_eff_block, _eff_group);  //��� ����Ʈ ��ƼŬ
        else
            eff = Instantiate(_eff_hit, _eff_group);

        var pos = _e_spr.transform.position;
        var sizeY = _e_spr.GetComponent<SpriteRenderer>().bounds.size.y;
        eff.transform.position = new Vector3(pos.x + 0.5f, pos.y + sizeY / 2, pos.z);
        eff.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
        eff.Play();

        //������ Ÿ�Կ� ���� �ǰ���

        //�ǰ� ȿ��
        _e_spr.StartCoroutine(_e_spr.HitFlash());

        //������ ��Ʈ
        var dmgText = Instantiate(_dmgText_prefab.transform, _eff_group);

        //�������ؽ�Ʈ ��ǥ ����
        dmgText.position = _e_spr.transform.position;
        //�������ؽ�Ʈ �� ����
        dmgText.GetChild(0).GetComponent<TextMeshPro>().text = dmg.ToString();
        dmgText.GetChild(1).GetComponent<TextMeshPro>().text = dmg.ToString();
    }

    public void Request_NextBtlAct()    //���� �ý��ۿ��� ���� �� �����ൿ�� ��û
    {
        _bt.scripts = new TextAsset[] { _data.BT };
        _bt.Compile();
        _bt.Apply();

        if (_btlActStack.Count == 0)  //�����ൿ ���ÿ� �ൿ�� ���� ���
        {
            //�ൿƮ���� ���� �����ൿ ����
            _bt.Tick();
        }

        var peek = _btlActStack.Peek().Data;    //�����ൿ ���� �� ���� �ൿ
        Debug.Log("���� ���� �� �����ൿ: " + peek.Name);

        //���� �� ���� �����ൿ�� �ൿ���� �Ҹ��ϰ�
        //���� �ൿ���� �ּ� �ൿ�º��� ������
        if (peek.NoDice == false && _ap < peek.DiceMin)  
        {
            _btlActStack.Push(_wait);    //��� �ൿ PUSH
            Debug.Log("������ �ൿ���� ������ " + _wait.Data.Name + " �ϱ�� ��");
        }

        BtlAct nowAct = _btlActStack.Pop();
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

        //���� �ý��ۿ� �����ൿ ���� ����
        _btlSys.SetBtlAct_Enemy(data, _result);

        //���� �� �����ൿ ���� �ʱ�ȭ
        _nowDice = 0;               //�����ൿ�� �ֻ��� ���� �ʱ�ȭ

        _result = new int[5] { -1, -1, -1, -1, -1 };    //�ֻ��� �� ���� �ʱ�ȭ
    }

    ///////////////////////BT���� ���Ǵ� �Լ���///////////////////////
    [Task]
    public bool Is_50Percent() //50% Ȯ����
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
    public bool Is_AtkSuccess() //���� �� ���� ���� �� True
    {
        if (_btlSys.E_HIT_ATK) return true;
        else return false;
    }

    [Task]
    public bool Is_DefSuccess() //���� �� ��� ���� �� True
    {
        if (_btlSys.E_HIT_DEF) return true;
        else return false;
    }

    [Task]
    public bool Is_DgeSuccess() //���� �� ȸ�� ���� �� True
    {
        if (_btlSys.E_HIT_DGE) return true;
        else return false;
    }

    [Task]
    public bool Is_TacSuccess() //���� �� ���� ��� �� True
    {
        if (_btlSys.E_HIT_TAC) return true;
        else return false;
    }

    [Task]
    public bool Is_P_Ap_0Under()  //�÷��̾� �ൿ�� 0 ���ϸ� True
    {
        if (PlayerSys.AP <= 0) return true;
        else return false;
    }

    [Task]
    public bool Is_P_Ap_2Under()    //�÷��̾� �ൿ�� 2 ���ϸ� True
    {
        if (PlayerSys.AP <= 2) return true;
        else return false;
    }

    [Task]
    public bool Is_P_Ap_3Over()     //�÷��̾� �ൿ�� 3 �̻��̸� True
    {
        if (PlayerSys.AP >= 3) return true;
        else return false;
    }

    [Task]
    public bool Is_P_Ap_4Over()     //�÷��̾� �ൿ�� 4 �̻��̸� True
    {
        if (PlayerSys.AP >= 4) return true;
        else return false;
    }

    [Task]
    public bool Is_P_FailDefDge()   //�÷��̾� ���/ȸ�� ���ǹ��ߴٸ� True
    {
        if ((_btlSys.P_LAST == BtlActData.ActType.Def && _btlSys.P_HIT_DEF == false) ||
            (_btlSys.P_LAST == BtlActData.ActType.Dge && _btlSys.P_HIT_DGE == false))
        {
            return true;
        }
        else
            return false;
    }

    [Task]
    public bool Is_P_LastWait() //�÷��̾� ������ �����ൿ�� ���� True
    {
        if (_btlSys.P_LAST == BtlActData.ActType.Wait)
            return true;
        else
            return false;
    }

    [Task]
    public bool Push_RandomBtlAct() //������ �����ൿ PUSH 
    {
        BtlActData.ActType type = BtlActData.ActType.No;

        Debug.Log("���� ������ �����ൿ Ÿ�� ����: " + _btlActTypeList.Count);

        if (_btlActTypeList.Count > 0)
            type = _btlActTypeList[Random.Range(0, _btlActTypeList.Count)];

        _btlActClueLog = "";
        switch (type)
        {
            case BtlActData.ActType.No:
                Debug.Log("���� �ƹ� �ൿ�� �� �� ����");
                break;
            case BtlActData.ActType.Atk:
                Push_RandomAtk();
                break;
            case BtlActData.ActType.Def:
                Push_RandomDef();
                break;
            case BtlActData.ActType.Dge:
                Push_RandomDge();
                break;
            case BtlActData.ActType.Tac:
                Push_RandomTac();
                break;
        }

        return true;
    }

    [Task]
    public bool Push_RandomAtk()    //������ ���� �ൿ PUSH
    {
        BtlAct act;

        if (_atkList.Count != 0)    //���� �ൿ�� �������� ���
            act = _atkList[Random.Range(0, _atkList.Count)];    //������ ���� �ൿ ����
        else
            return false;

        _btlActStack.Push(act);

        var actData = act.Data;

        if (actData.NoDice == false && _ap < actData.DiceMin)    //�ൿ���� �Ҹ��ϸ鼭, ���� �ൿ���� �ּ� �ൿ�º��� ������
        {
            Debug.Log("�ൿ�� �������� ���");
            Push_Wait();
        }

        return true;
    }

    [Task]
    public bool Push_RandomDef()    //������ ��� �ൿ PUSH
    {
        BtlAct act;

        if (_defList.Count != 0)    //��� �ൿ�� �������� ���
            act = _defList[Random.Range(0, _defList.Count)];    //������ ��� �ൿ ����
        else
            return false;

        var actData = act.Data;

        if (actData.NoDice == false && _ap < actData.DiceMin)    //�ൿ���� �Ҹ��ϸ鼭, ���� �ൿ���� �ּ� �ൿ�º��� ������
        {
            Debug.Log("�ൿ�� �������� ���");
            Push_Wait();
        }
        else
        {
            _btlActStack.Push(act);
        }

        return true;
    }

    [Task]
    public bool Push_RandomDge()    //������ ȸ�� �ൿ push
    {
        BtlAct act;

        if (_dgeList.Count != 0)    //ȸ�� �ൿ�� �������� ���
            act = _dgeList[Random.Range(0, _dgeList.Count)];    //������ ȸ�� �ൿ ����
        else
            return false;

        var actData = act.Data;

        if (actData.NoDice == false && _ap < actData.DiceMin)    //�ൿ���� �Ҹ��ϸ鼭, ���� �ൿ���� �ּ� �ൿ�º��� ������
        {
            Debug.Log("�ൿ�� �������� ���");
            Push_Wait();
        }
        else
        {
            _btlActStack.Push(act);
        }

        return true;
    }

    [Task]
    public bool Push_RandomTac()    //������ ���� �ൿ push
    {
        BtlAct act;

        if (_tacList.Count != 0)    //���� �ൿ�� �������� ���
            act = _tacList[Random.Range(0, _tacList.Count)];    //������ ���� �ൿ ����
        else
            return false;

        var actData = act.Data;

        if (actData.NoDice == false && _ap < actData.DiceMin)    //�ൿ���� �Ҹ��ϸ鼭, ���� �ൿ���� �ּ� �ൿ�º��� ������
        {
            Debug.Log("�ൿ�� �������� ���");
            Push_Wait();
        }
        else
        {
            _btlActStack.Push(act);
        }

        return true;
    }

    [Task]
    public bool Push_Wait() //��� �ൿ push
    {
        _btlActClueLog = "";
        _btlActStack.Push(_wait);

        return true;
    }

    [Task]
    public bool Set_ActClueLog0()
    {
        if (_data.ActClueLog.Length > 0) _btlActClueLog = _data.ActClueLog[0];

        return true;
    }

    [Task]
    public bool Set_ActClueLog1()
    {
        if (_data.ActClueLog.Length > 1) _btlActClueLog = _data.ActClueLog[1];

        return true;
    }

    [Task]
    public bool Set_ActClueLog2()
    {
        if (_data.ActClueLog.Length > 2) _btlActClueLog = _data.ActClueLog[2];

        return true;
    }

    [Task]
    public bool Set_ActClueLog3()
    {
        if (_data.ActClueLog.Length > 3) _btlActClueLog = _data.ActClueLog[3];

        return true;
    }

    [Task]
    public bool Set_ActClueLog4()
    {
        if (_data.ActClueLog.Length > 4) _btlActClueLog = _data.ActClueLog[4];

        return true;
    }

    [Task]
    public bool Set_ActClueLog5()
    {
        if (_data.ActClueLog.Length > 5) _btlActClueLog = _data.ActClueLog[5];

        return true;
    }
}
