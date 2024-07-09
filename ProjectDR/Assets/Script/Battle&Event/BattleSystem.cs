using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public static BattleSystem BtlSys = null;

    private DungeonSystem _dgnSys;  //전투 돌입 시, 전투를 유발한 던전 스크립트 넘겨받아 기록해야함
    private GameObject _camera_dgn; //전투 돌입 시, 던전 카메라 스크립트를 넘겨받아 기록해야함

    [SerializeField]
    private ActionController _actController;    //행동 컨트롤러
    [SerializeField]
    private DiceResultPannel _p_resultPannel;   //플레이어 주사위 결과창
    [SerializeField]
    private DiceResultPannel _e_resultPannel;   //적 주사위 결과창
    [SerializeField]
    private GameLog _btlLog;    //전투 로그
    [SerializeField]
    private ItemSystem _itemSys;                //아이템 시스템
    [SerializeField]
    private RewardPannel _rewardPannel;         //전리품 창

    [Header("# Camera")]
    [SerializeField]
    private GameObject _camera_btl;             //전투 카메라

    [Header("# Battle UI & Sprite")]
    [SerializeField]
    private SpriteSystem _p_spr;        //플레이어 스프라이트
    [SerializeField]
    private SpriteRenderer _p_sprRend;  //플레이어 스프라이트 렌더러

    [SerializeField]
    private SpriteSystem _e_spr;         //적 스프라이트
    [SerializeField]
    private SpriteRenderer _e_sprRend;  //적 스프라이트 렌더러

    [SerializeField]
    private GameObject _btn_eventEnd;     //(전투)이벤트 종료 버튼

    [Header("# Player Info")]
    [SerializeField]
    private PlayerSystem _playerSys;
    [SerializeField]
    private BtlActData _p_act;    //플레이어 행동
    [SerializeField]
    private int _p_nowDice;             //플레이어 주사위 개수
    public int P_DICE
    {
        get { return _p_nowDice; }
    }
    [SerializeField]
    private int[] _p_result;            //플레이어 주사위 결과
    [SerializeField]
    private int _p_total;               //플레이어 주사위 총합
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
    private BtlActData _e_act;    //적 행동
    [SerializeField]
    private int _e_nowDice;             //적 주사위 개수
    public int E_DICE
    {
        get { return _e_nowDice; }
    }
    [SerializeField]
    private int[] _e_result;            //적 주사위 결과
    [SerializeField]
    private int _e_total;               //적 주사위 총합
    public int E_TOTAL
    {
        get { return _e_total; }
    }


    [Header("# Battle Condition")]  //전투 처리 중의 상태 변수
    [SerializeField]
    private bool _p_isSlow;     //플레이어 우선도가 더 낮은지 여부
    [SerializeField]
    private bool _effectProcess = false;    //이 변수가 false일 때, 효과를 처리
    [SerializeField]
    private bool _battleProcess = false;    //이 변수가 false일 때, 전투 행동간 상호작용을 처리
    [SerializeField]
    private bool _rewardExpProcess = false; //이 변수가 true면, 적 처치 경험치처리 중인것

    public class BtlActInQueue
    {
        public bool IsPlayer;
        public BtlActData.ActionType Type;
    }

    private Queue<BtlActInQueue> _btlAct_queue;   //플레이어와 적의 행동 순서를 처리하는 큐

    [Header("# Battle Action Condition")]   //전투 행동 관련 상태 변수
    [SerializeField]
    private bool _p_endAct;         //플레이어 행동 처리 완료 여부
    [SerializeField]
    private bool _e_endAct;         //적 행동 처리 완료 여부

    [SerializeField]
    private bool _p_hitAtk;         //플레이어 이번 턴 공격 명중 여부
    public bool P_HIT_ATK
    {
        get { return _p_hitAtk; }
    }
    [SerializeField]
    private bool _p_makeDmg;        //플레이어 이번 턴 상대에게 피해를 준 여부
    public bool P_MAKE_DMG
    {
        get { return _p_makeDmg; }
    }
    [SerializeField]
    private bool _p_hitDef;         //플레이어 이번 턴 공격 방어 여부
    public bool P_HIT_DEF
    {
        get { return _p_hitDef; }
    }
    [SerializeField]
    private bool _p_hitDge;         //플레이어 이번 턴 공격 회피 여부
    public bool P_HIT_DGE
    {
        get { return _p_hitDge; }
    }
    [SerializeField]
    private bool _p_hitTac;         //플레이어 이번 턴 전술 사용 여부
    public bool P_HIT_TAC
    {
        get { return _p_hitTac; }
    }
    [SerializeField]
    private bool _p_hitWait;        //플레이어 이번 턴 대기 사용 여부
    public bool P_HIT_WAIT
    {
        get { return _p_hitWait; }
    }
    [SerializeField]
    private bool _e_hitAtk;         //적 이번 턴 공격 명중 여부
    public bool E_HIT_ATK
    {
        get { return _e_hitAtk; }
    }
    [SerializeField]
    private bool _e_makeDmg;        //적 이번 턴 상대에게 피해를 준 여부
    public bool E_MAKE_DMG
    {
        get { return _e_makeDmg; }
    }
    [SerializeField]
    private bool _e_hitDef;         //적 이번 턴 공격 방어 여부
    public bool E_HIT_DEF
    {
        get { return _e_hitDef; }
    }
    [SerializeField]
    private bool _e_hitDge;         //적 이번 턴 공격 회피 여부
    public bool E_HIT_DGE
    {
        get { return _e_hitDge; }
    }
    [SerializeField]
    private bool _e_hitTac;         //적 이번 턴 전술 사용 여부
    public bool E_HIT_TAC
    {
        get { return _e_hitTac; }
    }
    [SerializeField]
    private bool _e_hitWait;        //적 이번 턴 대기 사용 여부
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
        _dgnSys = dgnSys;   //현재 던전의 스크립트 할당
        _camera_dgn = camera;   //현재 던전의 카메라 오브젝트 할당
    }

    public void BattleStart(EnemyData enemy)    //전투 시작
    {
        _btlLog.gameObject.SetActive(true);

        _dgnSys.EVNT_PROCESS = true;    //전투 상황 시작
        Set_EffectProcess(false);
        Set_BattleProcess(false);

        _camera_btl.SetActive(true);    //전투 카메라 활성화.
        _camera_dgn.SetActive(false);   //던전 카메라 비활성화

        //플레이어 메뉴 버튼 Off
        _playerSys.MenuButton_OnOff_Status(false);
        _playerSys.MenuButton_OnOff_Inventory(false);
        _playerSys.MenuButton_OnOff_ActList(false);

        _enemySys.Set_BattleEnemy(true, enemy); //전투 시 상대하는 적 설정

        //전투 처리를 위한 각종 변수 초기화
        //적 전투행동, 주사위 정보, 효과 처리용 변수 등..
        //양측의 행동정보 초기화
        _p_act = null;
        _e_act = null;

        //양측의 행동 상태 변수 초기화
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

        //적 등장 로그
        Refresh_Log();
        _btlLog.SetLog_BattleStart(enemy.Name);
        //적 행동 요청
        _enemySys.Request_NextAction();

        //플레이어 전투 행동목록 활성화
        _actController.Set_ActListSituation(ActionController.Situation.Battle);
    }

    public void Set_BtlAct_Player(BtlActData act, int[] result)    //플레이어 전투 행동 결정 완료
    {
        //행동 정보 기록
        _p_act = act;
        _p_result = result.ToArray();

        _p_nowDice = 0;
        for (int i = 0; i < _p_result.Length; i++)  //주사위 개수
        {
            if (_p_result[i] != -1)
                _p_nowDice++;
            else
                break;
        }

        _p_total = 0;
        for (int i = 0; i < _p_nowDice; i++)    //주사위 총합
            _p_total += _p_result[i];

        //사용한 아이템이 있는 경우, 그 정보도 받아옴

        //주사위 결과 UI 세팅

        //적이 행동 결정을 완료한 경우, 다음 단계로 돌입
        if (_e_act != null)
            Battle_PreProcess();
    }

    public void Set_BtlAct_Enemy(BtlActData act, int[] diceResult)     //적 전투 행동 결정 완료
    {
        //행동 정보 기록
        _e_act = act;
        _e_result = diceResult.ToArray();

        _e_nowDice = 0;
        for (int i = 0; i < _e_result.Length; i++)  //주사위 개수
        {
            if (_e_result[i] != -1)
                _e_nowDice++;
            else
                break;
        }

        _e_total = 0;
        for (int i = 0; i < _e_nowDice; i++)    //주사위 총합
            _e_total += _e_result[i];

        //사용한 아이템이 있는 경우 그 정보도 받아옴

        //플레이어가 행동 결정을 완료한 경우, 다음 단계로 돌입
        if (_p_act != null)
            Battle_PreProcess();
    }

    //플레이어와 적의 <특성 + 행동 직전 효과> 처리
    public void Battle_PreProcess()
    {
        Change_DiceResult_Enemy();  //적의 행동 정보 공개

        //우선도 체크
        //적의 우선도가 플레이어보다 더 높으면
        _p_isSlow = true;
        //아니면
        _p_isSlow = false;

        //적의 아이템 사용 처리

        //플레이어의 효과 전처리 코루틴 시작
        if (_p_isSlow)
            StartCoroutine(AbilityProcess_Enemy(true));
        else
            StartCoroutine(AbilityProcess_Player(true));
    }

    //전투 처리
    public void BattleFlow_Start()
    {
        //양측의 행동 상태 변수 초기화
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

        //서로의 행동과 특성으로 인해 변경된 행동 정보를 다시 한번 표시
        Change_DiceResult_Player(); //플레이어 행동 정보 재표시
        Change_DiceResult_Enemy();  //적 행동 정보 재표시

        //행동 타입의 우선도에 따라 서로의 행동을 순차적으로 처리
        //행동 타입 우선도 (전술 > 방어 > 회피 > 공격 > 대기)
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
            if (_p_act.Type == type)    //플레이어: 이 타입
            {
                if (_e_act.Type == type)    //적: 이 타입
                {
                    if (_p_isSlow)   //둘 다 같은 행동 타입일 때, 플레이어가 느리면
                    {
                        //적 Enqueue
                        _btlAct_queue.Enqueue(new BtlActInQueue()
                        {
                            IsPlayer = false,
                            Type = type
                        });
                        //플레이어 Enqueue
                        _btlAct_queue.Enqueue(new BtlActInQueue()
                        {
                            IsPlayer = true,
                            Type = type
                        });
                    }
                    else    //둘 다 같은 행동 타입일 때, 플레이어가 빠르면
                    {
                        //플레이어 Enqueue
                        _btlAct_queue.Enqueue(new BtlActInQueue()
                        {
                            IsPlayer = true,
                            Type = type
                        });
                        //적 Enqueue
                        _btlAct_queue.Enqueue(new BtlActInQueue()
                        {
                            IsPlayer = false,
                            Type = type
                        });
                    }
                }
                else    //적이 이 행동 타입이 아닐 경우, 플레이어 Enqueue
                    //플레이어 Enqueue
                    _btlAct_queue.Enqueue(new BtlActInQueue()
                    {
                        IsPlayer = true,
                        Type = type
                    });
            }
            else if (_e_act.Type == type)   //적: 이 타입
                //적 Enqueue
                _btlAct_queue.Enqueue(new BtlActInQueue()
                {
                    IsPlayer = false,
                    Type = type
                });
        }

        _p_spr.Set_ActHitBox(HitBoxCollider.HitBoxType.No); //플레이어 행동 히트박스: X
        _p_spr.Set_HitBoxState(false);                      //플레이어 히트박스: 일반
        _e_spr.Set_ActHitBox(HitBoxCollider.HitBoxType.No); //적 행동 히트박스: X
        _e_spr.Set_HitBoxState(false);                      //적 히트박스: 일반

        StartCoroutine(Act_Dequeue());  //우선도에 따라 행동을 순차적으로 처리
    }

    //플레이어와 적의 <특성 + 행동 직후 효과> 처리
    public IEnumerator Battle_PostProcess()
    {
        //플레이어와 적이 행동이 모두 끝날 때까지 대기
        yield return new WaitUntil(() => _p_endAct && _e_endAct);


        Debug.Log("플레이어와 적이 행동이 모두 끝남");

        //플레이어 행동 직후 효과 처리 코루틴 시작
        if (_p_isSlow)
            StartCoroutine(AbilityProcess_Enemy(false));
        else
            StartCoroutine(AbilityProcess_Player(false));
    }

    //전투 턴 종료
    public void BattleFlow_End()
    {
        DiceResult_Off(true);   //플레이어 주사위 결과창 Off
        DiceResult_Off(false);  //적 주사위 결과창 Off

        //양측의 행동모션 원상복구
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

        //양측의 행동정보 초기화
        _p_lastActType = _p_act.Type;
        _e_lastActType = _e_act.Type;

        _p_act = null;
        _e_act = null;

        Set_EffectProcess(false);   //행동 효과 처리 종료
        Set_BattleProcess(false);   //전투 행동 처리 종료

        //어느 한 쪽 사망 시, 전투 종료
        if (_enemySys.HP <= 0 || _playerSys.HP <= 0)
        {
            //양측의 행동 상태 변수 초기화
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

            //플레이어 메뉴 버튼 On
            _playerSys.MenuButton_OnOff_Status(true);
            _playerSys.MenuButton_OnOff_Inventory(true);
            _playerSys.MenuButton_OnOff_ActList(true);

            var enemy = _enemySys.Data;

            _enemySys.Set_BattleEnemy(false, null); //적 데이터 Off
            //전투 행동 리스트, 주사위 보드, 재굴림 버튼, 행동 개시 버튼 Off
            _actController.Set_ActListSituation(ActionController.Situation.No);
            _actController.Dice_Off();  //주사위 오브젝트 Off

            _actController.DiceSelectPannel_OnOff(false);   //주사위 선택창 Off
            _actController.NoDiceButton_OnOff(false);       //주사위 없는 행동 개시 버튼 Off
            _actController.DiceResultPannel_Off();          //주사위 결과창 Off

            StopAllCoroutines();

            if (_enemySys.HP <= 0)  //적 사망 시
            {
                _btlLog.SetLog_BattleEnd(true);     //적 사망 로그

                StartCoroutine(Enemy_Reward(enemy));
            }
            else    //플레이어 사망 시
            {
                _btlLog.SetLog_BattleEnd(false);    //플레이어 사망 로그

                _btn_eventEnd.SetActive(true);
            }
        }
        else    //전투가 끝나지 않은 경우
        {
            //적 다음 행동 요청
            _enemySys.Request_NextAction();
            //플레이어 행동목록 재출력
            _actController.Set_ActListSituation(ActionController.Situation.Battle);
        }
    }

    public void Set_RewardExpProcess(bool b) => _rewardExpProcess = b;

    IEnumerator Enemy_Reward(EnemyData enemy)
    {
        var data = enemy;

        //경험치 획득
        var exp = Random.Range(data.Exp[0], data.Exp[1]);
        var amount = Random.Range(1, 4);
        _rewardPannel.RewardPannel_Exp_OnOff(true);     //경험치 획득 패널 On

        _rewardPannel.Set_RewardExpInfo();  //경험치 획득 패널의 수치 설정
        _rewardPannel.Set_GetExpText(exp);//획득 경험치 표시

        //모든 경험치 획득이 끝날 때까지 대기
        yield return new WaitUntil(() => _rewardExpProcess == false);

        _rewardPannel.RewardPannel_Item_OnOff(true);    //아이템 획득 패널 On
        _itemSys.Reward_Clear();    //이전 전리품 모두 제거
        _itemSys.ON_REWARD = true;  //전리품창 ON 상태

        for (int i = 0; i < amount; i++)    //드랍할 아이템 개수만큼 아이템 드랍
            _itemSys.Reward_Item(data.Item[Random.Range(0, data.Item.Length)], i);

        _itemSys.Set_RewardIcon();  //드랍한 아이템 표시

        //---------------

        //양측의 행동 상태 변수 초기화
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

        //플레이어 메뉴 버튼 On
        _playerSys.MenuButton_OnOff_Status(true);
        _playerSys.MenuButton_OnOff_Inventory(true);
        _playerSys.MenuButton_OnOff_ActList(true);

        _enemySys.Set_BattleEnemy(false, null); //적 데이터 Off
                                                //전투 행동 리스트, 주사위 보드, 재굴림 버튼, 행동 개시 버튼 Off
        _actController.Set_ActListSituation(ActionController.Situation.No);
        _actController.Dice_Off();  //주사위 오브젝트 Off

        _actController.DiceSelectPannel_OnOff(false);   //주사위 선택창 Off
        _actController.NoDiceButton_OnOff(false);       //주사위 없는 행동 개시 버튼 Off
        _actController.DiceResultPannel_Off();          //주사위 결과창 Off

        StopAllCoroutines();

        _btn_eventEnd.SetActive(true);
    }

    public void BattleEnd() //전투 종료
    {
        _btlLog.gameObject.SetActive(false);
        _camera_dgn.SetActive(true);    //던전 카메라 활성화
        _camera_btl.SetActive(false);   //전투 카메라 비활성화

        _dgnSys.EVNT_PROCESS = false;   //이벤트 진행 상황 종료

        StopAllCoroutines();

        _itemSys.Reward_Clear();    //전리품 모두 제거
        _rewardPannel.RewardPannel_Exp_OnOff(false);
        _rewardPannel.RewardPannel_Item_OnOff(false);

        _btn_eventEnd.SetActive(false);
    }

    public void Change_DiceResult_Player()  //플레이어 주사위 결과창 정보 변경
    {
        DiceResultPannel pannel = _p_resultPannel;
        BtlActData act = _p_act;
        int[] result = _p_result;
        int total = _p_total;
        int nowDice = _p_nowDice;

        //주사위 결과창 정보 변경
        pannel.ActionInfoPannel_OnOff(true);        //행동 결과창 On
        pannel.Change_ActInfo(act.Type, act.Name);  //행동 타입 아이콘, 행동명
        pannel.Change_DiceTotal(act.NoDice ? "" : total.ToString());    //행동의 주사위 총합

        pannel.DiceResultPannel_OnOff(true);    //주사위 결과창 On
        pannel.Set_NewDiceTotal(nowDice);       //주사위 결과창 초기화

        for (int i = 0; i < result.Length; i++) //주사위 결과 설정
        {
            if (result[i] != -1)
                pannel.Set_StopDiceResult(i, result[i]);
        }
    }

    public void Change_DiceResult_Enemy()  //적 주사위 결과창 정보 변경
    {
        DiceResultPannel pannel = _e_resultPannel;
        BtlActData act = _e_act;
        int[] result = _e_result;
        int total = _e_total;
        int nowDice = _e_nowDice;

        //주사위 결과창 정보 변경
        pannel.ActionInfoPannel_OnOff(true);        //행동 결과창 On
        pannel.Change_ActInfo(act.Type, act.Name);  //행동 타입 아이콘, 행동명
        pannel.Change_DiceTotal(act.NoDice ? "" : total.ToString());    //행동의 주사위 총합

        pannel.DiceResultPannel_OnOff(true);    //주사위 결과창 On
        pannel.Set_NewDiceTotal(nowDice);       //주사위 결과창 초기화

        for (int i = 0; i < result.Length; i++) //주사위 결과 설정
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

        pannel.ActionInfoPannel_OnOff(false);   //행동결과창 Off
        pannel.DiceResultPannel_OnOff(false);   //주사위 결과창 Off
    }

    //던전 시스템으로부터 주변 지형 정보를 받아, 전투 배경을 조정함
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
        _tileSet_12.GetChild(0).gameObject.SetActive(c_1_2 || c_10_11); //1_2 또는 10_11에 천장이 존재할 경우
        // 12_beyond
        _tileSet_12_beyond.GetChild(0).gameObject.SetActive(c_12_b);
    }

    //-------------------------행동 타입의 기본적인 상호작용-------------------------

    public IEnumerator Act_Dequeue()
    {
        while (_btlAct_queue.Count > 0)
        {
            yield return new WaitUntil(() => _battleProcess == false);

            //큐에서 행동을 하나 Dequeue
            var act = _btlAct_queue.Dequeue();
            Debug.Log((act.IsPlayer ? "플레이어의 " : "적의 ") + act.Type);

            //그 행동의 타입에 따라서, 사용자가 해당 행동을 사용했다는 것을 코루틴으로 처리
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

    public IEnumerator Tac(bool isPlayer)   //전술
    {
        yield return new WaitUntil(() => _battleProcess == false);  //다른 행동 처리가 끝날 때까지 대기

        //전술 처리 시작
        Set_BattleProcess(true);

        if (isPlayer)   //플레이어가 전술 행동 사용
        {
            //플레이어 행동력 소모 여부에 따라 행동력을 소모
            if (_p_act.NoDice == false)
            {
                _playerSys.Change_Ap(false, _p_nowDice);    //행동력 소모
                _playerSys.Change_Ap_UsePreview(0);         //소모 예정 행동력 표기 Off
            }

            _p_act.Effect_Tac(true, this);
            _p_hitTac = true;
        }
        else            //적이 전술 행동 사용
        {
            //적 행동력 소모 여부에 따라 행동력을 소모
            if (_e_act.NoDice == false)
                _playerSys.Change_Ap(false, _e_nowDice);

            _e_act.Effect_Tac(false, this);
            _e_hitTac = true;
        }

        yield return new WaitUntil(() => _battleProcess == false);  //로그 출력이 끝날때까지 대기

        //행동 종료 처리
        if (isPlayer) _p_endAct = true;
        else _e_endAct = true;
    }

    public IEnumerator Def(bool isPlayer)   //방어
    {
        yield return new WaitUntil(() => _battleProcess == false);  //다른 행동 처리가 끝날때까지 대기

        //방어 상태 돌입
        if (isPlayer)
        {
            //플레이어 행동력 소모 여부에 따라 행동력을 소모
            if (_p_act.NoDice == false)
            {
                _playerSys.Change_Ap(false, _p_nowDice); //행동력 소모
                _playerSys.Change_Ap_UsePreview(0);      //소모 예정 행동력 표기 Off
            }

            if (_e_act.Type == BtlActData.ActionType.Atk)   //적이 공격을 할 경우
            {
                _p_spr.Set_ActHitBox(HitBoxCollider.HitBoxType.Def);    //플레이어 행동 히트박스: 방어
                _p_spr.Set_HitBoxState(true);                           //플레이어 히트박스: 방어
                //플레이어 방어 무브셋
                _p_spr.ActHitBoxOn();
            }
            else    //적이 공격을 하지 않을 경우
            {
                //방어 실패 처리
                Set_BattleProcess(true);
                var log = _btlLog.Log_DefFail(true);    //플레이어 방어 실패 로그

                Refresh_Log();
                _btlLog.SetLog_BattleFlow(log); //로그 출력
                yield return new WaitUntil(() => _battleProcess == false);  //로그 출력이 끝날 때까지 대기
                _p_endAct = true;   //행동 종료 처리
            }
        }
        else
        {
            //적의 행동력 소모 여부에 따라 행동력을 소모
            if (_e_act.NoDice == false)
                _enemySys.Change_Ap(false, _e_nowDice);

            if (_p_act.Type == BtlActData.ActionType.Atk)   //플레이어가 공격을 할 경우
            {
                _e_spr.Set_ActHitBox(HitBoxCollider.HitBoxType.Def);    //적 행동 히트박스: 방어
                _e_spr.Set_HitBoxState(true);                           //적 히트박스: 방어
                //적 방어 무브셋
                _e_spr.ActHitBoxOn();
            }
            else    //플레이어가 공격을 하지 않았을 경우
            {
                //방어 실패 처리
                Set_BattleProcess(true);
                var log = _btlLog.Log_DefFail(false);   //적 방어 실패 로그

                Refresh_Log();
                _btlLog.SetLog_BattleFlow(log); //로그 출력
                yield return new WaitUntil(() => _battleProcess == false);  //로그 출력이 끝날때까지 대기
                _e_endAct = true;   //행동 종료 처리
            }
        }
    }

    public IEnumerator Dge(bool isPlayer)   //회피
    {
        yield return new WaitUntil(() => _battleProcess == false);  //다른 행동 처리가 끝날 때까지 대기

        //회피 상태 돌입 (이 상태에서 공격 받았을 때, 회피 체크 코루틴을 호출하는 방식)
        if (isPlayer)
        {
            //플레이어 행동력 소모 여부에 따라 행동력을 소모
            if (_p_act.NoDice == false)
            {
                _playerSys.Change_Ap(false, _p_nowDice); //행동력 소모
                _playerSys.Change_Ap_UsePreview(0);      //소모 예정 행동력 표기 Off
            }

            if (_e_act.Type == BtlActData.ActionType.Atk) //적이 공격을 할 경우
                _p_spr.Set_HitBoxState(true);                       //플레이어 히트박스: 회피
            else    //적이 공격을 하지 않을 경우
            {
                //회피 실패 처리
                Set_BattleProcess(true);

                var dgePos = new Vector3(_p_spr.transform.position.x - 3f, _p_spr.transform.position.y, _p_spr.transform.position.z);
                _p_spr.Set_SpriteMove(dgePos);

                var log = _btlLog.Log_DgeFail(true);    //플레이어 회피 실패 로그

                Refresh_Log();
                _btlLog.SetLog_BattleFlow(log); //로그 출력
                yield return new WaitUntil(() => _battleProcess == false);  //로그 출력이 끝날때까지 대기
                _p_endAct = true;   //행동 종료 처리
            }
        }
        else
        {
            //적 행동력 소모 여부에 따라 행동력을 소모
            if (_e_act.NoDice == false)
                _enemySys.Change_Ap(false, _e_nowDice);

            if (_p_act.Type == BtlActData.ActionType.Atk) //플레이어가 공격을 할 경우
                _e_spr.Set_HitBoxState(true);                       //적 히트박스: 회피
            else    //플레이어가 공격을 하지 않을 경우
            {
                //회피 실패 처리
                Set_BattleProcess(true);

                var dgePos = new Vector3(_e_spr.transform.position.x + 3f, _e_spr.transform.position.y, _e_spr.transform.position.z);
                _e_spr.Set_SpriteMove(dgePos);
                _e_spr.Set_ActionMoveSet_Dge(_e_act.DgeMS, true);

                var log = _btlLog.Log_DgeFail(false);   //적 회피 실패 로그

                Refresh_Log();
                _btlLog.SetLog_BattleFlow(log); //로그 출력
                yield return new WaitUntil(() => _battleProcess == false);  //로그 출력이 끝날때까지 대기
                _e_endAct = true;   //행동 종료 처리
            }
        }
    }

    public IEnumerator Atk(bool isPlayer)   //공격
    {
        yield return new WaitUntil(() => _battleProcess == false);  //다른 행동 처리가 끝날 때까지 대기

        Set_BattleProcess(true);

        Vector3 dest;

        //공격 상태 돌입 (이 상태에서 행동 히트박스가 충돌했을 때, 공격 체크 코루틴을 호출하는 방식)
        if (isPlayer)
        {
            if (_playerSys.HP <= 0)
            {
                _p_endAct = true;
                yield break;
            }

            //플레이어 행동력 소모 여부에 따라 행동력을 소모
            if (_p_act.NoDice == false)
            {
                _playerSys.Change_Ap(false, _p_nowDice); //행동력 소모
                _playerSys.Change_Ap_UsePreview(0);      //소모 예정 행동력 표기 Off
            }

            var pos = _e_spr.transform.position;
            dest = new Vector3(pos.x - _e_sprRend.bounds.size.x, pos.y, pos.z);

            _p_spr.ActHitBoxOn();
            _p_spr.Set_ActHitBox(HitBoxCollider.HitBoxType.Atk);    //플레이어 행동 히트박스: 공격
            _p_spr.Set_SpriteMove(dest);                            //플레이어의 공격을 위한 이동
            _p_spr.Set_ActionMoveSet_Atk(_p_act.AtkMS, true);       //공격 무브셋

        }
        else
        {
            if (_enemySys.HP <= 0)
            {
                _e_endAct = true;
                yield break;
            }

            //적 행동력 소모 여부에 따라 행동력을 소모
            if (_e_act.NoDice == false)
                _enemySys.Change_Ap(false, _e_nowDice);

            var pos = _p_spr.transform.position;
            dest = new Vector3(pos.x + _p_sprRend.bounds.size.x, pos.y, pos.z);

            _e_spr.ActHitBoxOn();
            _e_spr.Set_ActHitBox(HitBoxCollider.HitBoxType.Atk);    //적 행동 히트박스: 공격
            _e_spr.Set_SpriteMove(dest);                            //적의 공격을 위한 이동
            _e_spr.Set_ActionMoveSet_Atk(_e_act.AtkMS, true);       //공격 무브셋
        }
    }

    public IEnumerator Wait(bool isPlayer)  //대기
    {
        yield return new WaitUntil(() => _battleProcess == false);  //다른 행동 처리가 끝날 때까지 대기

        //대기 처리 시작
        Set_BattleProcess(true);
        var log = "";

        if (isPlayer)   //플레이어가 대기 행동 사용
        {
            if (_playerSys.HP <= 0)
            {
                _p_endAct = true;
                yield break;
            }

            //대기 행동으로 총합만큼 행동력 회복
            _playerSys.Change_Ap(true, 2);

            //대기 로그 추가
            log += _btlLog.Log_Wait(true, 2);

            _p_hitWait = true;  //플레이어 대기 처리
        }
        else            //적이 대기 행동 사용
        {
            if (_enemySys.HP <= 0)
            {
                _e_endAct = true;
                yield break;
            }

            //대기 행동으로 총합만큼 행동력 회복
            _enemySys.Change_Ap(true, 2);

            //대기 로그 추가
            log += _btlLog.Log_Wait(false, 2);

            _e_hitWait = true;  //적 대기 처리
        }

        Refresh_Log();
        _btlLog.SetLog_BattleFlow(log); //로그 출력

        yield return new WaitUntil(() => _battleProcess == false);  //로그 출력이 끝날때까지 대기
        if (isPlayer)
            _p_endAct = true;
        else
            _e_endAct = true;
    }

    public IEnumerator AtkHit(bool toEnemy) //대상을 공격함
    {
        var log = "";
        var finalDmg = 0;

        if (toEnemy)    //플레이어가 적에게 공격
        {
            finalDmg = (_p_total - _enemySys.AC) > 0 ? _p_total - _enemySys.AC : 0;

            _enemySys.TakeDamage(finalDmg, _p_act.DmgType); //적의 방어도를 반영한 피해를 줌
            _e_spr.Set_CommonMoveSet(SpriteSystem.CommonTrigger.Dmg);   //피격 애니메이션

            //적 넉백
            var pos = _e_spr.transform.position;
            var dest = new Vector3(pos.x + 1f, pos.y, pos.z);
            _e_spr.Set_SpriteMove(dest);

            _p_hitAtk = true;   //플레이어의 공격 명중 처리

            if (finalDmg > 0)   //플레이어가 공격으로 피해를 주었는지 처리
                _p_makeDmg = true;
        }
        else    //적이 플레이어에게 공격
        {
            finalDmg = (_e_total - _playerSys.AC) > 0 ? _e_total - _playerSys.AC : 0;

            _playerSys.TakeDamage(finalDmg, _e_act.DmgType); //플레이어의 방어도를 반영한 피해를 줌
            _p_spr.Set_CommonMoveSet(SpriteSystem.CommonTrigger.Dmg);   //피격 애니메이션

            //플레이어 넉백
            var pos = _p_spr.transform.position;
            var dest = new Vector3(pos.x - 1f, pos.y, pos.z);
            _p_spr.Set_SpriteMove(dest);

            _e_hitAtk = true;   //적의 공격 명중 처리

            if (finalDmg > 0)   //적이 공격으로 피해를 주었는지 처리
                _e_makeDmg = true;
        }

        log += _btlLog.Log_AtkDmg(toEnemy, finalDmg);   //공격 로그 추가
        Refresh_Log();
        _btlLog.SetLog_BattleFlow(log); //로그 출력

        yield return new WaitUntil(() => _battleProcess == false);  //로그 출력이 끝날 때까지 대기

        if (toEnemy) _p_endAct = true;
        else _e_endAct = true;
    }

    public IEnumerator AtkDef(bool fromEnemy)   //공격을 방어함
    {
        var log = "";
        var finalDmg = 0;

        if (fromEnemy)  //적의 공격을 플레이어가 방어
        {
            finalDmg = (_e_total - _p_total) > 0 ? _e_total - _p_total : 0;

            _playerSys.TakeDamage(finalDmg, _e_act.DmgType); //플레이어의 방어 총합과 방어도를 반영한 피해를 줌

            //플레이어 살짝 밀림
            var pos = _p_spr.transform.position;
            var dest = new Vector3(pos.x - 1f, pos.y, pos.z);
            _p_spr.Set_SpriteMove(dest);

            _p_hitDef = true;   //플레이어 공격 방어 처리
            _e_hitAtk = true;   //적 공격 명중 처리

            if (finalDmg > 0)   //적이 공격으로 피해를 주었는지 처리
                _e_makeDmg = true;
        }
        else    //플레이어의 공격을 적이 방어
        {
            finalDmg = (_p_total - _e_total) > 0 ? _p_total - _e_total : 0;

            _enemySys.TakeDamage(finalDmg, _p_act.DmgType); //적의 방어 총합과 방어도를 반영한 피해를 줌

            //적 살짝 밀림
            var pos = _e_spr.transform.position;
            var dest = new Vector3(pos.x - 1f, pos.y, pos.z);
            _e_spr.Set_SpriteMove(dest);

            _e_hitDef = true;   //적 공격 방어 처리
            _p_hitAtk = true;   //플레이어 공격 명중 처리

            if (finalDmg > 0)   //적이 공격으로 피해를 주었는지 처리
                _p_makeDmg = true;
        }

        log = _btlLog.Log_Def(fromEnemy, finalDmg);   //방어 로그 추가
        Refresh_Log();
        _btlLog.SetLog_BattleFlow(log); //로그 출력
        yield return new WaitUntil(() => _battleProcess == false);  //로그 출력이 끝날때까지 대기

        _p_endAct = true;   //플레이어 행동완료
        _e_endAct = true;   //적 행동완료
    }

    public IEnumerator AtkDge(bool fromEnemy)
    {
        var log = "";

        if (fromEnemy)  //적의 공격을 플레이어가 회피하는 상황
        {
            if (_p_act.Dodge_Check(fromEnemy, this))    //회피에 성공한 경우
            {
                var dgePos = new Vector3(_p_spr.transform.position.x - 3f, _p_spr.transform.position.y, _p_spr.transform.position.z);
                _p_spr.Set_SpriteMove(dgePos);

                _p_hitDge = true;   //플레이어 공격 회피 처리

                log = _btlLog.Log_Dge(fromEnemy, true, 0); //플레이어 회피 성공 로그 추가
                Refresh_Log();
                _btlLog.SetLog_BattleFlow(log); //로그 출력
            }
            else    //회피에 실패한 경우
            {
                var finalDmg = (_e_total - _playerSys.AC) > 0 ? _e_total - _playerSys.AC : 0;

                _playerSys.TakeDamage(finalDmg, _e_act.DmgType); //플레이어의 방어도를 반영한 피해를 줌
                _p_spr.Set_CommonMoveSet(SpriteSystem.CommonTrigger.Dmg);

                _e_hitAtk = true;   //적의 공격 명중 처리

                //플레이어 넉백
                var pos = _p_spr.transform.position;
                var dest = new Vector3(pos.x - 1f, pos.y, pos.z);
                _p_spr.Set_SpriteMove(dest);

                if (finalDmg > 0)   //적이 공격으로 피해를 주었는지 처리
                    _e_makeDmg = true;

                log = _btlLog.Log_Dge(fromEnemy, false, finalDmg); //플레이어 회피 실패 로그 추가
                Refresh_Log();
                _btlLog.SetLog_BattleFlow(log); //로그 출력
            }
        }
        else    //플레이어의 공격을 적이 회피하는 상황
        {
            if (_e_act.Dodge_Check(fromEnemy, this))    //회피에 성공한 경우
            {
                var dgePos = new Vector3(_e_spr.transform.position.x + 3f, _e_spr.transform.position.y, _e_spr.transform.position.z);
                _e_spr.Set_SpriteMove(dgePos);
                _e_spr.Set_ActionMoveSet_Dge(_e_act.DgeMS, true);

                _e_hitDge = true;   //적 공격 회피 처리

                log = _btlLog.Log_Dge(fromEnemy, true, 0); //적 회피 성공 로그 추가
                Refresh_Log();
                _btlLog.SetLog_BattleFlow(log); //로그 출력
            }
            else    //회피에 실패한 경우
            {
                var finalDmg = (_p_total - _enemySys.AC) > 0 ? _p_total - _enemySys.AC : 0;

                _enemySys.TakeDamage(finalDmg, _p_act.DmgType); //적의 방어도를 반영한 피해를 줌
                _e_spr.Set_CommonMoveSet(SpriteSystem.CommonTrigger.Dmg);

                _p_hitAtk = true;   //플레이어의 공격 명중 처리

                //적 넉백
                var pos = _e_spr.transform.position;
                var dest = new Vector3(pos.x + 1f, pos.y, pos.z);
                _e_spr.Set_SpriteMove(dest);

                if (finalDmg > 0)   //플레이어가 공격으로 피해를 주었는지 처리
                    _p_makeDmg = true;

                log = _btlLog.Log_Dge(fromEnemy, false, finalDmg); //적 회피 실패 로그 추가
                Refresh_Log();
                _btlLog.SetLog_BattleFlow(log); //로그 출력
            }
        }

        yield return new WaitUntil(() => _battleProcess == false);  //로그 출력이 끝날때까지 대기
        _p_endAct = true;   //플레이어 행동완료
        _e_endAct = true;   //적 행동완료
    }

    public void Set_BattleProcess(bool b) => _battleProcess = b;

    //-------------------------특성, 행동의 효과 처리-------------------------
    public IEnumerator AbilityProcess_Player(bool isPre)
    {
        Debug.Log("플레이어 효과 " + (isPre ? "전처리" : "후처리"));

        //플레이어 주사위 결과창 갱신

        yield return new WaitForSecondsRealtime(20f * Time.deltaTime);  //효과 처리 시작 딜레이

        //특성 체크

        yield return new WaitUntil(() => _effectProcess == false);  //특성 효과 처리가 끝날 때까지 대기

        //행동 직전, 직후 효과 체크 (isPre에 따라 갈림)
        if (isPre)
            _p_act.Effect_Pre(true, this);
        else
            _p_act.Effect_Post(true, this);

        yield return new WaitUntil(() => _effectProcess == false);  //행동 효과 처리가 끝날 때까지 대기

        if (_p_isSlow)  //플레이어 우선도가 더 느린 경우
        {
            if (isPre)
                BattleFlow_Start(); //전처리인 경우, 전투 처리 시작
            else
                BattleFlow_End();   //후처리인 경우, 전투 처리 종료
        }
        else    //플레이어 우선도가 더 높은 경우
        {
            StartCoroutine(AbilityProcess_Enemy(isPre));    //적 효과 처리 시작
        }
    }

    public IEnumerator AbilityProcess_Enemy(bool isPre)
    {
        Debug.Log("적 효과 " + (isPre ? "전처리" : "후처리"));

        //적 주사위 결과창 갱신

        yield return new WaitForSecondsRealtime(20f * Time.deltaTime);  //효과 처리 시작 딜레이

        //특성 체크

        yield return new WaitUntil(() => _effectProcess == false);  //특성 효과 처리가 끝날 때까지 대기

        //행동 직전, 직후 효과 체크 (isPre에 따라 갈림)
        if (isPre)
            _e_act.Effect_Pre(false, this);
        else
            _e_act.Effect_Post(false, this);

        yield return new WaitUntil(() => _effectProcess == false);  //행동 효과 처리가 끝날 때까지 대기

        if (_p_isSlow)  //플레이어 우선도가 더 느린 경우
        {
            StartCoroutine(AbilityProcess_Player(isPre));   //플레이어 효과 처리 시작
        }
        else    //플레이어 우선도가 더 높은 경우
        {
            if (isPre)
                BattleFlow_Start(); //전처리인 경우, 전투 처리 시작
            else
                BattleFlow_End();   //후처리인 경우, 전투 처리 종료
        }
    }

    public void Set_EffectProcess(bool b) => _effectProcess = b;

    public void SetLog_AtkHit(bool isP, string effText) //공격 명중 효과 로그
    {
        Refresh_Log();
        _btlLog.SetLog_AtkHit(isP, effText);
    }

    public void SetLog_AtkDmg(bool isP, string effText) //공격 피해 주었을 때 효과 로그
    {
        Refresh_Log();
        _btlLog.SetLog_AtkDmg(isP, effText);
    }

    public void SetLog_AtkBlocked(bool isP, string effText) //공격이 막혔을 때 효과 로그
    {
        Refresh_Log();
        _btlLog.SetLog_AtkBlocked(isP, effText);
    }

    public void SetLog_AtkMissed(bool isP, string effText)  //공격이 빗나갔을 때 효과 로그
    {
        Refresh_Log();
        _btlLog.SetLog_AtkMissed(isP, effText);
    }

    public void SetLog_DefEffect(bool isP, string effText)  //방어 효과 로그
    {
        Refresh_Log();
        _btlLog.SetLog_DefEffect(isP, effText);
    }

    public void SetLog_DefEffect_NoAtk(bool isP, string effText)    //상대가 공격하지 않았을 때 방어 효과 로그
    {
        Refresh_Log();
        _btlLog.SetLog_DefEffect_NoAtk(isP, effText);
    }

    public void SetLog_DefEffect_Wait(bool isP, string effText)     //상대가 대기했을 떄 방어 효과 로그
    {
        Refresh_Log();
        _btlLog.SetLog_DefEffect_Wait(isP, effText);
    }

    public void SetLog_DgeEffect(bool isP, string effText)  //회피 효과 로그
    {
        Refresh_Log();
        _btlLog.SetLog_DgeEffect(isP, effText);
    }

    public void SetLog_DgeEffect_Fail(bool isP, string effText) //회피 조건 실패 시 효과 로그
    {
        Refresh_Log();
        _btlLog.SetLog_DgeEffect_Fail(isP, effText);
    }

    public void SetLog_DgeEffect_NoAtk(bool isP, string effText)    //상대가 공격하지 않았을 때 회피 효과 로그
    {
        Refresh_Log();
        _btlLog.SetLog_DgeEffect_NoAtk(isP, effText);
    }

    public void SetLog_ActEffect(bool isP, string actName, string effText)  //행동 효과 로그
    {
        Refresh_Log();
        _btlLog.SetLog_ActEffect(isP, actName, effText);
    }

    public void SetLog_RunAct(bool isP, bool success)   //도망 행동 로그
    {
        Refresh_Log();
        _btlLog.SetLog_RunAct(isP, success);

        /*
        if (success)
            _isNowBattleEnd = true;
        */
    }

    //-------------------------행동 효과 처리에 의한 양측의 수치 조정-------------------------
    public void Change_Hp_Player(bool plus, int value)  //플레이어 HP 조정
    {
        _playerSys.Change_Hp(plus, value);  
        if (_playerSys.HP <= 0)
            _btlLog.SetLog_BattleEnd(false);    //플레이어 사망
    }

    public void Change_Hp_Enemy(bool plus, int value)   //적 HP 조정
    {
        _enemySys.Change_Hp(plus, value);
        if (_enemySys.HP <= 0)
            _btlLog.SetLog_BattleEnd(true);    //적 사망
    }

    public void TakeDamage_Player(int value, BtlActData.DamageType dmgType)
    {
        _playerSys.TakeDamage(value, dmgType);  //플레이어 데미지
        if (_playerSys.HP <= 0)
            _btlLog.SetLog_BattleEnd(false);    //플레이어 사망
    }

    public void TakeDamage_Enemy(int value, BtlActData.DamageType dmgType)
    {
        _enemySys.TakeDamage(value, dmgType);   //적 데미지

        if (_enemySys.HP <= 0)
            _btlLog.SetLog_BattleEnd(true);     //적 사망
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
