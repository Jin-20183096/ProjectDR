using UnityEngine;

[CreateAssetMenu(fileName = "Dge001", menuName = "ScrObj/BtlAct_Dge/000~025/Dge001", order = 1)]   //Ŀ���� �޴� �߰�

public class Dge001 : BtlActData
{
    //Dge001: ȸ��

    //ȸ�� ���� üũ
    public override bool Dodge_Check(bool isP, BattleSystem btlSys)
    {
        //ȸ�� ����: �ֻ��� ���� >= ��� �ֻ��� ����
        if (isP)
            return btlSys.P_TOTAL >= btlSys.E_TOTAL;
        else
            return btlSys.E_TOTAL >= btlSys.P_TOTAL;
    }

    public override void Effect_Post(bool isP, BattleSystem btlSys)
    {
        //ȸ�� ���� ��: �Ҹ��� �ൿ���� �����޴´�.
        if (isP ? btlSys.P_HIT_DGE : btlSys.E_HIT_DGE)
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
            btlSys.SetLog_DgeEffect(isP, null, "�Ҹ��� �ൿ���� �����޾Ҵ�");
        }
    }
}
