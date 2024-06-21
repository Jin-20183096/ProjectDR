using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ActionController;

public class ActionListSlot : MonoBehaviour, IActionListSlot, IPointerEnterHandler, IPointerExitHandler
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

    private RectTransform _rect;
    private Image _img;

    [SerializeField]
    private Sprite[] _spr_slotImage;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _img = GetComponent<Image>();
    }

    public void ClickSlot() //�ൿ ���� Ŭ�� �Լ�
    {
        SetSlotImage(false);
        _actController.ActionClick(_order);
    }

    public void Change_SlotContent(Sprite spr, string name, string stat)
    {
        _icon_actType.sprite = spr;
        _txt_actName.text = name;
        _txt_actStat.text = stat;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetSlotImage(true);

        if (_actController.SITUATION == Situation.Battle)
        {
            _actController.Set_ActionTooltipContent(_order);
            StartCoroutine("PrintTooltip");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetSlotImage(false);

        if (_actController.SITUATION == Situation.Battle)
        {
            _actController.ActionTooltip_Off();

            StopCoroutine("PrintTooltip");
        }
    }

    IEnumerator PrintTooltip()
    {
        yield return new WaitForSecondsRealtime(0.2f);

        //���� ��ġ�� �ൿ ���� ���
        _actController.ActionTooltip_On(transform.position, _rect.sizeDelta.x, _rect.sizeDelta.y);
    }

    public int Get_SlotOrder()
    {
        return _order;
    }

    public void SetSlotImage(bool isMouseOn)
    {
        _img.sprite = _spr_slotImage[isMouseOn ? 1 : 0];
    }
}
