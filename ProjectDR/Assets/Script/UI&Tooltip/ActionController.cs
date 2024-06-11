using UnityEngine;
using UnityEngine.UI;
using static PlayerSystem;

public class ActionController : MonoBehaviour
{
    [SerializeField]
    private BattleSystem _btlSys;

    public enum Situation { Off, No, Battle }

    [SerializeField]
    private Situation _situation;   //행동 결정하는 상황
    public Situation SITUATION
    {
        get { return _situation; }
    }

    [Header("# Select Action")]
    [SerializeField]
    private BtlActData _nowBtlAct;   //선택한 전투 행동

    [Header("# Action Dice Info")]
    [SerializeField]
    private ICreature.Stats _nowStat;   //선택한 행동의 스탯
    [SerializeField]
    private int _nowReroll;             //재굴림 횟수
    [SerializeField]
    private int _nowDice;               //선택한 주사위 개수
    [SerializeField]
    private int _stopDice;              //멈춘 주사위 개수
    [SerializeField]
    private int[] _nowResult = { -1, -1, -1, -1, -1 }; //주사위 결과
    [SerializeField]
    private int _nowTotal;

    [Header("# Action List")]
    [SerializeField]
    private VerticalLayoutGroup _actList;   //행동 리스트

    [SerializeField]
    private int _cursor_nowAct = -1;    //현재 선택한 행동의 인덱스
    public int NOW_CURSOR
    {
        get { return _cursor_nowAct; }
    }

    [SerializeField]
    private ActionListSlot[] _actSlot;      //행동 슬롯

    [SerializeField]
    private ActionListSlotSelected _actSlot_select; //선택한 행동 슬롯

    [SerializeField]
    private DiceSelectPannel _diceSelectPannel; //주사위 선택창
    [SerializeField]
    private RectTransform _btn_noDiceAct;       //주사위 없는 행동 결정 버튼

    [Header("# Dice & Board")]
    [SerializeField]
    private GameObject _diceBoard;  //주사위 보드
    [SerializeField]
    private DiceSetting[] _diceObj; //주사위 오브젝트

    [Header("# DiceResult")]
    [SerializeField]
    private DiceResultPannel _resultPannel; //주사위 결과창

    [Header("# Graphic Referernce")]
    [SerializeField]
    private Sprite[] _spr_actType;      //행동타입 스프라이트
    [SerializeField]
    private Material[] _mat_diceSide;   //주사위 면 material

    private string[] _statName_arr = { "", "힘", "지능", "손재주", "민첩", "건강", "의지" };

    public void Set_ActListSituation(Situation situation)   //행동목록 상황 설정
    {
        if (situation != Situation.Off)
            _situation = situation;

        switch (situation)
        {
            case Situation.No:  //행동 선택 상황 종료
                if (_cursor_nowAct != -1)
                    ActionSelect_Cancle(_cursor_nowAct);    //선택 중이던 행동 취소

                _actList.gameObject.SetActive(false);       //행동 목록 Off

                //주사위 & 주사위 보드 Off
                _diceBoard.SetActive(false);
                Dice_Off();

                _resultPannel.RerollButton_OnOff(false);        //재굴림 버튼 Off
                _resultPannel.ActStartButton_OnOff(false);      //행동 개시 버튼 Off
                break;
            case Situation.Off: //상황 내에서 행동 목록 표시 Off
                _actList.gameObject.SetActive(false);       //행동 목록 Off
                break;
            case Situation.Battle:  //전투 행동 목록 표시
                _actList.gameObject.SetActive(true);        //행동 목록 On
                ActList_Synch_Battle(); //행동 리스트 목록을 전투 행동 요소를 넣어서 동기화
                break;
        }

        DiceSelectPannel_OnOff(false);  //주사위 선택창 Off
        NoDiceButton_OnOff(false);      //주사위 없는 행동 결정 버튼 Off

        if (_situation != Situation.Off)    
            DiceResultPannel_Off(); //주사위 결과창 Off

        RefreshLayout();
    }

    public void ActList_Synch_Battle()  //행동 리스트 동기화 (전투)
    {
        var list = PlayerSys.ActList;

        for (int i = 0; i < _actSlot.Length; i++)
        {
            if (i < list.Count)
            {
                _actSlot[i].gameObject.SetActive(true);
                _actSlot[i].Change_SlotContent(_spr_actType[(int)list[i].Data.Type],
                                                list[i].Data.Name,
                                                _statName_arr[(int)list[i].Stat]);
            }
            else
                _actSlot[i].gameObject.SetActive(false);
        }

        //선택 중인 슬롯이 있을 경우, 선택 취소
        if (_cursor_nowAct != -1)
            ActionSelect_Cancle(_cursor_nowAct);

        //각종 변수 상태 초기화
        _nowDice = 0;
        _nowResult = new int[5] { -1, -1, -1, -1, -1 };
        _nowTotal = 0;
    }

