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
    private BattleSystem _btlSys;           //���� �ý���

    [SerializeField]
    private TextMeshProUGUI _log;   //�α� �ؽ�Ʈ
    [SerializeField]
    private GameObject _cursor_log; //�α� Ŀ��
    private Button _btn_cursor;     //Ŀ�� ��� �� �α��� ��ư �ν�

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
        _btn_cursor = GetComponent<Button>();
    }

    public void LogErase()
    {
        _log.text = "";   //�α� �����
        CurSor_OnOff(false);
    }

    public void SetLog_BattleStart(string e_name)   //�α� ����: ������ ���� 
    {
        NewLog(MakeSentence(e_name, "��Ÿ����"));
        LogPrint_Start(LogSituation.No);
        _e_name = e_name;
    }

    public void SetLog_DefEffect(bool isP, string effText)  //�α� ����: ��� ȿ�� ���
    {
        NewLog(MakeSentence((isP ? _p_name : _e_name), "������ �����,", effText));
        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_DgeEffect(bool isP, string effText)  //�α� ����: ȸ�� ȿ�� ���
    {
        NewLog(MakeSentence((isP ? _p_name : _e_name), "������ ����,", effText));
        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_ActEffect(bool isP, string actName, string effText)  //�α� ����: �ൿ ȿ�� ���
    {
        NewLog(MakeSentence((isP ? _p_name : _e_name), "\"" + actName + "\"�� ȿ����", effText));
        LogPrint_Start(LogSituation.ActEffect);
    }

    public void SetLog_RunAct(bool isP, bool success) //�α� ����: ���� �ൿ
    {
        if (success)
            NewLog(MakeSentence((isP ? _p_name : _e_name), success ? "�����ƴ�" : "������ �����ߴ�"));

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
            NewLog(MakeSentence(_e_name, "��������"));
        else
            NewLog(MakeSentence(_p_name, "����ߴ�"));

        LogPrint_Start(LogSituation.No);
    }

    public string Log_AtkDmg(bool isP, int final_dmg)   //�������� ���� �߻�
    {
        //����� ����� ������ X ���ظ� �־���
        return MakeSentence((isP ? _p_name : _e_name), (isP ? _e_name : _p_name), "������", final_dmg.ToString(), "���ظ� �־���");
    }

    public string Log_Def(bool isP, int final_dmg)  //���� ���
    {
        //<���� ���� 1 �̻�>	����� ������ ���� X ���ظ� �޾Ҵ�
        //<���� ���� 0>		����� ������ ���� ���ظ� ���� �ʾҴ�
        if (final_dmg >= 1) //���� ���ذ� 1 �̻�
            return MakeSentence((isP ? _p_name : _e_name), "������ ����,", final_dmg.ToString(), "���ظ� �޾Ҵ�");
        else
            return MakeSentence((isP ? _p_name : _e_name), "������ ����, ���ظ� ���� �ʾҴ�");
    }

    public string Log_DefFail(bool isP) //��� ����
    {
        //����� ������ ������ ������ ����� �������� �ʾҴ�
        return MakeSentence((isP ? _p_name : _e_name), "������ ������ ������,", (isP ? _e_name : _p_name), "�������� �ʾҴ�");
    }

    public string Log_Dge(bool isP, bool success, int final_dmg)    //ȸ��
    {
        //<ȸ�� ����>	����� ������ ���ߴ�
        //<ȸ�� ����>	����� ������ ������ ���ϰ� X ���ظ� �޾Ҵ�
        if (success)    //ȸ�� ����
            return MakeSentence((isP ? _p_name : _e_name), "������ ���ߴ�");
        else
            return MakeSentence((isP ? _p_name : _e_name), "������ ������ ���ϰ�,", final_dmg.ToString(), "���ظ� �޾Ҵ�");
    }

    public string Log_DgeFail(bool isP) //ȸ�� ����
    {
        //����� ������ ���Ϸ� ������ ����� �������� �ʾҴ�
        return MakeSentence((isP ? _p_name : _e_name), "������ ���Ϸ� ������,", (isP ? _e_name : _p_name), "�������� �ʾҴ�");
    }

    public string Log_TacCant(bool isP, string actName) //���� �Ұ�
    {
        //����� <> �� �� ����
        return MakeSentence(isP ? _p_name : _e_name, "\"" + actName + "\"", "�� �� ����");
    }

    public string Log_Wait(bool isP, int getAp)
    {
        //����� ��Ȳ�� ���Ѻ��鼭, �ൿ���� X �����.
        return MakeSentence((isP ? _p_name : _e_name), "��Ȳ�� ���Ѻ��鼭, �ൿ����", getAp.ToString(), "�����");
    }

    string MakeSentence(params string[] arr)    //���ڷ� �־��� �ܾ���� �����ؼ� ���� ����
    {
        var s = ""; //�ʱ� ����

        for (int i = 0; i < arr.Length; i++)
        {
            if (i < arr.Length - 1) //������ �ܾ �ƴϸ�
                s += arr[i] + " ";  //�ܾ� �ڿ� ����ֱ�
            else                    //������ �ܾ���
                s += arr[i] + ".";  //�ܾ� �ڿ� ��ħǥ �߰�
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
            if (_targetText == "" || _logSituation == LogSituation.No)  //����� �αװ� ������, �α� ��� ������� �ڵ� ����
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

        //�α��� ��� ��Ȳ�� ���� ��� �� ó�� �з�
        switch (_logSituation)
        {
            case LogSituation.ActEffect:        //�ൿ ȿ�� ó�� �α� ���
                _btlSys.Set_EffectProcess(false);
                break;
            case LogSituation.BtlFlow:          //���� ó�� �α� ���
                _btlSys.Set_BattleProcess(false);
                break;
            case LogSituation.RunEnd:           //�������� ���� ���� �α�
                //_btlSys.StartCoroutine(_btlSys.BattleFlow_End());
                break;
            case LogSituation.BtlEnd:

                break;
        }
    }
}
