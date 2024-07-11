using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using static PlayerSystem;
using static BattleSystem;

public class DungeonSystem : MonoBehaviour
{
    [SerializeField]
    private DungeonData _dgnData;

    [Header("# Camera")]
    [SerializeField]
    private GameObject _camera_dgn;

    [Header("# Dungeon Making")]
    [SerializeField]
    private int _tileMax;   //최대 타일 수
    [SerializeField]
    private int _tileMin;   //최소 타일 수

    [SerializeField]
    private int _tileCount;   //현재 타일 수 

    private float _tileGapX = 5.12f; //타일 간 가로 간격
    private float _tileGapY = 4.16f; //타일 간 세로 간격

    [SerializeField]
    private int _dgnSizeX = 79; //던전 가로 타일 최대 개수
    [SerializeField]
    private int _dgnSizeY = 39; //던전 세로 타일 최대 개수

    private Vector3 _posZero;
    private int _centerX;   //정중앙 타일 x
    private int _centerY;   //정중앙 타일 y

    private int[,] _dgn_grid;   //x, y 좌표로 해당 위치의 지형을 기록하는 구조체 (-1: 벽 / 0: 없음 / 1: 바닥)

    //던전 생성 시 활용되는 자료 구조와 변수들
    private Dictionary<int, Dictionary<int, Tile>> _tile_dic;           //위치(x, y)에 해당하는 타일을 저장하는 딕셔너리
    private Dictionary<int, Dictionary<int, Wall>> _wall_dic;           //위치(x, y)에 해당하는 벽을 저장하는 딕셔너리

    private Queue<Vector2Int> _tile_queue = new Queue<Vector2Int>();    //생성할 타일을 넣는 큐

    private List<Vector2Int> _tile_list = new List<Vector2Int>();       //현재 층의 타일 위치(x, y)를 기록하는 리스트
    private List<Vector2Int> _wall_list = new List<Vector2Int>();       //현재 층의 벽 위치(x, y)를 기록하는 리스트

    private bool _makeUpStair;      //위층 계단 생성 여부
    private bool _makeDownStair;    //아래층 계단 생성 여부
    private bool _makeDungeonEnd;   //던전 생성 종료 여부

    //타일 이동 시 활용하는 변수
    private List<Tile> _tempPath = new List<Tile>();    //마우스 오버로 생성된 임시 경로
    private List<Tile> _tilePath = new List<Tile>();    //이동 중인 경로

    private GameObject _mouseOverTile;  //현재 마우스 오버중인 타일 (언제든 마우스 오버 타일을 인식해, 경로 생성에 활용할 수 있도록)
    private bool _isForwarding;         //이동 경로로 나아가는 중 여부

    [SerializeField]
    private bool _evntProcess;
    public bool EVNT_PROCESS
    {
        get { return _evntProcess; }
        set { _evntProcess = value; }
    }

    enum SightDir { Up, Down, Left, Right }
    private List<Tile> _visibleTile = new List<Tile>(); //시야 내 타일

    [SerializeField]
    private Tile _tile_prefab;  //던전 타일 프리팹
    [SerializeField]
    private Wall _wall_prefab;  //던전 벽 프리팹
    [SerializeField]
    private Tile _upStair_prefab;   //위층 계단 프리팹
    [SerializeField]
    private Tile _downStair_prefab; //아래층 계단 프리팹

    //등장 이벤트 목록
    [SerializeField]
    private EventData[] _event_list;    //등장 이벤트 목록

    //등장 적 목록
    [SerializeField]
    private EnemyData[] _enemy_list;    //등장 적 목록

    void Awake()
    {
        _centerX = _dgnSizeX / 2;
        _centerY = _dgnSizeY / 2;

        _posZero = new Vector3((_tileGapX / 2) * _centerX * -1, 0, _tileGapY * _centerY * -1);

        _dgn_grid = new int[_dgnSizeX, _dgnSizeY];                      //x,y 좌표의 던전 지형 기록 구조체
        _tile_dic = new Dictionary<int, Dictionary<int, Tile>>();       //x,y 좌표의 타일 스크립트 구조체
        _wall_dic = new Dictionary<int, Dictionary<int, Wall>>();       //x,y 좌표의 벽 스크립트 구조체
        _tile_queue = new Queue<Vector2Int>();                          //생성할 타일을 넣는 큐

        _tempPath = new List<Tile>();       //마우스 오버로 생성된 이동 경로
        _tilePath = new List<Tile>();       //이동 경로
        _visibleTile = new List<Tile>();    //시야 내 타일

        //던전 생성
        if (_dgnData.DungeonMakeEnd == false)
            GenerateCenterTile();
    }

