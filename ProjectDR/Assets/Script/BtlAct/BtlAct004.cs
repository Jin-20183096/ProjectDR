using UnityEngine;

[CreateAssetMenu(fileName = "BtlAct004", menuName = "ScrObj/BtlAct000~025/BtlAct004", order = 3)]   //커스텀 메뉴 추가

public class BtlAct004 : BtlActData
{
    //004 대기

    /*
    public override void Effect_Pre(bool isP, BattleSystem btlSys)
    {
        //행동 효과 처리 시작 (전투 시스템의 효과 처리 루프 중단)
        //없음

        //실제 작동
        if (isP)
            btlSys.Change_Ac_Player(false, 5);
        else
            btlSys.Change_Ac_Enemy(false, 5);

        //로그 출력
        //없음
    }

    public override void Effect_Post(bool isP, BattleSystem btlSys)
    {
        //행동 효과 처리 시작 (전투 시스템의 효과 처리 루프 중단)
        //없음

        //실제 작동
        if (isP)
            btlSys.Change_Ac_Player(true, 5);
        else
            btlSys.Change_Ac_Enemy(true, 5);

        //로그 출력
        //없음
    }
    */
}
