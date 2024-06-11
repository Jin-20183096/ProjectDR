using UnityEngine;
using UnityEngine.UI;
using static PlayerSystem;

public class ActionController : MonoBehaviour
{
    [SerializeField]
    private BattleSystem _btlSys;

    public enum Situation { Off, No, Battle }

    [SerializeField]
    private Situation _situation;   //�ൿ �����ϴ� ��Ȳ
    public Situation SITUATION
    {
        get { return _situation; }
    }

    [Header("# Select Action")]
    [SerializeField]
    private BtlActData _nowBtlAct;   //������ ���� �ൿ

    [Header("# Action Dice Info")]
    [SerializeField]
    private ICreature.Stats _nowStat;   //������ �ൿ�� ����
    [SerializeField]
    private int _nowReroll;             //�籼�� Ƚ��
    [SerializeField]
    private int _nowDice;               //������ �ֻ��� ����
    [SerializeField]
    private int _stopDice;              //���� �ֻ��� ����
    [SerializeField]
    private int[] _nowResult = { -1, -1, -1, -1, -1 }; //�ֻ��� ���
    [SerializeField]
    private int _nowTotal;

    [Header("# Action List")]
    [SerializeField]
    private VerticalLayoutGroup _actList;   //�ൿ ����Ʈ

    [SerializeField]
    private int _cursor_nowAct = -1;    //���� ������ �ൿ�� �ε���
    public int NOW_CURSOR
    {
        get { return _cursor_nowAct; }
    }

    [SerializeField]
    private ActionListSlot[] _actSlot;      //�ൿ ����

    [SerializeField]
    private ActionListSlotSelected _actSlot_select; //������ �ൿ ����

    [SerializeField]
    private DiceSelectPannel _diceSelectPannel; //�ֻ��� ����â
    [SerializeField]
    private RectTransform _btn_noDiceAct;       //�ֻ��� ���� �ൿ ���� ��ư

    [Header("# Dice & Board")]
    [SerializeField]
    private GameObject _diceBoard;  //�ֻ��� ����
    [SerializeField]
    private DiceSetting[] _diceObj; //�ֻ��� ������Ʈ

    [Header("# DiceResult")]
    [SerializeField]
    private DiceResultPannel _resultPannel; //�ֻ��� ���â

    [Header("# Graphic Referernce")]
    [SerializeField]
    private Sprite[] _spr_actType;      //�ൿŸ�� ��������Ʈ
    [SerializeField]
    private Material[] _mat_diceSide;   //�ֻ��� �� material

    private string[] _statName_arr = { "", "��", "����", "������", "��ø", "�ǰ�", "����" };

    public void Set_ActListSituation(Situation situation)   //�ൿ��� ��Ȳ ����
    {
        if (situation != Situation.Off)
            _situation = situation;

        switch (situation)
        {
            case Situation.No:  //�ൿ ���� ��Ȳ ����
                if (_cursor_nowAct != -1)
                    ActionSelect_Cancle(_cursor_nowAct);    //���� ���̴� �ൿ ���

                _actList.gameObject.SetActive(false);       //�ൿ ��� Off

                //�ֻ��� & �ֻ��� ���� Off
                _diceBoard.SetActive(false);
                Dice_Off();

                _resultPannel.RerollButton_OnOff(false);        //�籼�� ��ư Off
                _resultPannel.ActStartButton_OnOff(false);      //�ൿ ���� ��ư Off
                break;
            case Situation.Off: //��Ȳ ������ �ൿ ��� ǥ�� Off
                _actList.gameObject.SetActive(false);       //�ൿ ��� Off
                break;
            case Situation.Battle:  //���� �ൿ ��� ǥ��
                _actList.gameObject.SetActive(true);        //�ൿ ��� On
                ActList_Synch_Battle(); //�ൿ ����Ʈ ����� ���� �ൿ ��Ҹ� �־ ����ȭ
                break;
        }

        DiceSelectPannel_OnOff(false);  //�ֻ��� ����â Off
        NoDiceButton_OnOff(false);      //�ֻ��� ���� �ൿ ���� ��ư Off

        if (_situation != Situation.Off)    
            DiceResultPannel_Off(); //�ֻ��� ���â Off

        RefreshLayout();
    }

    public void ActList_Synch_Battle()  //�ൿ ����Ʈ ����ȭ (����)
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

