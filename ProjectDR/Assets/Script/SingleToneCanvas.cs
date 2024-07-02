using UnityEngine;

public class SingleToneCanvas : MonoBehaviour
{
    public static SingleToneCanvas STCanvas = null;

    [SerializeField]
    private bool _drag;
    public bool DRAG
    {
        get { return _drag; }
        private set { _drag = value; }
    }
    [SerializeField]
    private bool _item_drag;
    public bool ITEM_DRAG
    {
        get { return _item_drag; }
        private set { _item_drag = value; }
    }

    private GameObject _drag_obj;
    public GameObject DRAG_OBJ
    {
        get { return _drag_obj; }
        private set { _drag_obj = value; }
    }

    private void Awake()
    {
        if (STCanvas)
        {
            DestroyImmediate(gameObject);
            return;
        }
        else
        {
            STCanvas = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Set_Drag(bool b)
        => DRAG = b;

    public void Set_ItemDrag(bool b)
        => ITEM_DRAG = b;

    public void Set_DragObj(GameObject obj)
    {
        DRAG_OBJ = obj;

        Set_Drag(obj != null);
    }
}
