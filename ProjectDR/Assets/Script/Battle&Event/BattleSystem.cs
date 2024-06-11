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
    private GameLog _btlLog;

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
    private bool _isNowBattleEnd;
    [SerializeField]
    private bool _effectProcess = false;    //이 변수가 false일 때, 효과를 처리
    [SerializeField]
    private bool _battleProcess = false;    //이 변수가 false일 때, 전투 행동간 상호작용을 처리

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
        _e_hitAtk = false;
        _e_makeDmg = false;
        _e_hitDef = false;
        _e_hitDge = false;
        _e_hitTac = false;

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

        //적의 아이템 사용 처리

        //플레이어의 행동 직전 효과 처리 코루틴 시작
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
        _e_hitAtk = false;
        _e_makeDmg = false;
        _e_hitDef = false;
        _e_hitDge = false;
        _e_hitTac = false;

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

        bool p_isSlow = false;  //[플레이어 속도 < 적 속도]일 때 true / [플레이어 속도 >= 적 속도]일 때 false

        foreach (BtlActData.ActionType type in actType_speed)
        {
            if (_p_act.Type == type)    //플레이어: 이 타입
            {
                if (_e_act.Type == type)    //적: 이 타입
                {
                    if (p_isSlow)   //둘 다 같은 행동 타입일 때, 플레이어가 느리면
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
                    IsPlayer = true,
                    Type = type
                });
        }

        _p_spr.Set_ActHitBox(HitBoxCollider.HitBoxType.No); //플레이어 행동 히트박스: X
        _p_spr.Set_HitBoxState(false);                      //플레이어 히트박스: 일반
        _e_spr.Set_ActHitBox(HitBoxCollider.HitBoxType.No); //적 행동 히트박스: X
        _e_spr.Set_HitBoxState(false);                      //적 히트박스: 일반

        //Act_Dequeue
    }

    /*
    //플레이어와 적의 <특성 + 행동 직후 효과> 처리
    public IEnumerator Battle_PostProcess()
    {
        // wait until 플레이어와 적이 행동이 모두 끝날 때까지

        //플레이어 행동 직후 효과 처리 코루틴 시작
    }
    */

    //전투 턴 종료
    public void BattleFlow_End()
    {
        //플레이어 주사위 결과창 Off
        //적 주사위 결과창 Off

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
        if (_enemySys.HP <= 0)  //적 사망 시
        {
            _isNowBattleEnd = true;

            _btlLog.SetLog_BattleEnd(true);     //전투 종료 로그 출력
        }
        else if (_playerSys.HP <= 0) //플레이어 사망 시
        {
            _isNowBattleEnd = true;

            _btlLog.SetLog_BattleEnd(false);    //플레이어 사망 로그
        }

        if (_isNowBattleEnd)    //전투가 종료된 경우
        {
            //양측의 행동 상태 변수 초기화
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
        else    //전투가 끝나지 않은 경우
        {
            //적 다음 행동 요청
            _enemySys.Request_NextAction();
            //플레이어 행동목록 재출력
            _actController.Set_ActListSituation(ActionController.Situation.Battle);
        }
    }

    public void BattleEnd() //전투 종료
    {
        
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
                pannel.Change_DiceResult(i, result[i]);
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

    public void Set_BattleProcess(bool b) => _battleProcess = b;

    //-------------------------행동 효과 처리-------------------------

    public void Set_EffectProcess(bool b) => _effectProcess = b;


    void Refresh_Log()
    {
        _btlLog.gameObject.SetActive(false);
        _btlLog.gameObject.SetActive(true);
    }
}
