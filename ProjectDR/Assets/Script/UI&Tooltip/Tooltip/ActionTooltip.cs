using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionTooltip : MonoBehaviour
{
    private Image _img;
    private RectTransform _rect;
    private Vector3 _out_vec;

    [SerializeField]
    private VerticalLayoutGroup _layout;

    [SerializeField]
    private Image _icon_actType;
    [SerializeField]
    private TextMeshProUGUI _txt_actName;
    [SerializeField]
    private TextMeshProUGUI _txt_actStat;

    [SerializeField]
    private GameObject[] _minDice;
    [SerializeField]
    private GameObject _hyphen;
    [SerializeField]
    private GameObject[] _maxDice;
    [SerializeField]
    private TextMeshProUGUI _txt_info;

    void Start()
    {
        _img = GetComponent<Image>();
        _rect = GetComponent<RectTransform>();
        _out_vec = _rect.transform.position;
    }

    public void Tooltip_On(Sprite icon, BtlActData act, string stat)
    {
        var data = act;

        _img.enabled = true;    //툴팁 이미지 온

        _icon_actType.gameObject.SetActive(true);
        _icon_actType.sprite = icon;    //행동 타입 아이콘

        _txt_actName.gameObject.SetActive(true);
        _txt_actName.text = data.Name;  //행동명 텍스트

        _txt_actStat.gameObject.SetActive(true);
        _txt_actStat.text = stat;       //행동 스탯 텍스트

        var min = data.DiceMin;
        var max = data.DiceMax;

        if (min != 0 && max != 0)
        {
            _hyphen.SetActive(min != max);  //최소 최대 주사위가 다를 때, 하이픈 활성화

            for (int i = 0; i < _minDice.Length; i++)
                _minDice[i].SetActive(i < min);

            if (min != max) //최소, 최대 주사위가 다를때
            {
                for (int i = 0; i < _maxDice.Length; i++)   //최대 주사위 표기 출력
                    _maxDice[i].SetActive(i < max);
            }
            else
            {
                for (int i = 0; i < _maxDice.Length; i++)   //최대 주사위 표기 off
                    _maxDice[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < _minDice.Length; i++)   //최소 주사위 표기 off
                _minDice[i].SetActive(false);

            for (int i = 0; i < _maxDice.Length; i++)   //최대 주사위 표기 off
                _maxDice[i].SetActive(false);
        }

        _txt_info.gameObject.SetActive(true);
        _txt_info.text = data.Info;

        Canvas.ForceUpdateCanvases();
        _layout.enabled = false;
        _layout.enabled = true;
    }

    public void Tooltip_Off()
    {
        _img.enabled = false;   //툴팁 이미지 off

        _icon_actType.gameObject.SetActive(false);
        _txt_actName.gameObject.SetActive(false);
        _txt_actStat.gameObject.SetActive(false);

        _hyphen.SetActive(false);

        for (int i = 0; i < _minDice.Length; i++)   //최소 주사위 표기
            _minDice[i].SetActive(false);

        for (int i = 0; i < _maxDice.Length; i++)   //최대 주사위 표기
            _maxDice[i].SetActive(false);

        _txt_info.gameObject.SetActive(false);
    }

    public void Set_TooltipOutScreen()
    {
        _rect.pivot = new Vector2(1, 1);
        transform.position = _out_vec;
    }

    public void Set_TooltipPosition(Vector3 slotPos, float slotX, float slotY)
    {
        Vector3 pos = slotPos;

        float pivotX;
        float pivotY;
        var addX = 0f;
        var addY = 0f;

        if (pos.x > Screen.width / 2)   //화면 오른쪽 위치
            pivotX = 1f;
        else                            //화면 왼쪽 위치
        {
            pivotX = 0f;
            addX = slotX;
        }

        if (pos.y > Screen.height * 2 / 3)  //화면 상단 위치
        {
            pivotY = 1f;
            addY = slotY / 2;
        }
        else if (pos.y <= Screen.height * 2 / 3 && pos.y > Screen.height * 1 / 3)   //화면 중단 위치
            pivotY = 0.5f;
        else    //화면 하단 위치
        {
            pivotY = 0f;
            addY = (slotY / 2) * -1;
        }

        _rect.pivot = new Vector2(pivotX, pivotY);
        transform.position = new Vector2(pos.x, pos.y);
        _rect.anchoredPosition = new Vector2(_rect.anchoredPosition.x + addX, _rect.anchoredPosition.y + addY);
    }
}
