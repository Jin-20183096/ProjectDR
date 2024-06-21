using UnityEngine;
using UnityEngine.EventSystems;

public class DiceSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private DiceSelectPannel _diceSelectPannel;

    [SerializeField]
    private int _order;

    public void OnPointerEnter(PointerEventData eventData)
        => _diceSelectPannel.NowDice_MouseOver(_order); //주사위 슬롯에 마우스 오버

    public void OnPointerExit(PointerEventData eventData)
        => _diceSelectPannel.NowDice_MouseOver_End();   //주사위 슬롯에서 마우스 오버 종료

    public void Click_DiceSlot()
    {
        _diceSelectPannel.NowDice_Change(_order);       //order에 해당하는 주사위 슬롯 클릭
    }
}
