using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    public enum TooltipType { Off, Main, Equipped }
    [SerializeField]
    private TooltipType _tooltipType;

    [Header("# Main Info")]
    [SerializeField]
    private Image _icon_itemType;           //������ Ÿ�� ������
    [SerializeField]
    private TextMeshProUGUI _itemName; //������ �̸� �ؽ�Ʈ

    [Header("# Stats")]
    [SerializeField]
    private TextMeshProUGUI _stat1;        //����1 �ؽ�Ʈ
    [SerializeField]
    private TextMeshProUGUI _stat1_value;  //����1 �� �ؽ�Ʈ
    [SerializeField]
    private TextMeshProUGUI _stat2;        //����2 �ؽ�Ʈ
    [SerializeField]
    private TextMeshProUGUI _stat2_value;  //����2 �� �ؽ�Ʈ

    [SerializeField]
    private TextMeshProUGUI _actStat1;     //�ൿ����1 �ؽ�Ʈ
    [SerializeField]
    private Image[] _img_actStat1;              //�ൿ����1 �ֻ��� �̹���
    [SerializeField]
    private TextMeshProUGUI _actStat2;     //�ൿ����2 �ؽ�Ʈ
    [SerializeField]
    private Image[] _img_actStat2;              //�ൿ����2 �ֻ��� �̹���

    [Header("# BattleAction")]
    [SerializeField]
    private GameObject _act1;               //�ൿ1
    [SerializeField]
    private Image _act1_typeIcon;      //�ൿ1 Ÿ�� ������
    [SerializeField]
    private TextMeshProUGUI _act1_name;     //�ൿ1 �̸�
    [SerializeField]
    private TextMeshProUGUI _act1_stat; //�ൿ1 ����
    [SerializeField]
    private GameObject _act2;               //�ൿ2
    [SerializeField]
    private Image _act2_typeIcon;      //�ൿ2 Ÿ�� ������
    [SerializeField]
    private TextMeshProUGUI _act2_name;     //�ൿ2 �̸�
    [SerializeField]
    private TextMeshProUGUI _act2_stat;     //�ൿ2 ����

    [Header("# Ability")]
    [SerializeField]
    private GameObject _ability;                //�ɷ� ����â
    [SerializeField]
    private TextMeshProUGUI _ability_name;  //�ɷ� �̸�
    [SerializeField]
    private TextMeshProUGUI _ability_info;  //�ɷ� ����

    [Header("# UI")]
    [SerializeField]
    private GameObject _partition1;
    [SerializeField]
    private GameObject _partition2;
    [SerializeField]
    private GameObject _partition3;
    [SerializeField]
    private GameObject _equipSign;    //��� ���� ������ ǥ��

    private HorizontalLayoutGroup _tooltipLayout;   //���� ���̾ƿ�
    private RectTransform _rect_tooltipLayout;  //���� ���̾ƿ� Rect
    private Image _img;

    private Vector3 _outVec;

    [SerializeField]
    private Sprite[] _spr_itemTypeIcon;
    [SerializeField]
    private Sprite[] _spr_diceSide;
    [SerializeField]
    private Sprite[] _spr_typeIcon;

    private string[] _statName_arr = { "", "��", "����", "������", "��ø", "�ǰ�", "����", "HP", "��",
                                        "�� �籼��", "���� �籼��", "������ �籼��", "��ø �籼��", "�ǰ� �籼��", "���� �籼��" };

    private void Start()
    {
        _tooltipLayout = transform.parent.GetComponent<HorizontalLayoutGroup>();
        _rect_tooltipLayout = transform.parent.GetComponent<RectTransform>();
        _img = GetComponent<Image>();

        _outVec = _rect_tooltipLayout.anchoredPosition;
    }

    public void ItemTooltip_On()
    {
        if (_equipSign != null)
            _equipSign.SetActive(true);

        _img.enabled = true;   //���� �г� �̹���
        _icon_itemType.gameObject.SetActive(true); //������ Ÿ�� ������
        _itemName.gameObject.SetActive(true); //������ �̸� �ؽ�Ʈ

        if (_partition1 != null)
            _partition1.SetActive(true);   //���� ���м� 1
    }

    public void ItemTooltip_Off()
    {
        if (_equipSign != null)
            _equipSign.SetActive(false);

        _img.enabled = false;   //���� �г� �̹���
        _icon_itemType.gameObject.SetActive(false); //������ Ÿ�� ������
        _itemName.gameObject.SetActive(false); //������ �̸� �ؽ�Ʈ

        if (_partition1 != null)
            _partition1.SetActive(false);   //���� ���м� 1

        if (_stat1 != null)
            _stat1.gameObject.SetActive(false);     //���� 1 ǥ��
        if (_stat2 != null)
            _stat2.gameObject.SetActive(false);     //���� 2 ǥ��
        if (_actStat1 != null)
            _actStat1.gameObject.SetActive(false);  //�ൿ ���� 1 ǥ��
        if (_actStat2 != null)
            _actStat2.gameObject.SetActive(false);  //�ൿ ���� 2 ǥ��

        if (_act1 != null)
            _act1.SetActive(false); //�ൿ1 ǥ��
        if (_act2 != null)
            _act2.SetActive(false); //�ൿ2 ǥ��

        if (_ability != null)
            _ability.SetActive(false);  //�ɷ� ǥ��

        if (_partition2 != null)
            _partition2.SetActive(false);   //���� ���м� 2
        if (_partition3 != null)
            _partition3.SetActive(false);   //���� ���м� 3
    }

    public void Change_Name(string s)
        => _itemName.text = s;

    public void Change_ItemType(ItemData.ItemType t)
        => _icon_itemType.sprite = _spr_itemTypeIcon[(int)t];

    public void Change_Stat1(ICreature.Stats stat, int v)
    {
        _stat1.gameObject.SetActive(true);

        _stat1.text = _statName_arr[(int)stat];
        _stat1_value.text = v.ToString();
    }

    public void Change_Stat2(ICreature.Stats stat, int v)
    {
        _stat2.gameObject.SetActive(true);

        _stat2.text = _statName_arr[(int)stat];
        _stat2_value.text = v.ToString();
    }

    public void Change_ActionStat1(ICreature.Stats stat, int[] arr)
    {
        _actStat1.gameObject.SetActive(true);

        _actStat1.text = _statName_arr[(int)stat];

        for (int i = 0; i < _img_actStat1.Length; i++)
        {
            if (arr[i] >= 10)
                _img_actStat1[i].sprite = _spr_diceSide[10];
            else if (arr[i] <= 0)
                _img_actStat1[i].sprite = _spr_diceSide[0];
            else
                _img_actStat1[i].sprite = _spr_diceSide[arr[i]];

            if (arr[i] == 0)
                _img_actStat1[i].color = new Color(1, 1, 1, 0.3f);
            else
                _img_actStat1[i].color = new Color(1, 1, 1, 1f);
        }
    }

    public void Change_ActionStat2(ICreature.Stats stat, int[] arr)
    {
        _actStat2.gameObject.SetActive(true);

        _actStat2.text = _statName_arr[(int)stat];

        for (int i = 0; i < _img_actStat2.Length; i++)
        {
            if (arr[i] >= 10)
                _img_actStat2[i].sprite = _spr_diceSide[10];
            else if (arr[i] <= 0)
                _img_actStat2[i].sprite = _spr_diceSide[0];
            else
                _img_actStat2[i].sprite = _spr_diceSide[arr[i]];

            if (arr[i] == 0)
                _img_actStat2[i].color = new Color(1, 1, 1, 0.3f);
            else
                _img_actStat2[i].color = new Color(1, 1, 1, 1f);
        }
    }

    public void Partition2_On()
    {
        if (_partition2 != null)
            _partition2.SetActive(true);    //���� ���м� 2 Ȱ��ȭ
    }

    public void Change_Action1(ICreature.BtlActClass btlAct)
    {
        var data = btlAct.Data;

        if (data == null)
        {
            if (_act1 != null)
                _act1.SetActive(false);
        }
        else
        {
            _act1.SetActive(true);
            _act1_typeIcon.sprite = _spr_typeIcon[(int)data.Type];

            string upgradeStr = "";
            if (btlAct.Upgrade > 0) upgradeStr = " +" + btlAct.Upgrade;

            _act1_name.text = data.Name + upgradeStr;
            _act1_stat.text = _statName_arr[(int)btlAct.Stat];
        }
    }

    public void Change_Action2(ICreature.BtlActClass btlAct)
    {
        var data = btlAct.Data;
        
        if (data == null)
        {
            if (_act2 != null)
                _act2.SetActive(false);
        }
        else
        {
            _act2.SetActive(true);
            _act2_typeIcon.sprite = _spr_typeIcon[(int)data.Type];

            string upgradeStr = "";
            if (btlAct.Upgrade > 0) upgradeStr = " +" + btlAct.Upgrade;

            _act2_name.text = data.Name + upgradeStr;
            _act2_stat.text = _statName_arr[(int)btlAct.Stat];
        }
    }

    public void Change_Ability(AbilityData ability)
    {
        if (ability == null)
        {
            if (_ability != null)
                _ability.SetActive(false);
        }
        else
        {
            _ability.SetActive(true);
            _ability_name.text = ability.NAME;
            _ability_info.text = ability.INFO;
        }
    }

    public void Set_TooltipOutScreen()
    {
        _rect_tooltipLayout.pivot = new Vector2(1, 1);
        transform.parent.position = _outVec;
    }

    public void Set_TooltipPosition(Vector3 slotPos)
    {
        if (_tooltipType == TooltipType.Main)
        {
            Vector2 pos = slotPos;

            float pivotX;
            float pivotY;
            var addX = 0f;
            var addY = 0f;

            if (pos.x > Screen.width / 2)   //ȭ�� ����
            {
                pivotX = 1f;
                transform.SetAsLastSibling();
            }
            else    //ȭ�� ����
            {
                pivotX = 0f;
                transform.SetAsFirstSibling();

                addX = 24f;     //������ xũ�⸸ŭ x�� ����
            }

            if (pos.y > Screen.height * 2 / 3)  //ȭ�� ���
            {
                _tooltipLayout.childAlignment = TextAnchor.UpperLeft;
                pivotY = 1f;
            }
            else if (pos.y <= Screen.height * 2 / 3 && pos.y > Screen.height * 1 / 3)   //ȭ�� �ߴ�
            {
                _tooltipLayout.childAlignment = TextAnchor.MiddleLeft;
                pivotY = 0.5f;

                addY = -12f;    //������ yũ���� ���ݸ�ŭ y�� ����
            }
            else    //ȭ�� �ϴ�
            {
                _tooltipLayout.childAlignment = TextAnchor.LowerLeft;
                pivotY = 0f;

                addY = -24f;    //������ yũ�⸸ŭ y�� ����
            }

            transform.parent.position = new Vector2(pos.x, pos.y);
            _rect_tooltipLayout.pivot = new Vector2(pivotX, pivotY);
            _rect_tooltipLayout.anchoredPosition =
                new Vector2(_rect_tooltipLayout.anchoredPosition.x + addX,
                            _rect_tooltipLayout.anchoredPosition.y + addY);
        }
        else if (_tooltipType == TooltipType.Equipped)
        {
            Vector2 pos = slotPos;

            if (pos.x > Screen.width / 2)   //ȭ�� �߾� ���� ������ ��ġ
                transform.SetAsFirstSibling();
            else
                transform.SetAsLastSibling();
        }
    }

    public void Refresh_Layout()
    {
        Canvas.ForceUpdateCanvases();
        _tooltipLayout.enabled = false;
        _tooltipLayout.enabled = true;
    }
}
