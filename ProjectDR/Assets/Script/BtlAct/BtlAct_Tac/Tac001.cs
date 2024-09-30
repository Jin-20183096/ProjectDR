using UnityEngine;

[CreateAssetMenu(fileName = "Tac001", menuName = "ScrObj/BtlAct_Tac/001~025/Tac001", order = 1)]   //Ŀ���� �޴� �߰�

public class Tac001 : BtlActData
{
    //Tac001: ���

    public override void Effect_Pre(bool isP, BattleSystem btlSys)
    {
        //�ൿ�� 2 ȸ��

        //�ൿ ȿ�� ó�� ���� (���� �ý����� ȿ�� ó�� ���� �ߴ�)
        btlSys.Set_EffectProcess(true);

        //���� �۵�
        if (isP)
            btlSys.Change_Ap_Player(true, 2);
        else
            btlSys.Change_Ap_Enemy(true, 2);

        //�α� ���
        btlSys.SetLog_Wait(isP);
    }
}
