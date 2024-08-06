using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EventData;
using Random = UnityEngine.Random;

public class DungeonEventSystem : MonoBehaviour
{
    public static DungeonEventSystem EvntSys = null;

    public enum DiceCheckResult { Success, MinFail, MaxFail }

    // 수준에 따른 평균적인 주사위 1개의 값:  1~4 / 3~7 / 5~10
    private int[] AvgDice_first = new int[] { 1, 4 };
    private int[] AvgDice_Mid = new int[] { 3, 7 };
    private int[] AvgDice_Last = new int[] { 5, 10 };

    [SerializeField]
    private PlayerSystem _playerSys;
    private DungeonSystem _dgnSys;  //이벤트 몰입 시, 이벤트를 유발한 던전 스크립트 넘겨받아 기록해야함
    private GameObject _camera_dgn; //이벤트 몰입 시, 던전 카메라 스크립트를 넘겨받아 기록해야함

    [SerializeField]
    private ActionController _actController;    //행동 컨트롤러
    [SerializeField]
    private DiceResultPannel _p_resultPannel;   //플레이어 주사위 결과창

    [SerializeField]
    private GameLog _evntLog;           //전투 로그
    [SerializeField]
    private ItemSystem _itemSys;        //아이템 시스템
    [SerializeField]
    private RewardPannel _rewardPannel; //전리품 창

    [Header("# Camera")]
    [SerializeField]
    private GameObject _camera_evnt;     //전투 카메라

    [Header("# Event UI & Sprite")]
    [SerializeField]
    private SpriteSystem _p_spr;        //플레이어 스프라이트
    [SerializeField]
    private SpriteRenderer _p_sprRend;  //플레이어 스프라이트 렌더러

    [SerializeField]
    private Animator _evnt_anima;       //이벤트 스프라이트 애니메이터

    [SerializeField]
    private GameObject _btn_eventEnd;      //이벤트 종료 버튼

    //주사위 조건 창
    //주사위 조건 텍스트
    [SerializeField]
    private HorizontalLayoutGroup _pannel_diceRule;
    [SerializeField]
    private GameObject[] _layout_diceRule;
    [SerializeField]
    private TextMeshProUGUI _txt_totalUp_checkMin;
    [SerializeField]
    private TextMeshProUGUI[] _txt_totalBetween_checkMinMax;
    [SerializeField]
    private TextMeshProUGUI _txt_eachUp_checkMin;
    [SerializeField]
    private TextMeshProUGUI[] _txt_eachBetween_checkMinMax;

    [Header("# NowEvent")]
    [SerializeField]
    private EventModule _nowEvnt;

    [Serializable]
    public class EvntAct
    {
        public EventAction Data;

        public CheckRule Rule;  //주사위 체크 규칙
        public int Dice;        //굴리는 주사위 개수

        public int CheckMin; //주사위 체크 행동 시 성공 최소값
        public int CheckMax; //주사위 체크 행동 시 성공 최대값
    }

    [SerializeField]
    private List<EvntAct> _nowEvnt_actList;
    public List<EvntAct> ActList
    {
        get { return _nowEvnt_actList; }
    }

    [SerializeField]
    private bool _resultProcess;    //이 변수가 false일 때, 이벤트 결과 처리

