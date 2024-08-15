using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static HitBoxCollider;

public class SpriteSystem : MonoBehaviour
{
    [SerializeField]
    private BattleSystem _btlSys;

    public enum AtkMoveSet { No, Atk1, Atk2, Atk3 }
    public enum DefMoveSet { No, Def }
    public enum DgeMoveSet { No, Dge }
    public enum TacMoveSet { No, Tac }

    public enum CommonTrigger { Idle, Walk, Dmg }

    private SpriteRenderer _spr;
    private Animator _anima;
    private Rigidbody _rigid;

    [SerializeField]
    private HitBoxHost _hitBoxHost;
    [SerializeField]
    private bool _def_or_dge;
    [SerializeField]
    private HitBoxCollider _actHitBox;

    [Header("# Move Animation Relate")]
    private Vector3 _moveDest;  //�̵� ������ ��ǥ
    private Vector3 _homePos;   //�̵� �� ���� ��ǥ

    [Header("# Equip Sprite")]
    [SerializeField]
    private EquipSprite[] _equipSpr;
    private bool[] _isSomeEquip = new bool[5];

    [Header("# Material")]
    [SerializeField]
    private Material _defaultMat;
    [SerializeField]
    private Material _hitMat;

    private string _actTrigger;

    void Awake()
    {
        _spr = GetComponent<SpriteRenderer>();
        _anima = GetComponent<Animator>();
        _rigid = GetComponent<Rigidbody>();
    }

    void Start()
    {
        _rigid.isKinematic = true;
        _homePos = transform.position;
    }

    public void Flip_X(bool isLeft)     //��������Ʈ �¿� ����
    {
        _spr.flipX = isLeft;

        for (int i = 0; i < _equipSpr.Length; i++)
            _equipSpr[i].Flip_X(isLeft);
    }

    public void Set_CommonMoveSet(CommonTrigger trigger)
    {
        _anima.SetTrigger(trigger.ToString());
        
        for (int i = 0; i < _equipSpr.Length; i++)
        {
            _equipSpr[i].Renderer_OnOff(_isSomeEquip[i]);
            _equipSpr[i].Set_AnimaTrigger(trigger.ToString());
        }
    }

    public void Set_ActionMoveSet_Atk(AtkMoveSet atk, bool isTrue)
    {
        _anima.SetBool(atk.ToString(), isTrue);

        for (int i = 0; i < _equipSpr.Length; i++)
        {
            _equipSpr[i].Renderer_OnOff(_isSomeEquip[i]);
            _equipSpr[i].Set_AnimaBool(atk.ToString(), isTrue);
        }

        _actTrigger = atk.ToString();
    }

    public void Set_ActionMoveSet_Def(DefMoveSet def, bool isTrue)
    {
        _anima.SetBool(def.ToString(), isTrue);

        for (int i = 0; i < _equipSpr.Length; i++)
        {
            _equipSpr[i].Renderer_OnOff(_isSomeEquip[i]);
            _equipSpr[i].Set_AnimaBool(def.ToString(), isTrue);
        }

        _actTrigger = def.ToString();
    }

    public void Set_ActionMoveSet_Dge(DgeMoveSet dge, bool isTrue)
    {
        _anima.SetBool(dge.ToString(), isTrue);

        for (int i = 0; i < _equipSpr.Length; i++)
        {
            _equipSpr[i].Renderer_OnOff(_isSomeEquip[i]);
            _equipSpr[i].Set_AnimaBool(dge.ToString(), isTrue);
        }

        _actTrigger = dge.ToString();
    }

    public void Set_ActionMoveSet_Tac(TacMoveSet tac, bool isTrue)
    {
        _anima.SetBool(tac.ToString(), isTrue);

        for (int i = 0; i < _equipSpr.Length; i++)
        {
            _equipSpr[i].Renderer_OnOff(_isSomeEquip[i]);
            _equipSpr[i].Set_AnimaBool(tac.ToString(), isTrue);
        }

        _actTrigger = tac.ToString();
    }

    public void Set_ActionTrigger()
    {
        var trigger = _actTrigger + "Start";

        _anima.SetTrigger(trigger);

        for (int i = 0; i < _equipSpr.Length; i++)
        {
            _equipSpr[i].Renderer_OnOff(_isSomeEquip[i]);
            _equipSpr[i].Set_AnimaTrigger(trigger);
        }
    }