    void Update()
    {
        //큐에 타일이 존재하면, Dequeue 후 그 타일의 무작위 6방향에 타일 생성 시도
        //큐에 타일이 없을 때 타일이 최대치에 도달하지 않았다면, 현재 생성한 타일 리스트의 뒤에서부터 일정 부분을 큐에 다시 Enqueue
        if (_dgnData.DungeonMakeEnd == false)
        {
            if (_makeDungeonEnd == false)
            {
                if (_tile_queue.Count > 0)
                {
                    Vector2Int tile_index = _tile_queue.Dequeue();
                    int gridX = tile_index.x;
                    int gridY = tile_index.y;

                    GenerateTile(gridX + 1, gridY + 1); //12-1-2
                    GenerateTile(gridX + 2, gridY);     //2-3-4
                    GenerateTile(gridX + 1, gridY - 1); //4-5-6
                    GenerateTile(gridX - 1, gridY - 1); //6-7-8
                    GenerateTile(gridX - 2, gridY);     //8-9-10
                    GenerateTile(gridX - 1, gridY + 1); //10-11-12
                }
                else if (_tileCount < _tileMax) //생성한 타일이 최대 타일 수에 못 미칠 때
                {
                    int listCount = _tile_list.Count;

                    //최근 생성한 타일 5개를 큐에 Enqueue
                    //생성한 타일 개수가 5개 미만이면, 있는 만큼만 넣기
                    for (int i = listCount - 1; i >= (listCount < 5 ? 0 : listCount - 5); i--)
                        _tile_queue.Enqueue(_tile_list[i]);
                }
                else
                {
                    _makeDungeonEnd = true;

                    int x, y;
                    Tile tile;

                    for (int i = 0; i < _tile_list.Count; i++)  //타일 리스트를 돌면서, 해당 타일 근처 벽과 천장을 설정
                    {
                        Vector2Int vec = _tile_list[i];
                        x = vec.x;
                        y = vec.y;
                        tile = _tile_dic[x][y];

                        //6방향을 체크한 뒤, 인접 타일이 있으면 NEIGHBOR로 등록, 아니면 벽 세우기
                        //12-1-2 방향
                        if (x + 1 <= _dgnSizeX && y + 1 <= _dgnSizeY && _dgn_grid[x + 1, y + 1] == 1)
                            tile.AddNeighbor(_tile_dic[x + 1][y + 1]);
                        else
                        {
                            if (x + 1 <= _dgnSizeX + 1 && y + 1 <= _dgnSizeY + 1)
                            {
                                if (_dgn_grid[x + 1, y + 1] != -1)  //12-1-2시 방향에 천장이 없다면 생성
                                    GenerateWall(x + 1, y + 1);
                                _wall_dic[x + 1][y + 1].SetWallSprite(3);    //천장의 6-7-8에 벽 설정
                            }
                        }
                        //2-3-4 방향
                        if (x + 2 <= _dgnSizeX && _dgn_grid[x + 2, y] == 1)
                            tile.AddNeighbor(_tile_dic[x + 2][y]);
                        else
                        {
                            if (x + 2 <= _dgnSizeX + 1)
                            {
                                if (_dgn_grid[x + 2, y] != -1)      //2-3-4시 방향에 천장이 없다면 생성
                                    GenerateWall(x + 2, y);
                                _wall_dic[x + 2][y].SetWallSprite(4);        //천장의 8-9-10에 벽 설정
                            }
                        }
                        //4-5-6 방향
                        if (x + 1 <= _dgnSizeX && y > 1 && _dgn_grid[x + 1, y - 1] == 1)
                            tile.AddNeighbor(_tile_dic[x + 1][y - 1]);
                        else
                        {
                            if (x + 1 <= _dgnSizeX + 1 && y > 0)
                            {
                                if (_dgn_grid[x + 1, y - 1] != -1)  //4-5-6시 방향에 천장이 없다면 생성
                                    GenerateWall(x + 1, y - 1);
                                _wall_dic[x + 1][y - 1].SetWallSprite(5);    //천장의 10-11-12에 벽 설정
                            }
                        }
                        //6-7-8 방향
                        if (x > 1 && y > 1 && _dgn_grid[x - 1, y - 1] == 1)
                            tile.AddNeighbor(_tile_dic[x - 1][y - 1]);
                        else
                        {
                            if (x > 0 && y > 0)
                            {
                                if (_dgn_grid[x - 1, y - 1] != -1)   //6-7-8시 방향에 천장이 없다면 생성
                                    GenerateWall(x - 1, y - 1);
                                _wall_dic[x - 1][y - 1].SetWallSprite(0);    //천장의 12-1-2에 벽 설정
                            }
                        }
                        //8-9-10 방향
                        if (x > 2 && _dgn_grid[x - 2, y] == 1)
                            tile.AddNeighbor(_tile_dic[x - 2][y]);
                        else
                        {
                            if (x > 1)
                            {
                                if (_dgn_grid[x - 2, y] != -1)      //8-9-10시 방향에 천장이 없다면 생성
                                    GenerateWall(x - 2, y);
                                _wall_dic[x - 2][y].SetWallSprite(1);        //천장의 2-3-4에 벽 생성
                            }
                        }
                        //10-11-12 방향
                        if (x > 1 && y + 1 <= _dgnSizeY && _dgn_grid[x - 1, y + 1] == 1)
                            tile.AddNeighbor(_tile_dic[x - 1][y + 1]);
                        else
                        {
                            if (x > 0 && y + 1 <= _dgnSizeY + 1)
                            {
                                if (_dgn_grid[x - 1, y + 1] != -1)  //10-11-12시 방향에 천장이 없다면 생성
                                    GenerateWall(x - 1, y + 1);
                                _wall_dic[x - 1][y + 1].SetWallSprite(2);    //천장의 4-5-6에 벽 생성
                            }
                        }
                    }

                    //위층 계단 방향 설정
                    var ustX = _dgnData.UpStairVec.x;
                    var ustY = _dgnData.UpStairVec.y;
                    int ustDir;

                    List<int> ustDirList = new List<int>(); //계단이 향할 방향 후보군

                    if (ustX + 1 <= _dgnSizeX && ustY + 1 <= _dgnSizeY && _dgn_grid[ustX + 1, ustY + 1] == -1 &&
                        ustX > 1 && ustY > 1 && _dgn_grid[ustX - 1, ustY - 1] == 1)
                        ustDirList.Add(0);   //12-1-2 천장 인접 시 0 추가
                    if (ustX + 2 <= _dgnSizeX && _dgn_grid[ustX + 2, ustY] == -1 &&
                        ustX > 2 && _dgn_grid[ustX - 2, ustY] == 1)
                        ustDirList.Add(1);   //2-3-4 천장 인접 시 1 추가
                    if (ustX + 1 <= _dgnSizeX && ustY > 1 && _dgn_grid[ustX + 1, ustY - 1] == -1 &&
                        ustX > 1 && ustY + 1 <= _dgnSizeY && _dgn_grid[ustX - 1, ustY + 1] == 1)
                        ustDirList.Add(2);   //4-5-6 천장 인접 시 2 추가
                    if (ustX > 1 && ustY > 1 && _dgn_grid[ustX - 1, ustY - 1] == -1 &&
                        ustX + 1 <= _dgnSizeX && ustY + 1 <= _dgnSizeY && _dgn_grid[ustX + 1, ustY + 1] == 1)
                        ustDirList.Add(3);   //6-7-8 천장 인접 시 3 추가
                    if (ustX > 2 && _dgn_grid[ustX - 2, ustY] == -1 &&
                        ustX + 2 <= _dgnSizeX && _dgn_grid[ustX + 2, ustY] == 1)
                        ustDirList.Add(4);   //8-9-10 천장 인접 시 4 추가
                    if (ustX > 1 && ustY + 1 <= _dgnSizeY && _dgn_grid[ustX - 1, ustY + 1] == -1 &&
                        ustX + 1 <= _dgnSizeX && ustY > 1 && _dgn_grid[ustX + 1, ustY - 1] == 1)
                        ustDirList.Add(5);   //10-11-12 천장 인접 시 5 추가

                    if (ustDirList.Count >= 1)
                        ustDir = ustDirList[Random.Range(0, ustDirList.Count)];
                    else
                        ustDir = Random.Range(0, 6);

                    _dgnData.UpStairDir = ustDir;
                    _tile_dic[ustX][ustY].RotateStair(ustDir);
                    //아래층 계단 방향 설정
                    var dstX = _dgnData.DownStairVec.x;
                    var dstY = _dgnData.DownStairVec.y;
                    int dstDir;

                    List<int> ds_dir_list = new List<int>();  //계단이 향할 방향 후보군

                    if (dstX + 1 < _dgnSizeX && dstY + 1 < _dgnSizeY && _dgn_grid[dstX + 1, dstY + 1] == -1 &&
                        dstX >= 1 && dstY >= 1 && _dgn_grid[dstX - 1, dstY - 1] == 1)
                        ds_dir_list.Add(1);   //12-1-2 천장 인접 시 0 추가
                    if (dstX + 2 < _dgnSizeX && _dgn_grid[dstX + 2, dstY] == -1 &&
                        dstX >= 2 && _dgn_grid[dstX - 2, dstY] == 1)
                        ds_dir_list.Add(1);   //2-3-4 천장 인접 시 1 추가
                    if (dstX + 1 < _dgnSizeX && dstY >= 1 && _dgn_grid[dstX + 1, dstY - 1] == -1 &&
                        dstX >= 1 && dstY + 1 < _dgnSizeY && _dgn_grid[dstX - 1, dstY + 1] == 1)
                        ds_dir_list.Add(1);   //4-5-6 천장 인접 시 2 추가
                    if (dstX >= 1 && dstY >= 1 && _dgn_grid[dstX - 1, dstY - 1] == -1 &&
                        dstX + 1 < _dgnSizeX && dstY + 1 < _dgnSizeY && _dgn_grid[dstX + 1, dstY + 1] == 1)
                        ds_dir_list.Add(4);   //6-7-8 천장 인접 시 3 추가
                    if (dstX >= 2 && _dgn_grid[dstX - 2, dstY] == -1 &&
                        dstX + 2 < _dgnSizeX && _dgn_grid[dstX + 2, dstY] == 1)
                        ds_dir_list.Add(4);   //8-9-10 천장 인접 시 4 추가
                    if (dstX >= 1 && dstY + 1 < _dgnSizeY && _dgn_grid[dstX - 1, dstY + 1] == -1 &&
                        dstX + 1 < _dgnSizeX && dstY >= 1 && _dgn_grid[dstX + 1, dstY - 1] == 1)
                        ds_dir_list.Add(4);   //10-11-12 천장 인접 시 5 추가

                    if (ds_dir_list.Count >= 1)
                        dstDir = ds_dir_list[Random.Range(0, ds_dir_list.Count)];
                    else
                        dstDir = (Random.value > 0.5f ? 1 : 4);

                    _dgnData.DownStairDir = dstDir;
                    _tile_dic[dstX][dstY].RotateStair(dstDir);
                    ////////////////////
                    _tile_list.Remove(_dgnData.UpStairVec);
                    _tile_list.Remove(_dgnData.DownStairVec);

                    _dgnData.TileVec = _tile_list.ToList();
                    _tile_list.Clear();
                    _dgnData.WallVec = _wall_list.ToList();
                    _wall_list.Clear();
                    //_dgnData.DungeonMakeEnd = true;


                    //플레이어 위치 이동
                    var ustVec = _dgnData.UpStairVec;
                    PlayerSys.Move_PlayerPosition(_tile_dic[ustVec.x][ustVec.y].transform.position, false);
                    PlayerSys.X = ustVec.x;
                    PlayerSys.Y = ustVec.y;
                    //플레이어 시야를 재설정
                    ChangeVisibleTile();
                }
            }
        }
        else    //던전 데이터에서 데이터 불러오기
        {

        }
    }

