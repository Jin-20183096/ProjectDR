using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static SingleToneCanvas;

public class ScreenControl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private CanvasScaler _canvas;

    [SerializeField]
    private Vector2 _screenBound;

    [SerializeField]
    private RectTransform _rect;

    [SerializeField]
    private Image _controlBar;

    bool _pointer = false;

    void Start()
    {
        _screenBound = _canvas.referenceResolution;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (STCanvas.DRAG == false)
        {
            _rect.transform.SetAsLastSibling();
            STCanvas.Set_DragObj(gameObject);
            _controlBar.color = new Color32(255, 255, 255, 128);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (STCanvas.DRAG_OBJ == gameObject)
        {
            _rect.transform.position += (Vector3)eventData.delta;

            Vector3 pos = _rect.anchoredPosition;

            pos.x = Mathf.Clamp(pos.x, 0, _screenBound.x - _rect.sizeDelta.x);
            pos.y = Mathf.Clamp(pos.y, (_screenBound.y * -1) + _rect.sizeDelta.y, 0);

            _rect.anchoredPosition = pos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        STCanvas.Set_Drag(false);
        STCanvas.Set_DragObj(null);

        _controlBar.enabled = _pointer;
        _controlBar.color = new Color32(255, 255, 255, 64);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _pointer = true;
        _controlBar.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _pointer = false;

        if (STCanvas.DRAG_OBJ != gameObject)
            _controlBar.enabled = false;
    }
}
