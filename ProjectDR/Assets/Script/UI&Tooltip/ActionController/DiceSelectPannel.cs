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
    private Image[] _img_statDice;  //�ൿ ���� �ֻ��� �̹��� ǥ��
    [SerializeField]
    private Button _btn_decision;       //�ൿ ���� ��ư
    [SerializeField]
    private Image[] _img_btn_decision;  //�ൿ ���� ��ư �̹���

    [Header("# DiceSlot")]
    [SerializeField]
    private GameObject _diceSlotSet_btl;    //�ֻ��� ���� ����(����)
    [SerializeField]
    private Image[] _diceSlot_btl;      //�ֻ��� ����(����)
    [SerializeField]
    private GameObject _diceSlotSet_evnt;   //�ֻ��� ���� ����(�̺�Ʈ)
    [SerializeField]
    private Image[] _diceSlot_evnt;     //�ֻ��� ����(�̺�Ʈ)

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

    public void NowDice_MouseOver(int order)    //�ֻ��� ���Կ� ���콺 ����
    {
        BtlActData btlAct = null;   //null�̸� ���� �ൿ�� �ƴ϶�� �ǹ�
        DungeonEventSystem.EvntAct evntAct = null;   //null�̸� �̺�Ʈ �ൿ�� �ƴ϶�� �ǹ�
        var realOrder = order + 1;
        Image[] diceSlot;

        if (_actController.SITUATION == ActionController.Situation.Battle)  //���� �ൿ�� ���, �ൿ ������ ����
        {
            btlAct = PlayerSys.ActList[_actController.NOW_CURSOR].Data;

            if (realOrder < btlAct.DiceMin)
                realOrder = btlAct.DiceMin;

            diceSlot = _diceSlot_btl;
        }
        else    //�̺�Ʈ �ൿ�� ���, 
        {
            evntAct = _evntSys.ActList[_actController.NOW_CURSOR];

            diceSlot = _diceSlot_evnt;
        }

        //�÷��̾��� �ൿ���� ���콺 ������ ���Ը�ŭ ������
        if (PlayerSys.AP >= realOrder)
        {
            for (int i = 0; i < diceSlot.Length; i++)
            {
                if (i < realOrder)
                {
                    if (i < _nowDice && _nowDice <= realOrder)
                        diceSlot[i].sprite = _spr_diceSlot[2]; //���� ������ �ֻ����� ��� ������ ��������Ʈ
                    else
                        diceSlot[i].sprite = _spr_diceSlot[1]; //�̿ܿ��� ���콺 ���� ��������Ʈ
                }
                else
                    diceSlot[i].sprite = _spr_diceSlot[0]; //�̿ܿ��� �̼��� ��������Ʈ
            }

            /*
            if (btlAct != null) //�ൿ���� �Ҹ��ϴ� �ൿ�� ���
            {
            */
                //�Ҹ� ���� �ൿ�� ǥ��
                PlayerSys.Change_Ap_UsePreview(realOrder);
            //}
        }
    }

    public void NowDice_MouseOver_End() //�ֻ��� ���Կ��� ���콺�� ���� ���
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

        //if (_actController.SITUATION == ActionController.Situation.Battle)  //���� ���õ� �ֻ��� ����ŭ �Ҹ� ���� �ൿ�� ǥ��
            PlayerSys.Change_Ap_UsePreview(_nowUseAp);
    }

    public void NowDice_Change(int order)   //�ֻ��� ������ ������ �ֻ��� ������ ������ ���
    {
        BtlActData btlAct = null;   //null�̸� ���� �ൿ�� �ƴ϶�� �ǹ�
        DungeonEventSystem.EvntAct evntAct = null;   //null�̸� �̺�Ʈ �ൿ�� �ƴ϶�� �ǹ�
        var realOrder = order + 1;
        Image[] diceSlot;

        if (_actController.SITUATION == ActionController.Situation.Battle)  //���� �ൿ�� ���, �ൿ ������ ����
        {
            btlAct = PlayerSys.ActList[_actController.NOW_CURSOR].Data;

            if (realOrder < btlAct.DiceMin)
                realOrder = btlAct.DiceMin;

            if (btlAct.NoDice == false && PlayerSys.AP < realOrder) //������ �ֻ��� ������ ���� �ൿ���� ���� ���, return
                return;

            diceSlot = _diceSlot_btl;
        }
        else    //�̺�Ʈ �ൿ�� ���, �ൿ ������ ����
        {
            evntAct = _evntSys.ActList[_actController.NOW_CURSOR];

            diceSlot = _diceSlot_evnt;
        }

        //return ���� �ʾ��� ��� �Լ� ��� ����

        if (_nowDice == realOrder)  //������ �ֻ��� ������ �ѹ� �� ������ ���
            NowDice_Reset();    //�ֻ��� ���� �ʱ�ȭ
        else
        {
            _nowDice = realOrder;   //�ֻ��� ���� ���� ����

            if (btlAct != null && btlAct.NoDice)
                _nowUseAp = 0;
            else
                _nowUseAp = _nowDice;

            //�ֻ��� ������ ���� ���� ����
            for (int i = 0; i < diceSlot.Length; i++)
            {
                if (i < _nowDice)
                    diceSlot[i].sprite = _spr_diceSlot[2];
                else
                    diceSlot[i].sprite = _spr_diceSlot[0];
            }

            if (_actController.SITUATION == ActionController.Situation.Battle)
                DecisionBtn_OnOff(PlayerSys.AP >= _nowUseAp);   //���� �ൿ���� ��������� ����, �ൿ ���� ��ư OnOff
            else if (_actController.SITUATION == ActionController.Situation.Event)
                DecisionBtn_OnOff(true);
        }

        if (_actController.SITUATION == ActionController.Situation.Battle)
            PlayerSys.Change_Ap_UsePreview(_nowUseAp);  //���� ������ �ֻ��� ������ŭ �Ҹ� ���� �ൿ�� ǥ��
    }

    public void NowDice_Reset()
    {
        //���� ������ �ֻ��� ����, �Ҹ� ���� �ൿ���� 0����
        DiceZero();
        Image[] diceSlot;

        if (_actController.SITUATION == ActionController.Situation.Battle)
            diceSlot = _diceSlot_btl;
        else
            diceSlot = _diceSlot_evnt;

        //���� ������ �ൿ�� �ִ� �ֻ��� ������ŭ ������ Ȱ��ȭ
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

        //�ൿ ���� ��ư ��Ȱ��ȭ
        DecisionBtn_OnOff(_actController.SITUATION == ActionController.Situation.Event);

        //�Ҹ� ���� �ൿ�� ǥ�� �ʱ�ȭ
        PlayerSys.Change_Ap_UsePreview(_nowUseAp);
    }

    public void DecisionBtn_OnOff(bool b)
    {
        _btn_decision.interactable = b;

        foreach (Image img in _img_btn_decision)
            img.color = new Color(1, 1, 1, b ? 1 : _offAlpha);
    }

    public void ActionDecision()    //�ൿ ����
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
