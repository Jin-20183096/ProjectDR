using UnityEngine;

[CreateAssetMenu(fileName = "BtlAct007", menuName = "ScrObj/BtlAct000~025/BtlAct007", order = 7)]   //커스텀 메뉴 추가

public class BtlAct007 : BtlActData
{
    //007 멀리 찌르기

    int x = 0;

    public override void Effect_Pre(bool isP, BattleSystem btlSys)
    {
        //멀리 찌르기 효과를 위해 첫 번째 주사위 눈을 총합에서 차감함

        //행동 효과 처리 시작 (전투 시스템의 효과 처리 루프 중단)
        btlSys.Set_EffectProcess(true);

        //실제 작동
        x = isP ? btlSys.P_RESULT[0] : btlSys.E_RESULT[0];

        if (isP)
        {
            btlSys.Change_DiceTotal_Player(false, x);
            btlSys.Change_Ac_Player(true, x);
        }
        else
        {
            btlSys.Change_DiceTotal_Enemy(false, x);
            btlSys.Change_Ac_Enemy(true, x);
        }

        //로그 출력
        //멀리 찌르기 효과로 주사위 총합이 X만큼 줄어들고, 그만큼 방어도를 높인다
        btlSys.SetLog_ActEffect(isP, Name, null, "주사위 총합을 " + x + " 줄이고, 줄인 만큼 방어도를 높인다");
    }

    public override void Effect_Post(bool isP, BattleSystem btlSys)
    {
        //행동이 끝나 방어도 원상복구

        //행동 효과 처리 시작 (전투 시스템의 효과 처리 루프 중단)
        btlSys.Set_EffectProcess(true);

        //실제 작동
        if (isP)
            btlSys.Change_Ac_Player(false, x);
        else
            btlSys.Change_Ac_Enemy(false, x);

        //로그 출력
        //멀리 찌르기 효과로 주사위 총합이 X만큼 줄어들고, 그만큼 방어도를 높인다
        btlSys.SetLog_ActEffect(isP, Name, null, "방어도가 원래대로 돌아왔다");
    }
}
