using UnityEngine;
using UnityEngine.EventSystems;
using static PlayerSystem;

public class ActionScreen : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private ActionScreenSlot[] _btlActSlot; //���� �ൿ ����� ����

    [SerializeField]
    private ActionTooltip _act_tooltip; //�����ൿ ����

    [SerializeField]
    private Sprite[] _spr_actType;      //�����ൿ Ÿ�� ������

    private string[] _statName_arr = { "", "��", "����", "������", "��ø", "����" };

    public void OnPointerClick(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }

    public void Change_BtlActList() //�÷��̾� �ý������κ���, �����ൿ ����� �Ҵ�޾� ����Ʈ ����
    {
        Debug.Log("�ൿ ��� ����");
        var list = PlayerSys.ActList;

        for (int i = 0; i < _btlActSlot.Length; i++)
        {
            if (i < list.Count)
            {
                _btlActSlot[i].gameObject.SetActive(true);

                _btlActSlot[i].Set_ActionSlotContent(_spr_actType[(int)list[i].Data.Type],
                                                    list[i].Data.Name,
                                                    _statName_arr[(int)list[i].Stat]);
            }
            else
                _btlActSlot[i].gameObject.SetActive(false);
        }
    }

    public void Set_ActionTooltipContent(int i) //�ൿ ���� ���� ����
    {
        var act = PlayerSys.ActList[i].Data;
        var stat = PlayerSys.ActList[i].Stat;

        _act_tooltip.Set_TooltipOutScreen();
        _act_tooltip.Tooltip_On(_spr_actType[(int)act.Type], act, _statName_arr[(int)stat]);
    }

    public void ActionTooltip_On(Vector3 vec, float x, float y) //�ൿ���� ���
    {
        _act_tooltip.Set_TooltipPosition(vec, x, y);
    }

    public void ActionTooltip_Off()
    {
        _act_tooltip.Tooltip_Off();
    }
}
