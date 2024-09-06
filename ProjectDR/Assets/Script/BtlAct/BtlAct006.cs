using UnityEngine;

[CreateAssetMenu(fileName = "BtlAct006", menuName = "ScrObj/BtlAct000~025/BtlAct006", order = 6)]   //커스텀 메뉴 추가

public class BtlAct006 : BtlAct_Atk
{
    //006 내려치기

    public override void Effect_Pre(bool isP, BattleSystem btlSys)
    {
        //내려치기 효과를 위해 첫 번째 주사위 눈을 총합에 한번 더 더함

        //행동 효과 처리 시작 (전투 시스템의 효과 처리 루프 중단)
        btlSys.Set_EffectProcess(true);

        //실제 작동
        var X = isP ? btlSys.P_RESULT[0] : btlSys.E_RESULT[0];

        if (isP)
            btlSys.Change_DiceTotal_Player(true, X);
        else
            btlSys.Change_DiceTotal_Enemy(true, X);

        //로그 출력
        //내려치기 효과로 주사위 총합이 X만큼 늘어난다.
        btlSys.SetLog_ActEffect(isP, Name, null, "주사위 총합을 " + X + " 더한다");
    }

    public override void Effect_Post(bool isP, BattleSystem btlSys)
    {
        //공격이 빗나가면, 자신의 주사위 개수만큼 방어도를 낮춘다.

        if (isP ? btlSys.E_HIT_DGE : btlSys.P_HIT_DGE)
        {
            //행동 효과 처리 시작 (전투 시스템의 효과 처리 루프 중단)
            btlSys.Set_EffectProcess(true);

            //실제 작동
            var Y = isP ? btlSys.P_DICE : btlSys.E_DICE;

            if (isP)
                btlSys.Change_Ac_Player(false, Y);
            else
                btlSys.Change_Ac_Enemy(false, Y);

            // 다음 턴 종료시에 방어도가 복구되도록 디버플 설정해야함

            //로그 출력
            //내려치기 효과로 공격이 빗나가 행동력을 X 잃는다.
            btlSys.SetLog_ActEffect(isP, Name, null, "공격이 빗나가 방어도를 " + Y + " 낮춘다");
        }
    }
}
