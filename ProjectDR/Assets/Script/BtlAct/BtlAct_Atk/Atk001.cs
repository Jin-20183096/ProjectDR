using UnityEngine;

[CreateAssetMenu(fileName = "Atk001", menuName = "ScrObj/BtlAct_Atk/001~025/Atk001", order = 1)]   //커스텀 메뉴 추가

public class Atk001 : BtlActData
{
    //Atk001: 주먹질

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