using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiceResultPannel : MonoBehaviour
{
    private float offAlpha = 0.2f;

    [SerializeField]
    private GameObject _pannel_actInfo;  //�ൿ ����â
    [SerializeField]
    private HorizontalLayoutGroup _layout_actInfo;   //�ൿ ����â ���̾ƿ�

    [Header("# Action Info")]
    [SerializeField]
    private Image _icon_actType;    //�ൿ Ÿ�� ������
    [SerializeField]
    private TextMeshProUGUI _txt_actName;   //�ൿ�� �ؽ�Ʈ
    [SerializeField]
    private TextMeshProUGUI _txt_diceTotal; //�ֻ��� ���� �ؽ�Ʈ

    [Header("# Dice Result")]
    [SerializeField]
    private GameObject _pannel_diceResult;  //�ֻ��� ���â
    [SerializeField]
    private Image[] _img_diceSide;          //�� �ֻ����� ��� �̹���

    [Header("# Reroll Button")]
    [SerializeField]
    private Button _btn_reroll;             //�籼�� ��ư
    [SerializeField]
    private Image[] _img_btnReroll;         //�籼�� ��ư�� �̹�����
    [SerializeField]
    private TextMeshProUGUI _txt_reroll;    //�籼�� Ƚ�� �ؽ�Ʈ

    [Header("# ActStart Button")]
    [SerializeField]
    private Button _btn_actStart;           //�ൿ ���� ��ư
    [SerializeField]
    private Image[] _img_btnActStart;       //�ൿ ���� ��ư�� �̹�����

    [Header("# Sprite Reference")]
    [SerializeField]
    private Sprite[] _spr_actType;  //�ൿ Ÿ�� ��������Ʈ
    [SerializeField]
    private Sprite[] _spr_dice;     //�ֻ��� �� ��������Ʈ

    public void ActionInfoPannel_OnOff(bool b)  //�ൿ ����â OnOff
        => _pannel_actInfo.SetActive(b);

    public void Change_ActInfo(BtlActData.ActType type, string actName)  //�ൿ ����â ���� ����
    {
        _icon_actType.sprite = _spr_actType[(int)type];
        _txt_actName.text = actName;
        _txt_diceTotal.text = "";
        RefreshLayout();
    }

    public void Set_NewDiceTotal(int nowDice)   //�ֻ��� ���â �ʱ�ȭ
    {
        for (int i = 0; i < _img_diceSide.Length; i++)
        {
            _img_diceSide[i].enabled = i < nowDice; //������ �ʴ� �ֻ����� �̹��� Off

            if (i < nowDice)
            {
                //�ֻ����� �̹��� ��Ȯ�� ���·� ǥ��
                _img_diceSide[i].sprite = _spr_dice[0];
                _img_diceSide[i].color = new Color(1, 1, 1, 0.3f);
            }
        }
        RefreshLayout();
    }

    public void DiceResultPannel_OnOff(bool b)  //�ֻ��� ���â OnOff
        => _pannel_diceResult.SetActive(b);

    public void Set_MomentDiceResult(int order, int value)  //�ֻ��� ���â ��� ����(�������� �ֻ����� ���)
    {
        _img_diceSide[order].color = new Color(1, 1, 1, 0.3f);
        _img_diceSide[order].sprite = _spr_dice[value];
    }

    public void Set_StopDiceResult(int order, int value)    //�ֻ��� ���â ��� ����(���� �ֻ����� ���)
    {
        _img_diceSide[order].color = new Color(1, 1, 1, 1);
        _img_diceSide[order].sprite = _spr_dice[value];
    }

    public void Change_DiceTotal(string total)  //�ֻ��� ���� �ؽ�Ʈ ����
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

    public void RerollButton_OnOff(bool b)  //�籼�� ��ư OnOff
    {
        if (_btn_reroll != null)
            _btn_reroll.gameObject.SetActive(b);
    }

    public void Change_RerollText(string count) //�籼�� Ƚ�� �ؽ�Ʈ ����
        => _txt_reroll.text = count;

    public void SetAble_RerollButton(bool b)  //�籼�� ��ư ��ȣ�ۿ� ���� OnOff
    {
        if (_btn_reroll != null)
        {
            _btn_reroll.interactable = b;

            foreach (Image img in _img_btnReroll)
                img.color = new Color(1, 1, 1, b ? 1f : offAlpha);

            _txt_reroll.color = new Color(1, 1, 1, b ? 1f : offAlpha);
        }
    }

    public void ActStartButton_OnOff(bool b)    //�ൿ ���� ��ư OnOff
    {
        if (_btn_actStart != null)
            _btn_actStart.gameObject.SetActive(b);
    }

    public void SetAble_ActStartButton(bool b)  //�ൿ ���� ��ư ��ȣ�ۿ� ���� OnOff
    {
        if (_btn_actStart != null)
        {
            _btn_actStart.interactable = b;

            foreach (Image img in _img_btnActStart)
                img.color = new Color(1, 1, 1, b ? 1f : offAlpha);
        }
    }
}
