using UnityEngine;

[CreateAssetMenu(fileName = "BtlAct001", menuName = "ScrObj/BtlAct000~025/BtlAct001", order = 1)]   //Ŀ���� �޴� �߰�

public class BtlAct001 : BtlActData
{
    //001 ����

    /*
    public override void Effect_Post(bool isP, BattleSystem btlSys)
    {
        //������ ������, �ൿ�� �ұ�
        if (isP ? btlSys.E_HIT_DEF : btlSys.P_HIT_DEF)
        {
            //�ൿ ȿ�� ó�� ���� (���� �ý����� ȿ�� ó�� ���� �ߴ�)
            btlSys.Set_EffectProcess(true);

            //���� �۵�
            var enDice = isP ? btlSys.E_DICE : btlSys.P_DICE;

            if (isP)
                btlSys.Change_Ap_Player(false, enDice);
            else
                btlSys.Change_Ap_Enemy(false, enDice);

            //�α� ���
            btlSys.SetLog_AtkBlocked(isP, enDice + " �ൿ���� �Ҹ��ߴ�");
        }
    }
    */
}
