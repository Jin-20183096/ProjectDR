using UnityEngine;

[CreateAssetMenu(fileName = "BtlAct002", menuName = "ScrObj/BtlAct000~025/BtlAct002", order = 2)]   //Ŀ���� �޴� �߰�

public class BtlAct002 : BtlActData
{
    //002 ���

    public override void Effect_Post(bool isP, BattleSystem btlSys)
    {
        //��밡 ����ϸ�, �ൿ�� �����ޱ�
        if (isP ? btlSys.E_HIT_WAIT : btlSys.P_HIT_WAIT)
        {
            //�ൿ ȿ�� ó�� ���� (���� �ý����� ȿ�� ó�� ���� �ߴ�)
            btlSys.Set_EffectProcess(true);

            //���� �۵�
            var myDice = isP ? btlSys.P_DICE : btlSys.E_DICE;

            if (isP)
                btlSys.Change_Ap_Player(true, myDice);
            else
                btlSys.Change_Ap_Enemy(true, myDice);

            //�α� ���
            btlSys.SetLog_DefEffect_Wait(isP, "�Ҹ��� �ൿ���� �����޾Ҵ�");
        }
    }
}
