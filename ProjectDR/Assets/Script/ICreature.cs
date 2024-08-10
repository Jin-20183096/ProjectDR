using System;

public interface ICreature
{
    public enum Stats
    {
        No, STR, INT, DEX, AGI, CON, WIL, HP, AC,
        RE_STR, RE_INT, RE_DEX, RE_AGI, RE_CON, RE_WIL
    }

    public bool IsPlayer(); //�÷��̾�����, �ƴ���

    public void Change_Hp(bool plus, int value);    //HP ����
    public void Change_HpMax(bool plus, int value); //�ִ� HP ����
    public void Change_AC(bool plus, int value);    //�� ����
    public void Change_ACMax(bool plus, int value); //�ִ� �� ����
    public void Change_Ap(bool plus, int value);    //�ൿ�� ����
    public void Change_ApMax(bool plus, int value); //�ִ� �ൿ�� ����
    public void TakeDamage(int dmg, BtlActData.DamageType dmgType); //���� ����

    [Serializable]
    public class BtlActClass
    {
        public BtlActData Data; //�ൿ ������
        public Stats Stat;      //�ൿ�� ����
        public int Upgrade;     //�ൿ ��ȭ ��ġ
    }
}
