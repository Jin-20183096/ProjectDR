using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static PlayerSystem;

public class RewardPannel : MonoBehaviour
{
    [SerializeField]
    private BattleSystem _btlSys;
    [SerializeField]
    private DungeonEventSystem _evntSys;

    [Header("# Reward Exp")]
    [SerializeField]
    private GameObject _pannel_exp;
    [SerializeField]
    private TextMeshProUGUI _text_lv;
    [SerializeField]
    private Image _expMeter;
    [SerializeField]
    private TextMeshProUGUI _text_exp;
    [SerializeField]
    private TextMeshProUGUI _text_expMax;
    [SerializeField]
    private TextMeshProUGUI _text_getExp;

    private int _exp_get;

    [Header("# Level Up")]
    [SerializeField]
    private LevelUpPannel _pannel_lvUp;

    private bool _levelUpProcess = false;

    [Header("# Reward Item")]
    [SerializeField]
    private VerticalLayoutGroup _pannel_item;
    [SerializeField]
    private HorizontalLayoutGroup[] _itemSlot_layout;


    public void RewardPannel_Exp_OnOff(bool isOn)
        => _pannel_exp.SetActive(isOn);

    public void RewardPannel_Item_OnOff(bool isOn)
        => _pannel_item.gameObject.SetActive(isOn);

    public void Set_RewardExpInfo() //����ġ ȹ�� �г��� ��ġ���� ��Ȳ�� �°� ����
    {
        _text_lv.text = PlayerSys.LV.ToString();
        _text_exp.text = PlayerSys.EXP.ToString();
        _text_expMax.text = PlayerSys.EXP_MAX.ToString();
        _expMeter.fillAmount = (float)PlayerSys.EXP / PlayerSys.EXP_MAX;
    }

    public void Set_GetExpText(int exp)
    {
        _btlSys.Set_RewardExpProcess(true);

        if (exp > 0)
        {
            _text_getExp.gameObject.SetActive(exp > 0);
            _exp_get = exp;
            StartCoroutine("ExpMeter_Up");
        }
    }

    IEnumerator ExpMeter_Up()
    {
        _text_getExp.text = "+" + _exp_get;
        yield return new WaitForSecondsRealtime(1/2f);
        _btlSys.Set_RewardExpProcess(true);

        while (_exp_get > 0)
        {
            yield return new WaitUntil(() => _levelUpProcess == false);

            if (_exp_get >= PlayerSys.EXP_MAX - PlayerSys.EXP)
                yield return new WaitForSecondsRealtime(1f / 50);
            else
                yield return new WaitForSecondsRealtime(1f / 20);

            PlayerSys.Change_Exp(true, 1);
            
            _exp_get--;

            if (_exp_get > 0)
                _text_getExp.text = "+" + _exp_get;
            else
                _text_getExp.text = "";

            _text_lv.text = PlayerSys.LV.ToString();
            _text_exp.text = PlayerSys.EXP.ToString();
            _text_expMax.text = PlayerSys.EXP_MAX.ToString();
            _expMeter.fillAmount = (float)PlayerSys.EXP / PlayerSys.EXP_MAX;

            //�÷��̾� ������ ���� üũ
            if (PlayerSys.EXP >= PlayerSys.EXP_MAX)   //�÷��̾� ����ġ�� �ִ��� ��
            {
                yield return new WaitForSeconds(1f);    //���� �� �� ��� ������
                LevelUpProcess_Start(); //������ ó�� ����

                yield return new WaitUntil(() => _levelUpProcess == false);
                PlayerSys.LvUp(1);

                _text_lv.text = PlayerSys.LV.ToString();
                _text_exp.text = PlayerSys.EXP.ToString();
                _text_expMax.text = PlayerSys.EXP_MAX.ToString();
                _expMeter.fillAmount = (float)PlayerSys.EXP / PlayerSys.EXP_MAX;
            }
        }

        _btlSys.Set_RewardExpProcess(false);
        _evntSys.Set_ResultProcess(false);
        StopCoroutine("ExpMeter_Up");
    }

    public void Set_LevelUpProcess(bool b) => _levelUpProcess = b;

    public void LevelUpProcess_Start()
    {
        Set_LevelUpProcess(true);   //������ ó�� �� On

        _pannel_lvUp.gameObject.SetActive(true);    //������ �г� Ȱ��ȭ

        _pannel_lvUp.Set_StatDisplay(); //������ �г� ���� ǥ�� 
    }

