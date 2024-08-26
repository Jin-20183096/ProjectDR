using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using static SingleToneCanvas;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    Color ColorUnknown = new Color32(0, 0, 0, 255);  //안 가본 색
    Color ColorDark = new Color32(80, 80, 80, 255);  //시야 밖의 가본 색
    Color ColorSightAround = new Color32(170, 170, 170, 255);   //가장자리 시야의 색
    Color ColorSight = new Color32(255, 255, 255, 255); //시야 내의 색

    [SerializeField]
    private DungeonSystem _dgnSys;

    [SerializeField]
    private SpriteRenderer _tile;
    [SerializeField]
    private SpriteRenderer _path;
    [SerializeField]
    private SpriteRenderer _eventSpr;
    [SerializeField]
    private Animator _anima_event;

    [SerializeField]
    private bool _isExplored;   //타일 발견 여부
    [SerializeField]
    private bool _isVisible;    //시야 내 여부
    [SerializeField]
    private bool _isUpStair;    //위층 계단
    [SerializeField]
    private bool _isDownStair;  //아래층 계단

    [SerializeField]
    private GameObject _stair;      //계단 오브젝트
    [SerializeField]
    private GameObject _stair_dark; //시야 밖 계단 오브젝트

    [SerializeField]
    private Sprite[] _spr_tile;

    public int X { get; private set; }
    public int Y { get; private set; }

    public List<Tile> NEIGHBOR { get; private set; }    //주변 타일 리스트
    public Tile CONNECT { get; private set; }           //이동 경로에서 연결된 타일

    public float G { get; private set; }
    public float H { get; private set; }
    public float F => G + H;

    private void OnEnable()
    {
        NEIGHBOR = new List<Tile>();

        SetVisible(false, false);
        SetTileSprite();
        EventSpriteOnOff(false);
    }

    //경로찾기 알고리즘에 활용하는 함수들
    public void SetX(int x) => X = x;
    public void SetY(int y)
    {
        Y = y;

        if (_eventSpr != null)
            _eventSpr.gameObject.GetComponent<SortingGroup>().sortingOrder = Y * -1;
        if (_stair != null)
            _stair.gameObject.GetComponent<SortingGroup>().sortingOrder = Y * -1;
        if (_stair_dark != null)
            _stair_dark.gameObject.GetComponent<SortingGroup>().sortingOrder = Y * -1;
    }

    public void AddNeighbor(Tile tile) => NEIGHBOR.Add(tile);
    public void SetConnect(Tile tile) => CONNECT = tile;
    public void SetG(float g) => G = g;
    public void SetH(float h) => H = h;
    public int GetDistance(Tile other)  //이 타일에서 다른 타일까지의 이동 거리
    {
        int x = X >= other.X ? (X - other.X) : (other.X - X);
        int y = Y >= other.Y ? (Y - other.Y) : (other.Y - Y);

        //x가 0일 때, y도 0이면
        if (x == 0 && y == 0) return 0; //현재 자신의 위치(거리: 0)
        //x가 0일 때, y는 0이 아니면
        if (x == 0 && y != 0) return y; //높이만 다른 타일 (y가 거리)
        //x가 0이 아닐 때, y는 0이면
        if (x != 0 && y == 0) return x / 2; //좌우 위치만 다른 타일 (x/2가 거리)
        //위 조건이 아닌 경우
        int distance = 0;
        if (x >= y)
        {
            //y값을 거리에 더함
            distance += y;
            //y값만큼 x와 y감소
            x -= y;
            y = 0;
            //(남은 x값 / 2)를 거리에 더함
            distance += (x / 2);
        }
        else if (x < y)
        {
            //x값을 거리에 더함
            distance += x;
            //x값만큼 x와 y감쇼ㅗ
            y -= x;
            x = 0;
            //(남은 y값)을 거리에 더함
            distance += y;
        }

        return distance;
    }
    ///////////////////////////

    public void SetDungeonSystem(DungeonSystem sys) => _dgnSys = sys;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (STCanvas.DRAG == false && (_isExplored || _isVisible))
            _dgnSys.TileMouseOver(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (STCanvas.DRAG == false && (_isExplored || _isVisible) && eventData.button == PointerEventData.InputButton.Left)
            _dgnSys.TileClick();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _dgnSys.TileMouseExit();
    }

    public void SetTileSprite() => _tile.sprite = _spr_tile[Random.Range(0, _spr_tile.Length)];

    public void SetPathColor(Color color)   //이동 경로 타일 색 표시
    {
        _path.enabled = true;
        _path.color = color;
    }

    public void PathColorOff()  //이동 경로 타일 표시 off
    {
        _path.color = new Color(1, 1, 1);
        _path.enabled = false;
    }

    public void EventSpriteOnOff(bool b)    //이벤트 스프라이트 활성화 여부
    {
        if (_eventSpr != null)
            _eventSpr.enabled = b;
    }

    public void SetEventAnimation(RuntimeAnimatorController anima) //이벤트 애니메이션 컨트롤러 설정
    {
        if (_eventSpr != null)
            _anima_event.runtimeAnimatorController = anima;
    }

    public void SetEventSpritePosition(bool isCenter, bool isLeft)  //이벤트 스프라이트의 위치 조정
    {
        if (isCenter)
            _eventSpr.transform.localPosition = new Vector3(0, 0, -1.25f);
        else
        {
            _eventSpr.flipX = isLeft;

            if (isLeft)
                _eventSpr.transform.localPosition = new Vector3(-1.25f, 0, -1.25f);
            else
                _eventSpr.transform.localPosition = new Vector3(1.25f, 0, -1.25f);
        }
    }

    public bool GetExplored()
    {
        return _isExplored;
    }

    public void SetExplored(bool b) => _isExplored = b;

    public void SetVisible(bool b, bool isCenter)
    {
        //시야에 들어온 순간 발견한 타일로 처리
        if (b && _isExplored == false)
            SetExplored(true);

        _isVisible = b;

        if (b)
        {
            if (isCenter)
                _tile.color = ColorSight;
            else
                _tile.color = ColorSightAround;

            if (_eventSpr != null)
            {
                EventSpriteOnOff(true);
                if (isCenter)
                    _eventSpr.color = ColorSight;
                else
                    _eventSpr.color = ColorSightAround;
            }

            if (_stair != null)
            {
                _stair.SetActive(true);
                _stair_dark.SetActive(false);
            }
        }
        else
        {
            _tile.color = (_isExplored ? ColorDark : ColorUnknown);
            if (_eventSpr != null)
            {
                _eventSpr.color = ColorUnknown;
                EventSpriteOnOff(_isExplored ? true : false);
            }
            if (_stair != null)
            {
                _stair_dark.SetActive(_isExplored ? true : false);
                _stair.SetActive(false);
            }
        }
    }

    //계단의 회전
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
