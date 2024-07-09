using UnityEngine;

public class EquipSprite : MonoBehaviour
{
    Color noItemColor = new Color(1, 1, 1, 0);

    SpriteRenderer _spr;
    Animator _anima;

    [SerializeField]
    private AnimatorOverrideController _aoc;

    void Awake()
    {
        _spr = GetComponent<SpriteRenderer>();
        _anima = GetComponent<Animator>();
    }

    void Start()
    {
        _spr.color = noItemColor;
    }

    public void Flip_X(bool isLeft)
        => _spr.flipX = isLeft;

    public void Change_Sprite(ItemData data)
    {
        if (data != null)   //�������� ������ ���
        {
            Debug.Log("����");
            _spr.color = Color.white;

            //_aoc�� ������ Ŭ������ ������ ��ü
            _aoc["Idle"] = data.Clip_Idle;
        }
        else    //�������� ������ ���
        {
            Debug.Log("����");
            _spr.color = noItemColor;

            //_aoc�� ������ Ŭ������ ������ null
            _aoc["Idle"] = null;
        }
    }

    public void Renderer_OnOff(bool b)
        => _spr.enabled = b;

    public void Set_AnimaTrigger(string trigger)
        => _anima.SetTrigger(trigger);

    public void Set_AnimaBool(string trigger, bool isTrue)
        => _anima.SetBool(trigger, isTrue);
}
