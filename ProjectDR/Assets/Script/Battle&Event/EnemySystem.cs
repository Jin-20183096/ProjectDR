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

    private PandaBehaviour _bt;     //행동 트리

    [Header("# Enemy UI")]
    [SerializeField]
    private GameObject _pannel_info;
    [SerializeField]
    private TextMeshProUGUI _txt_name;      //적 이름 UI 텍스트
    [SerializeField]
    private Image _hpMask;                  //적 HP바 마스크
    [SerializeField]
    private Image _hpMeter;                 //적 UI HP바 미터
    [SerializeField]
    private GameObject _icon_ac;            //적 방어도 UI
    [SerializeField]
    private TextMeshProUGUI _txt_ac;        //적 방어도 UI 텍스트
    [SerializeField]
    private TextMeshProUGUI _txt_acMax;     //적 최대 방어도 UI 텍스트
    [SerializeField]
    private Transform[] _meter_ap;          //행동력 미터(아이콘들)

    [Header("# Data Info")]
    [SerializeField]
    private EnemyData _data;    //적 데이터
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
    private int _hpMax;     //최대 HP
    public int HP_MAX
    {
        get { return _hpMax; }
    }
    [SerializeField]
    private int _ac;        //방어도
    public int AC
    {
        get { return _ac; }
    }
    [SerializeField]
    private int _acMax;     //최대 방어도
    public int AC_MAX
    {
        get { return _acMax; }
    }
    [SerializeField]
    private int _ap;        //행동력
    public int AP
    {
        get { return _ap; }
    }
    [SerializeField]
    private int _apMax;     //최대 행동력
    public int AP_MAX
    {
        get { return _apMax; }
    }
    [SerializeField]
    private int[] _stat_STR = { 0, 0, 0, 0, 0, 0 }; //힘
    public int[] STR
    {
        get { return _stat_STR; }
    }
    [SerializeField]
    private int[] _stat_INT = { 0, 0, 0, 0, 0, 0 }; //지능
    public int[] INT
    {
        get { return _stat_INT; }
    }
    [SerializeField]
    private int[] _stat_DEX = { 0, 0, 0, 0, 0, 0 }; //손재주
    public int[] DEX
    {
        get { return _stat_DEX; }
    }
    [SerializeField]
    private int[] _stat_AGI = { 0, 0, 0, 0, 0, 0 }; //민첩
    public int[] AGI
    {
        get { return _stat_AGI; }
    }
    [SerializeField]
    private int[] _stat_CON = { 0, 0, 0, 0, 0, 0 }; //건강
    public int[] CON
    {
        get { return _stat_CON; }
    }
    [SerializeField]
    private int[] _stat_WIL = { 0, 0, 0, 0, 0, 0 }; //의지
    public int[] WIL
    {
        get { return _stat_WIL; }
    }

    [Header("# Action Related")]
    [SerializeField]
    private List<BtlActData.ActType> _btlActTypeList;    //전투행동 타입 목록

    [SerializeField]
    private List<BtlAct> _atkList; //공격 행동 목록
    [SerializeField]
    private List<BtlAct> _defList; //방어 행동 목록
    [SerializeField]
    private List<BtlAct> _dgeList; //회피 행동 목록
    [SerializeField]
    private List<BtlAct> _tacList; //전술 행동 목록
    [SerializeField]
    private BtlAct _wait;     //대기 행동

    [SerializeField]
    private int _nowDice;               //적이 굴릴 주사위
    [SerializeField]
    private int[] _result;              //적이 굴릴 주사위 결과

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

    Stack<BtlAct> _btlActStack;    //전투행동 스택
    [SerializeField]
    private string _btlActClueLog;      //전투행동 단서 로그
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
        _pannel_info.SetActive(isNowBattle);    //적 기본정보창 on/off

        if (isNowBattle)
        {
            _data = new_enemy;

            //적 설정
            Set_EnemyInfo(Random.Range(_data.HP_MIN, _data.HP_MAX + 1));

            //적인지, 보스인지에 따라 스프라이트 설정
            _e_spr.gameObject.SetActive(true);
            _e_anima.runtimeAnimatorController = _data.Anima_Ctrl;
        }
        else    //전투 종료 시
        {
            //적 데이터, 관련 변수 초기화
            _data = null;
            _hp = 0;
            _hpMax = 0;
            _ac = 0;
            _acMax = 0;
            _ap = 0;
            _apMax = 0;

            //행동 트리 할당 off
            Destroy(_bt);
            _bt = null;

            _btlActTypeList.Clear(); //보유한 전투행동 타입 목록 초기화

            _e_spr.gameObject.SetActive(false);  //적 스프라이트 off
        }
    }

    public void Set_EnemyInfo(int hpMax)
    {
        //HP
        _hpMax = hpMax;
        _hp = _hpMax;
        //방어도
        _acMax = _data.AC;
        _ac = _acMax;
        //행동력
        _apMax = _data.ApMax;
        _ap = Random.Range(1, _apMax + 1);

        //전투행동 타입 목록 설정
        _btlActTypeList.Clear(); //전투행동 타입 목록 초기화

        _atkList.Clear();
        _atkList = _data.Act_Atk.ToList();  //공격 행동 목록에 추가
        if (_atkList.Count > 0) _btlActTypeList.Add(BtlActData.ActType.Atk);

        _defList.Clear();
        _defList = _data.Act_Def.ToList();  //방어 행동 목록에 추가
        if (_defList.Count > 0) _btlActTypeList.Add(BtlActData.ActType.Def);

        _dgeList.Clear();
        _dgeList = _data.Act_Dge.ToList();  //회피 행동 목록에 추가
        if (_dgeList.Count > 0) _btlActTypeList.Add(BtlActData.ActType.Dge);

        _tacList.Clear();
        _tacList = _data.Act_Tac.ToList();  //전술 행동 목록에 추가
        if (_tacList.Count > 0) _btlActTypeList.Add(BtlActData.ActType.Tac);

        //새로 설정된 값을 적용
        _txt_name.text = NAME;  //이름
        Change_HpMax(true, 0);
        Change_Hp(true, 0);
        _hpMask.fillAmount = _hp / (float)_hpMax;
        _hpMeter.fillAmount = _hp / (float)_hpMax;
        Change_AC(true, 0);
        Change_ACMax(true, 0);
        Change_ApMax(true, 0);
        Change_Ap(true, 0);

        //행동 스탯 새로 적용
        _stat_STR = _data.STR.ToArray();
        _stat_INT = _data.INT.ToArray();
        _stat_DEX = _data.DEX.ToArray();
        _stat_AGI = _data.AGI.ToArray();
        _stat_CON = _data.CON.ToArray();
        _stat_WIL = _data.WIL.ToArray();

        //행동 트리 할당 On
        _bt = gameObject.AddComponent(typeof(PandaBehaviour)) as PandaBehaviour;

        _bt.scripts = new TextAsset[] { _data.BT };
        _bt.tickOn = BehaviourTree.UpdateOrder.Manual;
        _bt.Compile();
        _bt.Apply();
    }

    public bool IsPlayer() => false;    //플레이어인가? => False

    public void Change_Hp(bool plus, int value)
    {
        var old_hp = _hp;

        if (value >= 0) //변경값이 0 이상일 때만 처리
        {
            if (plus)   //회복
            {
                if (_hpMax <= _hp + value)      //회복량이 최대 HP를 넘을 경우
                    _hp = _hpMax;               //최대 HP만큼 회복
                else
                    _hp += value;               //값만큼 회복
            }
            else
            {
                if (_hp <= value)   //피해량이 남은 HP를 넘을 경우
                    _hp = 0;        //HP가 0으로
                else
                    _hp -= value;   //값만큼 피해
            }
        }

        if (_hp > old_hp)   //HP 회복 시
        {
            //HP마스크 변경
            _hpMask.fillAmount = _hp / (float)_hpMax;
            StartCoroutine("HpMask_Up");
        }
        else
        {
            //HP미터 변경
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

        if (_hp > _hpMax)   //최대 HP가 현재 HP보다 낮아졌을 경우
            _hp = _hpMax;   //낮아진 최대 HP로 현재 HP를 설정

        //변경된 HP비율만큼 HP바 설정
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

    IEnumerator HpMask_Down()   //줄어든 미터만큼  HP바 마스크 지우기
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

    IEnumerator HpMask_Up()     //늘어단 마스크만큼 HP바 미터 늘리기
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
        //값만큼 증가
        if (plus)
            _acMax += value;
        else
            _acMax -= value;

        /*
        if (_ac >= _acMax)      //최대 방어도가 현재 방어도보다 낮아졌을 경우
            _ac = _acMax;       //낮아진 최대 방어도만큼 현재 방어도를 설정
        */

        //변경된 값만큼 수치 변경
        _icon_ac.SetActive(_acMax != 0);
        if (_acMax != 0)
            _txt_acMax.text = "/ " + _acMax.ToString();
    }

    public void Change_Ap(bool plus, int value)
    {
        var old_ap = _ap;

        if (value >= 0) //변경값이 0 이상일 때만 처리
        {
            if (plus)   //추가
            {
                if (_apMax <= _ap + value)  //추가량이 최대 행동력을 넘을 경우
                    _ap = _apMax;
                else
                    _ap += value;
            }
            else
            {
                if (_ap <= value)           //소모량이 남은 행동력을 넘을 경우
                    _ap = 0;
                else
                    _ap -= value;
            }
        }

        //행동력 미터 갱신
        for (int i = 0; i < _meter_ap.Length; i++)
        {
            if (_meter_ap[i].gameObject.activeSelf)
            {
                //현재 행동력에 해당하는 미터이면, 활성화. 아니면 비활성화
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

        if (_ap >= _apMax)  //최대 행동력이 현재 행동력보다 낮아졌을 경우,
            _ap = _apMax;   //현재 행동력을 낮아진 최대 행동력만큼 줄임

        for (int i = 0; i < _meter_ap.Length; i++)
            _meter_ap[i].gameObject.SetActive(i < _apMax);
    }

    public void TakeDamage(int dmg, BtlActData.DamageType dmgType)
    {
        if (_ac >= 1)    //방어도가 1 이상 존재할 때
        {
            int realDmg;    //실제 피해량

            if (dmg > _ac)  //피해량이 방어도보다 높을 때
            {
                realDmg = dmg - _ac;    //실제 피해량은 방어도를 제외한 나머지 수치

                Change_AC(false, _ac);  //방어도를 모두 없앰

                Change_Hp(false, realDmg);  //실제 피해량만큼 hp 줄어듬
            }
            else            //피해량이 방어도 이하일 때
            {
                //실제 피해량은 0 (방어도로 모든 피해를 경감)

                Change_AC(false, dmg);  //피해량만큼 방어도를 차감
            }
        }
        else    //방어도가 없을 때
            Change_Hp(false, dmg);

        ParticleSystem eff = null;

        if (dmgType == BtlActData.DamageType.Defense)
            eff = Instantiate(_eff_block, _eff_group);  //방어 이펙트 파티클
        else
            eff = Instantiate(_eff_hit, _eff_group);

        var pos = _e_spr.transform.position;
        var sizeY = _e_spr.GetComponent<SpriteRenderer>().bounds.size.y;
        eff.transform.position = new Vector3(pos.x + 0.5f, pos.y + sizeY / 2, pos.z);
        eff.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
        eff.Play();

        //데미지 타입에 따른 피격음

        //피격 효과
        _e_spr.StartCoroutine(_e_spr.HitFlash());

        //데미지 폰트
        var dmgText = Instantiate(_dmgText_prefab.transform, _eff_group);

        //데미지텍스트 좌표 설정
        dmgText.position = _e_spr.transform.position;
        //데미지텍스트 값 설정
        dmgText.GetChild(0).GetComponent<TextMeshPro>().text = dmg.ToString();
        dmgText.GetChild(1).GetComponent<TextMeshPro>().text = dmg.ToString();
    }

    public void Request_NextBtlAct()    //전투 시스템에서 다음 턴 전투행동을 요청
    {
        _bt.scripts = new TextAsset[] { _data.BT };
        _bt.Compile();
        _bt.Apply();

        if (_btlActStack.Count == 0)  //전투행동 스택에 행동이 없을 경우
        {
            //행동트리를 통해 전투행동 선택
            _bt.Tick();
        }

        var peek = _btlActStack.Peek().Data;    //전투행동 스택 맨 위의 행동
        Debug.Log("적이 다음 할 전투행동: " + peek.Name);

        //스택 맨 위의 전투행동이 행동력을 소모하고
        //현재 행동력이 최소 행동력보다 낮으면
        if (peek.NoDice == false && _ap < peek.DiceMin)  
        {
            _btlActStack.Push(_wait);    //대기 행동 PUSH
            Debug.Log("하지만 행동력이 부족해 " + _wait.Data.Name + " 하기로 함");
        }

        BtlAct nowAct = _btlActStack.Pop();
        BtlActData data = nowAct.Data;

        //주사위 개수가 사전에 지정되지 않았을 경우
        if (_nowDice == 0)  //최소 주사위 ~ 최대 주사위 사이에서 선정 (최대 주사위는 행동력이 가능한 선까지)
            _nowDice = Random.Range(data.DiceMin,
                                (_ap < data.DiceMax ? _ap : data.DiceMax));

        //주사위 개수에 따라 행동 결과 생성
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

        //전투 시스템에 전투행동 정보 전달
        _btlSys.SetBtlAct_Enemy(data, _result);

        //현재 고른 전투행동 정보 초기화
        _nowDice = 0;               //전투행동의 주사위 개수 초기화

        _result = new int[5] { -1, -1, -1, -1, -1 };    //주사위 값 전부 초기화
    }

    ///////////////////////BT에서 사용되는 함수들///////////////////////
    [Task]
    public bool Is_50Percent() //50% 확률로
    {
        if (Random.value < 0.5f) return true;
        else return false;
    }

    [Task]
    public bool Is_Ap_2Under()   //현재 행동력이 2 이하일 때
    {
        if (_ap <= 2) return true;
        else return false;
    }

    [Task]
    public bool Is_AtkSuccess() //이전 턴 공격 성공 시 True
    {
        if (_btlSys.E_HIT_ATK) return true;
        else return false;
    }

    [Task]
    public bool Is_DefSuccess() //이전 턴 방어 성공 시 True
    {
        if (_btlSys.E_HIT_DEF) return true;
        else return false;
    }

    [Task]
    public bool Is_DgeSuccess() //이전 턴 회피 성공 시 True
    {
        if (_btlSys.E_HIT_DGE) return true;
        else return false;
    }

    [Task]
    public bool Is_TacSuccess() //이전 턴 전술 사용 시 True
    {
        if (_btlSys.E_HIT_TAC) return true;
        else return false;
    }

    [Task]
    public bool Is_P_Ap_0Under()  //플레이어 행동력 0 이하면 True
    {
        if (PlayerSys.AP <= 0) return true;
        else return false;
    }

    [Task]
    public bool Is_P_Ap_2Under()    //플레이어 행동력 2 이하면 True
    {
        if (PlayerSys.AP <= 2) return true;
        else return false;
    }

    [Task]
    public bool Is_P_Ap_3Over()     //플레이어 행동력 3 이상이면 True
    {
        if (PlayerSys.AP >= 3) return true;
        else return false;
    }

    [Task]
    public bool Is_P_Ap_4Over()     //플레이어 행동력 4 이상이면 True
    {
        if (PlayerSys.AP >= 4) return true;
        else return false;
    }

    [Task]
    public bool Is_P_FailDefDge()   //플레이어 방어/회피 무의미했다면 True
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
    public bool Is_P_LastWait() //플레이어 마지막 전투행동이 대기면 True
    {
        if (_btlSys.P_LAST == BtlActData.ActType.Wait)
            return true;
        else
            return false;
    }

    [Task]
    public bool Push_RandomBtlAct() //무작위 전투행동 PUSH 
    {
        BtlActData.ActType type = BtlActData.ActType.No;

        Debug.Log("선택 가능한 전투행동 타입 개수: " + _btlActTypeList.Count);

        if (_btlActTypeList.Count > 0)
            type = _btlActTypeList[Random.Range(0, _btlActTypeList.Count)];

        _btlActClueLog = "";
        switch (type)
        {
            case BtlActData.ActType.No:
                Debug.Log("적은 아무 행동도 할 수 없음");
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
    public bool Push_RandomAtk()    //무작위 공격 행동 PUSH
    {
        BtlAct act;

        if (_atkList.Count != 0)    //공격 행동을 보유했을 경우
            act = _atkList[Random.Range(0, _atkList.Count)];    //무작위 공격 행동 지정
        else
            return false;

        _btlActStack.Push(act);

        var actData = act.Data;

        if (actData.NoDice == false && _ap < actData.DiceMin)    //행동력을 소모하면서, 현재 행동력이 최소 행동력보다 낮으면
        {
            Debug.Log("행동력 부족으로 대기");
            Push_Wait();
        }

        return true;
    }

    [Task]
    public bool Push_RandomDef()    //무작위 방어 행동 PUSH
    {
        BtlAct act;

        if (_defList.Count != 0)    //방어 행동을 보유했을 경우
            act = _defList[Random.Range(0, _defList.Count)];    //무작위 방어 행동 지정
        else
            return false;

        var actData = act.Data;

        if (actData.NoDice == false && _ap < actData.DiceMin)    //행동력을 소모하면서, 현재 행동력이 최소 행동력보다 낮으면
        {
            Debug.Log("행동력 부족으로 대기");
            Push_Wait();
        }
        else
        {
            _btlActStack.Push(act);
        }

        return true;
    }

    [Task]
    public bool Push_RandomDge()    //무작위 회피 행동 push
    {
        BtlAct act;

        if (_dgeList.Count != 0)    //회피 행동을 보유했을 경우
            act = _dgeList[Random.Range(0, _dgeList.Count)];    //무작위 회피 행동 지정
        else
            return false;

        var actData = act.Data;

        if (actData.NoDice == false && _ap < actData.DiceMin)    //행동력을 소모하면서, 현재 행동력이 최소 행동력보다 낮으면
        {
            Debug.Log("행동력 부족으로 대기");
            Push_Wait();
        }
        else
        {
            _btlActStack.Push(act);
        }

        return true;
    }

    [Task]
    public bool Push_RandomTac()    //무작위 전술 행동 push
    {
        BtlAct act;

        if (_tacList.Count != 0)    //전술 행동을 보유했을 경우
            act = _tacList[Random.Range(0, _tacList.Count)];    //무작위 전술 행동 지정
        else
            return false;

        var actData = act.Data;

        if (actData.NoDice == false && _ap < actData.DiceMin)    //행동력을 소모하면서, 현재 행동력이 최소 행동력보다 낮으면
        {
            Debug.Log("행동력 부족으로 대기");
            Push_Wait();
        }
        else
        {
            _btlActStack.Push(act);
        }

        return true;
    }

    [Task]
    public bool Push_Wait() //대기 행동 push
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
