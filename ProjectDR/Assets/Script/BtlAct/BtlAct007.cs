using UnityEngine;

[CreateAssetMenu(fileName = "BtlAct007", menuName = "ScrObj/BtlAct000~025/BtlAct007", order = 7)]   //Ŀ���� �޴� �߰�

public class BtlAct007 : BtlActData
{
    //007 �ָ� ���

    int x = 0;

    public override void Effect_Pre(bool isP, BattleSystem btlSys)
    {
        //�ָ� ��� ȿ���� ���� ù ��° �ֻ��� ���� ���տ��� ������

        //�ൿ ȿ�� ó�� ���� (���� �ý����� ȿ�� ó�� ���� �ߴ�)
        btlSys.Set_EffectProcess(true);

        //���� �۵�
        x = isP ? btlSys.P_RESULT[0] : btlSys.E_RESULT[0];

        if (isP)
        {
            btlSys.Change_DiceTotal_Player(false, x);
            btlSys.Change_Ac_Player(true, x);
        }
        else
        {
            btlSys.Change_DiceTotal_Enemy(false, x);
            btlSys.Change_Ac_Enemy(true, x);
        }

        //�α� ���
        //�ָ� ��� ȿ���� �ֻ��� ������ X��ŭ �پ���, �׸�ŭ ���� ���δ�
        btlSys.SetLog_ActEffect(isP, Name, null, "�ֻ��� ������ " + x + " ���̰�, ���� ��ŭ ���� ���δ�");
    }

    public override void Effect_Post(bool isP, BattleSystem btlSys)
    {
        //�ൿ�� ���� �� ���󺹱�

        //�ൿ ȿ�� ó�� ���� (���� �ý����� ȿ�� ó�� ���� �ߴ�)
        btlSys.Set_EffectProcess(true);

        //���� �۵�
        if (isP)
            btlSys.Change_Ac_Player(false, x);
        else
            btlSys.Change_Ac_Enemy(false, x);

        //�α� ���
        //�ָ� ��� ȿ���� �ֻ��� ������ X��ŭ �پ���, �׸�ŭ ���� ���δ�
        btlSys.SetLog_ActEffect(isP, Name, null, "���� ������� ���ƿԴ�");
    }
}
