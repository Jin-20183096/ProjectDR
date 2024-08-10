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
    private Image _icon_itemType;           //아이템 타입 아이콘
    [SerializeField]
    private TextMeshProUGUI _itemName; //아이템 이름 텍스트

    [Header("# Stats")]
    [SerializeField]
    private TextMeshProUGUI _stat1;        //스탯1 텍스트
    [SerializeField]
    private TextMeshProUGUI _stat1_value;  //스탯1 값 텍스트
    [SerializeField]
    private TextMeshProUGUI _stat2;        //스탯2 텍스트
    [SerializeField]
    private TextMeshProUGUI _stat2_value;  //스탯2 값 텍스트

    [SerializeField]
    private TextMeshProUGUI _actStat1;     //행동스탯1 텍스트
    [SerializeField]
    private Image[] _img_actStat1;              //행동스탯1 주사위 이미지
    [SerializeField]
    private TextMeshProUGUI _actStat2;     //행동스탯2 텍스트
    [SerializeField]
    private Image[] _img_actStat2;              //행동스탯2 주사위 이미지

    [Header("# BattleAction")]
    [SerializeField]
    private GameObject _act1;               //행동1
    [SerializeField]
    private Image _act1_typeIcon;      //행동1 타입 아이콘
    [SerializeField]
    private TextMeshProUGUI _act1_name;     //행동1 이름
    [SerializeField]
    private TextMeshProUGUI _act1_stat; //행동1 스탯
    [SerializeField]
    private GameObject _act2;               //행동2
    [SerializeField]
    private Image _act2_typeIcon;      //행동2 타입 아이콘
    [SerializeField]
    private TextMeshProUGUI _act2_name;     //행동2 이름
    [SerializeField]
    private TextMeshProUGUI _act2_stat;     //행동2 스탯

    [Header("# Ability")]
    [SerializeField]
    private GameObject _ability;                //능력 설명창
    [SerializeField]
    private TextMeshProUGUI _ability_name;  //능력 이름
    [SerializeField]
    private TextMeshProUGUI _ability_info;  //능력 설명

    [Header("# UI")]
    [SerializeField]
    private GameObject _partition1;
    [SerializeField]
    private GameObject _partition2;
    [SerializeField]
    private GameObject _partition3;
    [SerializeField]
    private GameObject _equipSign;    //장비 중인 아이템 표기

    private HorizontalLayoutGroup _tooltipLayout;   //툴팁 레이아웃
    private RectTransform _rect_tooltipLayout;  //툴팁 레이아웃 Rect
    private Image _img;

    private Vector3 _outVec;

    [SerializeField]
    private Sprite[] _spr_itemTypeIcon;
    [SerializeField]
    private Sprite[] _spr_diceSide;
    [SerializeField]
    private Sprite[] _spr_typeIcon;

    private string[] _statName_arr = { "", "힘", "지능", "손재주", "민첩", "건강", "의지", "HP", "방어도",
                                        "힘 재굴림", "지능 재굴림", "손재주 재굴림", "민첩 재굴림", "건강 재굴림", "의지 재굴림" };

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

        _img.enabled = true;   //툴팁 패널 이미지
        _icon_itemType.gameObject.SetActive(true); //아이템 타입 아이콘
        _itemName.gameObject.SetActive(true); //아이템 이름 텍스트

        if (_partition1 != null)
            _partition1.SetActive(true);   //툴팁 구분선 1
    }

    public void ItemTooltip_Off()
    {
        if (_equipSign != null)
            _equipSign.SetActive(false);

        _img.enabled = false;   //툴팁 패널 이미지
        _icon_itemType.gameObject.SetActive(false); //아이템 타입 아이콘
        _itemName.gameObject.SetActive(false); //아이템 이름 텍스트

        if (_partition1 != null)
            _partition1.SetActive(false);   //툴팁 구분선 1

        if (_stat1 != null)
            _stat1.gameObject.SetActive(false);     //스탯 1 표기
        if (_stat2 != null)
            _stat2.gameObject.SetActive(false);     //스탯 2 표기
        if (_actStat1 != null)
            _actStat1.gameObject.SetActive(false);  //행동 스탯 1 표기
        if (_actStat2 != null)
            _actStat2.gameObject.SetActive(false);  //행동 스탯 2 표기

        if (_act1 != null)
            _act1.SetActive(false); //행동1 표기
        if (_act2 != null)
            _act2.SetActive(false); //행동2 표기

        if (_ability != null)
            _ability.SetActive(false);  //능력 표기

        if (_partition2 != null)
            _partition2.SetActive(false);   //툴팁 구분선 2
        if (_partition3 != null)
            _partition3.SetActive(false);   //툴팁 구분선 3
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
            _partition2.SetActive(true);    //툴팁 구분선 2 활성화
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

            if (pos.x > Screen.width / 2)   //화면 우측
            {
                pivotX = 1f;
                transform.SetAsLastSibling();
            }
            else    //화면 좌측
            {
                pivotX = 0f;
                transform.SetAsFirstSibling();

                addX = 24f;     //아이콘 x크기만큼 x값 조작
            }

            if (pos.y > Screen.height * 2 / 3)  //화면 상단
            {
                _tooltipLayout.childAlignment = TextAnchor.UpperLeft;
                pivotY = 1f;
            }
            else if (pos.y <= Screen.height * 2 / 3 && pos.y > Screen.height * 1 / 3)   //화면 중단
            {
                _tooltipLayout.childAlignment = TextAnchor.MiddleLeft;
                pivotY = 0.5f;

                addY = -12f;    //아이콘 y크기의 절반만큼 y값 조작
            }
            else    //화면 하단
            {
                _tooltipLayout.childAlignment = TextAnchor.LowerLeft;
                pivotY = 0f;

                addY = -24f;    //아이콘 y크기만큼 y값 조작
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

            if (pos.x > Screen.width / 2)   //화면 중앙 기준 오른쪽 위치
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
