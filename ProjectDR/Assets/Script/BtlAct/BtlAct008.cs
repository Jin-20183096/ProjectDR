using UnityEngine;

[CreateAssetMenu(fileName = "BtlAct008", menuName = "ScrObj/BtlAct000~025/BtlAct008", order = 8)]   //커스텀 메뉴 추가

public class BtlAct008 : BtlActData
{
    //008 막기

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
            btlSys.SetLog_DefEffect(isP, "[은/는]"," 행동력을 " + myDice + " 잃었다");
        }
    }
}
