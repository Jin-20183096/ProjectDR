using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static PlayerSystem;

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
    private Image[] _diceSlot;      //�ֻ��� ����
    [SerializeField]
    private Button _btn_decision;       //�ൿ ���� ��ư
    [SerializeField]
    private Image[] _img_btn_decision;  //�ൿ ���� ��ư �̹���

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

    public void NowDice_MouseOver(int order)    //�ֻ��� ���Կ� ���콺 ����
    {
        BtlActData btlAct = null;   //null�̸� ���� �ൿ�� �ƴ϶�� �ǹ�
        DungeonEventSystem.EvntAct evntAct = null;   //null�̸� �̺�Ʈ �ൿ�� �ƴ϶�� �ǹ�
        var realOrder = order + 1;

        if (_actController.SITUATION == ActionController.Situation.Battle)  //���� �ൿ�� ���, �ൿ ������ ����
        {
            btlAct = PlayerSys.ActList[_actController.NOW_CURSOR].Data;

            if (realOrder < btlAct.DiceMin)
                realOrder = btlAct.DiceMin;

            //�÷��̾��� �ൿ���� ���콺 ������ ���Ը�ŭ ������

            if (PlayerSys.AP >= realOrder)
            {
                for (int i = 0; i < _diceSlot.Length; i++)
                {
                    if (i < realOrder)
                    {
                        if (i < _nowDice && _nowDice <= realOrder)
                            _diceSlot[i].sprite = _spr_diceSlot[2]; //���� ������ �ֻ����� ��� ������ ��������Ʈ
                        else
                            _diceSlot[i].sprite = _spr_diceSlot[1]; //�̿ܿ��� ���콺 ���� ��������Ʈ
                    }
                    else
                        _diceSlot[i].sprite = _spr_diceSlot[0]; //�̿ܿ��� �̼��� ��������Ʈ
                }

                if (btlAct != null) //�ൿ���� �Ҹ��ϴ� �ൿ�� ���
                {
                    //�Ҹ� ���� �ൿ�� ǥ��
                    PlayerSys.Change_Ap_UsePreview(realOrder);
                }
            }
        }
        else if (_actController.SITUATION == ActionController.Situation.Event)
        {
            evntAct = _evntSys.ActList[_actController.NOW_CURSOR];

            if (realOrder < evntAct.Dice)
                realOrder = evntAct.Dice;

            for (int i = 0; i < _diceSlot.Length; i++)
            {
                if (i < realOrder)
                {
                    if (i < _nowDice && _nowDice <= realOrder)
                        _diceSlot[i].sprite = _spr_diceSlot[2]; //���� ������ �ֻ����� ��� ������ ��������Ʈ
                    else
                        _diceSlot[i].sprite = _spr_diceSlot[1]; //�̿ܿ��� ���콺 ���� ��������Ʈ
                }
                else
                    _diceSlot[i].sprite = _spr_diceSlot[0]; //�̿ܿ��� �̼��� ��������Ʈ
            }
        }
    }

    public void NowDice_MouseOver_End() //�ֻ��� ���Կ��� ���콺�� ���� ���
    {
        for (int i = 0; i < _diceSlot.Length; i++)
        {
            if (i < _nowDice)
                _diceSlot[i].sprite = _spr_diceSlot[2];
            else
                _diceSlot[i].sprite = _spr_diceSlot[0];
        }

        if (_actController.SITUATION == ActionController.Situation.Battle)  //���� ���õ� �ֻ��� ����ŭ �Ҹ� ���� �ൿ�� ǥ��
            PlayerSys.Change_Ap_UsePreview(_nowUseAp);
    }

    public void NowDice_Change(int order)   //�ֻ��� ������ ������ �ֻ��� ������ ������ ���
    {
        BtlActData btlAct = null;   //null�̸� ���� �ൿ�� �ƴ϶�� �ǹ�
        DungeonEventSystem.EvntAct evntAct = null;   //null�̸� �̺�Ʈ �ൿ�� �ƴ϶�� �ǹ�
        var realOrder = order + 1;

        if (_actController.SITUATION == ActionController.Situation.Battle)  //���� �ൿ�� ���, �ൿ ������ ����
        {
            btlAct = PlayerSys.ActList[_actController.NOW_CURSOR].Data;

            if (realOrder < btlAct.DiceMin)
                realOrder = btlAct.DiceMin;

            if (btlAct.NoDice == false && PlayerSys.AP < realOrder) //������ �ֻ��� ������ ���� �ൿ���� ���� ���, return
                return;
        }
        else if (_actController.SITUATION == ActionController.Situation.Event)  //�̺�Ʈ �ൿ�� ���, �ൿ ������ ����
        {
            evntAct = _evntSys.ActList[_actController.NOW_CURSOR];

            if (realOrder < evntAct.Dice)
                realOrder = evntAct.Dice;
        }

        //return ���� �ʾ��� ��� �Լ� ��� ����

        if (_nowDice == realOrder)  //������ �ֻ��� ������ �ѹ� �� ������ ���
        {
            NowDice_Reset();    //�ֻ��� ���� �ʱ�ȭ
        }
        else
        {
            _nowDice = realOrder;   //�ֻ��� ���� ���� ����

            if (btlAct != null && btlAct.NoDice)
                _nowUseAp = 0;
            else
                _nowUseAp = _nowDice;

            //�ֻ��� ������ ���� ���� ����
            for (int i = 0; i < _diceSlot.Length; i++)
            {
                if (i < _nowDice)
                    _diceSlot[i].sprite = _spr_diceSlot[2];
                else
                    _diceSlot[i].sprite = _spr_diceSlot[0];
            }

            if (_actController.SITUATION == ActionController.Situation.Battle)
                DecisionBtn_OnOff(PlayerSys.AP >= _nowUseAp);   //���� �ൿ���� ��������� ����, �ൿ ���� ��ư OnOff
            else if (_actController.SITUATION == ActionController.Situation.Event)
                DecisionBtn_OnOff(_nowDice != 0);
        }

        if (_actController.SITUATION == ActionController.Situation.Battle)
            PlayerSys.Change_Ap_UsePreview(_nowUseAp);  //���� ������ �ֻ��� ������ŭ �Ҹ� ���� �ൿ�� ǥ��
    }

    public void NowDice_Reset()
    {
        //���� ������ �ֻ��� ����, �Ҹ� ���� �ൿ���� 0����
        DiceZero();

        //���� ������ �ൿ�� �ִ� �ֻ��� ������ŭ ������ Ȱ��ȭ
        for (int i = 0; i < _diceSlot.Length; i++)
        {
            if (_actController.SITUATION == ActionController.Situation.Battle)
            {
                if (i < PlayerSys.ActList[_actController.NOW_CURSOR].Data.DiceMax)
                {
                    _diceSlot[i].gameObject.SetActive(true);
                    _diceSlot[i].enabled = true;
                    _diceSlot[i].sprite = _spr_diceSlot[0];
                }
                else
                    _diceSlot[i].gameObject.SetActive(false);
            }
            else if (_actController.SITUATION == ActionController.Situation.Event)
            {
                if (i < _evntSys.ActList[_actController.NOW_CURSOR].Dice)
                {
                    _diceSlot[i].gameObject.SetActive(true);
                    _diceSlot[i].enabled = true;
                    _diceSlot[i].sprite = _spr_diceSlot[0];
                }
                else
                    _diceSlot[i].gameObject.SetActive(false);
            }
        }

        //�ൿ ���� ��ư ��Ȱ��ȭ
        DecisionBtn_OnOff(false);

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
