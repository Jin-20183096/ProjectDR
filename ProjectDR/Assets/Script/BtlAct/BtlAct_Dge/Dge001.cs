using UnityEngine;

[CreateAssetMenu(fileName = "Dge001", menuName = "ScrObj/BtlAct_Dge/000~025/Dge001", order = 1)]   //커스텀 메뉴 추가

public class Dge001 : BtlActData
{
    //Dge001: 회피

    //회피 조건 체크
    public override bool Dodge_Check(bool isP, BattleSystem btlSys)
    {
        //회피 조건: 주사위 총합 >= 상대 주사위 총합
        if (isP)
            return btlSys.P_TOTAL >= btlSys.E_TOTAL;
        else
            return btlSys.E_TOTAL >= btlSys.P_TOTAL;
    }

    public override void Effect_Post(bool isP, BattleSystem btlSys)
    {
        //회피 성공 시: 소모한 행동력을 돌려받는다.
        if (isP ? btlSys.P_HIT_DGE : btlSys.E_HIT_DGE)
        {
            //행동 효과 처리 시작 (전투 시스템의 효과 처리 루프 중단)
            btlSys.Set_EffectProcess(true);

            //실제 작동
            var myDice = isP ? btlSys.P_DICE : btlSys.E_DICE;

            if (isP)
                btlSys.Change_Ap_Player(true, myDice);
            else
                btlSys.Change_Ap_Enemy(true, myDice);

            //로그 출력
            btlSys.SetLog_DgeEffect(isP, null, "소모한 행동력을 돌려받았다");
        }
    }
}
