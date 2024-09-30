using UnityEngine;

[CreateAssetMenu(fileName = "Atk003", menuName = "ScrObj/BtlAct_Atk/001~025/Atk003", order = 3)]   //커스텀 메뉴 추가

public class Atk003 : BtlActData
{
    //Atk003: 타격

    //공격: 주사위 총합
    public override int Calc_Total(int[] my_rslt)
    {
        var total = 0;  //반환할 총합

        for (int i = 0; i < my_rslt.Length; i++)
        {
            if (my_rslt[i] != -1)
                total += my_rslt[i];
        }

        return total;
    }
}
