using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameLog : MonoBehaviour
{
    public enum LogSituation
    {
        No, ActEffect, BtlFlow, RunEnd, BtlEnd
    }

    [SerializeField]
    private BattleSystem _btlSys;           //전투 시스템

    [SerializeField]
    private TextMeshProUGUI _log;   //로그 텍스트
    [SerializeField]
    private GameObject _cursor_log; //로그 커서
    private Button _btn_cursor;     //커서 출력 중 로그의 버튼 인식

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
        _btn_cursor = GetComponent<Button>();
    }

    public void LogErase()
    {
        _log.text = "";   //로그 지우기
        CurSor_OnOff(false);
    }

    public void SetLog_BattleStart(string e_name)   //로그 설정: 전투의 시작 
    {
        NewLog(MakeSentence(e_name, "나타났다"));
        LogPrint_Start(LogSituation.No);
        _e_name = e_name;
    }

    public void SetLog_DefEffect(bool isP, string effText)  //로그 설정: 방어 효과 출력
    {
        NewLog(MakeSentence((isP ? _p_name : _e_name), "공격을 방어해,", effText));
        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_DgeEffect(bool isP, string effText)  //로그 설정: 회피 효과 출력
    {
        NewLog(MakeSentence((isP ? _p_name : _e_name), "공격을 피해,", effText));
        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_ActEffect(bool isP, string actName, string effText)  //로그 설정: 행동 효과 출력
    {
        NewLog(MakeSentence((isP ? _p_name : _e_name), "\"" + actName + "\"의 효과로", effText));
        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_RunAct(bool isP, bool success) //로그 설정: 도망 행동
    {
        if (success)
            NewLog(MakeSentence((isP ? _p_name : _e_name), success ? "도망쳤다" : "도망에 실패했다"));

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
            NewLog(MakeSentence(_e_name, "쓰러졌다"));
        else
            NewLog(MakeSentence(_p_name, "사망했다"));

        LogPrint_Start(LogSituation.No);
    }

    public string Log_AtkDmg(bool isP, int final_dmg)   //공격으로 피해 발생
    {
        //당신은 고블린을 공격해 X 피해를 주었다
        return MakeSentence((isP ? _p_name : _e_name), (isP ? _e_name : _p_name), "공격해", final_dmg.ToString(), "피해를 주었다");
    }

    public string Log_Def(bool isP, int final_dmg)  //공격 방어
    {
        //<받은 피해 1 이상>	당신은 공격을 막아 X 피해만 받았다
        //<받은 피해 0>		당신은 공격을 막아 피해를 받지 않았다
        if (final_dmg >= 1) //받은 피해가 1 이상
            return MakeSentence((isP ? _p_name : _e_name), "공격을 막아,", final_dmg.ToString(), "피해만 받았다");
        else
            return MakeSentence((isP ? _p_name : _e_name), "공격을 막아, 피해를 받지 않았다");
    }

    public string Log_DefFail(bool isP) //방어 실패
    {
        //당신은 공격을 막으려 했지만 고블린은 공격하지 않았다
        return MakeSentence((isP ? _p_name : _e_name), "공격을 막으려 했지만,", (isP ? _e_name : _p_name), "공격하지 않았다");
    }

    public string Log_Dge(bool isP, bool success, int final_dmg)    //회피
    {
        //<회피 성공>	당신은 공격을 피했다
        //<회피 실패>	당신은 공격을 피하지 못하고 X 피해를 받았다
        if (success)    //회피 성공
            return MakeSentence((isP ? _p_name : _e_name), "공격을 피했다");
        else
            return MakeSentence((isP ? _p_name : _e_name), "공격을 피하지 못하고,", final_dmg.ToString(), "피해를 받았다");
    }

    public string Log_DgeFail(bool isP) //회피 실패
    {
        //당신은 공격을 피하려 했지만 고블린은 공격하지 않았다
        return MakeSentence((isP ? _p_name : _e_name), "공격을 피하려 했지만,", (isP ? _e_name : _p_name), "공격하지 않았다");
    }

    public string Log_TacCant(bool isP, string actName) //전술 불가
    {
        //당신은 <> 할 수 없다
        return MakeSentence(isP ? _p_name : _e_name, "\"" + actName + "\"", "할 수 없다");
    }

    public string Log_Wait(bool isP, int getAp)
    {
        //당신은 상황을 지켜보면서, 행동력을 X 얻었다.
        return MakeSentence((isP ? _p_name : _e_name), "상황을 지켜보면서, 행동력을", getAp.ToString(), "얻었다");
    }

    string MakeSentence(params string[] arr)    //인자로 주어진 단어들을 나열해서 문장 생성
    {
        var s = ""; //초기 문장

        for (int i = 0; i < arr.Length; i++)
        {
            if (i < arr.Length - 1) //마지막 단어가 아니면
                s += arr[i] + " ";  //단어 뒤에 띄어주기
            else                    //마지막 단어라면
                s += arr[i] + ".";  //단어 뒤에 마침표 추가
        }

        return s;
    }

    void NewLog(string newLog)
    {
        _targetText = newLog;
        CurSor_OnOff(false);
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
            if (_targetText == "" || _logSituation == LogSituation.No)  //출력할 로그가 없으면, 로그 출력 종료까지 자동 실행
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
        => _cursor_log.SetActive(b);

    void LogPrint_End()
    {
        CurSor_OnOff(false);

        //로그의 출력 상황에 따라 출력 후 처리 분류
        switch (_logSituation)
        {
            case LogSituation.ActEffect:        //행동 효과 처리 로그 출력
                _btlSys.Set_EffectProcess(false);
                break;
            case LogSituation.BtlFlow:          //전투 처리 로그 출력
                _btlSys.Set_BattleProcess(false);
                break;
            case LogSituation.RunEnd:           //도망으로 전투 종료 로그
                //_btlSys.StartCoroutine(_btlSys.BattleFlow_End());
                break;
            case LogSituation.BtlEnd:

                break;
        }
    }
}