    public void Set_ActHitBox(HitBoxType type)
    {
        if (type == HitBoxType.Atk || type == HitBoxType.Def)
        {
            _actHitBox.Set_HitBox(type);
        }
        else
            _actHitBox.gameObject.SetActive(false);
    }

    public void ActHitBoxOn()
        => _actHitBox.gameObject.SetActive(true);

    public void Set_HitBoxState(bool def_or_dge)    //��Ʈ�ڽ��� ���/ȸ�� ���� ���� ����
        => _def_or_dge = def_or_dge;

    public void Set_SpriteMove(Vector3 dest) //��ǥ ��ǥ�� �̵�
    {
        Debug.Log((_hitBoxHost == HitBoxHost.Player ? "�÷��̾� " : "�� ") + "�̵� ����");
        _moveDest = dest;
        _rigid.isKinematic = false;

        StartCoroutine("Move_Coroutine");
    }

    public IEnumerator Move_Coroutine()    //�̵�
    {
        yield return new WaitForFixedUpdate();

        var dir = _moveDest - transform.position;

        _rigid.MovePosition(transform.position + dir * Time.deltaTime * 20f);

        if (Vector3.Distance(transform.position, _moveDest) <= 0.5f)
        {
            Set_ActionTrigger();
            StopCoroutine(Move_Coroutine());
        }
        else
            StartCoroutine(Move_Coroutine());
    }

    public IEnumerator Return_Coroutine()    //����ġ�� ����
    {
        yield return new WaitForFixedUpdate();

        var dir = _homePos - transform.position;

        _rigid.MovePosition(transform.position + dir * Time.deltaTime * 10f);

        if (Vector3.Distance(transform.position, _homePos) <= 0.0001f)
        {
            transform.position = _homePos;
            _rigid.isKinematic = true;
            StopCoroutine(Return_Coroutine());
        }
        else
            StartCoroutine(Return_Coroutine());
    }

    void OnTriggerEnter(Collider other)
    {
        HitBoxCollider hitBox;

        if (other.GetComponent<HitBoxCollider>())   //�浹�� ����� ��Ʈ�ڽ� �ݶ��̴��� ������ ���� ������ ����
            hitBox = other.GetComponent<HitBoxCollider>();
        else
            return;

        //�ڽ��� ��Ʈ�ڽ��̰ų� �浹�� ����� ���������� �ƴ� ��� ����
        if (hitBox.HOST == _hitBoxHost || hitBox.TYPE != HitBoxType.Atk)
            return;

        //������� �ʾ��� ���(�浹�� ����� ���� ����)
        if (_def_or_dge)
        {
            if (_actHitBox.gameObject.activeSelf && _actHitBox.TYPE == HitBoxType.Def)
                _btlSys.StartCoroutine(_btlSys.AtkDef(_hitBoxHost == HitBoxHost.Player));  //������ ����ϴ� �ڷ�ƾ
            else
                _btlSys.StartCoroutine(_btlSys.AtkDge(_hitBoxHost == HitBoxHost.Player));  //������ ȸ���ϴ� �ڷ�ƾ
        }
        else
        {
            _btlSys.StartCoroutine(_btlSys.AtkHit(_hitBoxHost == HitBoxHost.Enemy));       //������ �޴� �ڷ�ƾ
        }

        hitBox.HitBoxOff();
    }

    public void Change_Item(ItemData data, ItemData.ItemType type)  //��� ������ ����
    {
        //��� Ÿ�Կ� ���� �ش� equipSpr�� �ִϸ��̼� ��Ʈ�ѷ��� ��ü
        _equipSpr[(int)type].Change_Sprite(data);

        _isSomeEquip[(int)type] = (data != null);

        _equipSpr[(int)type].Renderer_OnOff(data != null);
    }

    public IEnumerator HitFlash()
    {
        _spr.material = _hitMat;

        for (int i = 0; i < _equipSpr.Length; i++)
            _equipSpr[i].StartCoroutine(_equipSpr[i].HitFlash());

        yield return new WaitForSecondsRealtime(0.125f);
        _spr.material = _defaultMat;
    }
}