    void GenerateCenterTile()   //중앙 타일 생성
    {
        //큐에 중앙 타일 Enqueue
        _tile_queue.Enqueue(new Vector2Int(_centerX, _centerY));
        _dgn_grid[_centerX, _centerY] = 1;
        _tileCount++;
        //중앙 타일 프리팹 생성
        var tile = Instantiate(_tile_prefab.transform);
        var tileScr = tile.GetComponent<Tile>();

        tile.position = GridIndex_ToPosition(_centerX, _centerY);
        tile.parent = transform.GetChild(0).transform;
        tile.name = _centerX + "." + _centerY;
        tileScr.SetX(_centerX);
        tileScr.SetY(_centerY);
        tileScr.SetDungeonSystem(this);
        //프리팹의 정보를 딕셔너리에 저장, 타일 리스트에 추가
        if (_tile_dic.ContainsKey(_centerX) == false)
            _tile_dic.Add(_centerX, new Dictionary<int, Tile>());

        _tile_dic[_centerX].Add(_centerY, tileScr);

        _tile_list.Add(new Vector2Int(_centerX, _centerY));
    }

    void GenerateTile(int x, int y) //타일 생성
    {
        if (_tileCount >= _tileMax) //타일 갯수가 최대 타일 개수에 도달하면 생성 중지
            return;
        if (x < 1 || x > _dgnSizeX || y < 1 || y > _dgnSizeY)    //타일 생성 좌표가 허용 좌표 밖이면 생성 X
            return;
        if (_dgn_grid[x, y] != 0)   //이미 타일이 존재하는 좌표라면 생성 X
            return;
        if (Random.value < 0.5f)    //일정 확률로 생성 X
            return;
        if (CountNeighborTile(x, y) > Random.Range(1, 4))   //인접 타일 갯수가 일정 이상이면 생성 X
            return;

        if (!_makeUpStair && _tileMax - _tileCount == _tileMax / 2)   //남은 타일 수가 최대 타일 수의 절반일 때, 위층 계단 생성
        {
            GenerateUpStair(x, y);
            return;
        }
        else if (!_makeDownStair && _tileMax - _tileCount == 1)   //남은 타일 수가 1개일 때, 아래층 계단 생성
        {
            GenerateDownStair(x, y);
            return;
        }

        //큐에 생성할 타일 넣기
        _tile_queue.Enqueue(new Vector2Int(x, y));
        _dgn_grid[x, y] = 1;
        _tileCount++;
        //타일 프리팹 생성
        var tile = Instantiate(_tile_prefab.transform);
        var tileScr = tile.GetComponent<Tile>();

        tile.position = GridIndex_ToPosition(x, y);
        tile.parent = transform.GetChild(0).transform;
        tile.name = x + "." + y;
        tileScr.SetX(x);
        tileScr.SetY(y);
        tileScr.SetDungeonSystem(this);
        //프리팹의 정보를 딕셔너리에 저장, 타일 리스트에 추가
        if (_tile_dic.ContainsKey(x) == false)
            _tile_dic.Add(x, new Dictionary<int, Tile>());

        _tile_dic[x].Add(y, tileScr);
        _tile_list.Add(new Vector2Int(x, y));

        //일정 확률로 타일에 이벤트 삽입
        if (Random.value < 0.4f)
        {
            var evnt = _event_list[Random.Range(0, _event_list.Length)];
            RuntimeAnimatorController anima;

            //던전 데이터의 이 좌표에 이벤트 데이터 기록
            if (_dgnData.TileEvent.ContainsKey(x) == false)
                _dgnData.TileEvent.Add(x, new Dictionary<int, EventData>());
            if (_dgnData.TileEvent.ContainsKey(y) == false)
                _dgnData.TileEvent[x].Add(y, evnt);
            else
                _dgnData.TileEvent[x][y] = evnt;

            var enemy = _enemy_list[Random.Range(0, _enemy_list.Length)];

            //던전 데이터의 이 좌표에 적 데이터 기록
            if (_dgnData.TileEnemy.ContainsKey(x) == false)
                _dgnData.TileEnemy.Add(x, new Dictionary<int, EnemyData>());
            _dgnData.TileEnemy[x].Add(y, enemy);

            anima = enemy.Anima_Ctrl;

            _tile_dic[x][y].SetEventAnimation(anima);
        }
    }

