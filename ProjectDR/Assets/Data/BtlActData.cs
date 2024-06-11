using UnityEngine;

public class BtlActData : ScriptableObject
{
    public enum ActionType { No, Atk, Def, Dge, Tac, Wait }
    public enum DamageType { No, Slash, Strike, Pierce, Defense }

    [TextArea(3, 5)]
    public string Name;                 //�ൿ��
    public ICreature.Stats[] Stats_Arr; //�ൿ ���� �ĺ�
    public ActionType Type;             //�ൿ Ÿ��
    public DamageType DmgType;          //(���ظ� �ִ� �ൿ�� ���)���� Ÿ��

    [SerializeField]
    private int[] Dice_MinMax;          //�ֻ��� �ּ�,�ִ�
    public int DiceMin { get { return Dice_MinMax[0]; } }   //�ּ� �ֻ���
    public int DiceMax { get { return Dice_MinMax[1]; } }   //�ִ� �ֻ���

    [TextArea(3, 8)]
    public string ActionInfo;           //�ൿ ����

    [Header("# Action SubType")]
    public bool NoDice;                 //�ֻ����� ������ ���� ����

    //�ൿ �����
    [Header("# Action MoveSet")]
    public SpriteSystem.AtkMoveSet AtkMS;
    public SpriteSystem.DefMoveSet DefMS;
    public SpriteSystem.DgeMoveSet DgeMS;
    public SpriteSystem.TacMoveSet TacMS;

    public virtual void Effect_Pre(bool isP, BattleSystem btlSys) { }   //�ൿ ó�� ������ �ߵ��Ǵ� ȿ��
    public virtual void Effect_Post(bool isP, BattleSystem btlSys) { }  //�ൿ ó�� ���Ŀ� �ߵ��Ǵ� ȿ��
    public virtual bool Dodge_Check(bool isP, BattleSystem btlSys) { return false; }    //ȸ�� ���� üũ ���
    public virtual void Effect_Tac(bool isP, BattleSystem btlSys) { }   //�����ൿ�� ȿ��
}
