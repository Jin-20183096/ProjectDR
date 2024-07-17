using UnityEngine;

[CreateAssetMenu(fileName = "BtlAct008", menuName = "ScrObj/BtlAct000~025/BtlAct008", order = 8)]   //Ŀ���� �޴� �߰�

public class BtlAct008 : BtlActData
{
    //008 ����

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
            btlSys.SetLog_DefEffect(isP, "[��/��]"," �ൿ���� " + myDice + " �Ҿ���");
        }
    }
}
