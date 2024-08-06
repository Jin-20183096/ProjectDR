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

    // ���ؿ� ���� ������� �ֻ��� 1���� ��:  1~4 / 3~7 / 5~10
    private int[] AvgDice_first = new int[] { 1, 4 };
    private int[] AvgDice_Mid = new int[] { 3, 7 };
    private int[] AvgDice_Last = new int[] { 5, 10 };

    [SerializeField]
    private PlayerSystem _playerSys;
    private DungeonSystem _dgnSys;  //�̺�Ʈ ���� ��, �̺�Ʈ�� ������ ���� ��ũ��Ʈ �Ѱܹ޾� ����ؾ���
    private GameObject _camera_dgn; //�̺�Ʈ ���� ��, ���� ī�޶� ��ũ��Ʈ�� �Ѱܹ޾� ����ؾ���

    [SerializeField]
    private ActionController _actController;    //�ൿ ��Ʈ�ѷ�
    [SerializeField]
    private DiceResultPannel _p_resultPannel;   //�÷��̾� �ֻ��� ���â

    [SerializeField]
    private GameLog _evntLog;           //���� �α�
    [SerializeField]
    private ItemSystem _itemSys;        //������ �ý���
    [SerializeField]
    private RewardPannel _rewardPannel; //����ǰ â

    [Header("# Camera")]
    [SerializeField]
    private GameObject _camera_evnt;     //���� ī�޶�

    [Header("# Event UI & Sprite")]
    [SerializeField]
    private SpriteSystem _p_spr;        //�÷��̾� ��������Ʈ
    [SerializeField]
    private SpriteRenderer _p_sprRend;  //�÷��̾� ��������Ʈ ������

    [SerializeField]
    private Animator _evnt_anima;       //�̺�Ʈ ��������Ʈ �ִϸ�����

    [SerializeField]
    private GameObject _btn_eventEnd;      //�̺�Ʈ ���� ��ư

    //�ֻ��� ���� â
    //�ֻ��� ���� �ؽ�Ʈ
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

        public CheckRule Rule;  //�ֻ��� üũ ��Ģ
        public int Dice;        //������ �ֻ��� ����

        public int CheckMin; //�ֻ��� üũ �ൿ �� ���� �ּҰ�
        public int CheckMax; //�ֻ��� üũ �ൿ �� ���� �ִ밪
    }

    [SerializeField]
    private List<EvntAct> _nowEvnt_actList;
    public List<EvntAct> ActList
    {
        get { return _nowEvnt_actList; }
    }

    [SerializeField]
    private bool _resultProcess;    //�� ������ false�� ��, �̺�Ʈ ��� ó��

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
        _dgnSys = dgnSys;   //���� ������ ��ũ��Ʈ �Ҵ�
        _camera_dgn = camera;   //���� ������ ī�޶� ������Ʈ �Ҵ�
    }

    public void EventStart(EventModule evnt)    //�̺�Ʈ ���� & �̺�Ʈ �ൿ �� �ֻ��� üũ ���� ����
    {
        _evntLog.gameObject.SetActive(true);

        _nowEvnt = evnt;    //�̺�Ʈ ���� ����
        _nowEvnt_actList = new List<EvntAct>();     //�̺�Ʈ ����Ʈ �ʱ�ȭ

        //�̺�Ʈ �ൿ ����Ʈ�� �ൿ���� �߰�
        for (int i = 0; i < _nowEvnt.ActList.Count; i++)
        {
            if (_nowEvnt.ActList[i].Name != "")
                Add_NewEventAction(i, _nowEvnt.ActList[i]); //�̺�Ʈ �ൿ �߰�
        }

        _dgnSys.EVNT_PROCESS = true;    //�̺�Ʈ ���� ��Ȳ ����

        _camera_evnt.SetActive(true);   //�̺�Ʈ ī�޶� Ȱ��ȭ
        _camera_dgn.SetActive(false);   //���� ī�޶� ��Ȱ��ȭ

        //�÷��̾� �޴� ��ư Off
        _playerSys.MenuButton_OnOff_Status(false);
        _playerSys.MenuButton_OnOff_Inventory(false);
        _playerSys.MenuButton_OnOff_ActList(false);

        //�̺�Ʈ ������Ʈ ��������Ʈ ����
        _evnt_anima.gameObject.SetActive(true);
        _evnt_anima.runtimeAnimatorController = _nowEvnt.EventObj_Anima;

        Refresh_Log();  //�α� ���ΰ�ħ
        _evntLog.SetLog_EventStart(_nowEvnt.StartLog);  //���� �α� ���

        //�̺�Ʈ �ൿ ��� ���
        _actController.Set_ActListSituation(ActionController.Situation.Event);
    }

    void Add_NewEventAction(int index, EventAction act) //�ൿ�� �߰��ϰ�, �ֻ��� üũ�� �ִ� �ൿ�� üũ ���ǰ� ���� ����
    {
        var rule = act.Check.Rule;
        var dice = 0;
        var checkMin = 0;
        var checkMax = 0;

        //���� ���� ���� �ֻ��� ��հ��� ������
        var avgDice = AvgDice_first;

        if (act.IsDiceCheck)    //�ֻ��� üũ �ൿ�� ���
        {
            //�ֻ��� ���� ������ ����
            dice = Random.Range(1, 6);

            //���� ���� �ֻ��� ������ ���� �ּҰ��� �ִ밪 ����
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

        //�̺�Ʈ �ൿ Ŭ���� ����
        EvntAct newAct = new EvntAct()
        {
            Data = act,
            Rule = rule,
            Dice = dice,
            CheckMin = checkMin,
            CheckMax = checkMax
        };

        //������ �̺�Ʈ �ൿ�� ����Ʈ�� �߰�
        if (index >= 0)
            _nowEvnt_actList.Insert(index, newAct);
        else
            _nowEvnt_actList.Add(newAct);
    }

    //�ֻ��� ���� �ؽ�Ʈ ���� �Լ�

    public DiceCheckResult Check_DiceCondition(int index, int total, int[] result)    //�ֻ��� ���� üũ ��� ��ȯ �Լ�  
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

    public void DiceCheck_Success()    //�ֻ��� üũ ����
    {
        Refresh_Log();
        _evntLog.SetLog_DiceCheck_Success("");
    }

    public void DiceCheck_MinFail()    //�ֻ��� üũ ���� (�� �̴�)
    {
        Refresh_Log();
        _evntLog.SetLog_DiceCheck_MinFail("");
    }

    public void DiceCheck_MaxFail()    //�ֻ��� üũ ���� (�� �ʰ�)
    {
        Refresh_Log();
        _evntLog.SetLog_DiceCheck_MaxFail("");
    }

    //�̺�Ʈ ��� ���� �Լ�
    public IEnumerator EventResultFlow(bool isSuccess, bool failMax)
    {
        var act = _nowEvnt_actList[_actController.NOW_CURSOR];  //�ൿ�� ����� �ֻ��� üũ ����� ���� �̺�Ʈ ��� ����
        EventResult result = null;
        var index = -1;

        if (isSuccess)  //�ൿ ���� ��
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

        _evntLog.SetLog_EventResult(result.Log);    //�α� ���

        for (int i = 0; i < result.Type.Length; i++)
        {
            yield return new WaitUntil(() => _resultProcess == false);  //�ٸ� ����� ���� ���� ��� ���� ��� �ߴ�

            switch (result.Type[i])
            {
                case ResultType.ActRemove:  //�� �ൿ ����
                    _nowEvnt_actList.Remove(act);
                    break;
                case ResultType.ActAdd:     //�� �ൿ �߰�
                    foreach (EventAction ea in result.NewAct)
                        Add_NewEventAction(_actController.NOW_CURSOR, ea);
                    break;
                case ResultType.Exp:        //����ġ
                    Set_ResultProcess(true);    //�̺�Ʈ ��� ���� ����
                    _rewardPannel.RewardPannel_Exp_OnOff(true); //����ġ �г� On
                    _rewardPannel.Set_RewardExpInfo();  //����ġ ȹ�� �г��� ��ġ ����
                    _rewardPannel.Set_GetExpText(result.Exp);   //ȹ�� ����ġ ǥ��
                    break;
                case ResultType.Item:       //������
                    _rewardPannel.RewardPannel_Item_OnOff(true);    //������ ȹ�� �г� On
                    _itemSys.Reward_Clear();    //���� ����ǰ ��� ����
                    _itemSys.ON_REWARD = true;

                    //������ ���� ������
                    var amount = Random.Range(2, 5);

                    ItemData[] item;

                    if (result.Item.Length == 0)
                        item = _nowEvnt.Item.ToArray();
                    else
                        item = result.Item.ToArray();

                    for (int j = 0; j < amount; j++)
                        _itemSys.Create_Item(item[Random.Range(0, item.Length)], ItemSystem.ItemSlotType.Reward, i);

                    _itemSys.Set_RewardIcon();  //����ǰ �����۵��� ������ ����
                    break;
                case ResultType.Buff:       //����

                    break;
                case ResultType.Debuff:     //�����

                    break;
                case ResultType.Btl:    //����

                    break;
                case ResultType.EvntEnd:    //�̺�Ʈ ����
                    //�÷��̾� �޴� ��ư On                    
                    _playerSys.MenuButton_OnOff_Status(true);
                    _playerSys.MenuButton_OnOff_Inventory(true);
                    _playerSys.MenuButton_OnOff_ActList(true);

                    _actController.Set_ActListSituation(ActionController.Situation.No); //�̺�Ʈ �ൿ ���, �ֻ��� ���� off
                    _actController.Dice_Off();  //�ֻ��� off
                    
                    DiceRulePannel_OnOff(false, 0);    //�ֻ��� ���� â off
                    _actController.DiceSelectPannel_OnOff(false);   //�ֻ��� ����â off
                    _actController.NoDiceButton_OnOff(false);       //�ֻ��� ���� �ൿ ���� ��ư off
                    _actController.DiceResultPannel_Off();          //�ֻ��� ���â off

                    _evnt_anima.gameObject.SetActive(false);    //�̺�Ʈ ������Ʈ ��������Ʈ off
                    _btn_eventEnd.SetActive(true); //�̺�Ʈ ���� ��ư on
                    yield break;
            }

            Debug.Log("���� �̺�Ʈ ������� ����");

            DiceRulePannel_OnOff(false, 0);    //�ֻ��� ���� â off
            _actController.DiceResultPannel_Off();  //�ֻ��� ���â off

            _actController.Set_ActListSituation(ActionController.Situation.Event);  //�ൿ ����Ʈ �����
        }
    }

    public void Set_ResultProcess(bool b) => _resultProcess = b;

    public void EventEnd()  //�̺�Ʈ ����
    {
        _nowEvnt = null;    //�̺�Ʈ ���� ����
        _nowEvnt_actList = null;    //�̺�Ʈ �ൿ ��� ����

        _dgnSys.EVNT_PROCESS = false;   //�̺�Ʈ ���� ��Ȳ off

        _camera_dgn.SetActive(true);    //���� ī�޶� Ȱ��ȭ
        _camera_evnt.SetActive(false);  //�̺�Ʈ ī�޶� ��Ȱ��ȭ

        _actController.Set_ActListSituation(ActionController.Situation.No); //�̺�Ʈ �ൿ ���, �ֻ��� ���� off

        _actController.DiceSelectPannel_OnOff(false);   //�ֻ��� ����â off
        _actController.DiceResultPannel_Off();          //�ֻ��� ���â off
        _actController.NoDiceButton_OnOff(false);       //�ֻ��� ���� �ൿ ���� ��ư Off

        //�ֻ��� ���� â off
        _evnt_anima.gameObject.SetActive(false);    //�̺�Ʈ ������Ʈ ��������Ʈ off

        _evntLog.gameObject.SetActive(false);   //�̺�Ʈ �α� off

        _itemSys.Reward_Clear();    //����ǰ ��� ����
        _rewardPannel.RewardPannel_Exp_OnOff(false);
        _rewardPannel.RewardPannel_Item_OnOff(false);
        _itemSys.ON_REWARD = false;

        _btn_eventEnd.SetActive(false); //�̺�Ʈ ���� ��ư off
    }

    public void DiceRulePannel_OnOff(bool isOn, int index) //�ֻ��� ���� �г� OnOff
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

    //���� �ý������κ��� �ֺ� ���� ������ �޾�, ���� ����� ������
    public void Set_EventField(bool[] wall, Sprite[] tileSprite, Sprite[] wallSprite)
    {
        //�� ���� �迭
        var wallBool = wall.ToArray();   // [1_2] [1_2_b] [3] [3_b] [4_5] [7_8] [9] [9_b] [10_11] [10_11_b] [12_b] 
        //Ÿ�� ��������Ʈ �迭
        var spr_tile = tileSprite.ToArray();
        //�� ��������Ʈ �迭
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

        _tileSet_12.GetChild(0).gameObject.SetActive(wallBool[0] || wallBool[8]); //1_2 �Ǵ� 10_11�� õ���� ������ ���

        _tileSet_12_beyond.GetChild(0).gameObject.SetActive(wallBool[10]);

        //�� �׷��� ����
        if (wallSprite != null)
        {

        }

        //Ÿ�� �׷��� ����
        if (tileSprite != null)
        {
            //center Ÿ�� ���� ��������Ʈ
            for (int i = 0; i < _tileSet_center.childCount; i++)
            {
                _tileSet_center.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite
                    = spr_tile[Random.Range(0, spr_tile.Length)];
            }

            //1_2 Ÿ�Ͽ� ���� ���ٸ� ���� ��������Ʈ
            if (wallBool[0] == false)
            {
                for (int i = 2; i < _tileSet_1_2.childCount; i++)
                {
                    _tileSet_1_2.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite
                        = spr_tile[Random.Range(0, spr_tile.Length)];
                }
            }
            //10_11 Ÿ�Ͽ� ���� ���ٸ� ���� ��������Ʈ
            if (wallBool[8] == false)
            {
                for (int i = 2; i < _tileSet_10_11.childCount; i++)
                {
                    _tileSet_10_11.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite
                        = spr_tile[Random.Range(0, spr_tile.Length)];
                }
            }
        }

        //Ÿ�� ��� ����
    }

    void Refresh_Log()
    {
        _evntLog.gameObject.SetActive(false);
        _evntLog.gameObject.SetActive(true);
    }
}
