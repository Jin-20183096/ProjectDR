using UnityEngine;

[CreateAssetMenu(fileName = "Tac001", menuName = "ScrObj/BtlAct_Tac/001~025/Tac001", order = 1)]   //커스텀 메뉴 추가

public class Tac001 : BtlActData
{
    //Tac001: 대기

    public override void Effect_Pre(bool isP, BattleSystem btlSys)
    {
        //행동력 2 회복

        //행동 효과 처리 시작 (전투 시스템의 효과 처리 루프 중단)
        btlSys.Set_EffectProcess(true);

        //실제 작동
        if (isP)
            btlSys.Change_Ap_Player(true, 2);
        else
            btlSys.Change_Ap_Enemy(true, 2);

        //로그 출력
        btlSys.SetLog_Wait(isP);
    }
}
