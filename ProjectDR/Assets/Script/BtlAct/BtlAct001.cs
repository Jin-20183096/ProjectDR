using UnityEngine;

[CreateAssetMenu(fileName = "BtlAct001", menuName = "ScrObj/BtlAct000~025/BtlAct001", order = 1)]   //커스텀 메뉴 추가

public class BtlAct001 : BtlActData
{
    //001 공격

    /*
    public override void Effect_Post(bool isP, BattleSystem btlSys)
    {
        //공격이 막히면, 행동력 잃기
        if (isP ? btlSys.E_HIT_DEF : btlSys.P_HIT_DEF)
        {
            //행동 효과 처리 시작 (전투 시스템의 효과 처리 루프 중단)
            btlSys.Set_EffectProcess(true);

            //실제 작동
            var enDice = isP ? btlSys.E_DICE : btlSys.P_DICE;

            if (isP)
                btlSys.Change_Ap_Player(false, enDice);
            else
                btlSys.Change_Ap_Enemy(false, enDice);

            //로그 출력
            btlSys.SetLog_AtkBlocked(isP, enDice + " 행동력을 소모했다");
        }
    }
    */
}
