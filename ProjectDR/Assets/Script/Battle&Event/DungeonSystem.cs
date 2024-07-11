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
    private int _tileMax;   //�ִ� Ÿ�� ��
    [SerializeField]
    private int _tileMin;   //�ּ� Ÿ�� ��

    [SerializeField]
    private int _tileCount;   //���� Ÿ�� �� 

    private float _tileGapX = 5.12f; //Ÿ�� �� ���� ����
    private float _tileGapY = 4.16f; //Ÿ�� �� ���� ����

    [SerializeField]
    private int _dgnSizeX = 79; //���� ���� Ÿ�� �ִ� ����
    [SerializeField]
    private int _dgnSizeY = 39; //���� ���� Ÿ�� �ִ� ����

    private Vector3 _posZero;
    private int _centerX;   //���߾� Ÿ�� x
    private int _centerY;   //���߾� Ÿ�� y

    private int[,] _dgn_grid;   //x, y ��ǥ�� �ش� ��ġ�� ������ ����ϴ� ����ü (-1: �� / 0: ���� / 1: �ٴ�)

    //���� ���� �� Ȱ��Ǵ� �ڷ� ������ ������
    private Dictionary<int, Dictionary<int, Tile>> _tile_dic;           //��ġ(x, y)�� �ش��ϴ� Ÿ���� �����ϴ� ��ųʸ�
    private Dictionary<int, Dictionary<int, Wall>> _wall_dic;           //��ġ(x, y)�� �ش��ϴ� ���� �����ϴ� ��ųʸ�

    private Queue<Vector2Int> _tile_queue = new Queue<Vector2Int>();    //������ Ÿ���� �ִ� ť

    private List<Vector2Int> _tile_list = new List<Vector2Int>();       //���� ���� Ÿ�� ��ġ(x, y)�� ����ϴ� ����Ʈ
    private List<Vector2Int> _wall_list = new List<Vector2Int>();       //���� ���� �� ��ġ(x, y)�� ����ϴ� ����Ʈ

    private bool _makeUpStair;      //���� ��� ���� ����
    private bool _makeDownStair;    //�Ʒ��� ��� ���� ����
    private bool _makeDungeonEnd;   //���� ���� ���� ����

    //Ÿ�� �̵� �� Ȱ���ϴ� ����
    private List<Tile> _tempPath = new List<Tile>();    //���콺 ������ ������ �ӽ� ���
    private List<Tile> _tilePath = new List<Tile>();    //�̵� ���� ���

    private GameObject _mouseOverTile;  //���� ���콺 �������� Ÿ�� (������ ���콺 ���� Ÿ���� �ν���, ��� ������ Ȱ���� �� �ֵ���)
    private bool _isForwarding;         //�̵� ��η� ���ư��� �� ����

    [SerializeField]
    private bool _evntProcess;
    public bool EVNT_PROCESS
    {
        get { return _evntProcess; }
        set { _evntProcess = value; }
    }

    enum SightDir { Up, Down, Left, Right }
    private List<Tile> _visibleTile = new List<Tile>(); //�þ� �� Ÿ��

    [SerializeField]
    private Tile _tile_prefab;  //���� Ÿ�� ������
    [SerializeField]
    private Wall _wall_prefab;  //���� �� ������
    [SerializeField]
    private Tile _upStair_prefab;   //���� ��� ������
    [SerializeField]
    private Tile _downStair_prefab; //�Ʒ��� ��� ������

    //���� �̺�Ʈ ���
    [SerializeField]
    private EventData[] _event_list;    //���� �̺�Ʈ ���

    //���� �� ���
    [SerializeField]
    private EnemyData[] _enemy_list;    //���� �� ���

    void Awake()
    {
        _centerX = _dgnSizeX / 2;
        _centerY = _dgnSizeY / 2;

        _posZero = new Vector3((_tileGapX / 2) * _centerX * -1, 0, _tileGapY * _centerY * -1);

        _dgn_grid = new int[_dgnSizeX, _dgnSizeY];                      //x,y ��ǥ�� ���� ���� ��� ����ü
        _tile_dic = new Dictionary<int, Dictionary<int, Tile>>();       //x,y ��ǥ�� Ÿ�� ��ũ��Ʈ ����ü
        _wall_dic = new Dictionary<int, Dictionary<int, Wall>>();       //x,y ��ǥ�� �� ��ũ��Ʈ ����ü
        _tile_queue = new Queue<Vector2Int>();                          //������ Ÿ���� �ִ� ť

        _tempPath = new List<Tile>();       //���콺 ������ ������ �̵� ���
        _tilePath = new List<Tile>();       //�̵� ���
        _visibleTile = new List<Tile>();    //�þ� �� Ÿ��

        //���� ����
        if (_dgnData.DungeonMakeEnd == false)
            GenerateCenterTile();
    }

    void Update()
    {
        //ť�� Ÿ���� �����ϸ�, Dequeue �� �� Ÿ���� ������ 6���⿡ Ÿ�� ���� �õ�
        //ť�� Ÿ���� ���� �� Ÿ���� �ִ�ġ�� �������� �ʾҴٸ�, ���� ������ Ÿ�� ����Ʈ�� �ڿ������� ���� �κ��� ť�� �ٽ� Enqueue
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
                else if (_tileCount < _tileMax) //������ Ÿ���� �ִ� Ÿ�� ���� �� ��ĥ ��
                {
                    int listCount = _tile_list.Count;

                    //�ֱ� ������ Ÿ�� 5���� ť�� Enqueue
                    //������ Ÿ�� ������ 5�� �̸��̸�, �ִ� ��ŭ�� �ֱ�
                    for (int i = listCount - 1; i >= (listCount < 5 ? 0 : listCount - 5); i--)
                        _tile_queue.Enqueue(_tile_list[i]);
                }
                else
                {
                    _makeDungeonEnd = true;

                    int x, y;
                    Tile tile;

                    for (int i = 0; i < _tile_list.Count; i++)  //Ÿ�� ����Ʈ�� ���鼭, �ش� Ÿ�� ��ó ���� õ���� ����
                    {
                        Vector2Int vec = _tile_list[i];
                        x = vec.x;
                        y = vec.y;
                        tile = _tile_dic[x][y];

                        //6������ üũ�� ��, ���� Ÿ���� ������ NEIGHBOR�� ���, �ƴϸ� �� �����
                        //12-1-2 ����
                        if (x + 1 <= _dgnSizeX && y + 1 <= _dgnSizeY && _dgn_grid[x + 1, y + 1] == 1)
                            tile.AddNeighbor(_tile_dic[x + 1][y + 1]);
                        else
                        {
                            if (x + 1 <= _dgnSizeX + 1 && y + 1 <= _dgnSizeY + 1)
                            {
                                if (_dgn_grid[x + 1, y + 1] != -1)  //12-1-2�� ���⿡ õ���� ���ٸ� ����
                                    GenerateWall(x + 1, y + 1);
                                _wall_dic[x + 1][y + 1].SetWallSprite(3);    //õ���� 6-7-8�� �� ����
                            }
                        }
                        //2-3-4 ����
                        if (x + 2 <= _dgnSizeX && _dgn_grid[x + 2, y] == 1)
                            tile.AddNeighbor(_tile_dic[x + 2][y]);
                        else
                        {
                            if (x + 2 <= _dgnSizeX + 1)
                            {
                                if (_dgn_grid[x + 2, y] != -1)      //2-3-4�� ���⿡ õ���� ���ٸ� ����
                                    GenerateWall(x + 2, y);
                                _wall_dic[x + 2][y].SetWallSprite(4);        //õ���� 8-9-10�� �� ����
                            }
                        }
                        //4-5-6 ����
                        if (x + 1 <= _dgnSizeX && y > 1 && _dgn_grid[x + 1, y - 1] == 1)
                            tile.AddNeighbor(_tile_dic[x + 1][y - 1]);
                        else
                        {
                            if (x + 1 <= _dgnSizeX + 1 && y > 0)
                            {
                                if (_dgn_grid[x + 1, y - 1] != -1)  //4-5-6�� ���⿡ õ���� ���ٸ� ����
                                    GenerateWall(x + 1, y - 1);
                                _wall_dic[x + 1][y - 1].SetWallSprite(5);    //õ���� 10-11-12�� �� ����
                            }
                        }
                        //6-7-8 ����
                        if (x > 1 && y > 1 && _dgn_grid[x - 1, y - 1] == 1)
                            tile.AddNeighbor(_tile_dic[x - 1][y - 1]);
                        else
                        {
                            if (x > 0 && y > 0)
                            {
                                if (_dgn_grid[x - 1, y - 1] != -1)   //6-7-8�� ���⿡ õ���� ���ٸ� ����
                                    GenerateWall(x - 1, y - 1);
                                _wall_dic[x - 1][y - 1].SetWallSprite(0);    //õ���� 12-1-2�� �� ����
                            }
                        }
                        //8-9-10 ����
                        if (x > 2 && _dgn_grid[x - 2, y] == 1)
                            tile.AddNeighbor(_tile_dic[x - 2][y]);
                        else
                        {
                            if (x > 1)
                            {
                                if (_dgn_grid[x - 2, y] != -1)      //8-9-10�� ���⿡ õ���� ���ٸ� ����
                                    GenerateWall(x - 2, y);
                                _wall_dic[x - 2][y].SetWallSprite(1);        //õ���� 2-3-4�� �� ����
                            }
                        }
                        //10-11-12 ����
                        if (x > 1 && y + 1 <= _dgnSizeY && _dgn_grid[x - 1, y + 1] == 1)
                            tile.AddNeighbor(_tile_dic[x - 1][y + 1]);
                        else
                        {
                            if (x > 0 && y + 1 <= _dgnSizeY + 1)
                            {
                                if (_dgn_grid[x - 1, y + 1] != -1)  //10-11-12�� ���⿡ õ���� ���ٸ� ����
                                    GenerateWall(x - 1, y + 1);
                                _wall_dic[x - 1][y + 1].SetWallSprite(2);    //õ���� 4-5-6�� �� ����
                            }
                        }
                    }

                    //���� ��� ���� ����
                    var ustX = _dgnData.UpStairVec.x;
                    var ustY = _dgnData.UpStairVec.y;
                    int ustDir;

                    List<int> ustDirList = new List<int>(); //����� ���� ���� �ĺ���

                    if (ustX + 1 <= _dgnSizeX && ustY + 1 <= _dgnSizeY && _dgn_grid[ustX + 1, ustY + 1] == -1 &&
                        ustX > 1 && ustY > 1 && _dgn_grid[ustX - 1, ustY - 1] == 1)
                        ustDirList.Add(0);   //12-1-2 õ�� ���� �� 0 �߰�
                    if (ustX + 2 <= _dgnSizeX && _dgn_grid[ustX + 2, ustY] == -1 &&
                        ustX > 2 && _dgn_grid[ustX - 2, ustY] == 1)
                        ustDirList.Add(1);   //2-3-4 õ�� ���� �� 1 �߰�
                    if (ustX + 1 <= _dgnSizeX && ustY > 1 && _dgn_grid[ustX + 1, ustY - 1] == -1 &&
                        ustX > 1 && ustY + 1 <= _dgnSizeY && _dgn_grid[ustX - 1, ustY + 1] == 1)
                        ustDirList.Add(2);   //4-5-6 õ�� ���� �� 2 �߰�
                    if (ustX > 1 && ustY > 1 && _dgn_grid[ustX - 1, ustY - 1] == -1 &&
                        ustX + 1 <= _dgnSizeX && ustY + 1 <= _dgnSizeY && _dgn_grid[ustX + 1, ustY + 1] == 1)
                        ustDirList.Add(3);   //6-7-8 õ�� ���� �� 3 �߰�
                    if (ustX > 2 && _dgn_grid[ustX - 2, ustY] == -1 &&
                        ustX + 2 <= _dgnSizeX && _dgn_grid[ustX + 2, ustY] == 1)
                        ustDirList.Add(4);   //8-9-10 õ�� ���� �� 4 �߰�
                    if (ustX > 1 && ustY + 1 <= _dgnSizeY && _dgn_grid[ustX - 1, ustY + 1] == -1 &&
                        ustX + 1 <= _dgnSizeX && ustY > 1 && _dgn_grid[ustX + 1, ustY - 1] == 1)
                        ustDirList.Add(5);   //10-11-12 õ�� ���� �� 5 �߰�

                    if (ustDirList.Count >= 1)
                        ustDir = ustDirList[Random.Range(0, ustDirList.Count)];
                    else
                        ustDir = Random.Range(0, 6);

                    _dgnData.UpStairDir = ustDir;
                    _tile_dic[ustX][ustY].RotateStair(ustDir);
                    //�Ʒ��� ��� ���� ����
                    var dstX = _dgnData.DownStairVec.x;
                    var dstY = _dgnData.DownStairVec.y;
                    int dstDir;

                    List<int> ds_dir_list = new List<int>();  //����� ���� ���� �ĺ���

                    if (dstX + 1 < _dgnSizeX && dstY + 1 < _dgnSizeY && _dgn_grid[dstX + 1, dstY + 1] == -1 &&
                        dstX >= 1 && dstY >= 1 && _dgn_grid[dstX - 1, dstY - 1] == 1)
                        ds_dir_list.Add(1);   //12-1-2 õ�� ���� �� 0 �߰�
                    if (dstX + 2 < _dgnSizeX && _dgn_grid[dstX + 2, dstY] == -1 &&
                        dstX >= 2 && _dgn_grid[dstX - 2, dstY] == 1)
                        ds_dir_list.Add(1);   //2-3-4 õ�� ���� �� 1 �߰�
                    if (dstX + 1 < _dgnSizeX && dstY >= 1 && _dgn_grid[dstX + 1, dstY - 1] == -1 &&
                        dstX >= 1 && dstY + 1 < _dgnSizeY && _dgn_grid[dstX - 1, dstY + 1] == 1)
                        ds_dir_list.Add(1);   //4-5-6 õ�� ���� �� 2 �߰�
                    if (dstX >= 1 && dstY >= 1 && _dgn_grid[dstX - 1, dstY - 1] == -1 &&
                        dstX + 1 < _dgnSizeX && dstY + 1 < _dgnSizeY && _dgn_grid[dstX + 1, dstY + 1] == 1)
                        ds_dir_list.Add(4);   //6-7-8 õ�� ���� �� 3 �߰�
                    if (dstX >= 2 && _dgn_grid[dstX - 2, dstY] == -1 &&
                        dstX + 2 < _dgnSizeX && _dgn_grid[dstX + 2, dstY] == 1)
                        ds_dir_list.Add(4);   //8-9-10 õ�� ���� �� 4 �߰�
                    if (dstX >= 1 && dstY + 1 < _dgnSizeY && _dgn_grid[dstX - 1, dstY + 1] == -1 &&
                        dstX + 1 < _dgnSizeX && dstY >= 1 && _dgn_grid[dstX + 1, dstY - 1] == 1)
                        ds_dir_list.Add(4);   //10-11-12 õ�� ���� �� 5 �߰�

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


                    //�÷��̾� ��ġ �̵�
                    var ustVec = _dgnData.UpStairVec;
                    PlayerSys.Move_PlayerPosition(_tile_dic[ustVec.x][ustVec.y].transform.position, false);
                    PlayerSys.X = ustVec.x;
                    PlayerSys.Y = ustVec.y;
                    //�÷��̾� �þ߸� �缳��
                    ChangeVisibleTile();
                }
            }
        }
        else    //���� �����Ϳ��� ������ �ҷ�����
        {

        }
    }

    void GenerateCenterTile()   //�߾� Ÿ�� ����
    {
        //ť�� �߾� Ÿ�� Enqueue
        _tile_queue.Enqueue(new Vector2Int(_centerX, _centerY));
        _dgn_grid[_centerX, _centerY] = 1;
        _tileCount++;
        //�߾� Ÿ�� ������ ����
        var tile = Instantiate(_tile_prefab.transform);
        var tileScr = tile.GetComponent<Tile>();

        tile.position = GridIndex_ToPosition(_centerX, _centerY);
        tile.parent = transform.GetChild(0).transform;
        tile.name = _centerX + "." + _centerY;
        tileScr.SetX(_centerX);
        tileScr.SetY(_centerY);
        tileScr.SetDungeonSystem(this);
        //�������� ������ ��ųʸ��� ����, Ÿ�� ����Ʈ�� �߰�
        if (_tile_dic.ContainsKey(_centerX) == false)
            _tile_dic.Add(_centerX, new Dictionary<int, Tile>());

        _tile_dic[_centerX].Add(_centerY, tileScr);

        _tile_list.Add(new Vector2Int(_centerX, _centerY));
    }

    void GenerateTile(int x, int y) //Ÿ�� ����
    {
        if (_tileCount >= _tileMax) //Ÿ�� ������ �ִ� Ÿ�� ������ �����ϸ� ���� ����
            return;
        if (x < 1 || x > _dgnSizeX || y < 1 || y > _dgnSizeY)    //Ÿ�� ���� ��ǥ�� ��� ��ǥ ���̸� ���� X
            return;
        if (_dgn_grid[x, y] != 0)   //�̹� Ÿ���� �����ϴ� ��ǥ��� ���� X
            return;
        if (Random.value < 0.5f)    //���� Ȯ���� ���� X
            return;
        if (CountNeighborTile(x, y) > Random.Range(1, 4))   //���� Ÿ�� ������ ���� �̻��̸� ���� X
            return;

        if (!_makeUpStair && _tileMax - _tileCount == _tileMax / 2)   //���� Ÿ�� ���� �ִ� Ÿ�� ���� ������ ��, ���� ��� ����
        {
            GenerateUpStair(x, y);
            return;
        }
        else if (!_makeDownStair && _tileMax - _tileCount == 1)   //���� Ÿ�� ���� 1���� ��, �Ʒ��� ��� ����
        {
            GenerateDownStair(x, y);
            return;
        }

        //ť�� ������ Ÿ�� �ֱ�
        _tile_queue.Enqueue(new Vector2Int(x, y));
        _dgn_grid[x, y] = 1;
        _tileCount++;
        //Ÿ�� ������ ����
        var tile = Instantiate(_tile_prefab.transform);
        var tileScr = tile.GetComponent<Tile>();

        tile.position = GridIndex_ToPosition(x, y);
        tile.parent = transform.GetChild(0).transform;
        tile.name = x + "." + y;
        tileScr.SetX(x);
        tileScr.SetY(y);
        tileScr.SetDungeonSystem(this);
        //�������� ������ ��ųʸ��� ����, Ÿ�� ����Ʈ�� �߰�
        if (_tile_dic.ContainsKey(x) == false)
            _tile_dic.Add(x, new Dictionary<int, Tile>());

        _tile_dic[x].Add(y, tileScr);
        _tile_list.Add(new Vector2Int(x, y));

        //���� Ȯ���� Ÿ�Ͽ� �̺�Ʈ ����
        if (Random.value < 0.4f)
        {
            var evnt = _event_list[Random.Range(0, _event_list.Length)];
            RuntimeAnimatorController anima;

            //���� �������� �� ��ǥ�� �̺�Ʈ ������ ���
            if (_dgnData.TileEvent.ContainsKey(x) == false)
                _dgnData.TileEvent.Add(x, new Dictionary<int, EventData>());
            if (_dgnData.TileEvent.ContainsKey(y) == false)
                _dgnData.TileEvent[x].Add(y, evnt);
            else
                _dgnData.TileEvent[x][y] = evnt;

            var enemy = _enemy_list[Random.Range(0, _enemy_list.Length)];

            //���� �������� �� ��ǥ�� �� ������ ���
            if (_dgnData.TileEnemy.ContainsKey(x) == false)
                _dgnData.TileEnemy.Add(x, new Dictionary<int, EnemyData>());
            _dgnData.TileEnemy[x].Add(y, enemy);

            anima = enemy.Anima_Ctrl;

            _tile_dic[x][y].SetEventAnimation(anima);
        }
    }

    void GenerateUpStair(int x, int y)  //���� ��� ����
    {
        _dgn_grid[x, y] = 1;
        _tileCount++;
        _makeUpStair = true;
        //Ÿ�� ������ ����
        var tile = Instantiate(_upStair_prefab.transform);
        var tileScr = tile.GetComponent<Tile>();
        _dgnData.UpStairVec = new Vector2Int(x, y);  //���� �����Ϳ� ��� ��ġ ���

        tile.position = GridIndex_ToPosition(x, y);
        tile.parent = transform.GetChild(0).transform;
        tile.name = x + "." + y;
        tileScr.SetX(x);
        tileScr.SetY(y);
        tileScr.SetDungeonSystem(this);

        //�������� ������ ��ųʸ��� ����, Ÿ�ϸ���Ʈ�� �߰�
        if (_tile_dic.ContainsKey(x) == false)
            _tile_dic.Add(x, new Dictionary<int, Tile>());

        _tile_dic[x].Add(y, tileScr);

        _tile_list.Add(new Vector2Int(x, y)); //õ�� ������ ���� �ӽ÷� ����Ʈ�� �־��ٰ� ���� ����

        //��� �̺�Ʈ ����
    }

    void GenerateDownStair(int x, int y)    //�Ʒ��� ��� ����
    {
        _dgn_grid[x, y] = 1;
        _tileCount++;
        _makeDownStair = true;
        //Ÿ�� ������ ����
        var tile = Instantiate(_downStair_prefab.transform);
        var tileScr = tile.GetComponent<Tile>();
        _dgnData.DownStairVec = new Vector2Int(x, y);    //���� �����Ϳ� ��� ��ġ ���

        tile.position = GridIndex_ToPosition(x, y);
        tile.parent = transform.GetChild(0).transform;
        tile.name = x + "." + y;
        tileScr.SetX(x);
        tileScr.SetY(y);
        tileScr.SetDungeonSystem(this);

        //�������� ������ ��ųʸ��� ����, Ÿ�ϸ���Ʈ�� �߰�
        if (_tile_dic.ContainsKey(x) == false)
            _tile_dic.Add(x, new Dictionary<int, Tile>());

        _tile_dic[x].Add(y, tileScr);

        _tile_list.Add(new Vector2Int(x, y)); //õ�� ������ ���� �ӽ÷� ����Ʈ�� �־��ٰ� ���� ����

        //��� �̺�Ʈ ����
    }

    void GenerateWall(int x, int y) //�� ����
    {
        if (x < 0 || x > _dgnSizeX + 1 || y < 0 || y > _dgnSizeY + 1) //Ÿ�� ���� ��ǥ�� ��� ��ǥ ���̸� ���� X
            return;
        if (_dgn_grid[x, y] != 0)   //�̹� Ÿ���� �����ϴ� ��ǥ�� ���� x
            return;

        _dgn_grid[x, y] = -1;
        //õ�� ������ ����
        var ceiling = Instantiate(_wall_prefab.transform);
        var ceilingScr = ceiling.GetComponent<Wall>();

        ceiling.position = GridIndex_ToPosition(x, y);
        ceiling.parent = transform.GetChild(0).transform;
        ceiling.name = "c_" + x + "." + y;
        ceilingScr.SetY(y);
        //�������� ������ ��ųʸ��� ����
        if (_wall_dic.ContainsKey(x) == false)
            _wall_dic.Add(x, new Dictionary<int, Wall>());

        _wall_dic[x].Add(y, ceilingScr);

        _wall_list.Add(new Vector2Int(x, y));
    }

    int CountNeighborTile(int gridX, int gridY) //������ Ÿ�� ���� ����
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

    Vector3 GridIndex_ToPosition(int x, int z)  //�׸��� ��ǥ�� ���� ��ǥ��
    {
        //Ÿ�� ���ݿ� ����, x�� y ��ǥ ���
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

    IEnumerator PathForward()   //�̵� ��θ� ���� �̵�
    {
        if (_tilePath.Count <= 0)
        {
            yield return new WaitForSecondsRealtime(0.25f);

            _tile_dic[PlayerSys.X][PlayerSys.Y].PathColorOff();

            _isForwarding = false;

            //�̵� ���� �� ���콺 ������ Ÿ���� �ִٸ� path������
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
                var preEvent = _dgnData.TileEvent[preX][preY];  //�÷��̾� ���� Ÿ���� �̺�Ʈ

                //���� Ÿ�� �̺�Ʈ ��������Ʈ ��ġ ������
                if (preEvent != null && preEvent.Event.Type != EventData.EventType.No)
                {
                    _tile_dic[preX][preY].SetEventSpritePosition(true, false);
                    PlayerSys.Set_PlayerLocalPos(true, false);
                }
            }

            var tile = _tilePath[_tilePath.Count - 1];  //���� ���� Ÿ��
            var x = tile.X;
            var y = tile.Y;

            //�÷��̾� ��ǥ �̵�
            PlayerSys.Move_PlayerPosition(tile.transform.position, preX > x);
            PlayerSys.X = x;    //�÷��̾� x ��ǥ ����
            PlayerSys.Y = y;    //�÷��̾� y ��ǥ ����

            //�̵� ȿ����

            //�÷��̾� �þ� �缳��
            ChangeVisibleTile();

            _tilePath.Remove(tile);         //������ Ÿ���� ��ο��� ����
            tile.CONNECT.PathColorOff();    //������ Ÿ���� ���ǥ�� ����

            //������ Ÿ�Ͽ� �̺�Ʈ�� ���� ���, ����
            EventData nowEvent = null;

            if (_dgnData.TileEvent.ContainsKey(x) && _dgnData.TileEvent[x].ContainsKey(y))
            {
                nowEvent = _dgnData.TileEvent[x][y];

                if (nowEvent != null && nowEvent.Event.Type != EventData.EventType.No)  //�̺�Ʈ�� ������ ��
                {
                    //preX <= x�� ��, �÷��̾� ���� / �̺�Ʈ ��������Ʈ �����ʿ� ��ġ
                    //preX > x�� ��, �÷��̾� ������ / �̺�Ʈ ��������Ʈ ���ʿ� ��ġ
                    _tile_dic[x][y].SetEventSpritePosition(false, preX > x);
                    PlayerSys.Set_PlayerLocalPos(false, preX <= x);

                    yield return new WaitForSeconds(0.5f);

                    //�̺�Ʈ �߻��� Ÿ�� ��ó�� Ÿ�� ���� ������ �Ѱ��ְ� ���� ��濡 ����
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
                    
                    BtlSys.Record_DungeonScript(this, _camera_dgn); //�� ��ũ��Ʈ�� ���� ī�޶� �Ҵ����ֱ� ���� ����
                    BtlSys.BattleStart(_dgnData.TileEnemy[x][y]);   //�ش� ��ġ�� ���� ���� ����
                }

                PathClear();
            }

            yield return new WaitUntil(() => EVNT_PROCESS == false);

            StartCoroutine("PathForward");
        }
    }

    List<Tile> FindPath(Tile start, Tile target)    //���� Ÿ�ϰ� ��ǥ Ÿ�ϰ��� ��� ã��
    {
        var search_list = new List<Tile>() { start };
        var process_list = new List<Tile>();

        while (search_list.Count > 0)   //�˻��� Ÿ�� ��Ͽ� Ÿ���� ����������
        {
            var current = search_list[0];

            foreach (var t in search_list)
                if (t.F < current.F || t.F == current.F && t.H < current.H) //���� Ÿ�� ��ó�� Ÿ�� �� ��ΰ� �� Ÿ�� ã��
                    current = t;

            process_list.Add(current);
            search_list.Remove(current);

            if (current == target)  //���� ����� Ÿ���� ��ǥ Ÿ���� ���
            {
                var now_path = target;  //��θ� ã�� ���� Ÿ�� Ŀ�� (������ ��ǥ Ÿ��)
                var path = new List<Tile>();
                var count = 100;

                while (now_path != start)
                {
                    path.Add(now_path);
                    now_path = now_path.CONNECT;    //�� Ÿ�ϰ� ����� Ÿ�Ϸ� Ŀ���� �ű�
                    count--;
                    if (count < 0) throw new Exception();
                }

                foreach (var tile in path)
                    tile.SetPathColor(new Color(1, 1, 1, 0.7f));

                start.SetPathColor(new Color(1, 1, 1, 0.7f));

                return path;
            }

            //return ���� �ʾ��� ���, ���� ��ǥ Ÿ���� ã�� ���Ѱ�
            foreach (var neighbor in current.NEIGHBOR.Where(t => !process_list.Contains(t) && t.GetExplored()))
            //ó������ ���� Ÿ�� �� �߰��ߴ� Ÿ�ϸ� ��ȸ
            {
                var inSearch = search_list.Contains(neighbor);  //�� Ÿ���� �˻� ����Ʈ�� ���Ե��ִ��� �Ǵ�
                var costToNeighbor = current.G + current.GetDistance(neighbor); //�� Ÿ�Ϸ� ���� �Ÿ�
                                                                                // = ��������� �Ÿ� + (���� Ÿ�ϰ� ���� Ÿ�ϱ����� �Ÿ�)
                if (!inSearch || costToNeighbor < neighbor.G)   //�� Ÿ�Ϸ� ���� �Ÿ��� �� ª�ٴ� ���� �˾Ƴ´ٸ�
                {
                    neighbor.SetG(costToNeighbor);  //�Ÿ� ��ġ ����
                    neighbor.SetConnect(current);   //���� Ÿ�ϰ� �� Ÿ���� ����

                    if (!inSearch)
                    {
                        neighbor.SetH(neighbor.GetDistance(target));    //�� Ÿ�Ͽ��� ���� Ÿ�ϱ��� ���� �Ÿ��� ���
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

        _visibleTile.Clear();   //���� �þ� �ʱ�ȭ

        if (_dgn_grid[x, y] != 0)
        {
            _tile_dic[x][y].SetVisible(true, true);   //�÷��̾ ��ġ�� Ÿ�� �þ߿� ����
            _visibleTile.Add(_tile_dic[x][y]);  //�÷��̾ ��ġ�� Ÿ���� �þ� ��Ͽ� �߰�
        }

        //�þ߰� SIGHT�� ���� ���̴� Ÿ���� üũ�ϴ� ����Լ�����
        CheckSight(s - 1, SightDir.Left, x - 2, y);
        CheckSight(s - 1, SightDir.Right, x + 2, y);
        CheckSight(s - 1, SightDir.Up, x + 1, y + 1);
        CheckSight(s - 1, SightDir.Up, x - 1, y + 1);
        CheckSight(s - 1, SightDir.Down, x + 1, y - 1);
        CheckSight(s - 1, SightDir.Down, x - 1, y - 1);
        //

        //�÷��̾� 1Ÿ�� �̳� 4-5-6-/6-7-8 ���� ���� ������ȭ
        if (_dgn_grid[x + 1, y - 1] == -1)
            _wall_dic[x + 1][y - 1].InvinsibleWall(true);
        if (_dgn_grid[x - 1, y - 1] == -1)
            _wall_dic[x - 1][y - 1].InvinsibleWall(true);

        IEnumerable<Tile> outSight = visibleTile_prev.Except(_visibleTile); //�þ߿��� ��� Ÿ�� ���

        foreach (var t in outSight)
        {
            t.SetVisible(false, false);

            var tx = t.X;
            var ty = t.Y;

            //t�� 12-1-2 ������ õ���� ���� �߰ߵ� ���¸�, visible false
            if (_dgn_grid[tx + 1, ty + 1] == -1 && _wall_dic[tx + 1][ty + 1].GetVisible(3))
                _wall_dic[tx + 1][ty + 1].SetVisible(3, false, false);
            //t�� 2-3-4 ������ õ���� ���� �߰ߵ� ���¸�, visible false
            if (_dgn_grid[tx + 2, ty] == -1 && _wall_dic[tx + 2][ty].GetVisible(4))
                _wall_dic[tx + 2][ty].SetVisible(4, false, false);
            //t�� 4-5-6 ������ õ���� ���� �߰ߵ� ���¸�, visible false
            if (_dgn_grid[tx + 1, ty - 1] == -1 && _wall_dic[tx + 1][ty - 1].GetVisible(5))
                _wall_dic[tx + 1][ty - 1].SetVisible(5, false, false);
            //t�� 6-7-8 ������ õ���� ���� �߰ߵ� ���¸�, visible false
            if (_dgn_grid[tx - 1, ty - 1] == -1 && _wall_dic[tx - 1][ty - 1].GetVisible(0))
                _wall_dic[tx - 1][ty - 1].SetVisible(0, false, false);
            //t�� 8-9-10 ������ õ���� ���� �߰ߵ� ���¸�, visible false
            if (_dgn_grid[tx - 2, ty] == -1 && _wall_dic[tx - 2][ty].GetVisible(1))
                _wall_dic[tx - 2][ty].SetVisible(1, false, false);
            //t�� 10-11-12 ������ õ���� ���� �߰ߵ� ���¸�, visible false
            if (_dgn_grid[tx - 1, ty + 1] == -1 && _wall_dic[tx - 1][ty + 1].GetVisible(2))
                _wall_dic[tx - 1][ty + 1].SetVisible(2, false, false);
        }
    }

    void CheckSight(int s, SightDir dir, int x, int y)
    {
        //s�� 0�̸� ����
        if (s < 0)
            return;

        var gridValue = _dgn_grid[x, y];

        //�þ߰� ������ ��ǥ�� ���� ������ ��
        if (gridValue != 0)
        {
            if (gridValue == 1) //������ Ÿ���� ��
            {
                var tile = _tile_dic[x][y];

                tile.SetExplored(true); //�߰�ó��
                tile.SetVisible(s > 0, false); //1 �þ߰� ���� ���� �ڼ��� ������ ����

                if (s > 0 && !_visibleTile.Contains(tile))
                    _visibleTile.Add(tile); //�þ� ��Ͽ� ���� Ÿ���̸� �߰�

                //tile�� 12-1-2 ������ õ���� ���� �߰�
                if (_dgn_grid[x + 1, y + 1] == -1)
                    _wall_dic[x + 1][y + 1].SetExplored(3, true);
                //tile�� 12-1-2 ������ õ���� ���� �߰�
                if (_dgn_grid[x + 2, y] == -1)
                    _wall_dic[x + 2][y].SetExplored(4, true);
                //tile�� 12-1-2 ������ õ���� ���� �߰�
                if (_dgn_grid[x + 1, y - 1] == -1)
                    _wall_dic[x + 1][y - 1].SetExplored(5, true);
                //tile�� 12-1-2 ������ õ���� ���� �߰�
                if (_dgn_grid[x - 1, y - 1] == -1)
                    _wall_dic[x - 1][y - 1].SetExplored(0, true);
                //tile�� 12-1-2 ������ õ���� ���� �߰�
                if (_dgn_grid[x - 2, y] == -1)
                    _wall_dic[x - 2][y].SetExplored(1, true);
                //tile�� 12-1-2 ������ õ���� ���� �߰�
                if (_dgn_grid[x - 1, y + 1] == -1)
                    _wall_dic[x - 1][y + 1].SetExplored(2, true);
            }
            else if (gridValue == -1)   //������ ���̶��
            {
                var wall = _wall_dic[x][y];

                //�켱 �� ���� ������ȭ�� ���
                wall.InvinsibleWall(false);

                Debug.Log("[" + x + ", " + y + "]: " + s);

                //�� ���⿡ ���� Ư�� ���� visible, explored�� ����
                if (_dgn_grid[x + 1, y + 1] == 1 && _tile_dic[x + 1][y + 1].GetExplored() &&
                    (dir == SightDir.Down || dir == SightDir.Left))  //12-1-2 ���⿡ �߰��� Ÿ���� ������ ���, 
                {
                    wall.SetExplored(0, true);
                    wall.SetVisible(0, (s > 0), s == PlayerSys.SIGHT - 1);
                }
                if (_dgn_grid[x + 2, y] == 1 && _tile_dic[x + 2][y].GetExplored() &&
                    (dir == SightDir.Left))    //2-3-4 ���⿡ �߰ߵ� Ÿ���� ������ ���,
                {
                    wall.SetExplored(1, true);
                    wall.SetVisible(1, (s > 0), s == PlayerSys.SIGHT - 1);
                }
                if (_dgn_grid[x + 1, y - 1] == 1 && _tile_dic[x + 1][y - 1].GetExplored() &&
                    (dir == SightDir.Left || dir == SightDir.Up))    //4-5-6 ���⿡ �߰ߵ� Ÿ���� ������ ���,
                {
                    wall.SetExplored(2, true);
                    wall.SetVisible(2, (s > 0), s == PlayerSys.SIGHT - 1);
                }
                if (_dgn_grid[x - 1, y - 1] == 1 && _tile_dic[x - 1][y - 1].GetExplored() &&
                    (dir == SightDir.Up || dir == SightDir.Right))    //6-7-8 ���⿡ �߰ߵ� Ÿ���� ������ ���,
                {
                    wall.SetExplored(3, true);
                    wall.SetVisible(3, (s > 0), s == PlayerSys.SIGHT - 1);
                }
                if (_dgn_grid[x - 2, y] == 1 && _tile_dic[x - 2][y].GetExplored() &&
                    (dir == SightDir.Right))    //8-9-10 ���⿡ �߰ߵ� Ÿ���� ������ ���,
                {
                    wall.SetExplored(4, true);
                    wall.SetVisible(4, (s > 0), s == PlayerSys.SIGHT - 1);
                }
                if (_dgn_grid[x - 1, y + 1] == 1 && _tile_dic[x - 1][y + 1].GetExplored() &&
                    (dir == SightDir.Right || dir == SightDir.Down))    //10-11-12 ���⿡ �߰ߵ� Ÿ���� ������ ���,
                {
                    wall.SetExplored(5, true);
                    wall.SetVisible(5, (s > 0), s == PlayerSys.SIGHT - 1);
                }

                //Ÿ���� �ƴ� ���� �þ߰� ������ �þ� ���� (��� ����)
                return;
            }
        }

        //��� ������� �ʾ��� ���,
        //�þ� ������ ���� Ư�� x, y������ ��� ���� (��� �� s 1 ����)
        switch (dir)
        {
            case SightDir.Up:       //�� ����: Up  => (x-1, y+1�� ��� / x+1, y+1�� ���)
                CheckSight(s - 1, dir, x - 1, y + 1);
                CheckSight(s - 1, dir, x + 1, y + 1);
                break;
            case SightDir.Down:     //�� ����: Down => (x-1, y-1�� ��� / x+1, y-1�� ���)
                CheckSight(s - 1, dir, x - 1, y - 1);
                CheckSight(s - 1, dir, x + 1, y - 1);
                break;
            case SightDir.Left:     //�� ����: Left => (x-1, y+1�� ��� / x-2, y�� ��� / x-1, y-1�� ���)
                CheckSight(s - 1, dir, x - 1, y + 1);
                CheckSight(s - 1, dir, x - 2, y);
                CheckSight(s - 1, dir, x - 1, y - 1);
                break;
            case SightDir.Right:    //�� ����: Right => (x+1, y+1�� ��� / x+2, y�� ��� / x+1, y-1�� ���)
                CheckSight(s - 1, dir, x + 1, y + 1);
                CheckSight(s - 1, dir, x + 2, y);
                CheckSight(s - 1, dir, x + 1, y - 1);
                break;
        }
    }
}
