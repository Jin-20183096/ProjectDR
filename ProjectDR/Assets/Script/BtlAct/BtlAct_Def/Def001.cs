using UnityEngine;

[CreateAssetMenu(fileName = "Def001", menuName = "ScrObj/BtlAct_Def/000~025/Def001", order = 1)]

public class Def001 : BtlAct_Def
{
    //Def001: ����

    public override void Effect_Post(bool isP, BattleSystem btlSys)
    {
        //��� ���� ��: ���� �ൿ���� x��ŭ ���� (x: �ڽ��� �ֻ���)
        if (isP ? btlSys.P_HIT_DEF : btlSys.E_HIT_DEF)
        {
            //�ൿ ȿ�� ó�� ���� (���� �ý����� ȿ�� ó�� ���� �ߴ�)
            btlSys.Set_EffectProcess(true);

            //���� �۵�
            var myDice = isP ? btlSys.P_DICE : btlSys.E_DICE;

            if (isP)
                btlSys.Change_Ap_Enemy(false, myDice);
            else
                btlSys.Change_Ap_Player(false, myDice);

            //�α� ���
            btlSys.SetLog_DefEffect(isP, "[��/��]", " �ൿ���� " + myDice + " �Ҿ���");
        }
    }
}
