using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static PlayerSystem;

public class RewardPannel : MonoBehaviour
{
    [Header("# Reward Exp")]
    [SerializeField]
    private GameObject _pannel_exp;
    [SerializeField]
    private Image _expMeter;
    [SerializeField]
    private TextMeshProUGUI _text_exp;
    [SerializeField]
    private TextMeshProUGUI _text_expMax;
    [SerializeField]
    private TextMeshProUGUI _text_getExp;

    private int _exp;
    private int _expMax;

    [Header("# Reward Item")]
    [SerializeField]
    private VerticalLayoutGroup _pannel_item;
    [SerializeField]
    private HorizontalLayoutGroup[] _itemSlot_layout;

    public void RewardPannel_Exp_OnOff(bool isOn)
        => _pannel_exp.SetActive(isOn);

    public void RewardPannel_Item_OnOff(bool isOn)
        => _pannel_item.gameObject.SetActive(isOn);

    public void Set_RewardExpInfo() //경험치 획득 패널의 수치들을 상황에 맞게 설정
    {
        _exp = PlayerSys.EXP;
        _expMax = PlayerSys.EXP_MAX;

        _text_exp.text = _exp.ToString();
        _text_expMax.text = _expMax.ToString();
        _expMeter.fillAmount = (float)_exp / _expMax;
    }

    public void Set_GetExpText(int exp)
    {
        _text_getExp.gameObject.SetActive(exp > 0);

        if (exp > 0)
            _text_getExp.text = "(+" + exp + ")";
    }

    IEnumerator Get_Exp()
    {
        yield return new WaitForSeconds(1f);
    }
}
