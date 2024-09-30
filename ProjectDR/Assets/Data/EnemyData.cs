using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScrObj/EnemyData")] //Ŀ���� �޴��� �����ϴ� �Ӽ�

public class EnemyData : ScriptableObject
{
    [Header("# Main Info")]
    [TextArea(1, 3)]
    public string Name;     //�̸�
    [SerializeField]
    private int[] _hp_minMax = new int[2];  //�� �ּ� ~ �ִ� HP
    public int HP_MIN { get { return _hp_minMax[0]; } }
    public int HP_MAX { get { return _hp_minMax[1]; } }

    public int AC;    //�� ��

    public int ApMax;   //�� �ൿ��

    public int[] STR;  //�� ����
    public int[] INT;  //���� ����
    public int[] DEX;  //������ ����
    public int[] AGI;  //��ø ����
    public int[] WIL;  //���� ����

    public ICreature.BtlAct[] Act_Atk; //���� �ൿ ���
    public ICreature.BtlAct[] Act_Def; //��� �ൿ ���
    public ICreature.BtlAct[] Act_Dge; //ȸ�� �ൿ ���
    public ICreature.BtlAct[] Act_Tac; //���� �ൿ ���

    [Header("# BT")]
    public TextAsset BT;
    public string[] ActClueLog;

    [Header("# Sprite Animation")]
    public Sprite DefaultSprite;    //�� �⺻ ��������Ʈ
    public RuntimeAnimatorController Anima_Ctrl;    //�� �ִϸ��̼� ��Ʈ�ѷ�

    [Header("# Reward Info")]
    public int[] Exp = new int[2];  //�ּ� ����ġ ~ �ִ� ����ġ
    public ItemData[] Item;
}
