using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static PlayerSystem;

public class RewardPannel : MonoBehaviour
{
    [SerializeField]
    private BattleSystem _btlSys;
    [SerializeField]
    private DungeonEventSystem _evntSys;

    [Header("# Reward Exp")]
    [SerializeField]
    private GameObject _pannel_exp;
    [SerializeField]
    private TextMeshProUGUI _text_lv;
    [SerializeField]
    private Image _expMeter;
    [SerializeField]
    private TextMeshProUGUI _text_exp;
    [SerializeField]
    private TextMeshProUGUI _text_expMax;
    [SerializeField]
    private TextMeshProUGUI _text_getExp;

    private int _exp_get;

    [Header("# Level Up")]
    [SerializeField]
    private LevelUpPannel _pannel_lvUp;

    private bool _levelUpProcess = false;

    [Header("# Reward Item")]
    [SerializeField]
    private VerticalLayoutGroup _pannel_item;
    [SerializeField]
    private HorizontalLayoutGroup[] _itemSlot_layout;


    public void RewardPannel_Exp_OnOff(bool isOn)
        => _pannel_exp.SetActive(isOn);

    public void RewardPannel_Item_OnOff(bool isOn)
        => _pannel_item.gameObject.SetActive(isOn);

    public void Set_RewardExpInfo() //경험치 획득 패널의 수치들을 상황에 맞게 설정
    {
        _text_lv.text = PlayerSys.LV.ToString();
        _text_exp.text = PlayerSys.EXP.ToString();
        _text_expMax.text = PlayerSys.EXP_MAX.ToString();
        _expMeter.fillAmount = (float)PlayerSys.EXP / PlayerSys.EXP_MAX;
    }

    public void Set_GetExpText(int exp)
    {
        _btlSys.Set_RewardExpProcess(true);

        if (exp > 0)
        {
            _text_getExp.gameObject.SetActive(exp > 0);
            _exp_get = exp;
            StartCoroutine("ExpMeter_Up");
        }
    }

    IEnumerator ExpMeter_Up()
    {
        _text_getExp.text = "+" + _exp_get;
        yield return new WaitForSecondsRealtime(1/2f);
        _btlSys.Set_RewardExpProcess(true);

        while (_exp_get > 0)
        {
            yield return new WaitUntil(() => _levelUpProcess == false);

            if (_exp_get >= PlayerSys.EXP_MAX - PlayerSys.EXP)
                yield return new WaitForSecondsRealtime(1f / 50);
            else
                yield return new WaitForSecondsRealtime(1f / 20);

            PlayerSys.Change_Exp(true, 1);
            
            _exp_get--;

            if (_exp_get > 0)
                _text_getExp.text = "+" + _exp_get;
            else
                _text_getExp.text = "";

            _text_lv.text = PlayerSys.LV.ToString();
            _text_exp.text = PlayerSys.EXP.ToString();
            _text_expMax.text = PlayerSys.EXP_MAX.ToString();
            _expMeter.fillAmount = (float)PlayerSys.EXP / PlayerSys.EXP_MAX;

            //플레이어 레벨업 여부 체크
            if (PlayerSys.EXP >= PlayerSys.EXP_MAX)   //플레이어 경험치가 최대일 때
            {
                yield return new WaitForSeconds(1f);    //레벨 업 시 잠시 딜레이
                LevelUpProcess_Start(); //레벨업 처리 시작

                yield return new WaitUntil(() => _levelUpProcess == false);
                PlayerSys.LvUp(1);

                _text_lv.text = PlayerSys.LV.ToString();
                _text_exp.text = PlayerSys.EXP.ToString();
                _text_expMax.text = PlayerSys.EXP_MAX.ToString();
                _expMeter.fillAmount = (float)PlayerSys.EXP / PlayerSys.EXP_MAX;
            }
        }

        _btlSys.Set_RewardExpProcess(false);
        _evntSys.Set_ResultProcess(false);
        StopCoroutine("ExpMeter_Up");
    }

    public void Set_LevelUpProcess(bool b) => _levelUpProcess = b;

    public void LevelUpProcess_Start()
    {
        Set_LevelUpProcess(true);   //레벨업 처리 중 On

        _pannel_lvUp.gameObject.SetActive(true);    //레벨업 패널 활성화

        _pannel_lvUp.Set_StatDisplay(); //레벨업 패널 스탯 표시 
    }