    void GenerateUpStair(int x, int y)  //위층 계단 생성
    {
        _dgn_grid[x, y] = 1;
        _tileCount++;
        _makeUpStair = true;
        //타일 프리팹 생성
        var tile = Instantiate(_upStair_prefab.transform);
        var tileScr = tile.GetComponent<Tile>();
        _dgnData.UpStairVec = new Vector2Int(x, y);  //던전 데이터에 계단 위치 기록

        tile.position = GridIndex_ToPosition(x, y);
        tile.parent = transform.GetChild(0).transform;
        tile.name = x + "." + y;
        tileScr.SetX(x);
        tileScr.SetY(y);
        tileScr.SetDungeonSystem(this);

        //프리팹의 정보를 딕셔너리에 저장, 타일리스트에 추가
        if (_tile_dic.ContainsKey(x) == false)
            _tile_dic.Add(x, new Dictionary<int, Tile>());

        _tile_dic[x].Add(y, tileScr);

        _tile_list.Add(new Vector2Int(x, y)); //천장 생성을 위해 임시로 리스트에 넣었다가 이후 삭제

        //계단 이벤트 삽입
    }

    void GenerateDownStair(int x, int y)    //아래층 계단 생성
    {
        _dgn_grid[x, y] = 1;
        _tileCount++;
        _makeDownStair = true;
        //타일 프리팹 생성
        var tile = Instantiate(_downStair_prefab.transform);
        var tileScr = tile.GetComponent<Tile>();
        _dgnData.DownStairVec = new Vector2Int(x, y);    //던전 데이터에 계단 위치 기록

        tile.position = GridIndex_ToPosition(x, y);
        tile.parent = transform.GetChild(0).transform;
        tile.name = x + "." + y;
        tileScr.SetX(x);
        tileScr.SetY(y);
        tileScr.SetDungeonSystem(this);

        //프리팹의 정보를 딕셔너리에 저장, 타일리스트에 추가
        if (_tile_dic.ContainsKey(x) == false)
            _tile_dic.Add(x, new Dictionary<int, Tile>());

        _tile_dic[x].Add(y, tileScr);

        _tile_list.Add(new Vector2Int(x, y)); //천장 생성을 위해 임시로 리스트에 넣었다가 이후 삭제

        //계단 이벤트 삽입
    }

