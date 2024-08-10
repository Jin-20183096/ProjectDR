using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using static SingleToneCanvas;

public class ActionScreenSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private ActionScreen _actScreen;
    [SerializeField]
    private int _index;

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

    public void Set_ActionSlotContent(Sprite spr, string actName, int upgrade, string actStat)
    {
        _icon_actType.sprite = spr;

        string upgradeStr = "";
        if (upgrade > 0) upgradeStr = " +" + upgrade;

        _txt_actName.text = actName + upgradeStr;
        _txt_actStat.text = actStat;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (STCanvas.DRAG == false)
        {
            Set_SlotImage(true);

            _actScreen.Set_ActionTooltipContent(_index);

            StartCoroutine("Print_Tooltip");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (STCanvas.DRAG == false)
        {
            Set_SlotImage(false);

            _actScreen.ActionTooltip_Off();

            StopCoroutine("Print_Tooltip");
        }
    }

    IEnumerator Print_Tooltip()
    {
        yield return new WaitForSecondsRealtime(0.2f);

        //½½·Ô À§Ä¡¿¡ ÅøÆÁ Ãâ·Â
        _actScreen.ActionTooltip_On(transform.position, _rect.sizeDelta.x, _rect.sizeDelta.y);
    }

    public void Set_SlotImage(bool isMouseOn)
    {
        _img.sprite = _spr_slotImage[isMouseOn ? 1 : 0];
    }
}
