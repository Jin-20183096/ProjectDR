using UnityEngine;

[CreateAssetMenu(fileName = "BtlAct002", menuName = "ScrObj/BtlAct000~025/BtlAct002", order = 2)]   //Ŀ���� �޴� �߰�

public class BtlAct002 : BtlActData
{
    //002 ���

    public override void Effect_Post(bool isP, BattleSystem btlSys)
    {
        //��� ���� ��: ���� �ൿ���� x��ŭ �Ҹ� (x: �ڽ��� �ֻ���)
        if (isP ? btlSys.P_HIT_DEF : btlSys.E_HIT_DEF)
        {
            //�ൿ ȿ�� ó�� ���� (���� �ý����� ȿ�� ó�� �Ͻ�����)
            btlSys.Set_EffectProcess(true);

            //���� �۵�
            var myDice = isP ? btlSys.P_DICE : btlSys.E_DICE;

            if (isP)
                btlSys.Change_Ap_Enemy(false, myDice);
            else
                btlSys.Change_Ap_Player(false, myDice);

            //�α� ���
            btlSys.SetLog_DefEffect(isP, btlSys.E_NAME + "�ൿ���� " + myDice + " �Ҿ���");
        }
    }
}
