using UnityEngine;

public class HitBoxCollider : MonoBehaviour
{
    public enum HitBoxHost { Player, Enemy }
    public enum HitBoxType { No, Atk, Def }

    [SerializeField]
    private HitBoxHost _host;
    public HitBoxHost HOST
    {
        get { return _host; }
    }

    [SerializeField]
    private HitBoxType _hitBoxType;
    public HitBoxType TYPE
    {
        get { return _hitBoxType; }
    }

    public void Set_HitBox(HitBoxType type)
        => _hitBoxType = type;

    public void HitBoxOff()
        => gameObject.SetActive(false);
}