    public void LevelUpProcess_End()
    {
        _pannel_lvUp.gameObject.SetActive(false);   //������ �г� ��Ȱ��ȭ

        Set_LevelUpProcess(false);  //������ ó�� �� Off
    }

    public void Select_StatUp_STR() //�� ���� �ܷ�
    {
        //��� ������ Ŀ�� OFF
        _pannel_lvUp.AllStatUpCursor_Off();

        //�ٸ� ������ UI ��Ȱ��ȭ
        _pannel_lvUp.ActiveUI_STR(false);
        _pannel_lvUp.ActiveUI_INT(false);
        _pannel_lvUp.ActiveUI_DEX(false);
        _pannel_lvUp.ActiveUI_AGI(false);
        _pannel_lvUp.ActiveUI_CON(false);
        _pannel_lvUp.ActiveUI_WIL(false);

        //������ ��ġ�� ���� 1 ���
        int[] statArr = new int[6]; //������ ���� �迭
        var order = Random.Range(0, 6); //����� ���� ��ġ

        statArr[order] += 1;    //�ش� ��ġ�� ���� 1 ���

        PlayerSys.Change_ActionStat(true, ICreature.Stats.STR, statArr);    //�÷��̾�� ������ ���� ����
        _pannel_lvUp.Set_StatUpCursor(ICreature.Stats.STR, order, 1);       //���� Ŀ�� ����
        _pannel_lvUp.ButtonActive_STR(false);   //�� ������ ��ư ��Ȱ��ȭ

        Random_StatUp();    //������ ���� �߰� ���
    }

    public void Select_StatUp_INT() //���� ���� �ܷ�
    {
        //��� ������ Ŀ�� OFF
        _pannel_lvUp.AllStatUpCursor_Off();

        //�ٸ� ������ UI ��Ȱ��ȭ
        _pannel_lvUp.ActiveUI_STR(false);
        _pannel_lvUp.ActiveUI_INT(false);
        _pannel_lvUp.ActiveUI_DEX(false);
        _pannel_lvUp.ActiveUI_AGI(false);
        _pannel_lvUp.ActiveUI_CON(false);
        _pannel_lvUp.ActiveUI_WIL(false);

        //������ ��ġ�� ���� 1 ���
        int[] statArr = new int[6]; //������ ���� �迭
        var order = Random.Range(0, 6); //����� ���� ��ġ

        statArr[order] += 1;    //�ش� ��ġ�� ���� 1 ���

        PlayerSys.Change_ActionStat(true, ICreature.Stats.INT, statArr);    //�÷��̾�� ������ ���� ����
        _pannel_lvUp.Set_StatUpCursor(ICreature.Stats.INT, order, 1);       //���� Ŀ�� ����
        _pannel_lvUp.ButtonActive_INT(false);   //�� ������ ��ư ��Ȱ��ȭ

        Random_StatUp();    //������ ���� �߰� ���
    }

    public void Select_StatUp_DEX() //������ ���� �ܷ�
    {
        //��� ������ Ŀ�� OFF
        _pannel_lvUp.AllStatUpCursor_Off();

        //�ٸ� ������ UI ��Ȱ��ȭ
        _pannel_lvUp.ActiveUI_STR(false);
        _pannel_lvUp.ActiveUI_INT(false);
        _pannel_lvUp.ActiveUI_DEX(false);
        _pannel_lvUp.ActiveUI_AGI(false);
        _pannel_lvUp.ActiveUI_CON(false);
        _pannel_lvUp.ActiveUI_WIL(false);

        //������ ��ġ�� ���� 1 ���
        int[] statArr = new int[6]; //������ ���� �迭
        var order = Random.Range(0, 6); //����� ���� ��ġ

        statArr[order] += 1;    //�ش� ��ġ�� ���� 1 ���

        PlayerSys.Change_ActionStat(true, ICreature.Stats.DEX, statArr);    //�÷��̾�� ������ ���� ����
        _pannel_lvUp.Set_StatUpCursor(ICreature.Stats.DEX, order, 1);       //���� Ŀ�� ����
        _pannel_lvUp.ButtonActive_DEX(false);   //�� ������ ��ư ��Ȱ��ȭ

        Random_StatUp();    //������ ���� �߰� ���
    }

