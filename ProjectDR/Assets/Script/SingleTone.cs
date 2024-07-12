using UnityEngine;

public class SingleTone : MonoBehaviour
{
    public static SingleTone ST = null;

    private void Awake()
    {
        if (ST)
        {
            DestroyImmediate(gameObject);
            return;
        }
        else
        {
            ST = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
