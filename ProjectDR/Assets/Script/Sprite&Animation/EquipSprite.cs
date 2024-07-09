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
        if (data != null)   //아이템을 장착한 경우
        {
            Debug.Log("장착");
            _spr.color = Color.white;

            //_aoc에 연동될 클립들을 일일이 교체
            _aoc["Idle"] = data.Clip_Idle;
        }
        else    //아이템을 해제한 경우
        {
            Debug.Log("해제");
            _spr.color = noItemColor;

            //_aoc에 연동될 클립들을 일일이 null
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