    void GenerateWall(int x, int y) //벽 생성
    {
        if (x < 0 || x > _dgnSizeX + 1 || y < 0 || y > _dgnSizeY + 1) //타일 생성 좌표가 허용 좌표 밖이면 생성 X
            return;
        if (_dgn_grid[x, y] != 0)   //이미 타일이 존재하는 좌표면 생성 x
            return;

        _dgn_grid[x, y] = -1;
        //천장 프리팹 생성
        var ceiling = Instantiate(_wall_prefab.transform);
        var ceilingScr = ceiling.GetComponent<Wall>();

        ceiling.position = GridIndex_ToPosition(x, y);
        ceiling.parent = transform.GetChild(0).transform;
        ceiling.name = "c_" + x + "." + y;
        ceilingScr.SetY(y);
        //프리팹의 정보를 딕셔너리에 저장
        if (_wall_dic.ContainsKey(x) == false)
            _wall_dic.Add(x, new Dictionary<int, Wall>());

        _wall_dic[x].Add(y, ceilingScr);

        _wall_list.Add(new Vector2Int(x, y));
    }

    int CountNeighborTile(int gridX, int gridY) //인접한 타일 갯수 세기
    {
        int x = gridX;
        int y = gridY;
        int count = 0;

        if (x + 1 <= _dgnSizeX && y + 1 <= _dgnSizeY && _dgn_grid[x + 1, y + 1] == 1)
            count++;
        if (x + 2 <= _dgnSizeX && _dgn_grid[x + 2, y] == 1)
            count++;
        if (x + 1 <= _dgnSizeX && y > 1 && _dgn_grid[x + 1, y - 1] == 1)
            count++;
        if (x > 1 && y > 1 && _dgn_grid[x - 1, y - 1] == 1)
            count++;
        if (x > 2 && _dgn_grid[x - 2, y] == 1)
            count++;
        if (x > 1 && y + 1 <= _dgnSizeY && _dgn_grid[x - 1, y + 1] == 1)
            count++;

        return count;
    }

    Vector3 GridIndex_ToPosition(int x, int z)  //그리드 좌표를 실제 좌표로
    {
        //타일 간격에 따라, x와 y 좌표 계산
        var strX = (_posZero.x + x * _tileGapX / 2).ToString("0.00");
        var strZ = (_posZero.z + z * _tileGapY).ToString("0.00");

        float newX = float.Parse(strX);
        float newZ = float.Parse(strZ);

        return new Vector3(newX, 0, newZ);
    }

    public void TileMouseOver(Tile tile)
    {
        if (!_isForwarding)
        {
            if (_tile_dic[PlayerSys.X][PlayerSys.Y] != tile)
            {
                _tempPath.Clear();
                _tempPath = FindPath(_tile_dic[PlayerSys.X][PlayerSys.Y], tile).ToList();
            }
        }

        _mouseOverTile = tile.gameObject;
    }

    public void TileMouseExit()
    {
        if (_mouseOverTile != null && !_isForwarding)
        {
            foreach (Tile t in _tempPath)
                t.PathColorOff();

            _tile_dic[PlayerSys.X][PlayerSys.Y].PathColorOff();

            _tempPath.Clear();
        }

        _mouseOverTile = null;
    }

    public void TileClick()
    {
        if (!_isForwarding)
        {
            PathClear();

            _tilePath = _tempPath.ToList();
            _tempPath.Clear();

            foreach (Tile t in _tilePath)
                t.SetPathColor(new Color(0, 1, 0, 0.7f));

            _isForwarding = true;

            StartCoroutine("PathForward");
        }
    }

