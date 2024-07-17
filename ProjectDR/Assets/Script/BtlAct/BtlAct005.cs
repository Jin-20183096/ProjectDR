using UnityEngine;

[CreateAssetMenu(fileName = "BtlAct005", menuName = "ScrObj/BtlAct000~025/BtlAct005", order = 5)]   //Ŀ���� �޴� �߰�

public class BtlAct005 : BtlActData
{
    //005 ������

    int x = 0;

    public override void Effect_Pre(bool isP, BattleSystem btlSys)
    {
        //������ ȿ���� ���� ù ��° �ֻ��� ���� ���տ��� ������

        //�ൿ ȿ�� ó�� ���� (���� �ý����� ȿ�� ó�� ���� �ߴ�)
        btlSys.Set_EffectProcess(true);

        //���� �۵�
        x = isP ? btlSys.P_RESULT[0] : btlSys.E_RESULT[0];

        if (isP)
            btlSys.Change_DiceTotal_Player(false, x);
        else
            btlSys.Change_DiceTotal_Enemy(false, x);

        //�α� ���
        //������ ȿ���� �ֻ��� ������ X��ŭ �پ���.
        btlSys.SetLog_ActEffect(isP, Name, null, "�ֻ��� ������ " + x + " ���δ�");
    }

    public override void Effect_Post(bool isP, BattleSystem btlSys)
    {
        //�������� ���ظ� �ָ�, ���� �� ����� ���� X��ŭ �����.

        if (isP ? btlSys.P_HIT_ATK : btlSys.E_HIT_ATK)
        {
            //�ൿ ȿ�� ó�� ���� (���� �ý����� ȿ�� ó�� ���� �ߴ�)
            btlSys.Set_EffectProcess(true);

            //���� �۵�
            if (isP)
                btlSys.Change_Ac_Enemy(false, x);
            else
                btlSys.Change_Ac_Player(false, x);

            //�ٴ��� �Ͽ� ���� ������ �� �ֵ��� ������� �����ؾ���

            //�α� ���
            //������ ȿ���� ���� �� ����� ���� x �����.
            btlSys.SetLog_ActEffect(isP, Name, "[��/��]", "���� �� ���� " + x + " �����");
        }
    }
}
