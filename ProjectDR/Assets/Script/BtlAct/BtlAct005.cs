using UnityEngine;

[CreateAssetMenu(fileName = "BtlAct005", menuName = "ScrObj/BtlAct000~025/BtlAct005", order = 5)]   //커스텀 메뉴 추가

public class BtlAct005 : BtlActData
{
    //005 가르기

    int x = 0;

    public override void Effect_Pre(bool isP, BattleSystem btlSys)
    {
        //가르기 효과를 위해 첫 번째 주사위 눈을 총합에서 차감함

        //행동 효과 처리 시작 (전투 시스템의 효과 처리 루프 중단)
        btlSys.Set_EffectProcess(true);

        //실제 작동
        x = isP ? btlSys.P_RESULT[0] : btlSys.E_RESULT[0];

        if (isP)
            btlSys.Change_DiceTotal_Player(false, x);
        else
            btlSys.Change_DiceTotal_Enemy(false, x);

        //로그 출력
        //가르기 효과로 주사위 총합이 X만큼 줄어든다.
        btlSys.SetLog_ActEffect(isP, Name, null, "주사위 총합을 " + x + " 줄인다");
    }

    public override void Effect_Post(bool isP, BattleSystem btlSys)
    {
        //공격으로 피해를 주면, 다음 턴 상대의 방어도를 X만큼 낮춘다.

        if (isP ? btlSys.P_HIT_ATK : btlSys.E_HIT_ATK)
        {
            //행동 효과 처리 시작 (전투 시스템의 효과 처리 루프 중단)
            btlSys.Set_EffectProcess(true);

            //실제 작동
            if (isP)
                btlSys.Change_Ac_Enemy(false, x);
            else
                btlSys.Change_Ac_Player(false, x);

            //다다음 턴에 방어도가 복구될 수 있도록 디버프를 설정해야함

            //로그 출력
            //가르기 효과로 다음 턴 상대의 방어도를 x 낮춘다.
            btlSys.SetLog_ActEffect(isP, Name, "[의/의]", "다음 턴 방어도를 " + x + " 낮춘다");
        }
    }
}
