using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameLog : MonoBehaviour
{
    public enum LogSituation
    {
        No, EvntStart, BtlTurnStart, ActEffect, BtlFlow, RunEnd, EvntSuccess, EvntFail, EvntResult
    }

    [SerializeField]
    private BattleSystem _btlSys;           //전투 시스템
    [SerializeField]
    private DungeonEventSystem _evntSys;    //이벤트 시스템

    [SerializeField]
    private TextMeshProUGUI _log;   //로그 텍스트
    [SerializeField]
    private GameObject _cursor_log; //로그 커서
    private Button _btn_log;     //로그의 버튼 인식

    private string _p_name = "당신";
    [SerializeField]
    private string _e_name; //적 이름

    private int _printTime = 20;      //초당 출력 글자 수

    [SerializeField]
    [TextArea(3, 5)]
    private string _targetText;                     //출력되야할 전체 텍스트
    private int _targetIndex;                       //출력할 텍스트 중에서 현재 인덱스
    private LogSituation _logSituation;   //로그가 출력된 상황

    private void Awake()
    {
        _btn_log = GetComponent<Button>();
    }

    public void LogErase()
    {
        _log.text = "";   //로그 지우기
        CurSor_OnOff(false);
    }

    public void SetLog_EventStart(string log)   //로그 설정: 이벤트 시작
    {
        NewLog(log);
        LogPrint_Start(LogSituation.EvntStart);
    }

    public void SetLog_DiceCheck_Success(string log)    //로그 설정: 이벤트 주사위 체크 성공
    {
        NewLog(log);
        LogPrint_Start(LogSituation.EvntSuccess);
    }

    public void SetLog_DiceCheck_Fail(string log)       //로그 설정: 이벤트 주사위 체크 실패
    {
        NewLog(log);
        LogPrint_Start(LogSituation.EvntFail);
    }

    public void SetLog_EventResult(string log)          //로그 설정: 이벤트 결과
    {
        Debug.Log("이벤트 로그");
        _evntSys.Set_ResultProcess(true);
        NewLog(log);
        LogPrint_Start(LogSituation.EvntResult);
    }

    public void SetLog_BattleStart(string e_name)   //로그 설정: 전투 시작 
    {
        NewLog(MakeSentence(e_name, "[이/가]","나타났다"));
        LogPrint_Start(LogSituation.BtlTurnStart);
        _e_name = e_name;
    }

    public void SetLog_TurnStart(string log) //로그 설정: 턴 시작
    {
        var finalLog = log;

        if (finalLog == "")
            finalLog = "당신은 무엇을 할까?";

        NewLog(finalLog);
        LogPrint_Start(LogSituation.BtlTurnStart);
    }

    public void SetLog_AtkHit(bool isP, string targetPost, string effText) //로그 설정: 공격 명중 효과 출력
    {
        //당신의 공격이 명중해, ~~

        if (targetPost == null)
            NewLog(MakeSentence((isP ? _p_name : _e_name) + "의 공격이 명중해,\n", effText));
        else
            NewLog(MakeSentence((isP ? _p_name : _e_name) + "의 공격이 명중해,\n",
                                (isP ? _e_name : _p_name), targetPost, effText));

        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_AtkDmg(bool isP, string targetPost, string effText) //로그 설정: 공격 피해 주었을 때 효과 출력
    {
        //당신의 공격이 상대에게 피해를 주어, ~~

        if (targetPost == null)
            NewLog(MakeSentence((isP ? _p_name : _e_name) + "의 공격이", (isP ? _e_name : _p_name) + "에게 피해를 주어,\n",
                                effText));
        else
            NewLog(MakeSentence((isP ? _p_name : _e_name) + "의 공격이", (isP ? _e_name : _p_name) + "에게 피해를 주어,\n",
                                (isP ? _e_name : _p_name), targetPost, effText));

        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_AtkBlocked(bool isP, string targetPost, string effText) //로그 설정: 공격이 막혔을 때 효과 출력
    {
        //당신의 공격이 막혀, ~~

        if (targetPost == null)
            NewLog(MakeSentence((isP ? _p_name : _e_name) + "의 공격이 막혀,\n", effText));
        else
            NewLog(MakeSentence((isP ? _p_name : _e_name) + "의 공격이 막혀,\n",
                                (isP ? _e_name : _p_name), targetPost, effText));
            
        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_AtkMissed(bool isP, string targetPost, string effText)  //로그 설정: 공격이 빗나갔을 때 효과 출력
    {
        //당신의 공격이 빗나가, ~~

        if (targetPost == null)
            NewLog(MakeSentence((isP ? _p_name : _e_name) + "의 공격이 빗나가,\n", effText));
        else
            NewLog(MakeSentence((isP ? _p_name : _e_name) + "의 공격이 빗나가,\n",
                                (isP ? _e_name : _p_name), targetPost, effText));

        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_DefEffect(bool isP, string targetPost, string effText)  //로그 설정: 방어 효과 출력
    {
        //당신은 공격을 방어해, ~~

        if (targetPost == null)
            NewLog(MakeSentence((isP ? _p_name : _e_name), "[은/는]", "공격을 방어해,\n", effText));
        else
            NewLog(MakeSentence((isP ? _p_name : _e_name), "[은/는]", "공격을 방어해,\n",
                                (isP ? _e_name : _p_name), targetPost, effText));
        
        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_DefEffect_NoAtk(bool isP, string targetPost, string effText)    //로그 설정: 상대가 공격하지 않았을 때 방어 효과 출력
    {
        //상대가 당신을 공격하지 않아, ~~

        if (targetPost == null)
            NewLog(MakeSentence((isP ? _e_name : _p_name), "[이/가]", "공격하지 않아,\n", effText));
        else
            NewLog(MakeSentence((isP ? _e_name : _p_name), "[이/가]", "공격하지 않아,\n",
                                (isP ? _p_name : _e_name), targetPost, effText));

        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_DefEffect_Wait(bool isP, string targetPost, string effText)     //로그 설정: 상대가 대기했을 떄 방어 효과 출력
    {
        //상대가 상황을 지켜볼 때, ~~

        if (targetPost == null)
            NewLog(MakeSentence((isP ? _e_name : _p_name), "[이/가]", "상황을 지켜볼 때,\n", effText));
        else
            NewLog(MakeSentence((isP ? _e_name : _p_name), "[이/가]", "상황을 지켜볼 때,\n",
                                (isP ? _p_name : _e_name), targetPost, effText));

        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_DgeEffect(bool isP, string targetPost, string effText)  //로그 설정: 회피 효과 출력
    {
        //당신은 공격을 피해, ~~

        if (targetPost == null)
            NewLog(MakeSentence((isP ? _p_name : _e_name), "[은/는]", "공격을 피해,\n", effText));
        else
            NewLog(MakeSentence((isP ? _p_name : _e_name), "[은/는]", "공격을 피해,\n",
                                (isP ? _e_name : _p_name), targetPost, effText));

        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_DgeEffect_Fail(bool isP, string targetPost, string effText) //로그 설정: 회피 조건 실패 시 효과 출력
    {
        //당신은 공격을 피하지 못해, ~~

        if (targetPost == null)
            NewLog(MakeSentence((isP ? _p_name : _e_name), "[은/는]", "공격을 피하지 못해,\n", effText));
        else
            NewLog(MakeSentence((isP ? _p_name : _e_name), "[은/는]", "공격을 피하지 못해,\n",
                                (isP ? _e_name : _p_name), targetPost, effText));

        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_DgeEffect_NoAtk(bool isP, string targetPost, string effText)    //로그 설정: 상대가 공격하지 않았을 때 회피 효과 출력
    {
        //상대가 공격하지 않아, ~~

        if (targetPost == null)
            NewLog(MakeSentence((isP ? _e_name : _p_name), "[이/가]", "공격하지 않아,\n", effText));
        else
            NewLog(MakeSentence((isP ? _e_name : _p_name), "[이/가]", "공격하지 않아,\n",
                                (isP ? _p_name : _e_name), targetPost, effText));

        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_AbilityEffect(bool isP, string ability, string targetPost, string effText)  //로그 설정: 능력 효과 출력
    {
        if (targetPost == null)
            NewLog(MakeSentence((isP ? _p_name : _e_name), "[은/는]", "\"" + ability + "\"의 효과로\n", effText));
        else
            NewLog(MakeSentence((isP ? _p_name : _e_name), "[은/는]", "\"" + ability + "\"의 효과로\n",
                                (isP ? _e_name : _p_name), targetPost, effText));

        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_RunAct(bool isP, bool success) //로그 설정: 도망 행동
    {
        if (success)
            NewLog(MakeSentence((isP ? _p_name : _e_name), "[은/는]", success ? "도망쳤다" : "도망에 실패했다"));

        LogPrint_Start(LogSituation.RunEnd);
    }

    public void SetLog_BattleFlow(string text)      //로그 설정: 전투 처리
    {
        NewLog(text);
        LogPrint_Start(LogSituation.BtlFlow);
    }

    public void SetLog_BattleEnd(bool isPWin)
    {
        if (isPWin)
            NewLog(MakeSentence(_e_name, "[은/는]", "쓰러졌다"));
        else
            NewLog(MakeSentence(_p_name, "[은/는]", "사망했다"));

        LogPrint_Start(LogSituation.No);
    }

    public string Log_AtkDmg(bool isP, int final_dmg)   //공격으로 피해 발생
    {
        //당신은 고블린을 공격해 X 피해를 주었다
        return MakeSentence((isP ? _p_name : _e_name), "[은/는]",
                (isP ? _e_name : _p_name), "[을/를]", "공격해", final_dmg.ToString(), "피해를 주었다");
    }

    public string Log_Def(bool isP, int final_dmg)  //공격 방어
    {
        //<받은 피해 1 이상>	당신은 공격을 막아 X 피해만 받았다
        //<받은 피해 0>		당신은 공격을 막아 피해를 받지 않았다
        if (final_dmg >= 1) //받은 피해가 1 이상
            return MakeSentence((isP ? _p_name : _e_name), "[은/는]", "공격을 막아,", final_dmg.ToString(), "피해만 받았다");
        else
            return MakeSentence((isP ? _p_name : _e_name), "[은/는]", "공격을 막아, 피해를 받지 않았다");
    }

    public string Log_DefFail(bool isP) //방어 실패
    {
        //당신은 공격을 막으려 했지만 고블린은 공격하지 않았다
        return MakeSentence((isP ? _p_name : _e_name), "[은/는]", "공격을 막으려 했지만,",
                (isP ? _e_name : _p_name), "[은/는]", "공격하지 않았다");
    }

    public string Log_Dge(bool isP, bool success, int final_dmg)    //회피
    {
        //<회피 성공>	당신은 공격을 피했다
        //<회피 실패>	당신은 공격을 피하지 못하고 X 피해를 받았다
        if (success)    //회피 성공
            return MakeSentence((isP ? _p_name : _e_name), "[은/는]", "공격을 피했다");
        else
            return MakeSentence((isP ? _p_name : _e_name), "[은/는]", "공격을 피하지 못하고,",
                    final_dmg.ToString(), "피해를 받았다");
    }

    public string Log_DgeFail(bool isP) //회피 실패
    {
        //당신은 공격을 피하려 했지만 고블린은 공격하지 않았다
        return MakeSentence((isP ? _p_name : _e_name), "[은/는]", "공격을 피하려 했지만,",
                (isP ? _e_name : _p_name), "[은/는]", "공격하지 않았다");
    }

    public string Log_TacCant(bool isP, string actName) //전술 불가
    {
        //당신은 <> 할 수 없다
        return MakeSentence(isP ? _p_name : _e_name, "\"" + actName + "\"", "할 수 없다");
    }

    public string Log_Wait(bool isP, int getAp)
    {
        //당신은 상황을 지켜보면서, 행동력을 X 얻었다.
        return MakeSentence((isP ? _p_name : _e_name), "[은/는]", "상황을 지켜보면서, 행동력을", getAp.ToString(), "얻었다");
    }

    string MakeSentence(params string[] arr)    //인자로 주어진 단어들을 나열해서 문장 생성
    {
        var s = ""; //초기 문장

        for (int i = 0; i < arr.Length; i++)
        {
            if (i == 0) //첫 단어면 앞을 띄우지 않음
                s += arr[i];
            //종성에 따라 달라지는 조사가 있을 경우 띄우지 않음
            else if (arr[i][0] == '[' && arr[i][arr[i].Length - 1] == ']')
            {
                string str;

                if (arr[i].Length <= 2)
                    str = "";
                else
                {
                    var prevWordLast = arr[i - 1][arr[i - 1].Length - 1];   //앞 단어의 마지막 문자
                    var thisWord = arr[i].Substring(1, arr[i].Length - 2);  //조사 선택지
                    string[] split = thisWord.Split('/');   //조사 선택지를 /를 기준으로 둘로 분리
                    var case1 = split[0];   //조사 선택지 1
                    var case2 = split[1];   //조사 선택지 2

                    str = CheckConsonant_Korean(prevWordLast, case1, case2);
                }

                s += str;
            }
            else    //이외에는 앞 단어와의 띄어쓰기를 추가
                s += " " + arr[i];

            if (i >= arr.Length - 1)    //마지막 단어라면
                s += ".";      //단어 뒤에 마침표 추가
        }

        return s;
    }

    void NewLog(string newLog)
    {
        _targetText = newLog;
        CurSor_OnOff(false);
        _btn_log.interactable = true;
    }

    void LogPrint_Start(LogSituation situ)
    {
        //이전 로그 출력 코루틴 정지
        StopCoroutine(LogPrinting());

        _log.text = "";
        CurSor_OnOff(false);

        _targetIndex = 0;
        _logSituation = situ;

        StartCoroutine(LogPrinting());
    }

    IEnumerator LogPrinting()
    {
        if (_targetText != "")
        {
            _log.text += _targetText[_targetIndex];
            _targetIndex++;
        }

        yield return new WaitForSeconds(1f / _printTime);

        if (_log.text != _targetText)
            StartCoroutine(LogPrinting());
        else
        {
            //출력할 로그가 없으면, 로그 출력 종료까지 자동 실행
            if (_targetText == "" ||
                _logSituation == LogSituation.No ||
                _logSituation == LogSituation.EvntStart ||
                _logSituation == LogSituation.BtlTurnStart)
                LogPrint_End();
            else
                CurSor_OnOff(true);
        }
    }

    public void LogClick()
    {
        if (_log.text != _targetText)
            _log.text = _targetText;
        else
            LogPrint_End();
    }

    void CurSor_OnOff(bool b)
    {
        _cursor_log.SetActive(b);
    }

    void LogPrint_End()
    {
        CurSor_OnOff(false);
        _btn_log.interactable = false;

        //로그의 출력 상황에 따라 출력 후 처리 분류
        switch (_logSituation)
        {
            case LogSituation.EvntStart:
                _evntSys.EvntActList_OnOff(true);   //이벤트 시작(로그 출력이 완료된 시점에, 이벤트 행동 목록 출력)
                break;
            case LogSituation.BtlTurnStart:
                _btlSys.BtlActList_OnOff(true); //전투 턴 시작(로그 출력이 완료된 시점에, 전투 행동 목록 출력)
                break;
            case LogSituation.ActEffect:        //행동 효과 처리 로그 출력
                _btlSys.Set_EffectProcess(false);
                break;
            case LogSituation.BtlFlow:          //전투 처리 로그 출력
                _btlSys.Set_BattleProcess(false);
                break;
            case LogSituation.EvntSuccess:      //이벤트 성공 결과
                _evntSys.StartCoroutine(_evntSys.EventResultFlow(true));
                break;
            case LogSituation.EvntFail:      //이벤트 실패 결과 (값 미달 실패)
                _evntSys.StartCoroutine(_evntSys.EventResultFlow(false));
                break;
            case LogSituation.EvntResult:       //이벤트 결과 처리 종료
                _evntSys.Set_ResultProcess(false);
                break;
        }
    }

    bool LastConsonant_Korean(char last)
    {
        //한글이 아니면 종성이 없다고 알림
        if (last < 44032 || last > 55215)
            return false;

        int ch = last;

        ch -= 44032;    //한글의 시작 위치 44032를 뺌

        //28로 나눠서 나머지가 존재하면 종성으로 판단(28번째마다 종성이 없는 글자가 나오므로)
        return (ch % 28) > 0;
    }

    string CheckConsonant_Korean(char prevLast, string case1, string case2)
    {
        if (LastConsonant_Korean(prevLast)) //이전 단어의 마지막 문자에 종성이 있다면
            return case1;
        else    //종성이 없다면
            return case2;
    }
}
