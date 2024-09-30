using UnityEngine;

[CreateAssetMenu(fileName = "Atk004", menuName = "ScrObj/BtlAct_Atk/001~025/Atk004", order = 4)]   //Ŀ���� �޴� �߰�

public class Atk004 : BtlActData
{
    //Atk004: ���

    //����: �ֻ��� ����
    public override int Calc_Total(int[] my_rslt)
    {
        var total = 0;  //��ȯ�� ����

        for (int i = 0; i < my_rslt.Length; i++)
        {
            if (my_rslt[i] != -1)
                total += my_rslt[i];
        }

        return total;
    }
}
