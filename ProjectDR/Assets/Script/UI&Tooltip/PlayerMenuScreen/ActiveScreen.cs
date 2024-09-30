using UnityEngine;
using UnityEngine.EventSystems;
using static PlayerSystem;

public class ActionScreen : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private ActionScreenSlot[] _btlActSlot; //전투 행동 목록의 슬롯

    [SerializeField]
    private ActionTooltip _act_tooltip; //전투행동 툴팁

    [SerializeField]
    private Sprite[] _spr_actType;      //전투행동 타입 아이콘

    private string[] _statName_arr = { "", "힘", "지능", "손재주", "민첩", "의지" };

    public void OnPointerClick(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }

    public void Change_BtlActList() //플레이어 시스템으로부터, 전투행동 목록을 할당받아 리스트 갱신
    {
        Debug.Log("행동 목록 갱신");
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

    public void Set_ActionTooltipContent(int i) //행동 툴팁 내용 설정
    {
        var act = PlayerSys.ActList[i].Data;
        var stat = PlayerSys.ActList[i].Stat;

        _act_tooltip.Set_TooltipOutScreen();
        _act_tooltip.Tooltip_On(_spr_actType[(int)act.Type], act, _statName_arr[(int)stat]);
    }

    public void ActionTooltip_On(Vector3 vec, float x, float y) //행동툴팁 출력
    {
        _act_tooltip.Set_TooltipPosition(vec, x, y);
    }

    public void ActionTooltip_Off()
    {
        _act_tooltip.Tooltip_Off();
    }
}