    [Header("# Background")]
    [SerializeField]
    private Transform _tileSet_center;
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
        if (EvntSys)
        {
            DestroyImmediate(gameObject);
            return;
        }
        else
        {
            EvntSys = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Record_DungeonScript(DungeonSystem dgnSys, GameObject camera)
    {
        _dgnSys = dgnSys;   //현재 던전의 스크립트 할당
        _camera_dgn = camera;   //현재 던전의 카메라 오브젝트 할당
    }

    public void EventStart(EventModule evnt)    //이벤트 시작 & 이벤트 행동 별 주사위 체크 변수 설정
    {
        _evntLog.gameObject.SetActive(true);

        _nowEvnt = evnt;    //이벤트 정보 받음
        _nowEvnt_actList = new List<EvntAct>();     //이벤트 리스트 초기화

        //이벤트 행동 리스트에 행동들을 추가
        for (int i = 0; i < _nowEvnt.ActList.Count; i++)
        {
            if (_nowEvnt.ActList[i].Name != "")
                Add_NewEventAction(i, _nowEvnt.ActList[i]); //이벤트 행동 추가
        }

        _dgnSys.EVNT_PROCESS = true;    //이벤트 진행 상황 시작

        _camera_evnt.SetActive(true);   //이벤트 카메라 활성화
        _camera_dgn.SetActive(false);   //던전 카메라 비활성화

        //플레이어 메뉴 버튼 Off
        _playerSys.MenuButton_OnOff_Status(false);
        _playerSys.MenuButton_OnOff_Inventory(false);
        _playerSys.MenuButton_OnOff_ActList(false);

        //이벤트 오브젝트 스프라이트 설정
        _evnt_anima.gameObject.SetActive(true);
        _evnt_anima.runtimeAnimatorController = _nowEvnt.EventObj_Anima;

        Refresh_Log();  //로그 새로고침
        _evntLog.SetLog_EventStart(_nowEvnt.StartLog);  //시작 로그 출력

        //이벤트 행동 목록 출력
        _actController.Set_ActListSituation(ActionController.Situation.Event);
    }

    void Add_NewEventAction(int index, EventAction act) //행동을 추가하고, 주사위 체크가 있는 행동은 체크 조건과 값도 생성
    {
        var rule = act.Check.Rule;
        var dice = 0;
        var checkMin = 0;
        var checkMax = 0;

        //현재 층에 따라 주사위 평균값을 선정함
        var avgDice = AvgDice_first;

        if (act.IsDiceCheck)    //주사위 체크 행동인 경우
        {
            //주사위 개수 무작위 선정
            dice = Random.Range(1, 6);

            //현재 층과 주사위 개수에 따라 최소값과 최대값 선정
            switch (rule)
            {
                case CheckRule.Total_Up:
                    for (int i = 0; i < dice; i++)
                        checkMin += Random.Range(avgDice[0], avgDice[1] + 1);
                    break;
                case CheckRule.Total_Between:
                    for (int i = 0; i < dice; i++)
                        checkMin += Random.Range(avgDice[0], avgDice[1] + 1);

                    for (int i = 0; i < dice; i++)
                        checkMax += Random.Range(avgDice[0], avgDice[1] + 1);

                    if (checkMin > checkMax)
                    {
                        var temp = checkMin;
                        checkMin = checkMax;
                        checkMax = temp;
                    }
                    break;
                case CheckRule.Each_Up:
                    checkMin = Random.Range(avgDice[0], avgDice[1]);
                    break;
                case CheckRule.Each_Between:
                    checkMin = Random.Range(avgDice[0], avgDice[1]);
                    checkMax = Random.Range(avgDice[0], avgDice[1]);

                    if (checkMin > checkMax)
                    {
                        var temp = checkMin;
                        checkMin = checkMax;
                        checkMax = temp;
                    }
                    break;
            }
        }

        //이벤트 행동 클래스 생성
        EvntAct newAct = new EvntAct()
        {
            Data = act,
            Rule = rule,
            Dice = dice,
            CheckMin = checkMin,
            CheckMax = checkMax
        };

        //생성한 이벤트 행동을 리스트에 추가
        if (index >= 0)
            _nowEvnt_actList.Insert(index, newAct);
        else
            _nowEvnt_actList.Add(newAct);
    }

    //주사위 조건 텍스트 생성 함수

    public DiceCheckResult Check_DiceCondition(int index, int total, int[] result)    //주사위 조건 체크 결과 반환 함수  
    {
        var act = _nowEvnt_actList[index];

        switch (act.Rule)
        {
            case CheckRule.Total_Up:
                if (total >= act.CheckMin)
                    return DiceCheckResult.Success;
                else
                    return DiceCheckResult.MinFail;
            case CheckRule.Total_Between:
                if (total < act.CheckMin)
                    return DiceCheckResult.MinFail;
                else if (total > act.CheckMax)
                    return DiceCheckResult.MaxFail;
                else
                    return DiceCheckResult.Success;
            case CheckRule.Each_Up:
                for (int i = 0; i < result.Length; i++)
                {
                    if (result[i] == -1)
                        break;
                    else if (result[i] >= act.CheckMin)
                        continue;
                    else
                        return DiceCheckResult.MinFail;
                }

                return DiceCheckResult.Success;
            case CheckRule.Each_Between:
                for (int i = 0; i < result.Length; i++)
                {
                    if (result[i] == -1)
                        break;
                    else if (result[i] >= act.CheckMin && result[i] <= act.CheckMax)
                        continue;
                    else
                        return DiceCheckResult.MinFail;
                }
                return DiceCheckResult.Success;
        }
        return DiceCheckResult.MinFail;
    }

    public void DiceCheck_Success()    //주사위 체크 성공
    {
        Refresh_Log();
        _evntLog.SetLog_DiceCheck_Success("");
    }

    public void DiceCheck_MinFail()    //주사위 체크 실패 (값 미달)
    {
        Refresh_Log();
        _evntLog.SetLog_DiceCheck_MinFail("");
    }

    public void DiceCheck_MaxFail()    //주사위 체크 실패 (값 초과)
    {
        Refresh_Log();
        _evntLog.SetLog_DiceCheck_MaxFail("");
    }

    //이벤트 결과 실행 함수
    public IEnumerator EventResultFlow(bool isSuccess, bool failMax)
    {
        var act = _nowEvnt_actList[_actController.NOW_CURSOR];  //행동의 결과나 주사위 체크 결과에 의해 이벤트 결과 실행
        EventResult result = null;
        var index = -1;

        if (isSuccess)  //행동 성공 시
            index = act.Data.Result_Success[Random.Range(0, act.Data.Result_Success.Length)];
        else
        {
            if (failMax)
                index = act.Data.Result_FailMax[Random.Range(0, act.Data.Result_FailMax.Length)];
            else
                index = act.Data.Result_Fail[Random.Range(0, act.Data.Result_Fail.Length)];
        }

        result = _nowEvnt.Result[index];

        Refresh_Log();

        _evntLog.SetLog_EventResult(result.Log);    //로그 출력

        for (int i = 0; i < result.Type.Length; i++)
        {
            yield return new WaitUntil(() => _resultProcess == false);  //다른 결과가 진행 중일 경우 실행 잠시 중단

            switch (result.Type[i])
            {
                case ResultType.ActRemove:  //이 행동 제거
                    _nowEvnt_actList.Remove(act);
                    break;
                case ResultType.ActAdd:     //새 행동 추가
                    foreach (EventAction ea in result.NewAct)
                        Add_NewEventAction(_actController.NOW_CURSOR, ea);
                    break;
                case ResultType.Exp:        //경험치
                    Set_ResultProcess(true);    //이벤트 결과 진행 시작
                    _rewardPannel.RewardPannel_Exp_OnOff(true); //경험치 패널 On
                    _rewardPannel.Set_RewardExpInfo();  //경험치 획득 패널의 수치 설정
                    _rewardPannel.Set_GetExpText(result.Exp);   //획득 경험치 표시
                    break;
                case ResultType.Item:       //아이템
                    _rewardPannel.RewardPannel_Item_OnOff(true);    //아이템 획득 패널 On
                    _itemSys.Reward_Clear();    //이전 전리품 모두 제거
                    _itemSys.ON_REWARD = true;

                    //아이템 개수 무작위
                    var amount = Random.Range(2, 5);

                    ItemData[] item;

                    if (result.Item.Length == 0)
                        item = _nowEvnt.Item.ToArray();
                    else
                        item = result.Item.ToArray();

                    for (int j = 0; j < amount; j++)
                        _itemSys.Create_Item(item[Random.Range(0, item.Length)], ItemSystem.ItemSlotType.Reward, i);

                    _itemSys.Set_RewardIcon();  //전리품 아이템들의 아이콘 설정
                    break;
                case ResultType.Buff:       //버프

                    break;
                case ResultType.Debuff:     //디버프

                    break;
                case ResultType.Btl:    //전투

                    break;
                case ResultType.EvntEnd:    //이벤트 종료
                    //플레이어 메뉴 버튼 On                    
                    _playerSys.MenuButton_OnOff_Status(true);
                    _playerSys.MenuButton_OnOff_Inventory(true);
                    _playerSys.MenuButton_OnOff_ActList(true);

                    _actController.Set_ActListSituation(ActionController.Situation.No); //이벤트 행동 목록, 주사위 보드 off
                    _actController.Dice_Off();  //주사위 off
                    
                    DiceRulePannel_OnOff(false, 0);    //주사위 조건 창 off
                    _actController.DiceSelectPannel_OnOff(false);   //주사위 선택창 off
                    _actController.NoDiceButton_OnOff(false);       //주사위 없는 행동 개시 버튼 off
                    _actController.DiceResultPannel_Off();          //주사위 결과창 off

                    _evnt_anima.gameObject.SetActive(false);    //이벤트 오브젝트 스프라이트 off
                    _btn_eventEnd.SetActive(true); //이벤트 종료 버튼 on
                    yield break;
            }

            Debug.Log("아직 이벤트 종료되지 않음");

            DiceRulePannel_OnOff(false, 0);    //주사위 조건 창 off
            _actController.DiceResultPannel_Off();  //주사위 결과창 off

            _actController.Set_ActListSituation(ActionController.Situation.Event);  //행동 리스트 재출력
        }
    }

    public void Set_ResultProcess(bool b) => _resultProcess = b;

    public void EventEnd()  //이벤트 종료
    {
        _nowEvnt = null;    //이벤트 정보 제거
        _nowEvnt_actList = null;    //이벤트 행동 목록 제거

        _dgnSys.EVNT_PROCESS = false;   //이벤트 진행 상황 off

        _camera_dgn.SetActive(true);    //던전 카메라 활성화
        _camera_evnt.SetActive(false);  //이벤트 카메라 비활성화

        _actController.Set_ActListSituation(ActionController.Situation.No); //이벤트 행동 목록, 주사위 보드 off

        _actController.DiceSelectPannel_OnOff(false);   //주사위 선택창 off
        _actController.DiceResultPannel_Off();          //주사위 결과창 off
        _actController.NoDiceButton_OnOff(false);       //주사위 없는 행동 개시 버튼 Off

        //주사위 조건 창 off
        _evnt_anima.gameObject.SetActive(false);    //이벤트 오브젝트 스프라이트 off

        _evntLog.gameObject.SetActive(false);   //이벤트 로그 off

        _itemSys.Reward_Clear();    //전리품 모두 제거
        _rewardPannel.RewardPannel_Exp_OnOff(false);
        _rewardPannel.RewardPannel_Item_OnOff(false);
        _itemSys.ON_REWARD = false;

        _btn_eventEnd.SetActive(false); //이벤트 종료 버튼 off
    }

    public void DiceRulePannel_OnOff(bool isOn, int index) //주사위 조건 패널 OnOff
    {
        _pannel_diceRule.gameObject.SetActive(isOn);

        if (isOn)
        {
            var rule = _nowEvnt_actList[index].Rule;

            for (int i = 0; i < _layout_diceRule.Length; i++)
            {
                if (i + 1 == (int)rule)
                    _layout_diceRule[i].gameObject.SetActive(true);
                else
                    _layout_diceRule[i].gameObject.SetActive(false);
            }

            var evntAct = _nowEvnt_actList[index];

            switch (rule)
            {
                case CheckRule.Total_Up:
                    _txt_totalUp_checkMin.text = evntAct.CheckMin.ToString(); ;
                    break;
                case CheckRule.Total_Between:
                    _txt_totalBetween_checkMinMax[0].text = evntAct.CheckMin.ToString();
                    _txt_totalBetween_checkMinMax[1].text = evntAct.CheckMax.ToString();
                    break;
                case CheckRule.Each_Up:
                    _txt_eachUp_checkMin.text = evntAct.CheckMin.ToString();
                    break;
                case CheckRule.Each_Between:
                    _txt_eachBetween_checkMinMax[0].text = evntAct.CheckMin.ToString();
                    _txt_eachBetween_checkMinMax[1].text = evntAct.CheckMax.ToString();
                    break;
            }
        }

        Canvas.ForceUpdateCanvases();
        _pannel_diceRule.enabled = false;
        _pannel_diceRule.enabled = true;
    }

    //던전 시스템으로부터 주변 지형 정보를 받아, 전투 배경을 조정함
    public void Set_EventField(bool[] wall, Sprite[] tileSprite, Sprite[] wallSprite)
    {
        //벽 유무 배열
        var wallBool = wall.ToArray();   // [1_2] [1_2_b] [3] [3_b] [4_5] [7_8] [9] [9_b] [10_11] [10_11_b] [12_b] 
        //타일 스프라이트 배열
        var spr_tile = tileSprite.ToArray();
        //벽 스프라이트 배열
        var spr_wall = wallSprite.ToArray();

        _tileSet_1_2.GetChild(0).gameObject.SetActive(wallBool[0]);
        _tileSet_1_2.GetChild(1).gameObject.SetActive(wallBool[0]);

        _tileSet_1_2_beyond.GetChild(0).gameObject.SetActive(wallBool[1]);
        _tileSet_1_2_beyond.GetChild(1).gameObject.SetActive(wallBool[1]);

        _tileSet_3.GetChild(0).gameObject.SetActive(wallBool[2]);
        _tileSet_3.GetChild(1).gameObject.SetActive(wallBool[2]);

        _tileSet_3_beyond.GetChild(0).gameObject.SetActive(wallBool[3]);
        _tileSet_3_beyond.GetChild(1).gameObject.SetActive(wallBool[3]);

        _tileSet_4_5.GetChild(0).gameObject.SetActive(wallBool[4]);

        _tileSet_7_8.GetChild(0).gameObject.SetActive(wallBool[5]);

        _tileSet_9.GetChild(0).gameObject.SetActive(wallBool[6]);
        _tileSet_9.GetChild(1).gameObject.SetActive(wallBool[6]);

        _tileSet_9_beyond.GetChild(0).gameObject.SetActive(wallBool[7]);
        _tileSet_9_beyond.GetChild(1).gameObject.SetActive(wallBool[7]);

        _tileSet_10_11.GetChild(0).gameObject.SetActive(wallBool[8]);
        _tileSet_10_11.GetChild(1).gameObject.SetActive(wallBool[8]);

        _tileSet_10_11_beyond.GetChild(0).gameObject.SetActive(wallBool[9]);
        _tileSet_10_11_beyond.GetChild(1).gameObject.SetActive(wallBool[9]);

        _tileSet_12.GetChild(0).gameObject.SetActive(wallBool[0] || wallBool[8]); //1_2 또는 10_11에 천장이 존재할 경우

        _tileSet_12_beyond.GetChild(0).gameObject.SetActive(wallBool[10]);

        //벽 그래픽 조정
        if (wallSprite != null)
        {

        }

        //타일 그래픽 조정
        if (tileSprite != null)
        {
            //center 타일 랜덤 스프라이트
            for (int i = 0; i < _tileSet_center.childCount; i++)
            {
                _tileSet_center.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite
                    = spr_tile[Random.Range(0, spr_tile.Length)];
            }

            //1_2 타일에 벽이 없다면 랜덤 스프라이트
            if (wallBool[0] == false)
            {
                for (int i = 2; i < _tileSet_1_2.childCount; i++)
                {
                    _tileSet_1_2.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite
                        = spr_tile[Random.Range(0, spr_tile.Length)];
                }
            }
            //10_11 타일에 벽이 없다면 랜덤 스프라이트
            if (wallBool[8] == false)
            {
                for (int i = 2; i < _tileSet_10_11.childCount; i++)
                {
                    _tileSet_10_11.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite
                        = spr_tile[Random.Range(0, spr_tile.Length)];
                }
            }
        }

        //타일 장식 조정
    }

    void Refresh_Log()
    {
        _evntLog.gameObject.SetActive(false);
        _evntLog.gameObject.SetActive(true);
    }
}
