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
    private GameLog _btlLog;    //���� �α�
    [SerializeField]
    private ItemSystem _itemSys;                //������ �ý���
    [SerializeField]
    private RewardPannel _rewardPannel;         //����ǰ â

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
    private bool _p_isSlow;     //�÷��̾� �켱���� �� ������ ����
    [SerializeField]
    private bool _effectProcess = false;    //�� ������ false�� ��, ȿ���� ó��
    [SerializeField]
    private bool _battleProcess = false;    //�� ������ false�� ��, ���� �ൿ�� ��ȣ�ۿ��� ó��
    [SerializeField]
    private bool _rewardExpProcess = false; //�� ������ true��, �� óġ ����ġó�� ���ΰ�

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
    private bool _p_hitWait;        //�÷��̾� �̹� �� ��� ��� ����
    public bool P_HIT_WAIT
    {
        get { return _p_hitWait; }
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
    private bool _e_hitWait;        //�� �̹� �� ��� ��� ����
    public bool E_HIT_WAIT
    {
        get { return _e_hitWait; }
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

        //�÷��̾� �޴� ��ư Off
        _playerSys.MenuButton_OnOff_Status(false);
        _playerSys.MenuButton_OnOff_Inventory(false);
        _playerSys.MenuButton_OnOff_ActList(false);

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
        _p_hitWait = false;
        _e_hitAtk = false;
        _e_makeDmg = false;
        _e_hitDef = false;
        _e_hitDge = false;
        _e_hitTac = false;
        _e_hitWait = false;

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

        //�켱�� üũ
        //���� �켱���� �÷��̾�� �� ������
        _p_isSlow = true;
        //�ƴϸ�
        _p_isSlow = false;

        //���� ������ ��� ó��

        //�÷��̾��� ȿ�� ��ó�� �ڷ�ƾ ����
        if (_p_isSlow)
            StartCoroutine(AbilityProcess_Enemy(true));
        else
            StartCoroutine(AbilityProcess_Player(true));
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
        _p_hitWait = false;
        _e_hitAtk = false;
        _e_makeDmg = false;
        _e_hitDef = false;
        _e_hitDge = false;
        _e_hitTac = false;
        _e_hitWait = false;

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

        foreach (BtlActData.ActionType type in actType_speed)
        {
            if (_p_act.Type == type)    //�÷��̾�: �� Ÿ��
            {
                if (_e_act.Type == type)    //��: �� Ÿ��
                {
                    if (_p_isSlow)   //�� �� ���� �ൿ Ÿ���� ��, �÷��̾ ������
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
                    IsPlayer = false,
                    Type = type
                });
        }

        _p_spr.Set_ActHitBox(HitBoxCollider.HitBoxType.No); //�÷��̾� �ൿ ��Ʈ�ڽ�: X
        _p_spr.Set_HitBoxState(false);                      //�÷��̾� ��Ʈ�ڽ�: �Ϲ�
        _e_spr.Set_ActHitBox(HitBoxCollider.HitBoxType.No); //�� �ൿ ��Ʈ�ڽ�: X
        _e_spr.Set_HitBoxState(false);                      //�� ��Ʈ�ڽ�: �Ϲ�

        StartCoroutine(Act_Dequeue());  //�켱���� ���� �ൿ�� ���������� ó��
    }

    //�÷��̾�� ���� <Ư�� + �ൿ ���� ȿ��> ó��
    public IEnumerator Battle_PostProcess()
    {
        //�÷��̾�� ���� �ൿ�� ��� ���� ������ ���
        yield return new WaitUntil(() => _p_endAct && _e_endAct);


        Debug.Log("�÷��̾�� ���� �ൿ�� ��� ����");

        //�÷��̾� �ൿ ���� ȿ�� ó�� �ڷ�ƾ ����
        if (_p_isSlow)
            StartCoroutine(AbilityProcess_Enemy(false));
        else
            StartCoroutine(AbilityProcess_Player(false));
    }

    //���� �� ����
    public void BattleFlow_End()
    {
        DiceResult_Off(true);   //�÷��̾� �ֻ��� ���â Off
        DiceResult_Off(false);  //�� �ֻ��� ���â Off

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
        if (_enemySys.HP <= 0 || _playerSys.HP <= 0)
        {
            //������ �ൿ ���� ���� �ʱ�ȭ
            _p_endAct = false;
            _e_endAct = false;

            _p_hitAtk = false;
            _p_makeDmg = false;
            _p_hitDef = false;
            _p_hitDge = false;
            _p_hitTac = false;
            _p_hitWait = false;
            _e_hitAtk = false;
            _e_makeDmg = false;
            _e_hitDef = false;
            _e_hitDge = false;
            _e_hitTac = false;
            _e_hitWait = false;

            _p_lastActType = BtlActData.ActionType.No;
            _e_lastActType = BtlActData.ActionType.No;

            //�÷��̾� �޴� ��ư On
            _playerSys.MenuButton_OnOff_Status(true);
            _playerSys.MenuButton_OnOff_Inventory(true);
            _playerSys.MenuButton_OnOff_ActList(true);

            var enemy = _enemySys.Data;

            _enemySys.Set_BattleEnemy(false, null); //�� ������ Off
            //���� �ൿ ����Ʈ, �ֻ��� ����, �籼�� ��ư, �ൿ ���� ��ư Off
            _actController.Set_ActListSituation(ActionController.Situation.No);
            _actController.Dice_Off();  //�ֻ��� ������Ʈ Off

            _actController.DiceSelectPannel_OnOff(false);   //�ֻ��� ����â Off
            _actController.NoDiceButton_OnOff(false);       //�ֻ��� ���� �ൿ ���� ��ư Off
            _actController.DiceResultPannel_Off();          //�ֻ��� ���â Off

            StopAllCoroutines();

            if (_enemySys.HP <= 0)  //�� ��� ��
            {
                _btlLog.SetLog_BattleEnd(true);     //�� ��� �α�

                StartCoroutine(Enemy_Reward(enemy));
            }
            else    //�÷��̾� ��� ��
            {
                _btlLog.SetLog_BattleEnd(false);    //�÷��̾� ��� �α�

                _btn_eventEnd.SetActive(true);
            }
        }
        else    //������ ������ ���� ���
        {
            //�� ���� �ൿ ��û
            _enemySys.Request_NextAction();
            //�÷��̾� �ൿ��� �����
            _actController.Set_ActListSituation(ActionController.Situation.Battle);
        }
    }

    public void Set_RewardExpProcess(bool b) => _rewardExpProcess = b;

    IEnumerator Enemy_Reward(EnemyData enemy)
    {
        var data = enemy;

        //����ġ ȹ��
        var exp = Random.Range(data.Exp[0], data.Exp[1]);
        var amount = Random.Range(1, 4);
        _rewardPannel.RewardPannel_Exp_OnOff(true);     //����ġ ȹ�� �г� On

        _rewardPannel.Set_RewardExpInfo();  //����ġ ȹ�� �г��� ��ġ ����
        _rewardPannel.Set_GetExpText(exp);//ȹ�� ����ġ ǥ��

        //��� ����ġ ȹ���� ���� ������ ���
        yield return new WaitUntil(() => _rewardExpProcess == false);

        _rewardPannel.RewardPannel_Item_OnOff(true);    //������ ȹ�� �г� On
        _itemSys.Reward_Clear();    //���� ����ǰ ��� ����
        _itemSys.ON_REWARD = true;  //����ǰâ ON ����

        for (int i = 0; i < amount; i++)    //����� ������ ������ŭ ������ ���
            _itemSys.Reward_Item(data.Item[Random.Range(0, data.Item.Length)], i);

        _itemSys.Set_RewardIcon();  //����� ������ ǥ��

        //---------------

        //������ �ൿ ���� ���� �ʱ�ȭ
        _p_endAct = false;
        _e_endAct = false;

        _p_hitAtk = false;
        _p_makeDmg = false;
        _p_hitDef = false;
        _p_hitDge = false;
        _p_hitTac = false;
        _p_hitWait = false;
        _e_hitAtk = false;
        _e_makeDmg = false;
        _e_hitDef = false;
        _e_hitDge = false;
        _e_hitTac = false;
        _e_hitWait = false;

        _p_lastActType = BtlActData.ActionType.No;
        _e_lastActType = BtlActData.ActionType.No;

        //�÷��̾� �޴� ��ư On
        _playerSys.MenuButton_OnOff_Status(true);
        _playerSys.MenuButton_OnOff_Inventory(true);
        _playerSys.MenuButton_OnOff_ActList(true);

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

    public void BattleEnd() //���� ����
    {
        _btlLog.gameObject.SetActive(false);
        _camera_dgn.SetActive(true);    //���� ī�޶� Ȱ��ȭ
        _camera_btl.SetActive(false);   //���� ī�޶� ��Ȱ��ȭ

        _dgnSys.EVNT_PROCESS = false;   //�̺�Ʈ ���� ��Ȳ ����

        StopAllCoroutines();

        _itemSys.Reward_Clear();    //����ǰ ��� ����
        _rewardPannel.RewardPannel_Exp_OnOff(false);
        _rewardPannel.RewardPannel_Item_OnOff(false);

        _btn_eventEnd.SetActive(false);
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
                pannel.Set_StopDiceResult(i, result[i]);
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
                pannel.Set_StopDiceResult(i, result[i]);
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

    public IEnumerator Act_Dequeue()
    {
        while (_btlAct_queue.Count > 0)
        {
            yield return new WaitUntil(() => _battleProcess == false);

            //ť���� �ൿ�� �ϳ� Dequeue
            var act = _btlAct_queue.Dequeue();
            Debug.Log((act.IsPlayer ? "�÷��̾��� " : "���� ") + act.Type);

            //�� �ൿ�� Ÿ�Կ� ����, ����ڰ� �ش� �ൿ�� ����ߴٴ� ���� �ڷ�ƾ���� ó��
            switch (act.Type)
            {
                case BtlActData.ActionType.Atk:
                    StartCoroutine(Atk(act.IsPlayer));
                    break;
                case BtlActData.ActionType.Def:
                    StartCoroutine(Def(act.IsPlayer));
                    break;
                case BtlActData.ActionType.Dge:
                    StartCoroutine(Dge(act.IsPlayer));
                    break;
                case BtlActData.ActionType.Tac:
                    StartCoroutine(Tac(act.IsPlayer));
                    break;
                case BtlActData.ActionType.Wait:
                    StartCoroutine(Wait(act.IsPlayer));
                    break;
            }
        }

        StartCoroutine(Battle_PostProcess());
    }

    public IEnumerator Tac(bool isPlayer)   //����
    {
        yield return new WaitUntil(() => _battleProcess == false);  //�ٸ� �ൿ ó���� ���� ������ ���

        //���� ó�� ����
        Set_BattleProcess(true);

        if (isPlayer)   //�÷��̾ ���� �ൿ ���
        {
            //�÷��̾� �ൿ�� �Ҹ� ���ο� ���� �ൿ���� �Ҹ�
            if (_p_act.NoDice == false)
            {
                _playerSys.Change_Ap(false, _p_nowDice);    //�ൿ�� �Ҹ�
                _playerSys.Change_Ap_UsePreview(0);         //�Ҹ� ���� �ൿ�� ǥ�� Off
            }

            _p_act.Effect_Tac(true, this);
            _p_hitTac = true;
        }
        else            //���� ���� �ൿ ���
        {
            //�� �ൿ�� �Ҹ� ���ο� ���� �ൿ���� �Ҹ�
            if (_e_act.NoDice == false)
                _playerSys.Change_Ap(false, _e_nowDice);

            _e_act.Effect_Tac(false, this);
            _e_hitTac = true;
        }

        yield return new WaitUntil(() => _battleProcess == false);  //�α� ����� ���������� ���

        //�ൿ ���� ó��
        if (isPlayer) _p_endAct = true;
        else _e_endAct = true;
    }

    public IEnumerator Def(bool isPlayer)   //���
    {
        yield return new WaitUntil(() => _battleProcess == false);  //�ٸ� �ൿ ó���� ���������� ���

        //��� ���� ����
        if (isPlayer)
        {
            //�÷��̾� �ൿ�� �Ҹ� ���ο� ���� �ൿ���� �Ҹ�
            if (_p_act.NoDice == false)
            {
                _playerSys.Change_Ap(false, _p_nowDice); //�ൿ�� �Ҹ�
                _playerSys.Change_Ap_UsePreview(0);      //�Ҹ� ���� �ൿ�� ǥ�� Off
            }

            if (_e_act.Type == BtlActData.ActionType.Atk)   //���� ������ �� ���
            {
                _p_spr.Set_ActHitBox(HitBoxCollider.HitBoxType.Def);    //�÷��̾� �ൿ ��Ʈ�ڽ�: ���
                _p_spr.Set_HitBoxState(true);                           //�÷��̾� ��Ʈ�ڽ�: ���
                //�÷��̾� ��� �����
                _p_spr.ActHitBoxOn();
            }
            else    //���� ������ ���� ���� ���
            {
                //��� ���� ó��
                Set_BattleProcess(true);
                var log = _btlLog.Log_DefFail(true);    //�÷��̾� ��� ���� �α�

                Refresh_Log();
                _btlLog.SetLog_BattleFlow(log); //�α� ���
                yield return new WaitUntil(() => _battleProcess == false);  //�α� ����� ���� ������ ���
                _p_endAct = true;   //�ൿ ���� ó��
            }
        }
        else
        {
            //���� �ൿ�� �Ҹ� ���ο� ���� �ൿ���� �Ҹ�
            if (_e_act.NoDice == false)
                _enemySys.Change_Ap(false, _e_nowDice);

            if (_p_act.Type == BtlActData.ActionType.Atk)   //�÷��̾ ������ �� ���
            {
                _e_spr.Set_ActHitBox(HitBoxCollider.HitBoxType.Def);    //�� �ൿ ��Ʈ�ڽ�: ���
                _e_spr.Set_HitBoxState(true);                           //�� ��Ʈ�ڽ�: ���
                //�� ��� �����
                _e_spr.ActHitBoxOn();
            }
            else    //�÷��̾ ������ ���� �ʾ��� ���
            {
                //��� ���� ó��
                Set_BattleProcess(true);
                var log = _btlLog.Log_DefFail(false);   //�� ��� ���� �α�

                Refresh_Log();
                _btlLog.SetLog_BattleFlow(log); //�α� ���
                yield return new WaitUntil(() => _battleProcess == false);  //�α� ����� ���������� ���
                _e_endAct = true;   //�ൿ ���� ó��
            }
        }
    }

    public IEnumerator Dge(bool isPlayer)   //ȸ��
    {
        yield return new WaitUntil(() => _battleProcess == false);  //�ٸ� �ൿ ó���� ���� ������ ���

        //ȸ�� ���� ���� (�� ���¿��� ���� �޾��� ��, ȸ�� üũ �ڷ�ƾ�� ȣ���ϴ� ���)
        if (isPlayer)
        {
            //�÷��̾� �ൿ�� �Ҹ� ���ο� ���� �ൿ���� �Ҹ�
            if (_p_act.NoDice == false)
            {
                _playerSys.Change_Ap(false, _p_nowDice); //�ൿ�� �Ҹ�
                _playerSys.Change_Ap_UsePreview(0);      //�Ҹ� ���� �ൿ�� ǥ�� Off
            }

            if (_e_act.Type == BtlActData.ActionType.Atk) //���� ������ �� ���
                _p_spr.Set_HitBoxState(true);                       //�÷��̾� ��Ʈ�ڽ�: ȸ��
            else    //���� ������ ���� ���� ���
            {
                //ȸ�� ���� ó��
                Set_BattleProcess(true);

                var dgePos = new Vector3(_p_spr.transform.position.x - 3f, _p_spr.transform.position.y, _p_spr.transform.position.z);
                _p_spr.Set_SpriteMove(dgePos);

                var log = _btlLog.Log_DgeFail(true);    //�÷��̾� ȸ�� ���� �α�

                Refresh_Log();
                _btlLog.SetLog_BattleFlow(log); //�α� ���
                yield return new WaitUntil(() => _battleProcess == false);  //�α� ����� ���������� ���
                _p_endAct = true;   //�ൿ ���� ó��
            }
        }
        else
        {
            //�� �ൿ�� �Ҹ� ���ο� ���� �ൿ���� �Ҹ�
            if (_e_act.NoDice == false)
                _enemySys.Change_Ap(false, _e_nowDice);

            if (_p_act.Type == BtlActData.ActionType.Atk) //�÷��̾ ������ �� ���
                _e_spr.Set_HitBoxState(true);                       //�� ��Ʈ�ڽ�: ȸ��
            else    //�÷��̾ ������ ���� ���� ���
            {
                //ȸ�� ���� ó��
                Set_BattleProcess(true);

                var dgePos = new Vector3(_e_spr.transform.position.x + 3f, _e_spr.transform.position.y, _e_spr.transform.position.z);
                _e_spr.Set_SpriteMove(dgePos);
                _e_spr.Set_ActionMoveSet_Dge(_e_act.DgeMS, true);

                var log = _btlLog.Log_DgeFail(false);   //�� ȸ�� ���� �α�

                Refresh_Log();
                _btlLog.SetLog_BattleFlow(log); //�α� ���
                yield return new WaitUntil(() => _battleProcess == false);  //�α� ����� ���������� ���
                _e_endAct = true;   //�ൿ ���� ó��
            }
        }
    }

    public IEnumerator Atk(bool isPlayer)   //����
    {
        yield return new WaitUntil(() => _battleProcess == false);  //�ٸ� �ൿ ó���� ���� ������ ���

        Set_BattleProcess(true);

        Vector3 dest;

        //���� ���� ���� (�� ���¿��� �ൿ ��Ʈ�ڽ��� �浹���� ��, ���� üũ �ڷ�ƾ�� ȣ���ϴ� ���)
        if (isPlayer)
        {
            if (_playerSys.HP <= 0)
            {
                _p_endAct = true;
                yield break;
            }

            //�÷��̾� �ൿ�� �Ҹ� ���ο� ���� �ൿ���� �Ҹ�
            if (_p_act.NoDice == false)
            {
                _playerSys.Change_Ap(false, _p_nowDice); //�ൿ�� �Ҹ�
                _playerSys.Change_Ap_UsePreview(0);      //�Ҹ� ���� �ൿ�� ǥ�� Off
            }

            var pos = _e_spr.transform.position;
            dest = new Vector3(pos.x - _e_sprRend.bounds.size.x, pos.y, pos.z);

            _p_spr.ActHitBoxOn();
            _p_spr.Set_ActHitBox(HitBoxCollider.HitBoxType.Atk);    //�÷��̾� �ൿ ��Ʈ�ڽ�: ����
            _p_spr.Set_SpriteMove(dest);                            //�÷��̾��� ������ ���� �̵�
            _p_spr.Set_ActionMoveSet_Atk(_p_act.AtkMS, true);       //���� �����

        }
        else
        {
            if (_enemySys.HP <= 0)
            {
                _e_endAct = true;
                yield break;
            }

            //�� �ൿ�� �Ҹ� ���ο� ���� �ൿ���� �Ҹ�
            if (_e_act.NoDice == false)
                _enemySys.Change_Ap(false, _e_nowDice);

            var pos = _p_spr.transform.position;
            dest = new Vector3(pos.x + _p_sprRend.bounds.size.x, pos.y, pos.z);

            _e_spr.ActHitBoxOn();
            _e_spr.Set_ActHitBox(HitBoxCollider.HitBoxType.Atk);    //�� �ൿ ��Ʈ�ڽ�: ����
            _e_spr.Set_SpriteMove(dest);                            //���� ������ ���� �̵�
            _e_spr.Set_ActionMoveSet_Atk(_e_act.AtkMS, true);       //���� �����
        }
    }

    public IEnumerator Wait(bool isPlayer)  //���
    {
        yield return new WaitUntil(() => _battleProcess == false);  //�ٸ� �ൿ ó���� ���� ������ ���

        //��� ó�� ����
        Set_BattleProcess(true);
        var log = "";

        if (isPlayer)   //�÷��̾ ��� �ൿ ���
        {
            if (_playerSys.HP <= 0)
            {
                _p_endAct = true;
                yield break;
            }

            //��� �ൿ���� ���ո�ŭ �ൿ�� ȸ��
            _playerSys.Change_Ap(true, 2);

            //��� �α� �߰�
            log += _btlLog.Log_Wait(true, 2);

            _p_hitWait = true;  //�÷��̾� ��� ó��
        }
        else            //���� ��� �ൿ ���
        {
            if (_enemySys.HP <= 0)
            {
                _e_endAct = true;
                yield break;
            }

            //��� �ൿ���� ���ո�ŭ �ൿ�� ȸ��
            _enemySys.Change_Ap(true, 2);

            //��� �α� �߰�
            log += _btlLog.Log_Wait(false, 2);

            _e_hitWait = true;  //�� ��� ó��
        }

        Refresh_Log();
        _btlLog.SetLog_BattleFlow(log); //�α� ���

        yield return new WaitUntil(() => _battleProcess == false);  //�α� ����� ���������� ���
        if (isPlayer)
            _p_endAct = true;
        else
            _e_endAct = true;
    }

    public IEnumerator AtkHit(bool toEnemy) //����� ������
    {
        var log = "";
        var finalDmg = 0;

        if (toEnemy)    //�÷��̾ ������ ����
        {
            finalDmg = (_p_total - _enemySys.AC) > 0 ? _p_total - _enemySys.AC : 0;

            _enemySys.TakeDamage(finalDmg, _p_act.DmgType); //���� ���� �ݿ��� ���ظ� ��
            _e_spr.Set_CommonMoveSet(SpriteSystem.CommonTrigger.Dmg);   //�ǰ� �ִϸ��̼�

            //�� �˹�
            var pos = _e_spr.transform.position;
            var dest = new Vector3(pos.x + 1f, pos.y, pos.z);
            _e_spr.Set_SpriteMove(dest);

            _p_hitAtk = true;   //�÷��̾��� ���� ���� ó��

            if (finalDmg > 0)   //�÷��̾ �������� ���ظ� �־����� ó��
                _p_makeDmg = true;
        }
        else    //���� �÷��̾�� ����
        {
            finalDmg = (_e_total - _playerSys.AC) > 0 ? _e_total - _playerSys.AC : 0;

            _playerSys.TakeDamage(finalDmg, _e_act.DmgType); //�÷��̾��� ���� �ݿ��� ���ظ� ��
            _p_spr.Set_CommonMoveSet(SpriteSystem.CommonTrigger.Dmg);   //�ǰ� �ִϸ��̼�

            //�÷��̾� �˹�
            var pos = _p_spr.transform.position;
            var dest = new Vector3(pos.x - 1f, pos.y, pos.z);
            _p_spr.Set_SpriteMove(dest);

            _e_hitAtk = true;   //���� ���� ���� ó��

            if (finalDmg > 0)   //���� �������� ���ظ� �־����� ó��
                _e_makeDmg = true;
        }

        log += _btlLog.Log_AtkDmg(toEnemy, finalDmg);   //���� �α� �߰�
        Refresh_Log();
        _btlLog.SetLog_BattleFlow(log); //�α� ���

        yield return new WaitUntil(() => _battleProcess == false);  //�α� ����� ���� ������ ���

        if (toEnemy) _p_endAct = true;
        else _e_endAct = true;
    }

    public IEnumerator AtkDef(bool fromEnemy)   //������ �����
    {
        var log = "";
        var finalDmg = 0;

        if (fromEnemy)  //���� ������ �÷��̾ ���
        {
            finalDmg = (_e_total - _p_total) > 0 ? _e_total - _p_total : 0;

            _playerSys.TakeDamage(finalDmg, _e_act.DmgType); //�÷��̾��� ��� ���հ� ���� �ݿ��� ���ظ� ��

            //�÷��̾� ��¦ �и�
            var pos = _p_spr.transform.position;
            var dest = new Vector3(pos.x - 1f, pos.y, pos.z);
            _p_spr.Set_SpriteMove(dest);

            _p_hitDef = true;   //�÷��̾� ���� ��� ó��
            _e_hitAtk = true;   //�� ���� ���� ó��

            if (finalDmg > 0)   //���� �������� ���ظ� �־����� ó��
                _e_makeDmg = true;
        }
        else    //�÷��̾��� ������ ���� ���
        {
            finalDmg = (_p_total - _e_total) > 0 ? _p_total - _e_total : 0;

            _enemySys.TakeDamage(finalDmg, _p_act.DmgType); //���� ��� ���հ� ���� �ݿ��� ���ظ� ��

            //�� ��¦ �и�
            var pos = _e_spr.transform.position;
            var dest = new Vector3(pos.x - 1f, pos.y, pos.z);
            _e_spr.Set_SpriteMove(dest);

            _e_hitDef = true;   //�� ���� ��� ó��
            _p_hitAtk = true;   //�÷��̾� ���� ���� ó��

            if (finalDmg > 0)   //���� �������� ���ظ� �־����� ó��
                _p_makeDmg = true;
        }

        log = _btlLog.Log_Def(fromEnemy, finalDmg);   //��� �α� �߰�
        Refresh_Log();
        _btlLog.SetLog_BattleFlow(log); //�α� ���
        yield return new WaitUntil(() => _battleProcess == false);  //�α� ����� ���������� ���

        _p_endAct = true;   //�÷��̾� �ൿ�Ϸ�
        _e_endAct = true;   //�� �ൿ�Ϸ�
    }

    public IEnumerator AtkDge(bool fromEnemy)
    {
        var log = "";

        if (fromEnemy)  //���� ������ �÷��̾ ȸ���ϴ� ��Ȳ
        {
            if (_p_act.Dodge_Check(fromEnemy, this))    //ȸ�ǿ� ������ ���
            {
                var dgePos = new Vector3(_p_spr.transform.position.x - 3f, _p_spr.transform.position.y, _p_spr.transform.position.z);
                _p_spr.Set_SpriteMove(dgePos);

                _p_hitDge = true;   //�÷��̾� ���� ȸ�� ó��

                log = _btlLog.Log_Dge(fromEnemy, true, 0); //�÷��̾� ȸ�� ���� �α� �߰�
                Refresh_Log();
                _btlLog.SetLog_BattleFlow(log); //�α� ���
            }
            else    //ȸ�ǿ� ������ ���
            {
                var finalDmg = (_e_total - _playerSys.AC) > 0 ? _e_total - _playerSys.AC : 0;

                _playerSys.TakeDamage(finalDmg, _e_act.DmgType); //�÷��̾��� ���� �ݿ��� ���ظ� ��
                _p_spr.Set_CommonMoveSet(SpriteSystem.CommonTrigger.Dmg);

                _e_hitAtk = true;   //���� ���� ���� ó��

                //�÷��̾� �˹�
                var pos = _p_spr.transform.position;
                var dest = new Vector3(pos.x - 1f, pos.y, pos.z);
                _p_spr.Set_SpriteMove(dest);

                if (finalDmg > 0)   //���� �������� ���ظ� �־����� ó��
                    _e_makeDmg = true;

                log = _btlLog.Log_Dge(fromEnemy, false, finalDmg); //�÷��̾� ȸ�� ���� �α� �߰�
                Refresh_Log();
                _btlLog.SetLog_BattleFlow(log); //�α� ���
            }
        }
        else    //�÷��̾��� ������ ���� ȸ���ϴ� ��Ȳ
        {
            if (_e_act.Dodge_Check(fromEnemy, this))    //ȸ�ǿ� ������ ���
            {
                var dgePos = new Vector3(_e_spr.transform.position.x + 3f, _e_spr.transform.position.y, _e_spr.transform.position.z);
                _e_spr.Set_SpriteMove(dgePos);
                _e_spr.Set_ActionMoveSet_Dge(_e_act.DgeMS, true);

                _e_hitDge = true;   //�� ���� ȸ�� ó��

                log = _btlLog.Log_Dge(fromEnemy, true, 0); //�� ȸ�� ���� �α� �߰�
                Refresh_Log();
                _btlLog.SetLog_BattleFlow(log); //�α� ���
            }
            else    //ȸ�ǿ� ������ ���
            {
                var finalDmg = (_p_total - _enemySys.AC) > 0 ? _p_total - _enemySys.AC : 0;

                _enemySys.TakeDamage(finalDmg, _p_act.DmgType); //���� ���� �ݿ��� ���ظ� ��
                _e_spr.Set_CommonMoveSet(SpriteSystem.CommonTrigger.Dmg);

                _p_hitAtk = true;   //�÷��̾��� ���� ���� ó��

                //�� �˹�
                var pos = _e_spr.transform.position;
                var dest = new Vector3(pos.x + 1f, pos.y, pos.z);
                _e_spr.Set_SpriteMove(dest);

                if (finalDmg > 0)   //�÷��̾ �������� ���ظ� �־����� ó��
                    _p_makeDmg = true;

                log = _btlLog.Log_Dge(fromEnemy, false, finalDmg); //�� ȸ�� ���� �α� �߰�
                Refresh_Log();
                _btlLog.SetLog_BattleFlow(log); //�α� ���
            }
        }

        yield return new WaitUntil(() => _battleProcess == false);  //�α� ����� ���������� ���
        _p_endAct = true;   //�÷��̾� �ൿ�Ϸ�
        _e_endAct = true;   //�� �ൿ�Ϸ�
    }

    public void Set_BattleProcess(bool b) => _battleProcess = b;

    //-------------------------Ư��, �ൿ�� ȿ�� ó��-------------------------
    public IEnumerator AbilityProcess_Player(bool isPre)
    {
        Debug.Log("�÷��̾� ȿ�� " + (isPre ? "��ó��" : "��ó��"));

        //�÷��̾� �ֻ��� ���â ����

        yield return new WaitForSecondsRealtime(20f * Time.deltaTime);  //ȿ�� ó�� ���� ������

        //Ư�� üũ

        yield return new WaitUntil(() => _effectProcess == false);  //Ư�� ȿ�� ó���� ���� ������ ���

        //�ൿ ����, ���� ȿ�� üũ (isPre�� ���� ����)
        if (isPre)
            _p_act.Effect_Pre(true, this);
        else
            _p_act.Effect_Post(true, this);

        yield return new WaitUntil(() => _effectProcess == false);  //�ൿ ȿ�� ó���� ���� ������ ���

        if (_p_isSlow)  //�÷��̾� �켱���� �� ���� ���
        {
            if (isPre)
                BattleFlow_Start(); //��ó���� ���, ���� ó�� ����
            else
                BattleFlow_End();   //��ó���� ���, ���� ó�� ����
        }
        else    //�÷��̾� �켱���� �� ���� ���
        {
            StartCoroutine(AbilityProcess_Enemy(isPre));    //�� ȿ�� ó�� ����
        }
    }

    public IEnumerator AbilityProcess_Enemy(bool isPre)
    {
        Debug.Log("�� ȿ�� " + (isPre ? "��ó��" : "��ó��"));

        //�� �ֻ��� ���â ����

        yield return new WaitForSecondsRealtime(20f * Time.deltaTime);  //ȿ�� ó�� ���� ������

        //Ư�� üũ

        yield return new WaitUntil(() => _effectProcess == false);  //Ư�� ȿ�� ó���� ���� ������ ���

        //�ൿ ����, ���� ȿ�� üũ (isPre�� ���� ����)
        if (isPre)
            _e_act.Effect_Pre(false, this);
        else
            _e_act.Effect_Post(false, this);

        yield return new WaitUntil(() => _effectProcess == false);  //�ൿ ȿ�� ó���� ���� ������ ���

        if (_p_isSlow)  //�÷��̾� �켱���� �� ���� ���
        {
            StartCoroutine(AbilityProcess_Player(isPre));   //�÷��̾� ȿ�� ó�� ����
        }
        else    //�÷��̾� �켱���� �� ���� ���
        {
            if (isPre)
                BattleFlow_Start(); //��ó���� ���, ���� ó�� ����
            else
                BattleFlow_End();   //��ó���� ���, ���� ó�� ����
        }
    }

    public void Set_EffectProcess(bool b) => _effectProcess = b;

    public void SetLog_AtkHit(bool isP, string effText) //���� ���� ȿ�� �α�
    {
        Refresh_Log();
        _btlLog.SetLog_AtkHit(isP, effText);
    }

    public void SetLog_AtkDmg(bool isP, string effText) //���� ���� �־��� �� ȿ�� �α�
    {
        Refresh_Log();
        _btlLog.SetLog_AtkDmg(isP, effText);
    }

    public void SetLog_AtkBlocked(bool isP, string effText) //������ ������ �� ȿ�� �α�
    {
        Refresh_Log();
        _btlLog.SetLog_AtkBlocked(isP, effText);
    }

    public void SetLog_AtkMissed(bool isP, string effText)  //������ �������� �� ȿ�� �α�
    {
        Refresh_Log();
        _btlLog.SetLog_AtkMissed(isP, effText);
    }

    public void SetLog_DefEffect(bool isP, string effText)  //��� ȿ�� �α�
    {
        Refresh_Log();
        _btlLog.SetLog_DefEffect(isP, effText);
    }

    public void SetLog_DefEffect_NoAtk(bool isP, string effText)    //��밡 �������� �ʾ��� �� ��� ȿ�� �α�
    {
        Refresh_Log();
        _btlLog.SetLog_DefEffect_NoAtk(isP, effText);
    }

    public void SetLog_DefEffect_Wait(bool isP, string effText)     //��밡 ������� �� ��� ȿ�� �α�
    {
        Refresh_Log();
        _btlLog.SetLog_DefEffect_Wait(isP, effText);
    }

    public void SetLog_DgeEffect(bool isP, string effText)  //ȸ�� ȿ�� �α�
    {
        Refresh_Log();
        _btlLog.SetLog_DgeEffect(isP, effText);
    }

    public void SetLog_DgeEffect_Fail(bool isP, string effText) //ȸ�� ���� ���� �� ȿ�� �α�
    {
        Refresh_Log();
        _btlLog.SetLog_DgeEffect_Fail(isP, effText);
    }

    public void SetLog_DgeEffect_NoAtk(bool isP, string effText)    //��밡 �������� �ʾ��� �� ȸ�� ȿ�� �α�
    {
        Refresh_Log();
        _btlLog.SetLog_DgeEffect_NoAtk(isP, effText);
    }

    public void SetLog_ActEffect(bool isP, string actName, string effText)  //�ൿ ȿ�� �α�
    {
        Refresh_Log();
        _btlLog.SetLog_ActEffect(isP, actName, effText);
    }

    public void SetLog_RunAct(bool isP, bool success)   //���� �ൿ �α�
    {
        Refresh_Log();
        _btlLog.SetLog_RunAct(isP, success);

        /*
        if (success)
            _isNowBattleEnd = true;
        */
    }

    //-------------------------�ൿ ȿ�� ó���� ���� ������ ��ġ ����-------------------------
    public void Change_Hp_Player(bool plus, int value)  //�÷��̾� HP ����
    {
        _playerSys.Change_Hp(plus, value);  
        if (_playerSys.HP <= 0)
            _btlLog.SetLog_BattleEnd(false);    //�÷��̾� ���
    }

    public void Change_Hp_Enemy(bool plus, int value)   //�� HP ����
    {
        _enemySys.Change_Hp(plus, value);
        if (_enemySys.HP <= 0)
            _btlLog.SetLog_BattleEnd(true);    //�� ���
    }

    public void TakeDamage_Player(int value, BtlActData.DamageType dmgType)
    {
        _playerSys.TakeDamage(value, dmgType);  //�÷��̾� ������
        if (_playerSys.HP <= 0)
            _btlLog.SetLog_BattleEnd(false);    //�÷��̾� ���
    }

    public void TakeDamage_Enemy(int value, BtlActData.DamageType dmgType)
    {
        _enemySys.TakeDamage(value, dmgType);   //�� ������

        if (_enemySys.HP <= 0)
            _btlLog.SetLog_BattleEnd(true);     //�� ���
    }

    public void Change_Ac_Player(bool plus, int value) => _playerSys.Change_AC(plus, value);
    public void Change_Ac_Enemy(bool plus, int value) => _enemySys.Change_AC(plus, value);

    public void Change_Ap_Player(bool plus, int value) => _playerSys.Change_Ap(plus, value);
    public void Change_Ap_Enemy(bool plus, int value) => _enemySys.Change_Ap(plus, value);

    void Refresh_Log()
    {
        _btlLog.gameObject.SetActive(false);
        _btlLog.gameObject.SetActive(true);
    }
}
