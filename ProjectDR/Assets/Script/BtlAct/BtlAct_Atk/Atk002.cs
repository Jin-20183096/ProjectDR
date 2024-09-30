using UnityEngine;

[CreateAssetMenu(fileName = "Atk002", menuName = "ScrObj/BtlAct_Atk/001~025/Atk002", order = 2)]   //Ŀ���� �޴� �߰�

public class Atk002 : BtlActData
{
    //Atk002: ����

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
