using System;

public interface ICreature
{
    public enum Stats
    {
        No, STR, INT, DEX, AGI, CON, WIL, HP, AC,
        RE_STR, RE_INT, RE_DEX, RE_AGI, RE_CON, RE_WIL
    }

    public bool IsPlayer(); //플레이어인지, 아닌지

    public void Change_Hp(bool plus, int value);    //HP 변경
    public void Change_HpMax(bool plus, int value); //최대 HP 변경
    public void Change_AC(bool plus, int value);    //방어도 변경
    public void Change_ACMax(bool plus, int value); //최대 방어도 변경
    public void Change_Ap(bool plus, int value);    //행동력 변경
    public void Change_ApMax(bool plus, int value); //최대 행동력 변경
    public void TakeDamage(int dmg, BtlActData.DamageType dmgType); //피해 받음

    [Serializable]
    public class BtlActClass
    {
        public BtlActData Data; //행동 데이터
        public Stats Stat;      //행동의 스탯
        public int Upgrade;     //행동 강화 수치
    }
}
