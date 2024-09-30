using UnityEngine;

public class BtlActData : ScriptableObject
{
    public enum ActType { No, Atk, Def, Dge, Tac }
    public enum DamageType { No, Slash, Strike, Pierce, Defense }

    [TextArea(3, 5)]
    public string Name;                 //행동명
    public ICreature.Stats[] Stats_Arr; //스탯 후보
    public ActType Type;                //행동 타입
    public DamageType DmgType;          //(피해를 주는 행동의 경우)피해 타입

    [SerializeField]
    private int[] Dice_MinMax;          //주사위 최소,최대
    public int DiceMin { get { return Dice_MinMax[0]; } }   //최소 주사위
    public int DiceMax { get { return Dice_MinMax[1]; } }   //최대 주사위

    [TextArea(3, 8)]
    public string Info;                 //행동 설명

    [Header("# Action SubType")]
    public bool NoDice;                 //주사위를 굴리지 않음 여부
    public bool NoReroll;               //재굴림 가능 여부
    /*
    //행동 무브셋
    [Header("# Action MoveSet")]
    public SpriteSystem.AtkMoveSet AtkMS;
    public SpriteSystem.DefMoveSet DefMS;
    public SpriteSystem.DgeMoveSet DgeMS;
    public SpriteSystem.TacMoveSet TacMS;
    */

    public virtual int Calc_Total(int[] my_rslt) { return 0; }  //행동의 총합 계산
    public virtual void Effect_Pre(bool isP, BattleSystem btlSys) { }   //행동 사용 직전에 발동되는 효과
    public virtual void Effect_Post(bool isP, BattleSystem btlSys) { }  //행동 사용 직후에 발동되는 효과
    public virtual bool Dodge_Check(bool isP, BattleSystem btlSys) { return false; }    //회피 조건 체크 결과

    //가장 높은 주사위 눈이 몇 번째인지 반환

    //가장 낮은 주사위 눈이 몇 번째인지 반환

    //주사위 눈을 모두 더해서 총합 계산

    //X번째 주사위를 Y배해서 총합 계산
    
    //X
}