    public void Select_StatUp_AGI() //��ø ���� �ܷ�
    {
        //��� ������ Ŀ�� OFF
        _pannel_lvUp.AllStatUpCursor_Off();

        //�ٸ� ������ UI ��Ȱ��ȭ
        _pannel_lvUp.ActiveUI_STR(false);
        _pannel_lvUp.ActiveUI_INT(false);
        _pannel_lvUp.ActiveUI_DEX(false);
        _pannel_lvUp.ActiveUI_AGI(false);
        _pannel_lvUp.ActiveUI_CON(false);
        _pannel_lvUp.ActiveUI_WIL(false);

        //������ ��ġ�� ���� 1 ���
        int[] statArr = new int[6]; //������ ���� �迭
        var order = Random.Range(0, 6); //����� ���� ��ġ

        statArr[order] += 1;    //�ش� ��ġ�� ���� 1 ���

        PlayerSys.Change_ActionStat(true, ICreature.Stats.AGI, statArr);    //�÷��̾�� ������ ���� ����
        _pannel_lvUp.Set_StatUpCursor(ICreature.Stats.AGI, order, 1);       //���� Ŀ�� ����
        _pannel_lvUp.ButtonActive_AGI(false);   //�� ������ ��ư ��Ȱ��ȭ

        Random_StatUp();    //������ ���� �߰� ���
    }

    public void Select_StatUp_CON() //�ǰ� ���� �ܷ�
    {
        //��� ������ Ŀ�� OFF
        _pannel_lvUp.AllStatUpCursor_Off();

        //�ٸ� ������ UI ��Ȱ��ȭ
        _pannel_lvUp.ActiveUI_STR(false);
        _pannel_lvUp.ActiveUI_INT(false);
        _pannel_lvUp.ActiveUI_DEX(false);
        _pannel_lvUp.ActiveUI_AGI(false);
        _pannel_lvUp.ActiveUI_CON(false);
        _pannel_lvUp.ActiveUI_WIL(false);

        //������ ��ġ�� ���� 1 ���
        int[] statArr = new int[6]; //������ ���� �迭
        var order = Random.Range(0, 6); //����� ���� ��ġ

        statArr[order] += 1;    //�ش� ��ġ�� ���� 1 ���

        PlayerSys.Change_ActionStat(true, ICreature.Stats.CON, statArr);    //�÷��̾�� ������ ���� ����
        _pannel_lvUp.Set_StatUpCursor(ICreature.Stats.CON, order, 1);       //���� Ŀ�� ����
        _pannel_lvUp.ButtonActive_CON(false);   //�� ������ ��ư ��Ȱ��ȭ

        Random_StatUp();    //������ ���� �߰� ���
    }

    public void Select_StatUp_WIL() //���� ���� �ܷ�
    {
        //��� ������ Ŀ�� OFF
        _pannel_lvUp.AllStatUpCursor_Off();

        //�ٸ� ������ UI ��Ȱ��ȭ
        _pannel_lvUp.ActiveUI_STR(false);
        _pannel_lvUp.ActiveUI_INT(false);
        _pannel_lvUp.ActiveUI_DEX(false);
        _pannel_lvUp.ActiveUI_AGI(false);
        _pannel_lvUp.ActiveUI_CON(false);
        _pannel_lvUp.ActiveUI_WIL(false);

        //������ ��ġ�� ���� 1 ���
        int[] statArr = new int[6]; //������ ���� �迭
        var order = Random.Range(0, 6); //����� ���� ��ġ

        statArr[order] += 1;    //�ش� ��ġ�� ���� 1 ���

        PlayerSys.Change_ActionStat(true, ICreature.Stats.WIL, statArr);    //�÷��̾�� ������ ���� ����
        _pannel_lvUp.Set_StatUpCursor(ICreature.Stats.WIL, order, 1);       //���� Ŀ�� ����
        _pannel_lvUp.ButtonActive_WIL(false);   //�� ������ ��ư ��Ȱ��ȭ

        Random_StatUp();    //������ ���� �߰� ���
    }

    public void Random_StatUp()
    {
        int[] statArr = new int[6]; //������ ���� �迭

        //������ ����
        var stat = (ICreature.Stats)Random.Range((int)ICreature.Stats.STR, (int)ICreature.Stats.HP);
        //������ ��ġ
        var order = Random.Range(0, 6);

        statArr[order] += 1;    //�ش� ��ġ�� ���� 1 ���

        PlayerSys.Change_ActionStat(true, stat, statArr);   //�÷��̾�� ������ ���� ����
        _pannel_lvUp.Set_StatUpCursor(stat, order, 1);      //���� Ŀ�� ����

        //���� ����� �Ϸ�Ǿ����Ƿ�, ������
        _pannel_lvUp.LevelUpEndButton_OnOff(true);
    }
}