    public void ActionClick(int order)   //행동 슬롯 클릭
    {
        for (int i = 0; i < _actList.gameObject.transform.childCount; i++)
        {
            if (order == i)  //클릭한 행동 슬롯의 순서 취득
            {
                if (_cursor_nowAct == i)    //해당 순서의 행동을 이미 선택한 경우
                    ActionSelect_Cancle(i); //행동 선택 취소
                else
                {
                    if (_cursor_nowAct != -1)   //이미 다른 행동을 선택한 경우
                        ActionSelect_Cancle(_cursor_nowAct);    //선택 취소

                    ActionSelect(i);    //새 행동을 선택
                }
                break;
            }
        }
    }

    public void ActionSelect(int order) //order번째 행동 선택
    {
        _cursor_nowAct = order;
        _actSlot[_cursor_nowAct].gameObject.SetActive(false);

        //선택 슬롯 표시
        var selected = _actSlot_select.gameObject.transform;
        selected.gameObject.SetActive(true);    //선택 슬롯 On

        if (_situation == Situation.Battle)
        {
            _nowBtlAct = PlayerSys.ActList[_cursor_nowAct].Data;
            _nowStat = PlayerSys.ActList[_cursor_nowAct].Stat;
            _actSlot_select.Change_SlotContent(_spr_actType[(int)_nowBtlAct.Type],
                                                _nowBtlAct.Name, _statName_arr[(int)_nowStat]);
        }

        selected.SetParent(_actList.gameObject.transform);  //선택 슬롯의 패런츠 변경
        selected.SetSiblingIndex(_cursor_nowAct);           //선택 슬롯의 레이아웃 내 위치 설정
        _actSlot_select.Set_SlotOrder(_cursor_nowAct);      //선택 슬롯의 order 변경

        if (_nowStat != ICreature.Stats.No)
        {
            NoDiceButton_OnOff(false);      //주사위 없는 행동 결정 버튼 Off
            DiceSelectPannel_OnOff(true);   //주사위 선택창 On
        }
        else
        {
            NoDiceButton_OnOff(true);       //주사위 없는 행동 결정 버튼 On
            DiceSelectPannel_OnOff(false);  //주사위 선택창 Off
        }

        //툴팁 Off

        RefreshLayout();
    }

    public void ActionSelect_Cancle(int order)  //order번째 행동 선택 취소
    {
        DiceSelectPannel_OnOff(false);  //주사위 선택창 Off
        NoDiceButton_OnOff(false);      //주사위 없는 행동 결정 버튼 Off

        var selected = _actSlot_select.gameObject.transform;
        selected.SetParent(transform);          //선택 슬롯의 패런츠 변경
        selected.gameObject.SetActive(false);   //선택 슬롯 Off

        _actSlot[order].gameObject.SetActive(true);
        _cursor_nowAct = -1;    //선택 취소

        _nowBtlAct = null;
        _nowStat = ICreature.Stats.No;

        RefreshLayout();
    }

    public void DiceSelectPannel_OnOff(bool isOn)
    {
        _diceSelectPannel.gameObject.SetActive(isOn);

        if (isOn)
        {
            //주사위 선택창 위치 조정
            var upSlot = _cursor_nowAct < _actSlot.Length / 2;

            _diceSelectPannel.IsUpSlot(upSlot); //주사위 선택창 pivot 설정 (슬롯이 행동 목록의 전반or후반인지에 따라)
            _diceSelectPannel.Set_Position(_actSlot[_cursor_nowAct].transform.position);    //주사위 선택창 위치 설정
            _diceSelectPannel.Set_AnchoredPosition(_actSlot_select.Get_AddX(), _actSlot_select.Get_AddY(upSlot));

            //행동 스탯 지정
            int[] statArr = null;

            switch (_nowStat)
            {
                case ICreature.Stats.STR:   //힘
                    statArr = PlayerSys.STR;
                    break;
                case ICreature.Stats.INT:   //지능
                    statArr = PlayerSys.INT;
                    break;
                case ICreature.Stats.DEX:   //손재주
                    statArr = PlayerSys.DEX;
                    break;
                case ICreature.Stats.AGI:   //민첩
                    statArr = PlayerSys.AGI;
                    break;
                case ICreature.Stats.CON:   //건강
                    statArr = PlayerSys.CON;
                    break;
                case ICreature.Stats.WIL:   //의지
                    statArr = PlayerSys.WIL;
                    break;
            }

            //행동 스탯의 재굴림 설정
            _nowReroll = PlayerSys.GetReroll(_nowStat);

            _diceSelectPannel.Change_RerollText(_nowReroll.ToString());
            _diceSelectPannel.Change_StatDiceImage(statArr);
            _diceSelectPannel.NowDice_Reset();
        }
        else
            _diceSelectPannel.DiceZero();

        PlayerSys.Change_Ap_UsePreview(_nowDice);   //행동력 소모 미리보기 Off
    }

