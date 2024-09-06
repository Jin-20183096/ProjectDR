using UnityEngine;

[CreateAssetMenu(fileName = "BtlAct006", menuName = "ScrObj/BtlAct000~025/BtlAct006", order = 6)]   //Ŀ���� �޴� �߰�

public class BtlAct006 : BtlAct_Atk
{
    //006 ����ġ��

    public override void Effect_Pre(bool isP, BattleSystem btlSys)
    {
        //����ġ�� ȿ���� ���� ù ��° �ֻ��� ���� ���տ� �ѹ� �� ����

        //�ൿ ȿ�� ó�� ���� (���� �ý����� ȿ�� ó�� ���� �ߴ�)
        btlSys.Set_EffectProcess(true);

        //���� �۵�
        var X = isP ? btlSys.P_RESULT[0] : btlSys.E_RESULT[0];

        if (isP)
            btlSys.Change_DiceTotal_Player(true, X);
        else
            btlSys.Change_DiceTotal_Enemy(true, X);

        //�α� ���
        //����ġ�� ȿ���� �ֻ��� ������ X��ŭ �þ��.
        btlSys.SetLog_ActEffect(isP, Name, null, "�ֻ��� ������ " + X + " ���Ѵ�");
    }

    public override void Effect_Post(bool isP, BattleSystem btlSys)
    {
        //������ ��������, �ڽ��� �ֻ��� ������ŭ ���� �����.

        if (isP ? btlSys.E_HIT_DGE : btlSys.P_HIT_DGE)
        {
            //�ൿ ȿ�� ó�� ���� (���� �ý����� ȿ�� ó�� ���� �ߴ�)
            btlSys.Set_EffectProcess(true);

            //���� �۵�
            var Y = isP ? btlSys.P_DICE : btlSys.E_DICE;

            if (isP)
                btlSys.Change_Ac_Player(false, Y);
            else
                btlSys.Change_Ac_Enemy(false, Y);

            // ���� �� ����ÿ� ���� �����ǵ��� ����� �����ؾ���

            //�α� ���
            //����ġ�� ȿ���� ������ ������ �ൿ���� X �Ҵ´�.
            btlSys.SetLog_ActEffect(isP, Name, null, "������ ������ ���� " + Y + " �����");
        }
    }
}
