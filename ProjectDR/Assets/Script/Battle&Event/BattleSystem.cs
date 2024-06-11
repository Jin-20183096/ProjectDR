using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public static BattleSystem BtlSys = null;

    private DungeonSystem _dgnSys;  //���� ���� ��, ������ ������ ���� ��ũ��Ʈ �Ѱܹ޾� ����ؾ���
    private GameObject _camera_dgn; //���� ���� ��, ���� ī�޶� ��ũ��Ʈ�� �Ѱܹ޾� ����ؾ���

    [SerializeField]
    private ActionController _actController;    //�ൿ ��Ʈ�ѷ�
    [SerializeField]
    private DiceResultPannel _p_resultPannel;   //�÷��̾� �ֻ��� ���â
    [SerializeField]
    private DiceResultPannel _e_resultPannel;   //�� �ֻ��� ���â
    [SerializeField]
    private GameLog _btlLog;

    [Header("# Camera")]
    [SerializeField]
    private GameObject _camera_btl;             //���� ī�޶�

    [Header("# Battle UI & Sprite")]
    [SerializeField]
    private SpriteSystem _p_spr;        //�÷��̾� ��������Ʈ
    [SerializeField]
    private SpriteRenderer _p_sprRend;  //�÷��̾� ��������Ʈ ������

    [SerializeField]
    private SpriteSystem _e_spr;         //�� ��������Ʈ
    [SerializeField]
    private SpriteRenderer _e_sprRend;  //�� ��������Ʈ ������

    [SerializeField]
    private GameObject _btn_eventEnd;     //(����)�̺�Ʈ ���� ��ư

    [Header("# Player Info")]
    [SerializeField]
    private PlayerSystem _playerSys;
    [SerializeField]
    private BtlActData _p_act;    //�÷��̾� �ൿ
    [SerializeField]
    private int _p_nowDice;             //�÷��̾� �ֻ��� ����
    public int P_DICE
    {
        get { return _p_nowDice; }
    }
    [SerializeField]
    private int[] _p_result;            //�÷��̾� �ֻ��� ���
    [SerializeField]
    private int _p_total;               //�÷��̾� �ֻ��� ����
    public int P_TOTAL
    {
        get { return _p_total; }
    }

    [Header("# Enemy Info")]
    [SerializeField]
    private EnemySystem _enemySys;
    public string E_NAME
    {
        get { return _enemySys.NAME; }
    }
    [SerializeField]
    private BtlActData _e_act;    //�� �ൿ
    [SerializeField]
    private int _e_nowDice;             //�� �ֻ��� ����
    public int E_DICE
    {
        get { return _e_nowDice; }
    }
    [SerializeField]
    private int[] _e_result;            //�� �ֻ��� ���
    [SerializeField]
    private int _e_total;               //�� �ֻ��� ����
    public int E_TOTAL
    {
        get { return _e_total; }
    }


    [Header("# Battle Condition")]  //���� ó�� ���� ���� ����
    [SerializeField]
    private bool _isNowBattleEnd;
    [SerializeField]
    private bool _effectProcess = false;    //�� ������ false�� ��, ȿ���� ó��
    [SerializeField]
    private bool _battleProcess = false;    //�� ������ false�� ��, ���� �ൿ�� ��ȣ�ۿ��� ó��

    public class BtlActInQueue
    {
        public bool IsPlayer;
        public BtlActData.ActionType Type;
    }

    private Queue<BtlActInQueue> _btlAct_queue;   //�÷��̾�� ���� �ൿ ������ ó���ϴ� ť

    [Header("# Battle Action Condition")]   //���� �ൿ ���� ���� ����
    [SerializeField]
    private bool _p_endAct;         //�÷��̾� �ൿ ó�� �Ϸ� ����
    [SerializeField]
    private bool _e_endAct;         //�� �ൿ ó�� �Ϸ� ����

    [SerializeField]
    private bool _p_hitAtk;         //�÷��̾� �̹� �� ���� ���� ����
    public bool P_HIT_ATK
    {
        get { return _p_hitAtk; }
    }
    [SerializeField]
    private bool _p_makeDmg;        //�÷��̾� �̹� �� ��뿡�� ���ظ� �� ����
    public bool P_MAKE_DMG
    {
        get { return _p_makeDmg; }
    }
    [SerializeField]
    private bool _p_hitDef;         //�÷��̾� �̹� �� ���� ��� ����
    public bool P_HIT_DEF
    {
        get { return _p_hitDef; }
    }
    [SerializeField]
    private bool _p_hitDge;         //�÷��̾� �̹� �� ���� ȸ�� ����
    public bool P_HIT_DGE
    {
        get { return _p_hitDge; }
    }
    [SerializeField]
    private bool _p_hitTac;         //�÷��̾� �̹� �� ���� ��� ����
    public bool P_HIT_TAC
    {
        get { return _p_hitTac; }
    }
    [SerializeField]
    private bool _e_hitAtk;         //�� �̹� �� ���� ���� ����
    public bool E_HIT_ATK
    {
        get { return _e_hitAtk; }
    }
    [SerializeField]
    private bool _e_makeDmg;        //�� �̹� �� ��뿡�� ���ظ� �� ����
    public bool E_MAKE_DMG
    {
        get { return _e_makeDmg; }
    }
    [SerializeField]
    private bool _e_hitDef;         //�� �̹� �� ���� ��� ����
    public bool E_HIT_DEF
    {
        get { return _e_hitDef; }
    }
    [SerializeField]
    private bool _e_hitDge;         //�� �̹� �� ���� ȸ�� ����
    public bool E_HIT_DGE
    {
        get { return _e_hitDge; }
    }
    [SerializeField]
    private bool _e_hitTac;         //�� �̹� �� ���� ��� ����
    public bool E_HIT_TAC
    {
        get { return _e_hitTac; }
    }

    [SerializeField]
    private BtlActData.ActionType _p_lastActType;
    public BtlActData.ActionType P_LAST
    {
        get { return _p_lastActType; }
    }
    [SerializeField]
    private BtlActData.ActionType _e_lastActType;
    public BtlActData.ActionType E_LAST
    {
        get { return _e_lastActType; }
    }

    [Header("# Background")]
    [SerializeField]
    private Transform _tileSet_1_2;
    [SerializeField]
    private Transform _tileSet_1_2_beyond;
    [SerializeField]
    private Transform _tileSet_3;
    [SerializeField]
    private Transform _tileSet_3_beyond;
    [SerializeField]
    private Transform _tileSet_4_5;
    [SerializeField]
    private Transform _tileSet_7_8;
    [SerializeField]
    private Transform _tileSet_9;
    [SerializeField]
    private Transform _tileSet_9_beyond;
    [SerializeField]
    private Transform _tileSet_10_11;
    [SerializeField]
    private Transform _tileSet_10_11_beyond;
    [SerializeField]
    private Transform _tileSet_12;
    [SerializeField]
    private Transform _tileSet_12_beyond;

    void Awake()
    {
        if (BtlSys)
        {
            DestroyImmediate(gameObject);
            return;
        }
        else
        {
            BtlSys = this;
            DontDestroyOnLoad(gameObject);
        }

        _btlAct_queue = new Queue<BtlActInQueue>();
    }

    public void Record_DungeonScript(DungeonSystem dgnSys, GameObject camera)
    {
        _dgnSys = dgnSys;   //���� ������ ��ũ��Ʈ �Ҵ�
        _camera_dgn = camera;   //���� ������ ī�޶� ������Ʈ �Ҵ�
    }

    public void BattleStart(EnemyData enemy)    //���� ����
    {
        _btlLog.gameObject.SetActive(true);

        _dgnSys.EVNT_PROCESS = true;    //���� ��Ȳ ����
        Set_EffectProcess(false);
        Set_BattleProcess(false);

        _camera_btl.SetActive(true);    //���� ī�޶� Ȱ��ȭ.
        _camera_dgn.SetActive(false);   //���� ī�޶� ��Ȱ��ȭ

        _enemySys.Set_BattleEnemy(true, enemy); //���� �� ����ϴ� �� ����

        //���� ó���� ���� ���� ���� �ʱ�ȭ
        //�� �����ൿ, �ֻ��� ����, ȿ�� ó���� ���� ��..
        //������ �ൿ���� �ʱ�ȭ
        _p_act = null;
        _e_act = null;

        //������ �ൿ ���� ���� �ʱ�ȭ
        _p_endAct = false;
        _e_endAct = false;

        _p_hitAtk = false;
        _p_makeDmg = false;
        _p_hitDef = false;
        _p_hitDge = false;
        _p_hitTac = false;
        _e_hitAtk = false;
        _e_makeDmg = false;
        _e_hitDef = false;
        _e_hitDge = false;
        _e_hitTac = false;

        _p_lastActType = BtlActData.ActionType.No;
        _e_lastActType = BtlActData.ActionType.No;

        //�� ���� �α�
        Refresh_Log();
        _btlLog.SetLog_BattleStart(enemy.Name);
        //�� �ൿ ��û
        _enemySys.Request_NextAction();

        //�÷��̾� ���� �ൿ��� Ȱ��ȭ
        _actController.Set_ActListSituation(ActionController.Situation.Battle);
    }

    public void Set_BtlAct_Player(BtlActData act, int[] result)    //�÷��̾� ���� �ൿ ���� �Ϸ�
    {
        //�ൿ ���� ���
        _p_act = act;
        _p_result = result.ToArray();

        _p_nowDice = 0;
        for (int i = 0; i < _p_result.Length; i++)  //�ֻ��� ����
        {
            if (_p_result[i] != -1)
                _p_nowDice++;
            else
                break;
        }

        _p_total = 0;
        for (int i = 0; i < _p_nowDice; i++)    //�ֻ��� ����
            _p_total += _p_result[i];

        //����� �������� �ִ� ���, �� ������ �޾ƿ�

        //�ֻ��� ��� UI ����

        //���� �ൿ ������ �Ϸ��� ���, ���� �ܰ�� ����
        if (_e_act != null)
            Battle_PreProcess();
    }

    public void Set_BtlAct_Enemy(BtlActData act, int[] diceResult)     //�� ���� �ൿ ���� �Ϸ�
    {
        //�ൿ ���� ���
        _e_act = act;
        _e_result = diceResult.ToArray();

        _e_nowDice = 0;
        for (int i = 0; i < _e_result.Length; i++)  //�ֻ��� ����
        {
            if (_e_result[i] != -1)
                _e_nowDice++;
            else
                break;
        }

        _e_total = 0;
        for (int i = 0; i < _e_nowDice; i++)    //�ֻ��� ����
            _e_total += _e_result[i];

        //����� �������� �ִ� ��� �� ������ �޾ƿ�

        //�÷��̾ �ൿ ������ �Ϸ��� ���, ���� �ܰ�� ����
        if (_p_act != null)
            Battle_PreProcess();
    }

    //�÷��̾�� ���� <Ư�� + �ൿ ���� ȿ��> ó��
    public void Battle_PreProcess()
    {
        Change_DiceResult_Enemy();  //���� �ൿ ���� ����

        //���� ������ ��� ó��

        //�÷��̾��� �ൿ ���� ȿ�� ó�� �ڷ�ƾ ����
    }

    //���� ó��
    public void BattleFlow_Start()
    {
        //������ �ൿ ���� ���� �ʱ�ȭ
        _p_endAct = false;
        _e_endAct = false;

        _p_hitAtk = false;
        _p_makeDmg = false;
        _p_hitDef = false;
        _p_hitDge = false;
        _p_hitTac = false;
        _e_hitAtk = false;
        _e_makeDmg = false;
        _e_hitDef = false;
        _e_hitDge = false;
        _e_hitTac = false;

        _p_lastActType = BtlActData.ActionType.No;
        _e_lastActType = BtlActData.ActionType.No;

        //������ �ൿ�� Ư������ ���� ����� �ൿ ������ �ٽ� �ѹ� ǥ��
        Change_DiceResult_Player(); //�÷��̾� �ൿ ���� ��ǥ��
        Change_DiceResult_Enemy();  //�� �ൿ ���� ��ǥ��

        //�ൿ Ÿ���� �켱���� ���� ������ �ൿ�� ���������� ó��
        //�ൿ Ÿ�� �켱�� (���� > ��� > ȸ�� > ���� > ���)
        var actType_speed = new List<BtlActData.ActionType>()
        {
            BtlActData.ActionType.Tac,
            BtlActData.ActionType.Def,
            BtlActData.ActionType.Dge,
            BtlActData.ActionType.Atk,
            BtlActData.ActionType.Wait
        };

        bool p_isSlow = false;  //[�÷��̾� �ӵ� < �� �ӵ�]�� �� true / [�÷��̾� �ӵ� >= �� �ӵ�]�� �� false

        foreach (BtlActData.ActionType type in actType_speed)
        {
            if (_p_act.Type == type)    //�÷��̾�: �� Ÿ��
            {
                if (_e_act.Type == type)    //��: �� Ÿ��
                {
                    if (p_isSlow)   //�� �� ���� �ൿ Ÿ���� ��, �÷��̾ ������
                    {
                        //�� Enqueue
                        _btlAct_queue.Enqueue(new BtlActInQueue()
                        {
                            IsPlayer = false,
                            Type = type
                        });
                        //�÷��̾� Enqueue
                        _btlAct_queue.Enqueue(new BtlActInQueue()
                        {
                            IsPlayer = true,
                            Type = type
                        });
                    }
                    else    //�� �� ���� �ൿ Ÿ���� ��, �÷��̾ ������
                    {
                        //�÷��̾� Enqueue
                        _btlAct_queue.Enqueue(new BtlActInQueue()
                        {
                            IsPlayer = true,
                            Type = type
                        });
                        //�� Enqueue
                        _btlAct_queue.Enqueue(new BtlActInQueue()
                        {
                            IsPlayer = false,
                            Type = type
                        });
                    }
                }
                else    //���� �� �ൿ Ÿ���� �ƴ� ���, �÷��̾� Enqueue
                    //�÷��̾� Enqueue
                    _btlAct_queue.Enqueue(new BtlActInQueue()
                    {
                        IsPlayer = true,
                        Type = type
                    });
            }
            else if (_e_act.Type == type)   //��: �� Ÿ��
                //�� Enqueue
                _btlAct_queue.Enqueue(new BtlActInQueue()
                {
                    IsPlayer = true,
                    Type = type
                });
        }

        _p_spr.Set_ActHitBox(HitBoxCollider.HitBoxType.No); //�÷��̾� �ൿ ��Ʈ�ڽ�: X
        _p_spr.Set_HitBoxState(false);                      //�÷��̾� ��Ʈ�ڽ�: �Ϲ�
        _e_spr.Set_ActHitBox(HitBoxCollider.HitBoxType.No); //�� �ൿ ��Ʈ�ڽ�: X
        _e_spr.Set_HitBoxState(false);                      //�� ��Ʈ�ڽ�: �Ϲ�

        //Act_Dequeue
    }

    /*
    //�÷��̾�� ���� <Ư�� + �ൿ ���� ȿ��> ó��
    public IEnumerator Battle_PostProcess()
    {
        // wait until �÷��̾�� ���� �ൿ�� ��� ���� ������

        //�÷��̾� �ൿ ���� ȿ�� ó�� �ڷ�ƾ ����
    }
    */

    //���� �� ����
    public void BattleFlow_End()
    {
        //�÷��̾� �ֻ��� ���â Off
        //�� �ֻ��� ���â Off

        //������ �ൿ��� ���󺹱�
        switch (_p_act.Type)
        {
            case BtlActData.ActionType.Atk:
                _p_spr.Set_ActionMoveSet_Atk(_p_act.AtkMS, false);
                break;
            case BtlActData.ActionType.Def:
                _p_spr.Set_ActionMoveSet_Def(_p_act.DefMS, false);
                break;
            case BtlActData.ActionType.Dge:
                _p_spr.Set_ActionMoveSet_Dge(_p_act.DgeMS, false);
                break;
            case BtlActData.ActionType.Tac:
                _p_spr.Set_ActionMoveSet_Tac(_p_act.TacMS, false);
                break;
        }
        _p_spr.StartCoroutine(_p_spr.Return_Coroutine());

        switch (_e_act.Type)
        {
            case BtlActData.ActionType.Atk:
                _e_spr.Set_ActionMoveSet_Atk(_e_act.AtkMS, false);
                break;
            case BtlActData.ActionType.Def:
                _e_spr.Set_ActionMoveSet_Def(_e_act.DefMS, false);
                break;
            case BtlActData.ActionType.Dge:
                _e_spr.Set_ActionMoveSet_Dge(_e_act.DgeMS, false);
                break;
            case BtlActData.ActionType.Tac:
                _e_spr.Set_ActionMoveSet_Tac(_e_act.TacMS, false);
                break;
        }
        _e_spr.StartCoroutine(_e_spr.Return_Coroutine());

        //������ �ൿ���� �ʱ�ȭ
        _p_lastActType = _p_act.Type;
        _e_lastActType = _e_act.Type;

        _p_act = null;
        _e_act = null;

        Set_EffectProcess(false);   //�ൿ ȿ�� ó�� ����
        Set_BattleProcess(false);   //���� �ൿ ó�� ����

        //��� �� �� ��� ��, ���� ����
        if (_enemySys.HP <= 0)  //�� ��� ��
        {
            _isNowBattleEnd = true;

            _btlLog.SetLog_BattleEnd(true);     //���� ���� �α� ���
        }
        else if (_playerSys.HP <= 0) //�÷��̾� ��� ��
        {
            _isNowBattleEnd = true;

            _btlLog.SetLog_BattleEnd(false);    //�÷��̾� ��� �α�
        }

        if (_isNowBattleEnd)    //������ ����� ���
        {
            //������ �ൿ ���� ���� �ʱ�ȭ
            _p_endAct = false;
            _e_endAct = false;

            _p_hitAtk = false;
            _p_makeDmg = false;
            _p_hitDef = false;
            _p_hitDge = false;
            _p_hitTac = false;
            _e_hitAtk = false;
            _e_makeDmg = false;
            _e_hitDef = false;
            _e_hitDge = false;
            _e_hitTac = false;

            _p_lastActType = BtlActData.ActionType.No;
            _e_lastActType = BtlActData.ActionType.No;

            _enemySys.Set_BattleEnemy(false, null); //�� ������ Off
            //���� �ൿ ����Ʈ, �ֻ��� ����, �籼�� ��ư, �ൿ ���� ��ư Off
            _actController.Set_ActListSituation(ActionController.Situation.No);
            _actController.Dice_Off();  //�ֻ��� ������Ʈ Off

            _actController.DiceSelectPannel_OnOff(false);   //�ֻ��� ����â Off
            _actController.NoDiceButton_OnOff(false);       //�ֻ��� ���� �ൿ ���� ��ư Off
            _actController.DiceResultPannel_Off();          //�ֻ��� ���â Off

            StopAllCoroutines();

            _btn_eventEnd.SetActive(true);
        }
        else    //������ ������ ���� ���
        {
            //�� ���� �ൿ ��û
            _enemySys.Request_NextAction();
            //�÷��̾� �ൿ��� �����
            _actController.Set_ActListSituation(ActionController.Situation.Battle);
        }
    }

    public void BattleEnd() //���� ����
    {
        
    }

    public void Change_DiceResult_Player()  //�÷��̾� �ֻ��� ���â ���� ����
    {
        DiceResultPannel pannel = _p_resultPannel;
        BtlActData act = _p_act;
        int[] result = _p_result;
        int total = _p_total;
        int nowDice = _p_nowDice;

        //�ֻ��� ���â ���� ����
        pannel.ActionInfoPannel_OnOff(true);        //�ൿ ���â On
        pannel.Change_ActInfo(act.Type, act.Name);  //�ൿ Ÿ�� ������, �ൿ��
        pannel.Change_DiceTotal(act.NoDice ? "" : total.ToString());    //�ൿ�� �ֻ��� ����

        pannel.DiceResultPannel_OnOff(true);    //�ֻ��� ���â On
        pannel.Set_NewDiceTotal(nowDice);       //�ֻ��� ���â �ʱ�ȭ

        for (int i = 0; i < result.Length; i++) //�ֻ��� ��� ����
        {
            if (result[i] != -1)
                pannel.Change_DiceResult(i, result[i]);
        }
    }

    public void Change_DiceResult_Enemy()  //�� �ֻ��� ���â ���� ����
    {
        DiceResultPannel pannel = _e_resultPannel;
        BtlActData act = _e_act;
        int[] result = _e_result;
        int total = _e_total;
        int nowDice = _e_nowDice;

        //�ֻ��� ���â ���� ����
        pannel.ActionInfoPannel_OnOff(true);        //�ൿ ���â On
        pannel.Change_ActInfo(act.Type, act.Name);  //�ൿ Ÿ�� ������, �ൿ��
        pannel.Change_DiceTotal(act.NoDice ? "" : total.ToString());    //�ൿ�� �ֻ��� ����

        pannel.DiceResultPannel_OnOff(true);    //�ֻ��� ���â On
        pannel.Set_NewDiceTotal(nowDice);       //�ֻ��� ���â �ʱ�ȭ

        for (int i = 0; i < result.Length; i++) //�ֻ��� ��� ����
        {
            if (result[i] != -1)
                pannel.Change_DiceResult(i, result[i]);
        }
    }

    public void DiceResult_Off(bool isPlayer)
    {
        DiceResultPannel pannel;

        if (isPlayer)
            pannel = _p_resultPannel;
        else
            pannel = _e_resultPannel;

        pannel.ActionInfoPannel_OnOff(false);   //�ൿ���â Off
        pannel.DiceResultPannel_OnOff(false);   //�ֻ��� ���â Off
    }

    //���� �ý������κ��� �ֺ� ���� ������ �޾�, ���� ����� ������
    public void Set_BattleField(bool c_1_2, bool c_1_2_b, bool c_3, bool c_3_b,
                                bool c_4_5, bool c_7_8,
                                bool c_9, bool c_9_b, bool c_10_11, bool c_10_11_b,
                                bool c_12_b)
    {
        // 1_2
        _tileSet_1_2.GetChild(0).gameObject.SetActive(c_1_2);
        _tileSet_1_2.GetChild(1).gameObject.SetActive(c_1_2);
        // 1_2_beyond
        _tileSet_1_2_beyond.GetChild(0).gameObject.SetActive(c_1_2_b);
        _tileSet_1_2_beyond.GetChild(1).gameObject.SetActive(c_1_2_b);
        // 3
        _tileSet_3.GetChild(0).gameObject.SetActive(c_3);
        _tileSet_3.GetChild(1).gameObject.SetActive(c_3);
        // 3_beyond
        _tileSet_3_beyond.GetChild(0).gameObject.SetActive(c_3_b);
        _tileSet_3_beyond.GetChild(1).gameObject.SetActive(c_3_b);
        // 4_5
        _tileSet_4_5.GetChild(0).gameObject.SetActive(c_4_5);
        // 7_8
        _tileSet_7_8.GetChild(0).gameObject.SetActive(c_7_8);
        // 9
        _tileSet_9.GetChild(0).gameObject.SetActive(c_9);
        _tileSet_9.GetChild(1).gameObject.SetActive(c_9);
        // 9_beyond
        _tileSet_9_beyond.GetChild(0).gameObject.SetActive(c_9_b);
        _tileSet_9_beyond.GetChild(1).gameObject.SetActive(c_9_b);
        // 10_11
        _tileSet_10_11.GetChild(0).gameObject.SetActive(c_10_11);
        _tileSet_10_11.GetChild(1).gameObject.SetActive(c_10_11);
        // 10_11_beyond
        _tileSet_10_11_beyond.GetChild(0).gameObject.SetActive(c_10_11_b);
        _tileSet_10_11_beyond.GetChild(1).gameObject.SetActive(c_10_11_b);
        // 12
        _tileSet_12.GetChild(0).gameObject.SetActive(c_1_2 || c_10_11); //1_2 �Ǵ� 10_11�� õ���� ������ ���
        // 12_beyond
        _tileSet_12_beyond.GetChild(0).gameObject.SetActive(c_12_b);
    }

    //-------------------------�ൿ Ÿ���� �⺻���� ��ȣ�ۿ�-------------------------

    public void Set_BattleProcess(bool b) => _battleProcess = b;

    //-------------------------�ൿ ȿ�� ó��-------------------------

    public void Set_EffectProcess(bool b) => _effectProcess = b;


    void Refresh_Log()
    {
        _btlLog.gameObject.SetActive(false);
        _btlLog.gameObject.SetActive(true);
    }
}
