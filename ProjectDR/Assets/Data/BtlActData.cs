using UnityEngine;

public class BtlActData : ScriptableObject
{
    public enum ActType { No, Atk, Def, Dge, Tac }
    public enum DamageType { No, Slash, Strike, Pierce, Defense }

    [TextArea(3, 5)]
    public string Name;                 //�ൿ��
    public ICreature.Stats[] Stats_Arr; //���� �ĺ�
    public ActType Type;                //�ൿ Ÿ��
    public DamageType DmgType;          //(���ظ� �ִ� �ൿ�� ���)���� Ÿ��

    [SerializeField]
    private int[] Dice_MinMax;          //�ֻ��� �ּ�,�ִ�
    public int DiceMin { get { return Dice_MinMax[0]; } }   //�ּ� �ֻ���
    public int DiceMax { get { return Dice_MinMax[1]; } }   //�ִ� �ֻ���

    [TextArea(3, 8)]
    public string Info;                 //�ൿ ����

    [Header("# Action SubType")]
    public bool NoDice;                 //�ֻ����� ������ ���� ����
    public bool NoReroll;               //�籼�� ���� ����
    /*
    //�ൿ �����
    [Header("# Action MoveSet")]
    public SpriteSystem.AtkMoveSet AtkMS;
    public SpriteSystem.DefMoveSet DefMS;
    public SpriteSystem.DgeMoveSet DgeMS;
    public SpriteSystem.TacMoveSet TacMS;
    */

    public virtual int Calc_Total(int[] my_rslt) { return 0; }  //�ൿ�� ���� ���
    public virtual void Effect_Pre(bool isP, BattleSystem btlSys) { }   //�ൿ ��� ������ �ߵ��Ǵ� ȿ��
    public virtual void Effect_Post(bool isP, BattleSystem btlSys) { }  //�ൿ ��� ���Ŀ� �ߵ��Ǵ� ȿ��
    public virtual bool Dodge_Check(bool isP, BattleSystem btlSys) { return false; }    //ȸ�� ���� üũ ���

    //���� ���� �ֻ��� ���� �� ��°���� ��ȯ

    //���� ���� �ֻ��� ���� �� ��°���� ��ȯ

    //�ֻ��� ���� ��� ���ؼ� ���� ���

    //X��° �ֻ����� Y���ؼ� ���� ���
    
    //X
}
