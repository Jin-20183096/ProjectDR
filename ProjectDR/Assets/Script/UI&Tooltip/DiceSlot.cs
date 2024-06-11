using UnityEngine;
using UnityEngine.EventSystems;

public class DiceSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private DiceSelectPannel _diceSelectPannel;

    [SerializeField]
    private int _order;

    public void OnPointerEnter(PointerEventData eventData)
        => _diceSelectPannel.NowDice_MouseOver(_order); //�ֻ��� ���Կ� ���콺 ����

    public void OnPointerExit(PointerEventData eventData)
        => _diceSelectPannel.NowDice_MouseOver_End();   //�ֻ��� ���Կ��� ���콺 ���� ����

    public void Click_DiceSlot()
    {
        _diceSelectPannel.NowDice_Change(_order);       //order�� �ش��ϴ� �ֻ��� ���� Ŭ��
    }
}
