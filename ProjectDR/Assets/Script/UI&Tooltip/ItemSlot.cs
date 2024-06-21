using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ItemSystem;
using static SingleToneCanvas;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField]
    private bool _isItemExist;  //이 슬롯에 아이템이 있는지 여부
    public bool EXIST
    {
        get { return _isItemExist; }
        set { _isItemExist = value; }
    }

    [SerializeField]
    private bool _isDragging;
    public bool DRAGGING    //이 슬롯이 드래그 중인지 여부
    {
        set { _isDragging = value; }
    }

    [SerializeField]
    private ItemSlotType _slotType; //슬롯 타입
    public ItemSlotType SLOT_TYPE
    {
        get { return _slotType; }
    }

    [SerializeField]
    private ItemData.ItemType _equipType;   //장비 슬롯인 경우, 어떤 장비 타입인지
    public ItemData.ItemType EQUIP_TYPE
    {
        get { return _equipType; }
    }

    [SerializeField]
    private int _slotIndex; //슬롯 타입 내 인덱스

    private Image _img;

    void Awake()
    {
        _img = GetComponent<Image>();
    }

    public void Change_SlotIcon(Sprite spr)
    {
        _img.sprite = spr;

        if (_isDragging)
            Change_SlotIconAlpha(0.5f);
        else
            Change_SlotIconAlpha(1f);
    }

    public void Change_SlotIconAlpha(float f)
        => _img.color = new Color(1, 1, 1, f);

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isItemExist && STCanvas.DRAG == false)    //아이템이 존재하고, 드래그 중이 아닐 때
        {
            //툴팁 준비
            //툴팁 코루틴 on
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isItemExist)   //아이템이 존재할 때
        {
            if (STCanvas.DRAG == false) //드래그 중이 아닐 때
            {
                //툴팁 off
            }

            //툴팁 코루틴 정지
        }
    }

    IEnumerator Print_ItemTooltip()
    {
        yield return new WaitForSecondsRealtime(0.2f);

        //툴팁 출력 함수 호출
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)          //슬롯 위에서 좌클릭 발생 시
        {
            if (STCanvas.DRAG == false) //드래그 중이 아닌 경우
            {
                if (_isItemExist)
                {
                    //아이템 시스템에서 드래그 시작 요청
                }
            }
            else
            {
                //아이템 시스템에서 드래그 드롭 이벤트 실행
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)    //슬롯 위에서 우클릭 발생 시
        {
            if (STCanvas.DRAG == false) //드래그 중이 아니며
            {
                //아이템 시스템에서 아이템 장착 요청
            }
        }
    }
}
