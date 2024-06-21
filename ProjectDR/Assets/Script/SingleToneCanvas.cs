using UnityEngine;

public class SingleToneCanvas : MonoBehaviour
{
    public static SingleToneCanvas STCanvas = null;

    public bool DRAG
    {
        get;
        private set;
    }
    public bool ITEM_DRAG
    {
        get;
        private set;
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

    public void SetDrag(bool b)
        => DRAG = b;

    public void SetItemDrag(bool b)
        => ITEM_DRAG = b;
}
