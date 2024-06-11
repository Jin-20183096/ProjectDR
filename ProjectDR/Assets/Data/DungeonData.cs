using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dungeon", menuName = "ScrObj/Dungeon Data")]   //커스텀 메뉴 추가

public class DungeonData : ScriptableObject
{
    public bool DungeonMakeEnd; //던전 생성 완료 여부

    public List<Vector2Int> TileVec;    //던전 타일의 그리드 좌표들
    public List<int> TileSprIndex;  //던전 타일의 스프라이트 번호

    public List<Vector2Int> TileVec_NoEvent;    //이벤트가 없는 타일의 좌표들 (이벤트가 부여되면, 이 리스트에서 Remove)

    public List<Vector2Int> WallVec; //던전 벽의 그리드 좌표
    public List<int> WallSprIndex;  //던전 벽 스프라이트 번호

    public Vector2Int UpStairVec;   //위층 계단의 그리드 좌표
    public int UpStairDir;          //위층 계단의 방향

    public Vector2Int DownStairVec; //아래층 계단의 그리드 좌표
    public int DownStairDir;        //아래층 계단의 방향

    //타일 이벤트의 딕셔너리
    public Dictionary<int, Dictionary<int, EventData>> TileEvent
        = new Dictionary<int, Dictionary<int, EventData>>();
    //전투 이벤트 타일의 적 딕셔너리
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
