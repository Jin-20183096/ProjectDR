using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScrObj/EnemyData")] //커스텀 메뉴를 생성하는 속성

public class EnemyData : ScriptableObject
{
    [Header("# Main Info")]
    [TextArea(1, 3)]
    public string Name;     //이름
    [SerializeField]
    private int[] _hp_minMax = new int[2];  //적 최소 ~ 최대 HP
    public int HP_MIN { get { return _hp_minMax[0]; } }
    public int HP_MAX { get { return _hp_minMax[1]; } }

    public int AC;    //적 방어도

    public int ApMax;   //적 행동력

    public int[] STR;  //힘 스탯
    public int[] INT;  //지능 스탯
    public int[] DEX;  //손재주 스탯
    public int[] AGI;  //민첩 스탯
    public int[] WIL;  //의지 스탯

    public ICreature.BtlAct[] Act_Atk; //공격 행동 목록
    public ICreature.BtlAct[] Act_Def; //방어 행동 목록
    public ICreature.BtlAct[] Act_Dge; //회피 행동 목록
    public ICreature.BtlAct[] Act_Tac; //전술 행동 목록

    [Header("# BT")]
    public TextAsset BT;
    public string[] ActClueLog;

    [Header("# Sprite Animation")]
    public Sprite DefaultSprite;    //적 기본 스프라이트
    public RuntimeAnimatorController Anima_Ctrl;    //적 애니메이션 컨트롤러

    [Header("# Reward Info")]
    public int[] Exp = new int[2];  //최소 경험치 ~ 최대 경험치
    public ItemData[] Item;
}