    public void NoDiceButton_OnOff(bool isOn)
    {
        _btn_noDiceAct.gameObject.SetActive(isOn);

        if (isOn)
        {
            _btn_noDiceAct.transform.position = _actSlot[_cursor_nowAct].transform.position;    //버튼 위치 설정
            _btn_noDiceAct.anchoredPosition = new Vector2(_btn_noDiceAct.anchoredPosition.x + _actSlot_select.Get_AddX() + 8,
                                                            _btn_noDiceAct.anchoredPosition.y);
        }
    }

    public void ActionDecision(int nowDice)
    {
        _nowDice = nowDice; //주사위 개수 결정

        Set_ActListSituation(Situation.Off);

        switch (_situation)
        {
            case Situation.Battle: //전투 상황
                if (_nowBtlAct.NoDice)  //주사위가 없는 행동일 때
                {
                    // 즉시 행동 결정
                }
                else    //주사위가 있는 행동이면
                {
                    Set_DiceSide(_nowStat); //결정한 행동에 맞게 주사위 오브젝트 설정
                    DiceRoll(); //주사위 굴리기
                }
                break;
        }
    }

    public void Set_DiceSide(ICreature.Stats stat)  //주사위 오브젝트의 각 면을 스탯에 맞게 변경
    {
        Material[] mat = new Material[6];
        int[] temp_stat = new int[6];

        switch (stat)
        {
            case ICreature.Stats.STR:
                temp_stat = PlayerSys.STR;
                break;
            case ICreature.Stats.INT:
                temp_stat = PlayerSys.INT;
                break;
            case ICreature.Stats.DEX:
                temp_stat = PlayerSys.DEX;
                break;
            case ICreature.Stats.AGI:
                temp_stat = PlayerSys.AGI;
                break;
            case ICreature.Stats.CON:
                temp_stat = PlayerSys.CON;
                break;
            case ICreature.Stats.WIL:
                temp_stat = PlayerSys.WIL;
                break;
        }

        for (int i = 0; i < temp_stat.Length; i++)
        {
            if (temp_stat[i] >= 10)     //10 이상의 스탯은 10으로 취급
                mat[i] = _mat_diceSide[10];
            else if (temp_stat[i] <= 0) //0 이하의 스탯은 0으로 취급
                mat[i] = _mat_diceSide[0];
            else
                mat[i] = _mat_diceSide[temp_stat[i]];
        }

        foreach (DiceSetting d in _diceObj)
            d.Change_DiceSide(mat);
    }

    public void DiceRoll()  //주사위 굴림
    {
        _resultPannel.ActionInfoPannel_OnOff(true);     //행동 정보창 On
        if (_situation == Situation.Battle)
            _resultPannel.Change_ActInfo(_nowBtlAct.Type, _nowBtlAct.Name); //행동 타입 아이콘, 행동명 설정

        _resultPannel.DiceResultPannel_OnOff(true);     //주사위 결과창 On
        _resultPannel.Set_NewDiceTotal(_nowDice);       //주사위 결과창 초기화

        _diceBoard.SetActive(true); //주사위 보드 On

        _resultPannel.RerollButton_OnOff(true);         //재굴림 버튼 On
        _resultPannel.SetAble_RerollButton(false);      //재굴림 버튼 상호작용 Off

        _resultPannel.ActStartButton_OnOff(true);       //행동 개시 버튼 On
        _resultPannel.SetAble_ActStartButton(false);    //행동 개시 버튼 상호작용 Off

        Set_DiceObj();  //주사위 오브젝트 배치
    }

    public void DiceReroll()    //주사위 재굴림
    {
        _resultPannel.Set_NewDiceTotal(_nowDice);   //주사위 총합창 초기화

        if (_nowReroll > 0) //재굴림 횟수가 남아있다면
        {
            _nowReroll--;

            _resultPannel.SetAble_RerollButton(false);      //재굴림 버튼 상호작용 불가 처리
            _resultPannel.SetAble_ActStartButton(false);    //행동 개시 버튼 상호작용 불가 처리

            Set_DiceObj();  //주사위 오브젝트 배치
        }
    }

