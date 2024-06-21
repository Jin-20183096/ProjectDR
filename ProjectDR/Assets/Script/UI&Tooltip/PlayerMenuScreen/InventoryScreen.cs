using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryScreen : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }
}