    IEnumerator PathForward()   //이동 경로를 따라 이동
    {
        if (_tilePath.Count <= 0)
        {
            yield return new WaitForSecondsRealtime(0.25f);

            _tile_dic[PlayerSys.X][PlayerSys.Y].PathColorOff();

            _isForwarding = false;

            //이동 종료 후 마우스 오버된 타일이 있다면 path보여줌
            if (_mouseOverTile != null)
                TileMouseOver(_mouseOverTile.GetComponent<Tile>());

            StopCoroutine("PathForward");
        }
        else
        {
            yield return new WaitForSecondsRealtime(0.25f);

            var preX = PlayerSys.X;
            var preY = PlayerSys.Y;
            if (_dgnData.TileEvent.ContainsKey(preX) && _dgnData.TileEvent[preX].ContainsKey(preY))
            {
                var preEvent = _dgnData.TileEvent[preX][preY];  //플레이어 이전 타일의 이벤트

                //이전 타일 이벤트 스프라이트 위치 재조정
                if (preEvent != null && preEvent.Event.Type != EventData.EventType.No)
                {
                    _tile_dic[preX][preY].SetEventSpritePosition(true, false);
                    PlayerSys.Set_PlayerLocalPos(true, false);
                }
            }

            var tile = _tilePath[_tilePath.Count - 1];  //다음 도착 타일
            var x = tile.X;
            var y = tile.Y;

            //플레이어 좌표 이동
            PlayerSys.Move_PlayerPosition(tile.transform.position, preX > x);
            PlayerSys.X = x;    //플레이어 x 좌표 갱신
            PlayerSys.Y = y;    //플레이어 y 좌표 갱신

            //이동 효과음

            //플레이어 시야 재설정
            ChangeVisibleTile();

            _tilePath.Remove(tile);         //도착한 타일을 경로에서 제거
            tile.CONNECT.PathColorOff();    //도착한 타일의 경로표시 제거

            //도착한 타일에 이벤트가 있을 경우, 실행
            EventData nowEvent = null;

            if (_dgnData.TileEvent.ContainsKey(x) && _dgnData.TileEvent[x].ContainsKey(y))
            {
                nowEvent = _dgnData.TileEvent[x][y];

                if (nowEvent != null && nowEvent.Event.Type != EventData.EventType.No)  //이벤트가 존재할 때
                {
                    //preX <= x일 때, 플레이어 왼쪽 / 이벤트 스프라이트 오른쪽에 위치
                    //preX > x일 때, 플레이어 오른쪽 / 이벤트 스프라이트 왼쪽에 위치
                    _tile_dic[x][y].SetEventSpritePosition(false, preX > x);
                    PlayerSys.Set_PlayerLocalPos(false, preX <= x);

                    yield return new WaitForSeconds(0.5f);

                    //이벤트 발생한 타일 근처의 타일 상태 정보를 넘겨주고 전투 배경에 적용
                    BtlSys.gameObject.SetActive(false);
                    BtlSys.gameObject.SetActive(true);
                    BtlSys.Set_BattleField(x + 1 < _dgnSizeX && y + 1 < _dgnSizeY && _dgn_grid[x + 1, y + 1] == -1 ? true : false,
                                            x + 2 < _dgnSizeX && y + 2 < _dgnSizeY && _dgn_grid[x + 2, y + 2] == -1 ? true : false,
                                            x + 2 < _dgnSizeX && _dgn_grid[x + 2, y] == -1 ? true : false,
                                            x + 3 < _dgnSizeX && y + 1 < _dgnSizeY && _dgn_grid[x + 3, y + 1] == -1 ? true : false,
                                            x + 1 < _dgnSizeX && y >= 1 && _dgn_grid[x + 1, y - 1] == -1 ? true : false,
                                            x >= 1 && y >= 1 && _dgn_grid[x - 1, y - 1] == -1 ? true : false,
                                            x >= 2 && _dgn_grid[x - 2, y] == -1 ? true : false,
                                            x >= 3 && y + 1 < _dgnSizeY && _dgn_grid[x - 3, y + 1] == -1 ? true : false,
                                            x >= 1 && y + 1 < _dgnSizeY && _dgn_grid[x - 1, y + 1] == -1 ? true : false,
                                            x >= 2 && y + 2 < _dgnSizeY && _dgn_grid[x - 2, y + 2] == -1 ? true : false,
                                            y + 2 < _dgnSizeY && _dgn_grid[x, y + 2] == -1 ? true : false);
                    
                    BtlSys.Record_DungeonScript(this, _camera_dgn); //이 스크립트와 던전 카메라를 할당해주기 위해 전달
                    BtlSys.BattleStart(_dgnData.TileEnemy[x][y]);   //해당 위치의 적과 전투 시작
                }

                PathClear();
            }

            yield return new WaitUntil(() => EVNT_PROCESS == false);

            StartCoroutine("PathForward");
        }
    }

    List<Tile> FindPath(Tile start, Tile target)    //시작 타일과 목표 타일간의 경로 찾기
    {
        var search_list = new List<Tile>() { start };
        var process_list = new List<Tile>();

        while (search_list.Count > 0)   //검색할 타일 목록에 타일이 남아있으면
        {
            var current = search_list[0];

            foreach (var t in search_list)
                if (t.F < current.F || t.F == current.F && t.H < current.H) //현재 타일 근처의 타일 중 경로가 될 타일 찾기
                    current = t;

            process_list.Add(current);
            search_list.Remove(current);

            if (current == target)  //다음 경로의 타일이 목표 타일인 경우
            {
                var now_path = target;  //경로를 찾기 위한 타일 커서 (시작은 목표 타일)
                var path = new List<Tile>();
                var count = 100;

                while (now_path != start)
                {
                    path.Add(now_path);
                    now_path = now_path.CONNECT;    //이 타일과 연결된 타일로 커서를 옮김
                    count--;
                    if (count < 0) throw new Exception();
                }

                foreach (var tile in path)
                    tile.SetPathColor(new Color(1, 1, 1, 0.7f));

                start.SetPathColor(new Color(1, 1, 1, 0.7f));

                return path;
            }

            //return 되지 않았을 경우, 아직 목표 타일을 찾지 못한것
            foreach (var neighbor in current.NEIGHBOR.Where(t => !process_list.Contains(t) && t.GetExplored()))
            //처리되지 않은 타일 중 발견했던 타일만 순회
            {
                var inSearch = search_list.Contains(neighbor);  //이 타일이 검색 리스트에 포함되있는지 판단
                var costToNeighbor = current.G + current.GetDistance(neighbor); //이 타일로 가는 거리
                                                                                // = 현재까지의 거리 + (현재 타일과 인접 타일까지의 거리)
                if (!inSearch || costToNeighbor < neighbor.G)   //이 타일로 가는 거리가 더 짧다는 것을 알아냈다면
                {
                    neighbor.SetG(costToNeighbor);  //거리 수치 변경
                    neighbor.SetConnect(current);   //이전 타일과 이 타일을 연결

                    if (!inSearch)
                    {
                        neighbor.SetH(neighbor.GetDistance(target));    //이 타일에서 도착 타일까지 가는 거리를 계산
                        search_list.Add(neighbor);
                    }
                }
            }
        }
        return null;
    }

