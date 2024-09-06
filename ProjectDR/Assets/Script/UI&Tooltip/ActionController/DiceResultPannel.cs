using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiceResultPannel : MonoBehaviour
{
    private float offAlpha = 0.2f;

    [SerializeField]
    private GameObject _pannel_actInfo;  //행동 정보창
    [SerializeField]
    private HorizontalLayoutGroup _layout_actInfo;   //행동 정보창 레이아웃

    [Header("# Action Info")]
    [SerializeField]
    private Image _icon_actType;    //행동 타입 아이콘
    [SerializeField]
    private TextMeshProUGUI _txt_actName;   //행동명 텍스트
    [SerializeField]
    private TextMeshProUGUI _txt_diceTotal; //주사위 총합 텍스트

    [Header("# Dice Result")]
    [SerializeField]
    private GameObject _pannel_diceResult;  //주사위 결과창
    [SerializeField]
    private Image[] _img_diceSide;          //각 주사위의 결과 이미지

    [Header("# Reroll Button")]
    [SerializeField]
    private Button _btn_reroll;             //재굴림 버튼
    [SerializeField]
    private Image[] _img_btnReroll;         //재굴림 버튼의 이미지들
    [SerializeField]
    private TextMeshProUGUI _txt_reroll;    //재굴림 횟수 텍스트

    [Header("# ActStart Button")]
    [SerializeField]
    private Button _btn_actStart;           //행동 개시 버튼
    [SerializeField]
    private Image[] _img_btnActStart;       //행동 개시 버튼의 이미지들

    [Header("# Sprite Reference")]
    [SerializeField]
    private Sprite[] _spr_actType;  //행동 타입 스프라이트
    [SerializeField]
    private Sprite[] _spr_dice;     //주사위 면 스프라이트

    public void ActionInfoPannel_OnOff(bool b)  //행동 정보창 OnOff
        => _pannel_actInfo.SetActive(b);

    public void Change_ActInfo(BtlActData.ActType type, string actName)  //행동 정보창 정보 변경
    {
        _icon_actType.sprite = _spr_actType[(int)type];
        _txt_actName.text = actName;
        _txt_diceTotal.text = "";
        RefreshLayout();
    }

    public void Set_NewDiceTotal(int nowDice)   //주사위 결과창 초기화
    {
        for (int i = 0; i < _img_diceSide.Length; i++)
        {
            _img_diceSide[i].enabled = i < nowDice; //굴리지 않는 주사위의 이미지 Off

            if (i < nowDice)
            {
                //주사위의 이미지 미확정 상태로 표시
                _img_diceSide[i].sprite = _spr_dice[0];
                _img_diceSide[i].color = new Color(1, 1, 1, 0.3f);
            }
        }
        RefreshLayout();
    }

    public void DiceResultPannel_OnOff(bool b)  //주사위 결과창 OnOff
        => _pannel_diceResult.SetActive(b);

    public void Set_MomentDiceResult(int order, int value)  //주사위 결과창 결과 변경(순간적인 주사위의 결과)
    {
        _img_diceSide[order].color = new Color(1, 1, 1, 0.3f);
        _img_diceSide[order].sprite = _spr_dice[value];
    }

    public void Set_StopDiceResult(int order, int value)    //주사위 결과창 결과 변경(멈춘 주사위의 결과)
    {
        _img_diceSide[order].color = new Color(1, 1, 1, 1);
        _img_diceSide[order].sprite = _spr_dice[value];
    }

    public void Change_DiceTotal(string total)  //주사위 총합 텍스트 변경
    {
        _txt_diceTotal.text = total;
        RefreshLayout();
    }

    public void RefreshLayout()
    {
        Canvas.ForceUpdateCanvases();
        _layout_actInfo.enabled = false;
        _layout_actInfo.enabled = true;
    }

    public void RerollButton_OnOff(bool b)  //재굴림 버튼 OnOff
    {
        if (_btn_reroll != null)
            _btn_reroll.gameObject.SetActive(b);
    }

    public void Change_RerollText(string count) //재굴림 횟수 텍스트 변경
        => _txt_reroll.text = count;

    public void SetAble_RerollButton(bool b)  //재굴림 버튼 상호작용 여부 OnOff
    {
        if (_btn_reroll != null)
        {
            _btn_reroll.interactable = b;

            foreach (Image img in _img_btnReroll)
                img.color = new Color(1, 1, 1, b ? 1f : offAlpha);

            _txt_reroll.color = new Color(1, 1, 1, b ? 1f : offAlpha);
        }
    }

    public void ActStartButton_OnOff(bool b)    //행동 개시 버튼 OnOff
    {
        if (_btn_actStart != null)
            _btn_actStart.gameObject.SetActive(b);
    }

    public void SetAble_ActStartButton(bool b)  //행동 개시 버튼 상호작용 여부 OnOff
    {
        if (_btn_actStart != null)
        {
            _btn_actStart.interactable = b;

            foreach (Image img in _img_btnActStart)
                img.color = new Color(1, 1, 1, b ? 1f : offAlpha);
        }
    }
}
