using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static PlayerSystem;
using static EventData;

public class DiceSelectPannel : MonoBehaviour
{
    private float _offAlpha = 0.2f;

    private RectTransform _rect;

    [SerializeField]
    private DungeonEventSystem _evntSys;
    [SerializeField]
    private ActionController _actController;

    [Header("# Main Info")]
    [SerializeField]
    private TextMeshProUGUI _txt_reroll;
    [SerializeField]
    private Image[] _img_statDice;  //행동 스탯 주사위 이미지 표기
    [SerializeField]
    private Button _btn_decision;       //행동 결정 버튼
    [SerializeField]
    private Image[] _img_btn_decision;  //행동 결정 버튼 이미지

    [Header("# DiceSlot")]
    [SerializeField]
    private GameObject _diceSlotSet_btl;    //주사위 슬롯 집단(전투)
    [SerializeField]
    private Image[] _diceSlot_btl;      //주사위 슬롯(전투)
    [SerializeField]
    private GameObject _diceSlotSet_evnt;   //주사위 슬롯 집단(이벤트)
    [SerializeField]
    private Image[] _diceSlot_evnt;     //주사위 슬롯(이벤트)

    [Header("# DiceRule Pannel")]
    [SerializeField]
    private HorizontalLayoutGroup _pannel_diceRule_preview;
    [SerializeField]
    private VerticalLayoutGroup[] _layout_diceRule;
    [SerializeField]
    private TextMeshProUGUI _txt_totalUp_checkMin;
    [SerializeField]
    private TextMeshProUGUI[] _txt_totalBetween_checkMinMax;
    [SerializeField]
    private TextMeshProUGUI _txt_eachUp_checkMin;
    [SerializeField]
    private TextMeshProUGUI[] _txt_eachBetween_checkMinMax;

    [SerializeField]
    private int _nowDice = 0;
    [SerializeField]
    private int _nowUseAp = 0;

    [SerializeField]
    private Sprite[] _spr_dice;
    [SerializeField]
    private Sprite[] _spr_diceSlot;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void IsUpSlot(bool upSlot)
    {
        if (upSlot)
            _rect.pivot = new Vector2(0, 1);
        else
            _rect.pivot = new Vector2(0, 0);
    }

    public void Set_Position(Vector2 vec)
    {
        transform.position = vec;
    }

    public void Set_AnchoredPosition(float x, float y)
    {
        var addX = x;
        var addY = y;

        _rect.anchoredPosition = new Vector2(_rect.anchoredPosition.x + addX, _rect.anchoredPosition.y + addY);
    }

    public void Change_RerollText(string txt)
        => _txt_reroll.text = txt;

    public void Change_StatDiceImage(int[] stat)
    {
        for (int i = 0; i < _img_statDice.Length; i++)
            _img_statDice[i].sprite = _spr_dice[stat[i]];
    }

    public void Change_DiceSlotSet(bool isBtl)
    {
        _diceSlotSet_btl.SetActive(isBtl);
        _diceSlotSet_evnt.SetActive(isBtl == false);
    }

