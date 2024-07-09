using UnityEngine;

[CreateAssetMenu(fileName = "BtlAct002", menuName = "ScrObj/BtlAct000~025/BtlAct002", order = 2)]   //커스텀 메뉴 추가

public class BtlAct002 : BtlActData
{
    //002 방어

    public override void Effect_Post(bool isP, BattleSystem btlSys)
    {
        //상대가 대기하면, 행동력 돌려받기
        if (isP ? btlSys.E_HIT_WAIT : btlSys.P_HIT_WAIT)
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
            btlSys.SetLog_DefEffect_Wait(isP, "소모한 행동력을 돌려받았다");
        }
    }
}
