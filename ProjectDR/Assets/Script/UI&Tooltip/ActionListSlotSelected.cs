using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionListSlotSelected : MonoBehaviour, IActionListSlot
{
    [SerializeField]
    private ActionController _actController;
    [SerializeField]
    private int _order;

    [SerializeField]
    private Image _icon_actType;
    [SerializeField]
    private TextMeshProUGUI _txt_actName;
    [SerializeField]
    private TextMeshProUGUI _txt_actStat;

    [SerializeField]
    private RectTransform _rect_cursor;
    [SerializeField]
    private RectTransform _rect_slot;

    public void ClickSlot() //행동 슬롯 클릭 함수
    {
        _actController.ActionClick(_order);
    }

    public void Change_SlotContent(Sprite spr, string name, string stat)
    {
        _icon_actType.sprite = spr;
        _txt_actName.text = name;
        _txt_actStat.text = stat;
    }

    public float Get_AddX()
    {
        return _rect_cursor.sizeDelta.x + _rect_slot.sizeDelta.x;
    }

    public float Get_AddY(bool upSlot)
    {
        if (upSlot)
            return _rect_slot.sizeDelta.y / 2;
        else
            return (_rect_slot.sizeDelta.y / 2) * -1;
    }

    public int Get_SlotOrder()
    {
        return _order;
    }

    public void Set_SlotOrder(int order)
        => _order = order;
}