    void PathClear()
    {
        //StopCoroutine("PathForward");

        foreach (Tile t in _tilePath)
            t.PathColorOff();

        _tilePath.Clear();
    }

    void ChangeVisibleTile()
    {
        var visibleTile_prev = _visibleTile.ToList();

        var x = PlayerSys.X;
        var y = PlayerSys.Y;
        var s = PlayerSys.SIGHT;

        _visibleTile.Clear();   //현재 시야 초기화

        if (_dgn_grid[x, y] != 0)
        {
            _tile_dic[x][y].SetVisible(true, true);   //플레이어가 위치한 타일 시야에 들어옴
            _visibleTile.Add(_tile_dic[x][y]);  //플레이어가 위치한 타일을 시야 목록에 추가
        }

        //시야값 SIGHT에 따라 보이는 타일을 체크하는 재귀함수실행
        CheckSight(s - 1, SightDir.Left, x - 2, y);
        CheckSight(s - 1, SightDir.Right, x + 2, y);
        CheckSight(s - 1, SightDir.Up, x + 1, y + 1);
        CheckSight(s - 1, SightDir.Up, x - 1, y + 1);
        CheckSight(s - 1, SightDir.Down, x + 1, y - 1);
        CheckSight(s - 1, SightDir.Down, x - 1, y - 1);
        //

        //플레이어 1타일 이내 4-5-6-/6-7-8 방향 벽은 반투명화
        if (_dgn_grid[x + 1, y - 1] == -1)
            _wall_dic[x + 1][y - 1].InvinsibleWall(true);
        if (_dgn_grid[x - 1, y - 1] == -1)
            _wall_dic[x - 1][y - 1].InvinsibleWall(true);

        IEnumerable<Tile> outSight = visibleTile_prev.Except(_visibleTile); //시야에서 벗어난 타일 목록

        foreach (var t in outSight)
        {
            t.SetVisible(false, false);

            var tx = t.X;
            var ty = t.Y;

            //t의 12-1-2 방향의 천장의 벽이 발견된 상태면, visible false
            if (_dgn_grid[tx + 1, ty + 1] == -1 && _wall_dic[tx + 1][ty + 1].GetVisible(3))
                _wall_dic[tx + 1][ty + 1].SetVisible(3, false, false);
            //t의 2-3-4 방향의 천장의 벽이 발견된 상태면, visible false
            if (_dgn_grid[tx + 2, ty] == -1 && _wall_dic[tx + 2][ty].GetVisible(4))
                _wall_dic[tx + 2][ty].SetVisible(4, false, false);
            //t의 4-5-6 방향의 천장의 벽이 발견된 상태면, visible false
            if (_dgn_grid[tx + 1, ty - 1] == -1 && _wall_dic[tx + 1][ty - 1].GetVisible(5))
                _wall_dic[tx + 1][ty - 1].SetVisible(5, false, false);
            //t의 6-7-8 방향의 천장의 벽이 발견된 상태면, visible false
            if (_dgn_grid[tx - 1, ty - 1] == -1 && _wall_dic[tx - 1][ty - 1].GetVisible(0))
                _wall_dic[tx - 1][ty - 1].SetVisible(0, false, false);
            //t의 8-9-10 방향의 천장의 벽이 발견된 상태면, visible false
            if (_dgn_grid[tx - 2, ty] == -1 && _wall_dic[tx - 2][ty].GetVisible(1))
                _wall_dic[tx - 2][ty].SetVisible(1, false, false);
            //t의 10-11-12 방향의 천장의 벽이 발견된 상태면, visible false
            if (_dgn_grid[tx - 1, ty + 1] == -1 && _wall_dic[tx - 1][ty + 1].GetVisible(2))
                _wall_dic[tx - 1][ty + 1].SetVisible(2, false, false);
        }
    }