    public void LevelUpProcess_End()
    {
        _pannel_lvUp.gameObject.SetActive(false);   //레벨업 패널 비활성화

        Set_LevelUpProcess(false);  //레벨업 처리 중 Off
    }

    public void Select_StatUp_STR() //힘 스탯 단련
    {
        //모든 스탯의 커서 OFF
        _pannel_lvUp.AllStatUpCursor_Off();

        //다른 스탯의 UI 비활성화
        _pannel_lvUp.ActiveUI_STR(false);
        _pannel_lvUp.ActiveUI_INT(false);
        _pannel_lvUp.ActiveUI_DEX(false);
        _pannel_lvUp.ActiveUI_AGI(false);
        _pannel_lvUp.ActiveUI_CON(false);
        _pannel_lvUp.ActiveUI_WIL(false);

        //무작위 위치의 스탯 1 상승
        int[] statArr = new int[6]; //적용할 스탯 배열
        var order = Random.Range(0, 6); //상승할 스탯 위치

        statArr[order] += 1;    //해당 위치의 스탯 1 상승

        PlayerSys.Change_ActionStat(true, ICreature.Stats.STR, statArr);    //플레이어에게 레벨업 스탯 적용
        _pannel_lvUp.Set_StatUpCursor(ICreature.Stats.STR, order, 1);       //스탯 커서 적용
        _pannel_lvUp.ButtonActive_STR(false);   //이 스탯의 버튼 비활성화

        Random_StatUp();    //무작위 스탯 추가 상승
    }

    public void Select_StatUp_INT() //지능 스탯 단련
    {
        //모든 스탯의 커서 OFF
        _pannel_lvUp.AllStatUpCursor_Off();

        //다른 스탯의 UI 비활성화
        _pannel_lvUp.ActiveUI_STR(false);
        _pannel_lvUp.ActiveUI_INT(false);
        _pannel_lvUp.ActiveUI_DEX(false);
        _pannel_lvUp.ActiveUI_AGI(false);
        _pannel_lvUp.ActiveUI_CON(false);
        _pannel_lvUp.ActiveUI_WIL(false);

        //무작위 위치의 스탯 1 상승
        int[] statArr = new int[6]; //적용할 스탯 배열
        var order = Random.Range(0, 6); //상승할 스탯 위치

        statArr[order] += 1;    //해당 위치의 스탯 1 상승

        PlayerSys.Change_ActionStat(true, ICreature.Stats.INT, statArr);    //플레이어에게 레벨업 스탯 적용
        _pannel_lvUp.Set_StatUpCursor(ICreature.Stats.INT, order, 1);       //스탯 커서 적용
        _pannel_lvUp.ButtonActive_INT(false);   //이 스탯의 버튼 비활성화

        Random_StatUp();    //무작위 스탯 추가 상승
    }

    public void Select_StatUp_DEX() //손재주 스탯 단련
    {
        //모든 스탯의 커서 OFF
        _pannel_lvUp.AllStatUpCursor_Off();

        //다른 스탯의 UI 비활성화
        _pannel_lvUp.ActiveUI_STR(false);
        _pannel_lvUp.ActiveUI_INT(false);
        _pannel_lvUp.ActiveUI_DEX(false);
        _pannel_lvUp.ActiveUI_AGI(false);
        _pannel_lvUp.ActiveUI_CON(false);
        _pannel_lvUp.ActiveUI_WIL(false);

        //무작위 위치의 스탯 1 상승
        int[] statArr = new int[6]; //적용할 스탯 배열
        var order = Random.Range(0, 6); //상승할 스탯 위치

        statArr[order] += 1;    //해당 위치의 스탯 1 상승

        PlayerSys.Change_ActionStat(true, ICreature.Stats.DEX, statArr);    //플레이어에게 레벨업 스탯 적용
        _pannel_lvUp.Set_StatUpCursor(ICreature.Stats.DEX, order, 1);       //스탯 커서 적용
        _pannel_lvUp.ButtonActive_DEX(false);   //이 스탯의 버튼 비활성화

        Random_StatUp();    //무작위 스탯 추가 상승
    }

