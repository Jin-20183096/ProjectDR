using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static PlayerSystem;
using static EventData;
using Panda;

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
    [SerializeField]
    private GameObject _plusIcon_evnt;      //�ֻ��� ���� ����(�̺�Ʈ)�� �÷��� ������

    [Header("# DiceRule Pannel")]
    //�ֻ��� ���� â
    [SerializeField]
    private VerticalLayoutGroup _pannel_diceRule_preview;
    //�ֻ��� ���� �ؽ�Ʈ
    //_pannel_diceRule�� 0��° ���ϵ�� "�ֻ��� ����" (total_~)
    //_pannel_diceRule�� 1��° ���ϵ�� "��� �ֻ���" (each_~)
    [SerializeField]
    private TextMeshProUGUI _txt_checkValue1;
    [SerializeField]
    private TextMeshProUGUI _txt_checkValue2;
    [SerializeField]
    private TextMeshProUGUI _txt_ruleText_A;
    [SerializeField]
    private TextMeshProUGUI _txt_ruleText_B;

    [SerializeField]
    private int _nowDice = 0;

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
            var evntAct = _evntSys.ActList[index];

            if (rule != CheckRule.No)
            {
                //�ֻ��� ���� ~ ������ �� Ȱ��ȭ
                _pannel_diceRule_preview.transform.GetChild(0).gameObject.SetActive(rule < CheckRule.Each_Up);
                //��� �ֻ��� ~ ������ �� Ȱ��ȭ
                _pannel_diceRule_preview.transform.GetChild(1).gameObject.SetActive(rule >= CheckRule.Each_Up);

                if (rule == CheckRule.Total_Odd || rule == CheckRule.Total_Even ||
                    rule == CheckRule.Each_Odd || rule == CheckRule.Each_Even)
                {
                    _txt_checkValue1.gameObject.SetActive(false);

                    _txt_ruleText_A.gameObject.SetActive(false);

                    if (rule == CheckRule.Total_Odd || rule == CheckRule.Each_Odd)
                        _txt_ruleText_B.text = "Ȧ���� �� ����";
                    else
                        _txt_ruleText_B.text = "¦���� �� ����";
                }
                else
                {
                    _txt_checkValue1.gameObject.SetActive(true);
                    _txt_checkValue1.text = evntAct.CheckValue1.ToString();

                    if (rule == CheckRule.Total_Between ||
                    rule == CheckRule.Each_Between)
                    {
                        if (evntAct.CheckValue1 == evntAct.CheckValue2)
                        {
                            _txt_ruleText_A.gameObject.SetActive(false);

                            _txt_ruleText_B.text = "�� �� ����";
                        }
                        else
                        {
                            _txt_ruleText_A.gameObject.SetActive(true);
                            _txt_ruleText_A.text = "�̻�";

                            _txt_checkValue2.gameObject.SetActive(true);
                            _txt_checkValue2.text = evntAct.CheckValue2.ToString();

                            _txt_ruleText_B.text = "������ �� ����";
                        }
                    }
                    else
                    {
                        _txt_ruleText_A.gameObject.SetActive(false);
                        _txt_checkValue2.gameObject.SetActive(false);

                        if (rule == CheckRule.Total_Up || rule == CheckRule.Each_Up)
                            _txt_ruleText_B.text = "�̻��� �� ����";
                        else if (rule == CheckRule.Total_Down || rule == CheckRule.Each_Down)
                            _txt_ruleText_B.text = "������ �� ����";
                    }
                }
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

            PlayerSys.Change_Ap_UsePreview(realOrder);
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

        PlayerSys.Change_Ap_UsePreview(_nowDice);
    }

    public void NowDice_Change(int order)   //�ֻ��� ������ ������ �ֻ��� ������ ������ ���
    {
        BtlActData btlAct = null;   //null�̸� ���� �ൿ�� �ƴ϶�� �ǹ�
        DungeonEventSystem.EvntAct evntAct = null;   //null�̸� �̺�Ʈ �ൿ�� �ƴ϶�� �ǹ�
        var realOrder = order + 1;  //0���� �����ϴ� �ε����� �ڿ����� �����ϱ� ���� +1��
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

            if (PlayerSys.AP < realOrder)   //������ �ֻ��� ������ ���� �ൿ���� ���� ���, return
                return;

            diceSlot = _diceSlot_evnt;
        }

        //return ���� �ʾ��� ��� �Լ� ��� ����

        if (_nowDice == realOrder)  //������ �ֻ��� ������ �ѹ� �� ������ ���
            NowDice_Reset();    //�ֻ��� ���� �ʱ�ȭ
        else
        {
            _nowDice = realOrder;   //�ֻ��� ���� ���� ����

            //�ֻ��� ������ ���� ���� ����
            for (int i = 0; i < diceSlot.Length; i++)
            {
                if (i < _nowDice)
                    diceSlot[i].sprite = _spr_diceSlot[2];
                else
                    diceSlot[i].sprite = _spr_diceSlot[0];
            }

            if (_actController.SITUATION == ActionController.Situation.Battle)
                DecisionBtn_OnOff(PlayerSys.AP >= _nowDice);   //���� �ൿ���� ��������� ����, �ൿ ���� ��ư OnOff
            else if (_actController.SITUATION == ActionController.Situation.Event)
                DecisionBtn_OnOff(true);
        }

        if (_actController.SITUATION == ActionController.Situation.Battle)
            PlayerSys.Change_Ap_UsePreview(_nowDice);  //���� ������ �ֻ��� ������ŭ �Ҹ� ���� �ൿ�� ǥ��
    }

    public void NowDice_Reset()
    {
        //���� ������ �ֻ��� ����, �Ҹ� ���� �ൿ���� 0����
        DiceZero();
        Image[] diceSlot;

        BtlActData btlAct = null;
        EventAction evntAct = null;

        if (_actController.SITUATION == ActionController.Situation.Battle)
        {
            diceSlot = _diceSlot_btl;
            btlAct = PlayerSys.ActList[_actController.NOW_CURSOR].Data;
        }
        else
        {
            diceSlot = _diceSlot_evnt;
            evntAct = _evntSys.ActList[_actController.NOW_CURSOR].Data;

            //��� �ֻ��� �̺�Ʈ�� �ֻ��� �߰��� �� �� �����Ƿ�, + ������ OFF
            _plusIcon_evnt.SetActive(evntAct.CheckStat != ICreature.Stats.LUC);
        }

        //���� ������ �ൿ�� �ִ� �ֻ��� ������ŭ ������ Ȱ��ȭ
        for (int i = 0; i < diceSlot.Length; i++)
        {
            if (_actController.SITUATION == ActionController.Situation.Battle)
            {
                if (i < btlAct.DiceMax)
                {
                    diceSlot[i].gameObject.SetActive(true);
                    diceSlot[i].enabled = true;
                    diceSlot[i].sprite = _spr_diceSlot[0];
                }
                else
                    diceSlot[i].gameObject.SetActive(false);
            }
            else if (_actController.SITUATION == ActionController.Situation.Event)
            {
                if (evntAct.CheckStat == ICreature.Stats.LUC)  //��� �ֻ��� �̺�Ʈ �� ��� �ֻ��� ���԰� '+'�������� off
                    diceSlot[i].gameObject.SetActive(false);
                else if (evntAct.CheckStat != ICreature.Stats.No)  //������ ���� �̺�Ʈ�� �ƴ϶��
                {
                    diceSlot[i].gameObject.SetActive(true);
                    diceSlot[i].enabled = true;
                    diceSlot[i].sprite = _spr_diceSlot[0];
                }
            }
        }

        //�ൿ ���� ��ư ��Ȱ��ȭ
        DecisionBtn_OnOff(_actController.SITUATION == ActionController.Situation.Event);

        //�Ҹ� ���� �ൿ�� ǥ�� �ʱ�ȭ
        PlayerSys.Change_Ap_UsePreview(_nowDice);
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
    }
}
