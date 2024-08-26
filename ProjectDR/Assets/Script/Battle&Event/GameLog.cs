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
    private BattleSystem _btlSys;           //���� �ý���
    [SerializeField]
    private DungeonEventSystem _evntSys;    //�̺�Ʈ �ý���

    [SerializeField]
    private TextMeshProUGUI _log;   //�α� �ؽ�Ʈ
    [SerializeField]
    private GameObject _cursor_log; //�α� Ŀ��
    private Button _btn_log;     //�α��� ��ư �ν�

    private string _p_name = "���";
    [SerializeField]
    private string _e_name; //�� �̸�

    private int _printTime = 20;      //�ʴ� ��� ���� ��

    [SerializeField]
    [TextArea(3, 5)]
    private string _targetText;                     //��µǾ��� ��ü �ؽ�Ʈ
    private int _targetIndex;                       //����� �ؽ�Ʈ �߿��� ���� �ε���
    private LogSituation _logSituation;   //�αװ� ��µ� ��Ȳ

    private void Awake()
    {
        _btn_log = GetComponent<Button>();
    }

    public void LogErase()
    {
        _log.text = "";   //�α� �����
        CurSor_OnOff(false);
    }

    public void SetLog_EventStart(string log)   //�α� ����: �̺�Ʈ ����
    {
        NewLog(log);
        LogPrint_Start(LogSituation.EvntStart);
    }

    public void SetLog_DiceCheck_Success(string log)    //�α� ����: �̺�Ʈ �ֻ��� üũ ����
    {
        NewLog(log);
        LogPrint_Start(LogSituation.EvntSuccess);
    }

    public void SetLog_DiceCheck_Fail(string log)       //�α� ����: �̺�Ʈ �ֻ��� üũ ����
    {
        NewLog(log);
        LogPrint_Start(LogSituation.EvntFail);
    }

    public void SetLog_EventResult(string log)          //�α� ����: �̺�Ʈ ���
    {
        Debug.Log("�̺�Ʈ �α�");
        _evntSys.Set_ResultProcess(true);
        NewLog(log);
        LogPrint_Start(LogSituation.EvntResult);
    }

    public void SetLog_BattleStart(string e_name)   //�α� ����: ���� ���� 
    {
        NewLog(MakeSentence(e_name, "[��/��]","��Ÿ����"));
        LogPrint_Start(LogSituation.BtlTurnStart);
        _e_name = e_name;
    }

    public void SetLog_TurnStart(string log) //�α� ����: �� ����
    {
        var finalLog = log;

        if (finalLog == "")
            finalLog = "����� ������ �ұ�?";

        NewLog(finalLog);
        LogPrint_Start(LogSituation.BtlTurnStart);
    }

    public void SetLog_AtkHit(bool isP, string targetPost, string effText) //�α� ����: ���� ���� ȿ�� ���
    {
        //����� ������ ������, ~~

        if (targetPost == null)
            NewLog(MakeSentence((isP ? _p_name : _e_name) + "�� ������ ������,\n", effText));
        else
            NewLog(MakeSentence((isP ? _p_name : _e_name) + "�� ������ ������,\n",
                                (isP ? _e_name : _p_name), targetPost, effText));

        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_AtkDmg(bool isP, string targetPost, string effText) //�α� ����: ���� ���� �־��� �� ȿ�� ���
    {
        //����� ������ ��뿡�� ���ظ� �־�, ~~

        if (targetPost == null)
            NewLog(MakeSentence((isP ? _p_name : _e_name) + "�� ������", (isP ? _e_name : _p_name) + "���� ���ظ� �־�,\n",
                                effText));
        else
            NewLog(MakeSentence((isP ? _p_name : _e_name) + "�� ������", (isP ? _e_name : _p_name) + "���� ���ظ� �־�,\n",
                                (isP ? _e_name : _p_name), targetPost, effText));

        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_AtkBlocked(bool isP, string targetPost, string effText) //�α� ����: ������ ������ �� ȿ�� ���
    {
        //����� ������ ����, ~~

        if (targetPost == null)
            NewLog(MakeSentence((isP ? _p_name : _e_name) + "�� ������ ����,\n", effText));
        else
            NewLog(MakeSentence((isP ? _p_name : _e_name) + "�� ������ ����,\n",
                                (isP ? _e_name : _p_name), targetPost, effText));
            
        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_AtkMissed(bool isP, string targetPost, string effText)  //�α� ����: ������ �������� �� ȿ�� ���
    {
        //����� ������ ������, ~~

        if (targetPost == null)
            NewLog(MakeSentence((isP ? _p_name : _e_name) + "�� ������ ������,\n", effText));
        else
            NewLog(MakeSentence((isP ? _p_name : _e_name) + "�� ������ ������,\n",
                                (isP ? _e_name : _p_name), targetPost, effText));

        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_DefEffect(bool isP, string targetPost, string effText)  //�α� ����: ��� ȿ�� ���
    {
        //����� ������ �����, ~~

        if (targetPost == null)
            NewLog(MakeSentence((isP ? _p_name : _e_name), "[��/��]", "������ �����,\n", effText));
        else
            NewLog(MakeSentence((isP ? _p_name : _e_name), "[��/��]", "������ �����,\n",
                                (isP ? _e_name : _p_name), targetPost, effText));
        
        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_DefEffect_NoAtk(bool isP, string targetPost, string effText)    //�α� ����: ��밡 �������� �ʾ��� �� ��� ȿ�� ���
    {
        //��밡 ����� �������� �ʾ�, ~~

        if (targetPost == null)
            NewLog(MakeSentence((isP ? _e_name : _p_name), "[��/��]", "�������� �ʾ�,\n", effText));
        else
            NewLog(MakeSentence((isP ? _e_name : _p_name), "[��/��]", "�������� �ʾ�,\n",
                                (isP ? _p_name : _e_name), targetPost, effText));

        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_DefEffect_Wait(bool isP, string targetPost, string effText)     //�α� ����: ��밡 ������� �� ��� ȿ�� ���
    {
        //��밡 ��Ȳ�� ���Ѻ� ��, ~~

        if (targetPost == null)
            NewLog(MakeSentence((isP ? _e_name : _p_name), "[��/��]", "��Ȳ�� ���Ѻ� ��,\n", effText));
        else
            NewLog(MakeSentence((isP ? _e_name : _p_name), "[��/��]", "��Ȳ�� ���Ѻ� ��,\n",
                                (isP ? _p_name : _e_name), targetPost, effText));

        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_DgeEffect(bool isP, string targetPost, string effText)  //�α� ����: ȸ�� ȿ�� ���
    {
        //����� ������ ����, ~~

        if (targetPost == null)
            NewLog(MakeSentence((isP ? _p_name : _e_name), "[��/��]", "������ ����,\n", effText));
        else
            NewLog(MakeSentence((isP ? _p_name : _e_name), "[��/��]", "������ ����,\n",
                                (isP ? _e_name : _p_name), targetPost, effText));

        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_DgeEffect_Fail(bool isP, string targetPost, string effText) //�α� ����: ȸ�� ���� ���� �� ȿ�� ���
    {
        //����� ������ ������ ����, ~~

        if (targetPost == null)
            NewLog(MakeSentence((isP ? _p_name : _e_name), "[��/��]", "������ ������ ����,\n", effText));
        else
            NewLog(MakeSentence((isP ? _p_name : _e_name), "[��/��]", "������ ������ ����,\n",
                                (isP ? _e_name : _p_name), targetPost, effText));

        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_DgeEffect_NoAtk(bool isP, string targetPost, string effText)    //�α� ����: ��밡 �������� �ʾ��� �� ȸ�� ȿ�� ���
    {
        //��밡 �������� �ʾ�, ~~

        if (targetPost == null)
            NewLog(MakeSentence((isP ? _e_name : _p_name), "[��/��]", "�������� �ʾ�,\n", effText));
        else
            NewLog(MakeSentence((isP ? _e_name : _p_name), "[��/��]", "�������� �ʾ�,\n",
                                (isP ? _p_name : _e_name), targetPost, effText));

        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_AbilityEffect(bool isP, string ability, string targetPost, string effText)  //�α� ����: �ɷ� ȿ�� ���
    {
        if (targetPost == null)
            NewLog(MakeSentence((isP ? _p_name : _e_name), "[��/��]", "\"" + ability + "\"�� ȿ����\n", effText));
        else
            NewLog(MakeSentence((isP ? _p_name : _e_name), "[��/��]", "\"" + ability + "\"�� ȿ����\n",
                                (isP ? _e_name : _p_name), targetPost, effText));

        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_RunAct(bool isP, bool success) //�α� ����: ���� �ൿ
    {
        if (success)
            NewLog(MakeSentence((isP ? _p_name : _e_name), "[��/��]", success ? "�����ƴ�" : "������ �����ߴ�"));

        LogPrint_Start(LogSituation.RunEnd);
    }

    public void SetLog_BattleFlow(string text)      //�α� ����: ���� ó��
    {
        NewLog(text);
        LogPrint_Start(LogSituation.BtlFlow);
    }

    public void SetLog_BattleEnd(bool isPWin)
    {
        if (isPWin)
            NewLog(MakeSentence(_e_name, "[��/��]", "��������"));
        else
            NewLog(MakeSentence(_p_name, "[��/��]", "����ߴ�"));

        LogPrint_Start(LogSituation.No);
    }

    public string Log_AtkDmg(bool isP, int final_dmg)   //�������� ���� �߻�
    {
        //����� ����� ������ X ���ظ� �־���
        return MakeSentence((isP ? _p_name : _e_name), "[��/��]",
                (isP ? _e_name : _p_name), "[��/��]", "������", final_dmg.ToString(), "���ظ� �־���");
    }

    public string Log_Def(bool isP, int final_dmg)  //���� ���
    {
        //<���� ���� 1 �̻�>	����� ������ ���� X ���ظ� �޾Ҵ�
        //<���� ���� 0>		����� ������ ���� ���ظ� ���� �ʾҴ�
        if (final_dmg >= 1) //���� ���ذ� 1 �̻�
            return MakeSentence((isP ? _p_name : _e_name), "[��/��]", "������ ����,", final_dmg.ToString(), "���ظ� �޾Ҵ�");
        else
            return MakeSentence((isP ? _p_name : _e_name), "[��/��]", "������ ����, ���ظ� ���� �ʾҴ�");
    }

    public string Log_DefFail(bool isP) //��� ����
    {
        //����� ������ ������ ������ ����� �������� �ʾҴ�
        return MakeSentence((isP ? _p_name : _e_name), "[��/��]", "������ ������ ������,",
                (isP ? _e_name : _p_name), "[��/��]", "�������� �ʾҴ�");
    }

    public string Log_Dge(bool isP, bool success, int final_dmg)    //ȸ��
    {
        //<ȸ�� ����>	����� ������ ���ߴ�
        //<ȸ�� ����>	����� ������ ������ ���ϰ� X ���ظ� �޾Ҵ�
        if (success)    //ȸ�� ����
            return MakeSentence((isP ? _p_name : _e_name), "[��/��]", "������ ���ߴ�");
        else
            return MakeSentence((isP ? _p_name : _e_name), "[��/��]", "������ ������ ���ϰ�,",
                    final_dmg.ToString(), "���ظ� �޾Ҵ�");
    }

    public string Log_DgeFail(bool isP) //ȸ�� ����
    {
        //����� ������ ���Ϸ� ������ ����� �������� �ʾҴ�
        return MakeSentence((isP ? _p_name : _e_name), "[��/��]", "������ ���Ϸ� ������,",
                (isP ? _e_name : _p_name), "[��/��]", "�������� �ʾҴ�");
    }

    public string Log_TacCant(bool isP, string actName) //���� �Ұ�
    {
        //����� <> �� �� ����
        return MakeSentence(isP ? _p_name : _e_name, "\"" + actName + "\"", "�� �� ����");
    }

    public string Log_Wait(bool isP, int getAp)
    {
        //����� ��Ȳ�� ���Ѻ��鼭, �ൿ���� X �����.
        return MakeSentence((isP ? _p_name : _e_name), "[��/��]", "��Ȳ�� ���Ѻ��鼭, �ൿ����", getAp.ToString(), "�����");
    }

    string MakeSentence(params string[] arr)    //���ڷ� �־��� �ܾ���� �����ؼ� ���� ����
    {
        var s = ""; //�ʱ� ����

        for (int i = 0; i < arr.Length; i++)
        {
            if (i == 0) //ù �ܾ�� ���� ����� ����
                s += arr[i];
            //������ ���� �޶����� ���簡 ���� ��� ����� ����
            else if (arr[i][0] == '[' && arr[i][arr[i].Length - 1] == ']')
            {
                string str;

                if (arr[i].Length <= 2)
                    str = "";
                else
                {
                    var prevWordLast = arr[i - 1][arr[i - 1].Length - 1];   //�� �ܾ��� ������ ����
                    var thisWord = arr[i].Substring(1, arr[i].Length - 2);  //���� ������
                    string[] split = thisWord.Split('/');   //���� �������� /�� �������� �ѷ� �и�
                    var case1 = split[0];   //���� ������ 1
                    var case2 = split[1];   //���� ������ 2

                    str = CheckConsonant_Korean(prevWordLast, case1, case2);
                }

                s += str;
            }
            else    //�̿ܿ��� �� �ܾ���� ���⸦ �߰�
                s += " " + arr[i];

            if (i >= arr.Length - 1)    //������ �ܾ���
                s += ".";      //�ܾ� �ڿ� ��ħǥ �߰�
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
        //���� �α� ��� �ڷ�ƾ ����
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
            //����� �αװ� ������, �α� ��� ������� �ڵ� ����
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

        //�α��� ��� ��Ȳ�� ���� ��� �� ó�� �з�
        switch (_logSituation)
        {
            case LogSituation.EvntStart:
                _evntSys.EvntActList_OnOff(true);   //�̺�Ʈ ����(�α� ����� �Ϸ�� ������, �̺�Ʈ �ൿ ��� ���)
                break;
            case LogSituation.BtlTurnStart:
                _btlSys.BtlActList_OnOff(true); //���� �� ����(�α� ����� �Ϸ�� ������, ���� �ൿ ��� ���)
                break;
            case LogSituation.ActEffect:        //�ൿ ȿ�� ó�� �α� ���
                _btlSys.Set_EffectProcess(false);
                break;
            case LogSituation.BtlFlow:          //���� ó�� �α� ���
                _btlSys.Set_BattleProcess(false);
                break;
            case LogSituation.EvntSuccess:      //�̺�Ʈ ���� ���
                _evntSys.StartCoroutine(_evntSys.EventResultFlow(true));
                break;
            case LogSituation.EvntFail:      //�̺�Ʈ ���� ��� (�� �̴� ����)
                _evntSys.StartCoroutine(_evntSys.EventResultFlow(false));
                break;
            case LogSituation.EvntResult:       //�̺�Ʈ ��� ó�� ����
                _evntSys.Set_ResultProcess(false);
                break;
        }
    }

    bool LastConsonant_Korean(char last)
    {
        //�ѱ��� �ƴϸ� ������ ���ٰ� �˸�
        if (last < 44032 || last > 55215)
            return false;

        int ch = last;

        ch -= 44032;    //�ѱ��� ���� ��ġ 44032�� ��

        //28�� ������ �������� �����ϸ� �������� �Ǵ�(28��°���� ������ ���� ���ڰ� �����Ƿ�)
        return (ch % 28) > 0;
    }

    string CheckConsonant_Korean(char prevLast, string case1, string case2)
    {
        if (LastConsonant_Korean(prevLast)) //���� �ܾ��� ������ ���ڿ� ������ �ִٸ�
            return case1;
        else    //������ ���ٸ�
            return case2;
    }
}
