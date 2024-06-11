using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dungeon", menuName = "ScrObj/Dungeon Data")]   //Ŀ���� �޴� �߰�

public class DungeonData : ScriptableObject
{
    public bool DungeonMakeEnd; //���� ���� �Ϸ� ����

    public List<Vector2Int> TileVec;    //���� Ÿ���� �׸��� ��ǥ��
    public List<int> TileSprIndex;  //���� Ÿ���� ��������Ʈ ��ȣ

    public List<Vector2Int> TileVec_NoEvent;    //�̺�Ʈ�� ���� Ÿ���� ��ǥ�� (�̺�Ʈ�� �ο��Ǹ�, �� ����Ʈ���� Remove)

    public List<Vector2Int> WallVec; //���� ���� �׸��� ��ǥ
    public List<int> WallSprIndex;  //���� �� ��������Ʈ ��ȣ

    public Vector2Int UpStairVec;   //���� ����� �׸��� ��ǥ
    public int UpStairDir;          //���� ����� ����

    public Vector2Int DownStairVec; //�Ʒ��� ����� �׸��� ��ǥ
    public int DownStairDir;        //�Ʒ��� ����� ����

    //Ÿ�� �̺�Ʈ�� ��ųʸ�
    public Dictionary<int, Dictionary<int, EventData>> TileEvent
        = new Dictionary<int, Dictionary<int, EventData>>();
    //���� �̺�Ʈ Ÿ���� �� ��ųʸ�
    public Dictionary<int, Dictionary<int, EnemyData>> TileEnemy
        = new Dictionary<int, Dictionary<int, EnemyData>>();

    void Reset()
    {
        DungeonMakeEnd = false;

        TileVec.Clear();
        TileSprIndex.Clear();

        TileVec_NoEvent.Clear();

        WallVec.Clear();
        WallSprIndex.Clear();

        TileEvent.Clear();
        TileEnemy.Clear();
    }
}
