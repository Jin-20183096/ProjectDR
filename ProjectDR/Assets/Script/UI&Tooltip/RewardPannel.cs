using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static PlayerSystem;

public class RewardPannel : MonoBehaviour
{
    [SerializeField]
    private BattleSystem _btlSys;

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

        _text_getExp.gameObject.SetActive(exp > 0);
        _exp_get = exp;

        _text_getExp.text = "+" + _exp_get;
        StartCoroutine("ExpMeter_Up");
    }

    IEnumerator ExpMeter_Up()
    {
        yield return new WaitForSecondsRealtime(1f / 10);
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
}
