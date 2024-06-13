using UnityEngine;

[CreateAssetMenu(fileName = "BtlAct002", menuName = "ScrObj/BtlAct000~025/BtlAct002", order = 2)]   //커스텀 메뉴 추가

public class BtlAct002 : BtlActData
{
    //002 방어

    public override void Effect_Post(bool isP, BattleSystem btlSys)
    {
        //방어 성공 시: 상대는 행동력을 x만큼 소모 (x: 자신의 주사위)
        if (isP ? btlSys.P_HIT_DEF : btlSys.E_HIT_DEF)
        {
            //행동 효과 처리 시작 (전투 시스템의 효과 처리 일시정지)
            btlSys.Set_EffectProcess(true);

            //실제 작동
            var myDice = isP ? btlSys.P_DICE : btlSys.E_DICE;

            if (isP)
                btlSys.Change_Ap_Enemy(false, myDice);
            else
                btlSys.Change_Ap_Player(false, myDice);

            //로그 출력
            btlSys.SetLog_DefEffect(isP, btlSys.E_NAME + "행동력을 " + myDice + " 잃었다");
        }
    }
}
