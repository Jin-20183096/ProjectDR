using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static SingleToneCanvas;

public class PlayerMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Sprite[] _spr_btn;

    [SerializeField]
    private bool _isActive;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (STCanvas.DRAG == false)
            GetComponent<Image>().sprite = _spr_btn[1];
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (STCanvas.DRAG == false && _isActive == false)
            GetComponent<Image>().sprite = _spr_btn[0];
    }

    public void Button_OnOff()
    {
        if (STCanvas.DRAG == false)
        {
            _isActive = !_isActive; //버튼 클릭할 때마다 활성화 상태 반전

            if (_isActive)
                //버튼 활성화 아이콘 표시
                GetComponent<Image>().sprite = _spr_btn[1];
            else
                //버튼 비활성화 아이콘 표시
                GetComponent<Image>().sprite = _spr_btn[0];
        }
    }
}