    public void Set_DiceObj()   //주사위 오브젝트 설정
    {
        _resultPannel.Change_RerollText(_nowReroll.ToString()); //재굴림 횟수 설정
        _resultPannel.Change_DiceTotal(""); //주사위 총합 미확정

        _stopDice = 0;  //멈춘 주사위 0개로 초기화

        for (int i = 0; i < _diceObj.Length; i++)
        {
            if (i < _nowDice)   //굴릴 주사위라면
            {
                //주사위 오브젝트 활성화
                _diceObj[i].gameObject.SetActive(true);
                _diceObj[i].DiceObject_OnOff(true);
                _diceObj[i].Set_DiceTransform(_diceBoard.transform.position);
            }
            else    //굴리지 않는 주사위라면
            {
                //주사위 오브젝트 비활성화
                if (_diceObj[i].gameObject.activeSelf)
                    _diceObj[i].DiceObject_OnOff(false);

                _diceObj[i].gameObject.SetActive(false);
            }
        }
    }

    public void DiceStop(int order, int result) //정지한 주사위 하나의 값을 기록
    {
        var value = -1;

        switch (_nowStat)   //주사위 값 체크
        {
            case ICreature.Stats.STR:
                value = PlayerSys.STR[result];
                break;
            case ICreature.Stats.INT:
                value = PlayerSys.INT[result];
                break;
            case ICreature.Stats.DEX:
                value = PlayerSys.DEX[result];
                break;
            case ICreature.Stats.AGI:
                value = PlayerSys.AGI[result];
                break;
            case ICreature.Stats.CON:
                value = PlayerSys.CON[result];
                break;
            case ICreature.Stats.WIL:
                value = PlayerSys.WIL[result];
                break;
        }

        if (value >= 10)
            _nowResult[order] = 10; //10 이상의 스탯은 10으로 취급
        else if (value <= 0)
            _nowResult[order] = 0;  //0 이하의 스탯은 10으로 취급
        else
            _nowResult[order] = value;

        _resultPannel.Change_DiceResult(order, _nowResult[order]);
    }

    public void Add_StopDice()
        => _stopDice++; //멈춘 주사위 하나 추가

    public void Check_DiceTotal()   //굴린 주사위가 전부 멈췄는지 체크, 전부 멈췄을 시 총합을 기록
    {
        if (_stopDice == _nowDice)  //모든 주사위가 멈췄다면
        {
            _nowTotal = 0;

            for (int i = 0; i < _nowDice; i++)
                _nowTotal += _nowResult[i];

            if (_situation == Situation.Battle) //전투 중인 경우
                _resultPannel.SetAble_ActStartButton(true); //행동 개시 버튼 상호작용 On

            if (_nowReroll > 0)
                _resultPannel.SetAble_RerollButton(true);   //남은 재굴림 횟수 > 0이면, 재굴림 버튼 상호작용 가능
        }
        else
            return;

        _resultPannel.Change_DiceTotal(_nowTotal.ToString());
    }

    public void ActionStart()
    {
        Dice_Off(); //주사위 오브젝트 Off
        _diceBoard.SetActive(false);    //주사위 보드 Off

        _resultPannel.RerollButton_OnOff(false);    //재굴림 버튼 Off
        _resultPannel.ActStartButton_OnOff(false);  //행동 개시 버튼 Off

        if (_situation == Situation.Battle) //전투 행동을 개시할 경우
        {
            //전투 시스템에 행동 정보 전달
            _btlSys.Set_BtlAct_Player(_nowBtlAct, _nowResult);
        }

        //각종 상태 변수 초기화
        _nowDice = 0;
        _nowResult = new int[5] { -1, -1, -1, -1, -1 };
    }

    void RefreshLayout()  //행동리스트 새로고침
    {
        Canvas.ForceUpdateCanvases();
        _actList.enabled = false;
        _actList.enabled = true;
    }

    public void Dice_Off()  //주사위 오브젝트 Off
    {
        for (int i = 0; i < _diceObj.Length; i++)
        {
            if (_diceObj[i].gameObject.activeSelf)
                _diceObj[i].gameObject.SetActive(false);
        }
    }

    public void DiceResultPannel_Off()  //주사위 결과창 Off
    {
        _resultPannel.ActionInfoPannel_OnOff(false);    //행동결과창 Off
        _resultPannel.DiceResultPannel_OnOff(false);    //주사위 결과창 Off
        _resultPannel.RerollButton_OnOff(false);        //재굴림 버튼 Off
        _resultPannel.ActStartButton_OnOff(false);      //행동 개시 버튼 Off
    }
}