    public void DiceRulePannel_Preview_OnOff(bool isOn, int index)
    {
        _pannel_diceRule_preview.gameObject.SetActive(isOn);

        if (isOn)
        {
            var rule = _evntSys.ActList[index].Rule;

            for (int i = 0; i < _layout_diceRule.Length; i++)
            {
                if (i + 1 == (int)rule)
                    _layout_diceRule[i].gameObject.SetActive(true);
                else
                    _layout_diceRule[i].gameObject.SetActive(false);
            }

            var evntAct = _evntSys.ActList[index];

            switch (rule)
            {
                case CheckRule.Total_Up:
                    _txt_totalUp_checkMin.text = evntAct.CheckMin.ToString();
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
        _pannel_diceRule_preview.enabled = false;
        _pannel_diceRule_preview.enabled = true;
        
    }

    public void NowDice_MouseOver(int order)    //주사위 슬롯에 마우스 오버
    {
        BtlActData btlAct = null;   //null이면 전투 행동이 아니라는 의미
        DungeonEventSystem.EvntAct evntAct = null;   //null이면 이벤트 행동이 아니라는 의미
        var realOrder = order + 1;
        Image[] diceSlot;

        if (_actController.SITUATION == ActionController.Situation.Battle)  //전투 행동인 경우, 행동 데이터 지정
        {
            btlAct = PlayerSys.ActList[_actController.NOW_CURSOR].Data;

            if (realOrder < btlAct.DiceMin)
                realOrder = btlAct.DiceMin;

            diceSlot = _diceSlot_btl;
        }
        else    //이벤트 행동인 경우, 
        {
            evntAct = _evntSys.ActList[_actController.NOW_CURSOR];

            diceSlot = _diceSlot_evnt;
        }

        //플레이어의 행동력이 마우스 오버한 슬롯만큼 있으면
        if (PlayerSys.AP >= realOrder)
        {
            for (int i = 0; i < diceSlot.Length; i++)
            {
                if (i < realOrder)
                {
                    if (i < _nowDice && _nowDice <= realOrder)
                        diceSlot[i].sprite = _spr_diceSlot[2]; //현재 선택한 주사위의 경우 선택한 스프라이트
                    else
                        diceSlot[i].sprite = _spr_diceSlot[1]; //이외에는 마우스 오버 스프라이트
                }
                else
                    diceSlot[i].sprite = _spr_diceSlot[0]; //이외에는 미선택 스프라이트
            }

            /*
            if (btlAct != null) //행동력을 소모하는 행동의 경우
            {
            */
                //소모 예정 행동력 표기
                PlayerSys.Change_Ap_UsePreview(realOrder);
            //}
        }
    }

    public void NowDice_MouseOver_End() //주사위 슬롯에서 마우스가 나간 경우
    {
        Image[] diceSlot;

        if (_actController.SITUATION == ActionController.Situation.Battle)
            diceSlot = _diceSlot_btl;
        else
            diceSlot = _diceSlot_evnt;

        for (int i = 0; i < diceSlot.Length; i++)
        {
            if (i < _nowDice)
                diceSlot[i].sprite = _spr_diceSlot[2];
            else
                diceSlot[i].sprite = _spr_diceSlot[0];
        }

        //if (_actController.SITUATION == ActionController.Situation.Battle)  //현재 선택된 주사위 수만큼 소모 예정 행동력 표시
            PlayerSys.Change_Ap_UsePreview(_nowUseAp);
    }

    public void NowDice_Change(int order)   //주사위 슬롯을 조작해 주사위 개수가 변했을 경우
    {
        BtlActData btlAct = null;   //null이면 전투 행동이 아니라는 의미
        DungeonEventSystem.EvntAct evntAct = null;   //null이면 이벤트 행동이 아니라는 의미
        var realOrder = order + 1;
        Image[] diceSlot;

        if (_actController.SITUATION == ActionController.Situation.Battle)  //전투 행동인 경우, 행동 데이터 지정
        {
            btlAct = PlayerSys.ActList[_actController.NOW_CURSOR].Data;

            if (realOrder < btlAct.DiceMin)
                realOrder = btlAct.DiceMin;

            if (btlAct.NoDice == false && PlayerSys.AP < realOrder) //선택한 주사위 개수를 위한 행동력이 없을 경우, return
                return;

            diceSlot = _diceSlot_btl;
        }
        else    //이벤트 행동인 경우, 행동 데이터 지정
        {
            evntAct = _evntSys.ActList[_actController.NOW_CURSOR];

            diceSlot = _diceSlot_evnt;
        }

        //return 되지 않았을 경우 함수 계속 진행

        if (_nowDice == realOrder)  //동일한 주사위 개수를 한번 더 선택한 경우
            NowDice_Reset();    //주사위 개수 초기화
        else
        {
            _nowDice = realOrder;   //주사위 개수 새로 지정

            if (btlAct != null && btlAct.NoDice)
                _nowUseAp = 0;
            else
                _nowUseAp = _nowDice;

            //주사위 개수에 따른 슬롯 설정
            for (int i = 0; i < diceSlot.Length; i++)
            {
                if (i < _nowDice)
                    diceSlot[i].sprite = _spr_diceSlot[2];
                else
                    diceSlot[i].sprite = _spr_diceSlot[0];
            }

            if (_actController.SITUATION == ActionController.Situation.Battle)
                DecisionBtn_OnOff(PlayerSys.AP >= _nowUseAp);   //현재 행동력이 충분한지에 따라, 행동 결정 버튼 OnOff
            else if (_actController.SITUATION == ActionController.Situation.Event)
                DecisionBtn_OnOff(true);
        }

        if (_actController.SITUATION == ActionController.Situation.Battle)
            PlayerSys.Change_Ap_UsePreview(_nowUseAp);  //현재 선택한 주사위 개수만큼 소모 예정 행동력 표시
    }

    public void NowDice_Reset()
    {
        //현재 선택한 주사위 개수, 소모 예정 행동력을 0으로
        DiceZero();
        Image[] diceSlot;

        if (_actController.SITUATION == ActionController.Situation.Battle)
            diceSlot = _diceSlot_btl;
        else
            diceSlot = _diceSlot_evnt;

        //현재 선택한 행동의 최대 주사위 개수만큼 슬롯을 활성화
        for (int i = 0; i < diceSlot.Length; i++)
        {
            if (i < PlayerSys.ActList[_actController.NOW_CURSOR].Data.DiceMax)
            {
                diceSlot[i].gameObject.SetActive(true);
                diceSlot[i].enabled = true;
                diceSlot[i].sprite = _spr_diceSlot[0];
            }
            else
                diceSlot[i].gameObject.SetActive(false);
        }

        //행동 결정 버튼 비활성화
        DecisionBtn_OnOff(_actController.SITUATION == ActionController.Situation.Event);

        //소모 예정 행동력 표기 초기화
        PlayerSys.Change_Ap_UsePreview(_nowUseAp);
    }

    public void DecisionBtn_OnOff(bool b)
    {
        _btn_decision.interactable = b;

        foreach (Image img in _img_btn_decision)
            img.color = new Color(1, 1, 1, b ? 1 : _offAlpha);
    }

    public void ActionDecision()    //행동 결정
    {
        _actController.ActionDecision(_nowDice);
        DiceZero();
    }

    public void DiceZero()
    {
        _nowDice = 0;
        _nowUseAp = 0;
    }
}
