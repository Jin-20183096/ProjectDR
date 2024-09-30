using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class StatusScreen : MonoBehaviour, IPointerClickHandler
{
    [Header("# Main")]
    [SerializeField]
    private TextMeshProUGUI _txt_name;  //�̸�
    [SerializeField]
    private TextMeshProUGUI _txt_lv;    //����
    [SerializeField]
    private TextMeshProUGUI _txt_exp;   //����ġ
    [SerializeField]
    private TextMeshProUGUI _txt_expMax;    //�ִ� ����ġ
    [SerializeField]
    private Image _meter_exp;   //����ġ ����
    [SerializeField]
    private TextMeshProUGUI _txt_hp;    //HP
    [SerializeField]
    private TextMeshProUGUI _txt_hpMax; //�ִ� HP
    [SerializeField]
    private TextMeshProUGUI _txt_ap;    //�ൿ��
    [SerializeField]
    private TextMeshProUGUI _txt_apMax; //�ִ� �ൿ��
    [SerializeField]
    private TextMeshProUGUI _txt_ac;    //��
    [SerializeField]
    private TextMeshProUGUI _txt_acMax; //�ִ� ��

    [Header("# Stat")]
    [SerializeField]
    private TextMeshProUGUI _txt_reroll_STR;    //�� �籼��
    [SerializeField]
    private Image[] _img_STR;              //�� �ֻ��� ����
    [SerializeField]
    private TextMeshProUGUI _txt_reroll_INT;    //���� �籼��
    [SerializeField]
    private Image[] _img_INT;              //���� �ֻ��� ����
    [SerializeField]
    private TextMeshProUGUI _txt_reroll_DEX;    //������ �籼��
    [SerializeField]
    private Image[] _img_DEX;              //������ �ֻ��� ����
    [SerializeField]
    private TextMeshProUGUI _txt_reroll_AGI;    //��ø �籼��
    [SerializeField]
    private Image[] _img_AGI;              //��ø �ֻ��� ����
    [SerializeField]
    private TextMeshProUGUI _txt_reroll_WIL;    //���� �籼��
    [SerializeField]
    private Image[] _img_WIL;              //���� �ֻ��� ����

    [SerializeField]
    private Sprite[] _spr_diceSide;

    public void OnPointerClick(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }

    public void Change_Name(string name)
        => _txt_name.text = name;

    public void Change_Lv(int lv)
        => _txt_lv.text = lv.ToString();

    public void Change_Exp(int exp)
        => _txt_exp.text = exp.ToString();

    public void Change_ExpMax(int expMax)
        => _txt_expMax.text = expMax.ToString();

    public void Change_ExpMeter(float amount)
        => _meter_exp.fillAmount = amount;

    public void Change_Hp(int hp)
        => _txt_hp.text = hp.ToString();

    public void Change_HpMax(int hpMax)
        => _txt_hpMax.text = hpMax.ToString();

    public void Change_Ap(int ap)
        => _txt_ap.text = ap.ToString();

    public void Change_ApMax(int apMax)
        => _txt_apMax.text = apMax.ToString();

    public void Change_AC(int ac)
        => _txt_ac.text = ac.ToString();

    public void Change_ACMax(int acMax)
        => _txt_acMax.text = acMax.ToString();
    public void Change_Reroll(ICreature.Stats stat, int value)
    {
        switch (stat)
        {
            case ICreature.Stats.STR:
                _txt_reroll_STR.text = value.ToString();
                break;
            case ICreature.Stats.INT:
                _txt_reroll_INT.text = value.ToString();
                break;
            case ICreature.Stats.DEX:
                _txt_reroll_DEX.text = value.ToString();
                break;
            case ICreature.Stats.AGI:
                _txt_reroll_AGI.text = value.ToString();
                break;
            case ICreature.Stats.WIL:
                _txt_reroll_WIL.text = value.ToString();
                break;
        }
    }

    public void Change_ActionStat(ICreature.Stats stat, int[] arr)
    {
        Image[] temp_stat = null;

        switch (stat)
        {
            case ICreature.Stats.STR:
                temp_stat = _img_STR;
                break;
            case ICreature.Stats.INT:
                temp_stat = _img_INT;
                break;
            case ICreature.Stats.DEX:
                temp_stat = _img_DEX;
                break;
            case ICreature.Stats.AGI:
                temp_stat = _img_AGI;
                break;
            case ICreature.Stats.WIL:
                temp_stat = _img_WIL;
                break;
            default:
                Debug.Log("�߸��� ������ ���� �Ѿ��");
                break;
        }

        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] >= 10)
                temp_stat[i].sprite = _spr_diceSide[10];
            else if (arr[i] <= 0)
                temp_stat[i].sprite = _spr_diceSide[0];
            else
                temp_stat[i].sprite = _spr_diceSide[arr[i]];
        }
    }
}
