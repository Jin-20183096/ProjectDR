using UnityEngine;

[CreateAssetMenu(fileName = "Def001", menuName = "ScrObj/BtlAct_Def/000~025/Def001", order = 1)]

public class Def001 : BtlAct_Def
{
    //Def001: 막기

    public override void Effect_Post(bool isP, BattleSystem btlSys)
    {
        //방어 성공 시: 상대는 행동력을 x만큼 잃음 (x: 자신의 주사위)
        if (isP ? btlSys.P_HIT_DEF : btlSys.E_HIT_DEF)
        {
            //행동 효과 처리 시작 (전투 시스템의 효과 처리 루프 중단)
            btlSys.Set_EffectProcess(true);

            //실제 작동
            var myDice = isP ? btlSys.P_DICE : btlSys.E_DICE;

            if (isP)
                btlSys.Change_Ap_Enemy(false, myDice);
            else
                btlSys.Change_Ap_Player(false, myDice);

            //로그 출력
            btlSys.SetLog_DefEffect(isP, "[은/는]", " 행동력을 " + myDice + " 잃었다");
        }
    }
}