    public void Select_StatUp_AGI() //민첩 스탯 단련
    {
        //모든 스탯의 커서 OFF
        _pannel_lvUp.AllStatUpCursor_Off();

        //다른 스탯의 UI 비활성화
        _pannel_lvUp.ActiveUI_STR(false);
        _pannel_lvUp.ActiveUI_INT(false);
        _pannel_lvUp.ActiveUI_DEX(false);
        _pannel_lvUp.ActiveUI_AGI(false);
        _pannel_lvUp.ActiveUI_CON(false);
        _pannel_lvUp.ActiveUI_WIL(false);

        //무작위 위치의 스탯 1 상승
        int[] statArr = new int[6]; //적용할 스탯 배열
        var order = Random.Range(0, 6); //상승할 스탯 위치

        statArr[order] += 1;    //해당 위치의 스탯 1 상승

        PlayerSys.Change_ActionStat(true, ICreature.Stats.AGI, statArr);    //플레이어에게 레벨업 스탯 적용
        _pannel_lvUp.Set_StatUpCursor(ICreature.Stats.AGI, order, 1);       //스탯 커서 적용
        _pannel_lvUp.ButtonActive_AGI(false);   //이 스탯의 버튼 비활성화

        Random_StatUp();    //무작위 스탯 추가 상승
    }

    public void Select_StatUp_CON() //건강 스탯 단련
    {
        //모든 스탯의 커서 OFF
        _pannel_lvUp.AllStatUpCursor_Off();

        //다른 스탯의 UI 비활성화
        _pannel_lvUp.ActiveUI_STR(false);
        _pannel_lvUp.ActiveUI_INT(false);
        _pannel_lvUp.ActiveUI_DEX(false);
        _pannel_lvUp.ActiveUI_AGI(false);
        _pannel_lvUp.ActiveUI_CON(false);
        _pannel_lvUp.ActiveUI_WIL(false);

        //무작위 위치의 스탯 1 상승
        int[] statArr = new int[6]; //적용할 스탯 배열
        var order = Random.Range(0, 6); //상승할 스탯 위치

        statArr[order] += 1;    //해당 위치의 스탯 1 상승

        PlayerSys.Change_ActionStat(true, ICreature.Stats.CON, statArr);    //플레이어에게 레벨업 스탯 적용
        _pannel_lvUp.Set_StatUpCursor(ICreature.Stats.CON, order, 1);       //스탯 커서 적용
        _pannel_lvUp.ButtonActive_CON(false);   //이 스탯의 버튼 비활성화

        Random_StatUp();    //무작위 스탯 추가 상승
    }

    public void Select_StatUp_WIL() //의지 스탯 단련
    {
        //모든 스탯의 커서 OFF
        _pannel_lvUp.AllStatUpCursor_Off();

        //다른 스탯의 UI 비활성화
        _pannel_lvUp.ActiveUI_STR(false);
        _pannel_lvUp.ActiveUI_INT(false);
        _pannel_lvUp.ActiveUI_DEX(false);
        _pannel_lvUp.ActiveUI_AGI(false);
        _pannel_lvUp.ActiveUI_CON(false);
        _pannel_lvUp.ActiveUI_WIL(false);

        //무작위 위치의 스탯 1 상승
        int[] statArr = new int[6]; //적용할 스탯 배열
        var order = Random.Range(0, 6); //상승할 스탯 위치

        statArr[order] += 1;    //해당 위치의 스탯 1 상승

        PlayerSys.Change_ActionStat(true, ICreature.Stats.WIL, statArr);    //플레이어에게 레벨업 스탯 적용
        _pannel_lvUp.Set_StatUpCursor(ICreature.Stats.WIL, order, 1);       //스탯 커서 적용
        _pannel_lvUp.ButtonActive_WIL(false);   //이 스탯의 버튼 비활성화

        Random_StatUp();    //무작위 스탯 추가 상승
    }

    public void Random_StatUp()
    {
        int[] statArr = new int[6]; //적용할 스탯 배열

        //무작위 스탯
        var stat = (ICreature.Stats)Random.Range((int)ICreature.Stats.STR, (int)ICreature.Stats.HP);
        //무작위 위치
        var order = Random.Range(0, 6);

        statArr[order] += 1;    //해당 위치의 스탯 1 상승

        PlayerSys.Change_ActionStat(true, stat, statArr);   //플레이어에게 레벨업 스탯 적용
        _pannel_lvUp.Set_StatUpCursor(stat, order, 1);      //스탯 커서 적용

        //스탯 상승이 완료되었으므로, 레벨업
        _pannel_lvUp.LevelUpEndButton_OnOff(true);
    }
}
