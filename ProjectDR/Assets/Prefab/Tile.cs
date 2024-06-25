using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    Color ColorUnknown = new Color32(0, 0, 0, 255);  //�� ���� ��
    Color ColorDark = new Color32(120, 120, 120, 255);  //�þ� ���� ���� ��
    Color ColorSight = new Color32(255, 255, 255, 255); //�þ� ���� ��

    [SerializeField]
    private DungeonSystem _dgnSys;

    [SerializeField]
    private SpriteRenderer _spr_tile;
    [SerializeField]
    private SpriteRenderer _spr_path;
    [SerializeField]
    private SpriteRenderer _spr_event;
    [SerializeField]
    private Animator _anima_event;

    [SerializeField]
    private bool _isExplored;   //Ÿ�� �߰� ����
    [SerializeField]
    private bool _isVisible;    //�þ� �� ����
    [SerializeField]
    private bool _isUpStair;    //���� ���
    [SerializeField]
    private bool _isDownStair;  //�Ʒ��� ���

    [SerializeField]
    private GameObject _stair;      //��� ������Ʈ
    [SerializeField]
    private GameObject _stair_dark; //�þ� �� ��� ������Ʈ

    public int X { get; private set; }
    public int Y { get; private set; }

    public List<Tile> NEIGHBOR { get; private set; }    //�ֺ� Ÿ�� ����Ʈ
    public Tile CONNECT { get; private set; }           //�̵� ��ο��� ����� Ÿ��

    public float G { get; private set; }
    public float H { get; private set; }
    public float F => G + H;

    private void OnEnable()
    {
        NEIGHBOR = new List<Tile>();

        SetVisible(false);
        EventSpriteOnOff(false);
    }

    //���ã�� �˰��� Ȱ���ϴ� �Լ���
    public void SetX(int x) => X = x;
    public void SetY(int y)
    {
        Y = y;

        if (_spr_event != null)
            _spr_event.gameObject.GetComponent<SortingGroup>().sortingOrder = Y * -1;
        if (_stair != null)
            _stair.gameObject.GetComponent<SortingGroup>().sortingOrder = Y * -1;
        if (_stair_dark != null)
            _stair_dark.gameObject.GetComponent<SortingGroup>().sortingOrder = Y * -1;
    }

    public void AddNeighbor(Tile tile) => NEIGHBOR.Add(tile);
    public void SetConnect(Tile tile) => CONNECT = tile;
    public void SetG(float g) => G = g;
    public void SetH(float h) => H = h;
    public int GetDistance(Tile other)  //�� Ÿ�Ͽ��� �ٸ� Ÿ�ϱ����� �̵� �Ÿ�
    {
        int x = X >= other.X ? (X - other.X) : (other.X - X);
        int y = Y >= other.Y ? (Y - other.Y) : (other.Y - Y);

        //x�� 0�� ��, y�� 0�̸�
        if (x == 0 && y == 0) return 0; //���� �ڽ��� ��ġ(�Ÿ�: 0)
        //x�� 0�� ��, y�� 0�� �ƴϸ�
        if (x == 0 && y != 0) return y; //���̸� �ٸ� Ÿ�� (y�� �Ÿ�)
        //x�� 0�� �ƴ� ��, y�� 0�̸�
        if (x != 0 && y == 0) return x / 2; //�¿� ��ġ�� �ٸ� Ÿ�� (x/2�� �Ÿ�)
        //�� ������ �ƴ� ���
        int distance = 0;
        if (x >= y)
        {
            //y���� �Ÿ��� ����
            distance += y;
            //y����ŭ x�� y����
            x -= y;
            y = 0;
            //(���� x�� / 2)�� �Ÿ��� ����
            distance += (x / 2);
        }
        else if (x < y)
        {
            //x���� �Ÿ��� ����
            distance += x;
            //x����ŭ x�� y�����
            y -= x;
            x = 0;
            //(���� y��)�� �Ÿ��� ����
            distance += y;
        }

        return distance;
    }
    ///////////////////////////

    public void SetDungeonSystem(DungeonSystem sys) => _dgnSys = sys;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isExplored || _isVisible)
            _dgnSys.TileMouseOver(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if ((_isExplored || _isVisible) && eventData.button == PointerEventData.InputButton.Left)
            _dgnSys.TileClick();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _dgnSys.TileMouseExit();
    }

    public void SetPathColor(Color color)   //�̵� ��� Ÿ�� �� ǥ��
    {
        _spr_path.enabled = true;
        _spr_path.color = color;
    }

    public void PathColorOff()  //�̵� ��� Ÿ�� ǥ�� off
    {
        _spr_path.color = new Color(1, 1, 1);
        _spr_path.enabled = false;
    }

    public void EventSpriteOnOff(bool b)    //�̺�Ʈ ��������Ʈ Ȱ��ȭ ����
    {
        if (_spr_event != null)
            _spr_event.enabled = b;
    }

    public void SetEventAnimation(RuntimeAnimatorController anima) //�̺�Ʈ �ִϸ��̼� ��Ʈ�ѷ� ����
    {
        if (_spr_event != null)
            _anima_event.runtimeAnimatorController = anima;
    }

    public void SetEventSpritePosition(bool isCenter, bool isLeft)  //�̺�Ʈ ��������Ʈ�� ��ġ ����
    {
        if (isCenter)
            _spr_event.transform.localPosition = new Vector3(0, 0, -1.25f);
        else
        {
            _spr_event.flipX = isLeft;

            if (isLeft)
                _spr_event.transform.localPosition = new Vector3(-1.25f, 0, -1.25f);
            else
                _spr_event.transform.localPosition = new Vector3(1.25f, 0, -1.25f);
        }
    }

    public bool GetExplored()
    {
        return _isExplored;
    }

    public void SetExplored(bool b) => _isExplored = b;

    public void SetVisible(bool b)
    {
        //�þ߿� ���� ���� �߰��� Ÿ�Ϸ� ó��
        if (b && _isExplored == false)
            SetExplored(true);

        _isVisible = b;

        if (b)
        {
            _spr_tile.color = ColorSight;

            if (_spr_event != null)
            {
                EventSpriteOnOff(true);
                _spr_event.color = ColorSight;
            }

            if (_stair != null)
            {
                _stair.SetActive(true);
                _stair_dark.SetActive(false);
            }
        }
        else
        {
            _spr_tile.color = (_isExplored ? ColorDark : ColorUnknown);
            if (_spr_event != null)
            {
                _spr_event.color = ColorUnknown;
                EventSpriteOnOff(_isExplored ? true : false);
            }
            if (_stair != null)
            {
                _stair_dark.SetActive(_isExplored ? true : false);
                _stair.SetActive(false);
            }
        }
    }

    //����� ȸ��
    public void RotateStair(int dir)
    {
        switch (dir)
        {
            case 0:
                if (_stair) _stair.transform.rotation = Quaternion.Euler(0, 120f, 0);
                if (_stair_dark) _stair_dark.transform.rotation = Quaternion.Euler(0, 120f, 0);
                break;
            case 1:
                if (_stair) _stair.transform.rotation = Quaternion.Euler(0, 180f, 0);
                if (_stair_dark) _stair_dark.transform.rotation = Quaternion.Euler(0, 180f, 0);
                break;
            case 2:
                if (_stair) _stair.transform.rotation = Quaternion.Euler(0, -120f, 0);
                if (_stair_dark) _stair_dark.transform.rotation = Quaternion.Euler(0, -120f, 0);
                break;
            case 3:
                if (_stair) _stair.transform.rotation = Quaternion.Euler(0, -60f, 0);
                if (_stair_dark) _stair_dark.transform.rotation = Quaternion.Euler(0, -60f, 0);
                break;
            case 4:
                if (_stair) _stair.transform.rotation = Quaternion.Euler(0, 0f, 0);
                if (_stair_dark) _stair_dark.transform.rotation = Quaternion.Euler(0, 0f, 0);
                break;
            case 5:
                if (_stair) _stair.transform.rotation = Quaternion.Euler(0, 60f, 0);
                if (_stair_dark) _stair_dark.transform.rotation = Quaternion.Euler(0, 60f, 0);
                break;
        }
    }
}
