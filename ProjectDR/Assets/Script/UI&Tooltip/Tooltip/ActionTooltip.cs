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

        _img.enabled = true;    //���� �̹��� ��

        _icon_actType.gameObject.SetActive(true);
        _icon_actType.sprite = icon;    //�ൿ Ÿ�� ������

        _txt_actName.gameObject.SetActive(true);
        _txt_actName.text = data.Name;  //�ൿ�� �ؽ�Ʈ

        _txt_actStat.gameObject.SetActive(true);
        _txt_actStat.text = stat;       //�ൿ ���� �ؽ�Ʈ

        var min = data.DiceMin;
        var max = data.DiceMax;

        if (min != 0 && max != 0)
        {
            _hyphen.SetActive(min != max);  //�ּ� �ִ� �ֻ����� �ٸ� ��, ������ Ȱ��ȭ

            for (int i = 0; i < _minDice.Length; i++)
                _minDice[i].SetActive(i < min);

            if (min != max) //�ּ�, �ִ� �ֻ����� �ٸ���
            {
                for (int i = 0; i < _maxDice.Length; i++)   //�ִ� �ֻ��� ǥ�� ���
                    _maxDice[i].SetActive(i < max);
            }
            else
            {
                for (int i = 0; i < _maxDice.Length; i++)   //�ִ� �ֻ��� ǥ�� off
                    _maxDice[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < _minDice.Length; i++)   //�ּ� �ֻ��� ǥ�� off
                _minDice[i].SetActive(false);

            for (int i = 0; i < _maxDice.Length; i++)   //�ִ� �ֻ��� ǥ�� off
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
        _img.enabled = false;   //���� �̹��� off

        _icon_actType.gameObject.SetActive(false);
        _txt_actName.gameObject.SetActive(false);
        _txt_actStat.gameObject.SetActive(false);

        _hyphen.SetActive(false);

        for (int i = 0; i < _minDice.Length; i++)   //�ּ� �ֻ��� ǥ��
            _minDice[i].SetActive(false);

        for (int i = 0; i < _maxDice.Length; i++)   //�ִ� �ֻ��� ǥ��
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

        if (pos.x > Screen.width / 2)   //ȭ�� ������ ��ġ
            pivotX = 1f;
        else                            //ȭ�� ���� ��ġ
        {
            pivotX = 0f;
            addX = slotX;
        }

        if (pos.y > Screen.height * 2 / 3)  //ȭ�� ��� ��ġ
        {
            pivotY = 1f;
            addY = slotY / 2;
        }
        else if (pos.y <= Screen.height * 2 / 3 && pos.y > Screen.height * 1 / 3)   //ȭ�� �ߴ� ��ġ
            pivotY = 0.5f;
        else    //ȭ�� �ϴ� ��ġ
        {
            pivotY = 0f;
            addY = (slotY / 2) * -1;
        }

        _rect.pivot = new Vector2(pivotX, pivotY);
        transform.position = new Vector2(pos.x, pos.y);
        _rect.anchoredPosition = new Vector2(_rect.anchoredPosition.x + addX, _rect.anchoredPosition.y + addY);
    }
}
