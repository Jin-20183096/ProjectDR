using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ItemSystem;
using static SingleToneCanvas;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField]
    private bool _isItemExist;  //�� ���Կ� �������� �ִ��� ����
    public bool EXIST
    {
        get { return _isItemExist; }
        set { _isItemExist = value; }
    }

    [SerializeField]
    private bool _isDragging;
    public bool DRAGGING    //�� ������ �巡�� ������ ����
    {
        set { _isDragging = value; }
    }

    [SerializeField]
    private ItemSlotType _slotType; //���� Ÿ��
    public ItemSlotType SLOT_TYPE
    {
        get { return _slotType; }
    }

    [SerializeField]
    private ItemData.ItemType _equipType;   //��� ������ ���, � ��� Ÿ������
    public ItemData.ItemType EQUIP_TYPE
    {
        get { return _equipType; }
    }

    [SerializeField]
    private int _slotIndex; //���� Ÿ�� �� �ε���

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
        if (_isItemExist && STCanvas.DRAG == false)    //�������� �����ϰ�, �巡�� ���� �ƴ� ��
        {
            ItemSys.ItemTooltip_On(_slotType, _slotIndex);  //���� �غ�
            StartCoroutine("Print_ItemTooltip");        //���� �ڷ�ƾ on
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isItemExist)   //�������� ������ ��
        {
            if (STCanvas.DRAG == false) //�巡�� ���� �ƴ� ��
                ItemSys.ItemTooltip_Off();  //���� off

            StopCoroutine("Print_ItemTooltip"); //���� �ڷ�ƾ ����
        }
    }

    IEnumerator Print_ItemTooltip()
    {
        yield return new WaitForSecondsRealtime(0.2f);

        ItemSys.Set_ItemTooltipPosition();  //���� ��� �Լ� ȣ��
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)          //���� ������ ��Ŭ�� �߻� ��
        {
            if (STCanvas.DRAG == false) //�巡�� ���� �ƴ� ���
            {
                if (_isItemExist)
                    ItemSys.Drag_Start(_slotType, _slotIndex);  //�巡�� ����
            }
            else
                ItemSys.Drag_Drop(_slotType, _slotIndex);       //�巡�� ���
        }
        else if (eventData.button == PointerEventData.InputButton.Right)    //���� ������ ��Ŭ�� �߻� ��
        {
            if (STCanvas.DRAG == false &&   //�巡�� ���� �ƴϰ�
                _isItemExist && _slotType != ItemSlotType.Equip)    //��� ������ �ƴ� ������ �������� ��Ŭ���� ���
            {
                //������ �ý��ۿ��� ������ ���� ��û
                ItemSys.Item_RightClick(_slotType, _slotIndex);
            }
        }
    }
}