    void CheckSight(int s, SightDir dir, int x, int y)
    {
        //s가 0이면 종료
        if (s < 0)
            return;

        var gridValue = _dgn_grid[x, y];

        //시야가 도달한 좌표에 뭔가 존재할 때
        if (gridValue != 0)
        {
            if (gridValue == 1) //뭔가가 타일일 때
            {
                var tile = _tile_dic[x][y];

                tile.SetExplored(true); //발견처리
                tile.SetVisible(s > 0, false); //1 시야가 닿은 곳은 자세히 보이진 않음

                if (s > 0 && !_visibleTile.Contains(tile))
                    _visibleTile.Add(tile); //시야 목록에 없는 타일이면 추가

                //tile의 12-1-2 방향의 천장의 벽도 발견
                if (_dgn_grid[x + 1, y + 1] == -1)
                    _wall_dic[x + 1][y + 1].SetExplored(3, true);
                //tile의 12-1-2 방향의 천장의 벽도 발견
                if (_dgn_grid[x + 2, y] == -1)
                    _wall_dic[x + 2][y].SetExplored(4, true);
                //tile의 12-1-2 방향의 천장의 벽도 발견
                if (_dgn_grid[x + 1, y - 1] == -1)
                    _wall_dic[x + 1][y - 1].SetExplored(5, true);
                //tile의 12-1-2 방향의 천장의 벽도 발견
                if (_dgn_grid[x - 1, y - 1] == -1)
                    _wall_dic[x - 1][y - 1].SetExplored(0, true);
                //tile의 12-1-2 방향의 천장의 벽도 발견
                if (_dgn_grid[x - 2, y] == -1)
                    _wall_dic[x - 2][y].SetExplored(1, true);
                //tile의 12-1-2 방향의 천장의 벽도 발견
                if (_dgn_grid[x - 1, y + 1] == -1)
                    _wall_dic[x - 1][y + 1].SetExplored(2, true);
            }
            else if (gridValue == -1)   //뭔가가 벽이라면
            {
                var wall = _wall_dic[x][y];

                //우선 그 벽의 반투명화를 취소
                wall.InvinsibleWall(false);

                Debug.Log("[" + x + ", " + y + "]: " + s);

                //빛 방향에 따라 특정 벽의 visible, explored를 조작
                if (_dgn_grid[x + 1, y + 1] == 1 && _tile_dic[x + 1][y + 1].GetExplored() &&
                    (dir == SightDir.Down || dir == SightDir.Left))  //12-1-2 방향에 발견한 타일이 존재할 경우, 
                {
                    wall.SetExplored(0, true);
                    wall.SetVisible(0, (s > 0), s == PlayerSys.SIGHT - 1);
                }
                if (_dgn_grid[x + 2, y] == 1 && _tile_dic[x + 2][y].GetExplored() &&
                    (dir == SightDir.Left))    //2-3-4 방향에 발견된 타일이 존재할 경우,
                {
                    wall.SetExplored(1, true);
                    wall.SetVisible(1, (s > 0), s == PlayerSys.SIGHT - 1);
                }
                if (_dgn_grid[x + 1, y - 1] == 1 && _tile_dic[x + 1][y - 1].GetExplored() &&
                    (dir == SightDir.Left || dir == SightDir.Up))    //4-5-6 방향에 발견된 타일이 존재할 경우,
                {
                    wall.SetExplored(2, true);
                    wall.SetVisible(2, (s > 0), s == PlayerSys.SIGHT - 1);
                }
                if (_dgn_grid[x - 1, y - 1] == 1 && _tile_dic[x - 1][y - 1].GetExplored() &&
                    (dir == SightDir.Up || dir == SightDir.Right))    //6-7-8 방향에 발견된 타일이 존재할 경우,
                {
                    wall.SetExplored(3, true);
                    wall.SetVisible(3, (s > 0), s == PlayerSys.SIGHT - 1);
                }
                if (_dgn_grid[x - 2, y] == 1 && _tile_dic[x - 2][y].GetExplored() &&
                    (dir == SightDir.Right))    //8-9-10 방향에 발견된 타일이 존재할 경우,
                {
                    wall.SetExplored(4, true);
                    wall.SetVisible(4, (s > 0), s == PlayerSys.SIGHT - 1);
                }
                if (_dgn_grid[x - 1, y + 1] == 1 && _tile_dic[x - 1][y + 1].GetExplored() &&
                    (dir == SightDir.Right || dir == SightDir.Down))    //10-11-12 방향에 발견된 타일이 존재할 경우,
                {
                    wall.SetExplored(5, true);
                    wall.SetVisible(5, (s > 0), s == PlayerSys.SIGHT - 1);
                }

                //타일이 아닌 벽에 시야가 닿으면 시야 막힘 (재귀 종료)
                return;
            }
        }

        //재귀 종료되지 않았을 경우,
        //시야 각도에 따라 특정 x, y값으로 재귀 실행 (재귀 시 s 1 감소)
        switch (dir)
        {
            case SightDir.Up:       //빛 각도: Up  => (x-1, y+1로 재귀 / x+1, y+1로 재귀)
                CheckSight(s - 1, dir, x - 1, y + 1);
                CheckSight(s - 1, dir, x + 1, y + 1);
                break;
            case SightDir.Down:     //빛 각도: Down => (x-1, y-1로 재귀 / x+1, y-1로 재귀)
                CheckSight(s - 1, dir, x - 1, y - 1);
                CheckSight(s - 1, dir, x + 1, y - 1);
                break;
            case SightDir.Left:     //빛 각도: Left => (x-1, y+1로 재귀 / x-2, y로 재귀 / x-1, y-1로 재귀)
                CheckSight(s - 1, dir, x - 1, y + 1);
                CheckSight(s - 1, dir, x - 2, y);
                CheckSight(s - 1, dir, x - 1, y - 1);
                break;
            case SightDir.Right:    //빛 각도: Right => (x+1, y+1로 재귀 / x+2, y로 재귀 / x+1, y-1로 재귀)
                CheckSight(s - 1, dir, x + 1, y + 1);
                CheckSight(s - 1, dir, x + 2, y);
                CheckSight(s - 1, dir, x + 1, y - 1);
                break;
        }
    }
}