        //���� ���� ������ ���� ���, ���� ���
        if (_cursor_nowAct != -1)
            ActionSelect_Cancle(_cursor_nowAct);

        //���� ���� ���� �ʱ�ȭ
        _nowDice = 0;
        _nowResult = new int[5] { -1, -1, -1, -1, -1 };
        _nowTotal = 0;
    }

    public void ActionClick(int order)   //�ൿ ���� Ŭ��
    {
        for (int i = 0; i < _actList.gameObject.transform.childCount; i++)
        {
            if (order == i)  //Ŭ���� �ൿ ������ ���� ���
            {
                if (_cursor_nowAct == i)    //�ش� ������ �ൿ�� �̹� ������ ���
                    ActionSelect_Cancle(i); //�ൿ ���� ���
                else
                {
                    if (_cursor_nowAct != -1)   //�̹� �ٸ� �ൿ�� ������ ���
                        ActionSelect_Cancle(_cursor_nowAct);    //���� ���

                    ActionSelect(i);    //�� �ൿ�� ����
                }
                break;
            }
        }
    }

    public void ActionSelect(int order) //order��° �ൿ ����
    {
        _cursor_nowAct = order;
        _actSlot[_cursor_nowAct].gameObject.SetActive(false);

        //���� ���� ǥ��
        var selected = _actSlot_select.gameObject.transform;
        selected.gameObject.SetActive(true);    //���� ���� On

        if (_situation == Situation.Battle)
        {
            _nowBtlAct = PlayerSys.ActList[_cursor_nowAct].Data;
            _nowStat = PlayerSys.ActList[_cursor_nowAct].Stat;
            _actSlot_select.Change_SlotContent(_spr_actType[(int)_nowBtlAct.Type],
                                                _nowBtlAct.Name, _statName_arr[(int)_nowStat]);
        }

        selected.SetParent(_actList.gameObject.transform);  //���� ������ �з��� ����
        selected.SetSiblingIndex(_cursor_nowAct);           //���� ������ ���̾ƿ� �� ��ġ ����
        _actSlot_select.Set_SlotOrder(_cursor_nowAct);      //���� ������ order ����

        if (_nowStat != ICreature.Stats.No)
        {
            NoDiceButton_OnOff(false);      //�ֻ��� ���� �ൿ ���� ��ư Off
            DiceSelectPannel_OnOff(true);   //�ֻ��� ����â On
        }
        else
        {
            NoDiceButton_OnOff(true);       //�ֻ��� ���� �ൿ ���� ��ư On
            DiceSelectPannel_OnOff(false);  //�ֻ��� ����â Off
        }

        //���� Off

        RefreshLayout();
    }

    public void ActionSelect_Cancle(int order)  //order��° �ൿ ���� ���
    {
        DiceSelectPannel_OnOff(false);  //�ֻ��� ����â Off
        NoDiceButton_OnOff(false);      //�ֻ��� ���� �ൿ ���� ��ư Off

        var selected = _actSlot_select.gameObject.transform;
        selected.SetParent(transform);          //���� ������ �з��� ����
        selected.gameObject.SetActive(false);   //���� ���� Off

        _actSlot[order].gameObject.SetActive(true);
        _cursor_nowAct = -1;    //���� ���

        _nowBtlAct = null;
        _nowStat = ICreature.Stats.No;

        RefreshLayout();
    }

    public void DiceSelectPannel_OnOff(bool isOn)
    {
        _diceSelectPannel.gameObject.SetActive(isOn);

        if (isOn)
        {
            //�ֻ��� ����â ��ġ ����
            var upSlot = _cursor_nowAct < _actSlot.Length / 2;

            _diceSelectPannel.IsUpSlot(upSlot); //�ֻ��� ����â pivot ���� (������ �ൿ ����� ����or�Ĺ������� ����)
            _diceSelectPannel.Set_Position(_actSlot[_cursor_nowAct].transform.position);    //�ֻ��� ����â ��ġ ����
            _diceSelectPannel.Set_AnchoredPosition(_actSlot_select.Get_AddX(), _actSlot_select.Get_AddY(upSlot));

            //�ൿ ���� ����
            int[] statArr = null;

            switch (_nowStat)
            {
                case ICreature.Stats.STR:   //��
                    statArr = PlayerSys.STR;
                    break;
                case ICreature.Stats.INT:   //����
                    statArr = PlayerSys.INT;
                    break;
                case ICreature.Stats.DEX:   //������
                    statArr = PlayerSys.DEX;
                    break;
                case ICreature.Stats.AGI:   //��ø
                    statArr = PlayerSys.AGI;
                    break;
                case ICreature.Stats.CON:   //�ǰ�
                    statArr = PlayerSys.CON;
                    break;
                case ICreature.Stats.WIL:   //����
                    statArr = PlayerSys.WIL;
                    break;
            }

            //�ൿ ������ �籼�� ����
            _nowReroll = PlayerSys.GetReroll(_nowStat);

            _diceSelectPannel.Change_RerollText(_nowReroll.ToString());
            _diceSelectPannel.Change_StatDiceImage(statArr);
            _diceSelectPannel.NowDice_Reset();
        }
        else
            _diceSelectPannel.DiceZero();

        PlayerSys.Change_Ap_UsePreview(_nowDice);   //�ൿ�� �Ҹ� �̸����� Off
    }

    public void NoDiceButton_OnOff(bool isOn)
    {
        _btn_noDiceAct.gameObject.SetActive(isOn);

        if (isOn)
        {
            _btn_noDiceAct.transform.position = _actSlot[_cursor_nowAct].transform.position;    //��ư ��ġ ����
            _btn_noDiceAct.anchoredPosition = new Vector2(_btn_noDiceAct.anchoredPosition.x + _actSlot_select.Get_AddX() + 8,
                                                            _btn_noDiceAct.anchoredPosition.y);
        }
    }

    public void ActionDecision(int nowDice)
    {
        _nowDice = nowDice; //�ֻ��� ���� ����

        Set_ActListSituation(Situation.Off);

        switch (_situation)
        {
            case Situation.Battle: //���� ��Ȳ
                if (_nowBtlAct.NoDice)  //�ֻ����� ���� �ൿ�� ��
                {
                    // ��� �ൿ ����
                }
                else    //�ֻ����� �ִ� �ൿ�̸�
                {
                    Set_DiceSide(_nowStat); //������ �ൿ�� �°� �ֻ��� ������Ʈ ����
                    DiceRoll(); //�ֻ��� ������
                }
                break;
        }
    }

    public void Set_DiceSide(ICreature.Stats stat)  //�ֻ��� ������Ʈ�� �� ���� ���ȿ� �°� ����
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
            if (temp_stat[i] >= 10)     //10 �̻��� ������ 10���� ���
                mat[i] = _mat_diceSide[10];
            else if (temp_stat[i] <= 0) //0 ������ ������ 0���� ���
                mat[i] = _mat_diceSide[0];
            else
                mat[i] = _mat_diceSide[temp_stat[i]];
        }

        foreach (DiceSetting d in _diceObj)
            d.Change_DiceSide(mat);
    }

    public void DiceRoll()  //�ֻ��� ����
    {
        _resultPannel.ActionInfoPannel_OnOff(true);     //�ൿ ����â On
        if (_situation == Situation.Battle)
            _resultPannel.Change_ActInfo(_nowBtlAct.Type, _nowBtlAct.Name); //�ൿ Ÿ�� ������, �ൿ�� ����

        _resultPannel.DiceResultPannel_OnOff(true);     //�ֻ��� ���â On
        _resultPannel.Set_NewDiceTotal(_nowDice);       //�ֻ��� ���â �ʱ�ȭ

        _diceBoard.SetActive(true); //�ֻ��� ���� On

        _resultPannel.RerollButton_OnOff(true);         //�籼�� ��ư On
        _resultPannel.SetAble_RerollButton(false);      //�籼�� ��ư ��ȣ�ۿ� Off

        _resultPannel.ActStartButton_OnOff(true);       //�ൿ ���� ��ư On
        _resultPannel.SetAble_ActStartButton(false);    //�ൿ ���� ��ư ��ȣ�ۿ� Off

        Set_DiceObj();  //�ֻ��� ������Ʈ ��ġ
    }

    public void DiceReroll()    //�ֻ��� �籼��
    {
        _resultPannel.Set_NewDiceTotal(_nowDice);   //�ֻ��� ����â �ʱ�ȭ

        if (_nowReroll > 0) //�籼�� Ƚ���� �����ִٸ�
        {
            _nowReroll--;

            _resultPannel.SetAble_RerollButton(false);      //�籼�� ��ư ��ȣ�ۿ� �Ұ� ó��
            _resultPannel.SetAble_ActStartButton(false);    //�ൿ ���� ��ư ��ȣ�ۿ� �Ұ� ó��

            Set_DiceObj();  //�ֻ��� ������Ʈ ��ġ
        }
    }

    public void Set_DiceObj()   //�ֻ��� ������Ʈ ����
    {
        _resultPannel.Change_RerollText(_nowReroll.ToString()); //�籼�� Ƚ�� ����
        _resultPannel.Change_DiceTotal(""); //�ֻ��� ���� ��Ȯ��

        _stopDice = 0;  //���� �ֻ��� 0���� �ʱ�ȭ

        for (int i = 0; i < _diceObj.Length; i++)
        {
            if (i < _nowDice)   //���� �ֻ������
            {
                //�ֻ��� ������Ʈ Ȱ��ȭ
                _diceObj[i].gameObject.SetActive(true);
                _diceObj[i].DiceObject_OnOff(true);
                _diceObj[i].Set_DiceTransform(_diceBoard.transform.position);
            }
            else    //������ �ʴ� �ֻ������
            {
                //�ֻ��� ������Ʈ ��Ȱ��ȭ
                if (_diceObj[i].gameObject.activeSelf)
                    _diceObj[i].DiceObject_OnOff(false);

                _diceObj[i].gameObject.SetActive(false);
            }
        }
    }

    public void DiceStop(int order, int result) //������ �ֻ��� �ϳ��� ���� ���
    {
        var value = -1;

        switch (_nowStat)   //�ֻ��� �� üũ
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
            _nowResult[order] = 10; //10 �̻��� ������ 10���� ���
        else if (value <= 0)
            _nowResult[order] = 0;  //0 ������ ������ 10���� ���
        else
            _nowResult[order] = value;

        _resultPannel.Change_DiceResult(order, _nowResult[order]);
    }

    public void Add_StopDice()
        => _stopDice++; //���� �ֻ��� �ϳ� �߰�

    public void Check_DiceTotal()   //���� �ֻ����� ���� ������� üũ, ���� ������ �� ������ ���
    {
        if (_stopDice == _nowDice)  //��� �ֻ����� ����ٸ�
        {
            _nowTotal = 0;

            for (int i = 0; i < _nowDice; i++)
                _nowTotal += _nowResult[i];

            if (_situation == Situation.Battle) //���� ���� ���
                _resultPannel.SetAble_ActStartButton(true); //�ൿ ���� ��ư ��ȣ�ۿ� On

            if (_nowReroll > 0)
                _resultPannel.SetAble_RerollButton(true);   //���� �籼�� Ƚ�� > 0�̸�, �籼�� ��ư ��ȣ�ۿ� ����
        }
        else
            return;

        _resultPannel.Change_DiceTotal(_nowTotal.ToString());
    }

    public void ActionStart()
    {
        Dice_Off(); //�ֻ��� ������Ʈ Off
        _diceBoard.SetActive(false);    //�ֻ��� ���� Off

        _resultPannel.RerollButton_OnOff(false);    //�籼�� ��ư Off
        _resultPannel.ActStartButton_OnOff(false);  //�ൿ ���� ��ư Off

        if (_situation == Situation.Battle) //���� �ൿ�� ������ ���
        {
            //���� �ý��ۿ� �ൿ ���� ����
            _btlSys.Set_BtlAct_Player(_nowBtlAct, _nowResult);
        }

        //���� ���� ���� �ʱ�ȭ
        _nowDice = 0;
        _nowResult = new int[5] { -1, -1, -1, -1, -1 };
    }

    void RefreshLayout()  //�ൿ����Ʈ ���ΰ�ħ
    {
        Canvas.ForceUpdateCanvases();
        _actList.enabled = false;
        _actList.enabled = true;
    }

    public void Dice_Off()  //�ֻ��� ������Ʈ Off
    {
        for (int i = 0; i < _diceObj.Length; i++)
        {
            if (_diceObj[i].gameObject.activeSelf)
                _diceObj[i].gameObject.SetActive(false);
        }
    }

    public void DiceResultPannel_Off()  //�ֻ��� ���â Off
    {
        _resultPannel.ActionInfoPannel_OnOff(false);    //�ൿ���â Off
        _resultPannel.DiceResultPannel_OnOff(false);    //�ֻ��� ���â Off
        _resultPannel.RerollButton_OnOff(false);        //�籼�� ��ư Off
        _resultPannel.ActStartButton_OnOff(false);      //�ൿ ���� ��ư Off
    }
}
