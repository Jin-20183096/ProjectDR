using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Coffee.UIExtensions;
using static PlayerSystem;

public class LevelUpPannel : MonoBehaviour
{
    [SerializeField]
    private Button _btn_STR;    //힘 스탯 버튼
    [SerializeField]
    private Image _trigger_STR; //힘 스탯 버튼 트리거
    [SerializeField]
    private TextMeshProUGUI _text_STR;  //힘 스탯 텍스트
    [SerializeField]
    private Image[] _img_STR;   //힘 주사위 이미지
    [SerializeField]
    private GameObject[] _statUpCursor_STR;     //힘 스탯 상승 커서
    [SerializeField]
    private TextMeshProUGUI[] _statUpText_STR;  //힘 스탯 상승 텍스트

    [SerializeField]
    private Button _btn_INT;    //지능 스탯 버튼
    [SerializeField]
    private Image _trigger_INT; //지능 스탯 버튼 트리거
    [SerializeField]
    private TextMeshProUGUI _text_INT;  //지능 스탯 텍스트
    [SerializeField]
    private Image[] _img_INT;   //지능 주사위 이미지
    [SerializeField]
    private GameObject[] _statUpCursor_INT;     //지능 스탯 상승 커서
    [SerializeField]
    private TextMeshProUGUI[] _statUpText_INT;  //지능 스탯 상승 텍스트

    [SerializeField]
    private Button _btn_DEX;    //손재주 스탯 버튼
    [SerializeField]
    private Image _trigger_DEX; //손재주 스탯 버튼 트리거
    [SerializeField]
    private TextMeshProUGUI _text_DEX;  //손재주 스탯 텍스트
    [SerializeField]
    private Image[] _img_DEX;   //손재주 주사위 이미지
    [SerializeField]
    private GameObject[] _statUpCursor_DEX;     //손재주 스탯 상승 커서
    [SerializeField]
    private TextMeshProUGUI[] _statUpText_DEX;  //손재주 스탯 상승 텍스트

    [SerializeField]
    private Button _btn_AGI;    //민첩 스탯 버튼
    [SerializeField]
    private Image _trigger_AGI; //민첩 스탯 버튼 트리거
    [SerializeField]
    private TextMeshProUGUI _text_AGI;  //민첩 스탯 텍스트
    [SerializeField]
    private Image[] _img_AGI;   //민첩 주사위 이미지
    [SerializeField]
    private GameObject[] _statUpCursor_AGI;     //민첩 스탯 상승 커서
    [SerializeField]
    private TextMeshProUGUI[] _statUpText_AGI;  //민첩 스탯 상승 텍스트

    [SerializeField]
    private Button _btn_CON;    //건강 스탯 버튼
    [SerializeField]
    private Image _trigger_CON; //건강 스탯 버튼 트리거
    [SerializeField]
    private TextMeshProUGUI _text_CON;  //건강 스탯 텍스트
    [SerializeField]
    private Image[] _img_CON;   //건강 주사위 이미지
    [SerializeField]
    private GameObject[] _statUpCursor_CON;     //건강 스탯 상승 커서
    [SerializeField]
    private TextMeshProUGUI[] _statUpText_CON;  //건강 스탯 상승 텍스트

    [SerializeField]
    private Button _btn_WIL;    //의지 스탯 버튼
    [SerializeField]
    private Image _trigger_WIL; //의지 스탯 버튼 트리거
    [SerializeField]
    private TextMeshProUGUI _text_WIL;  //의지 스탯 텍스트
    [SerializeField]
    private Image[] _img_WIL;   //의지 주사위 이미지
    [SerializeField]
    private GameObject[] _statUpCursor_WIL;     //의지 스탯 상승 커서
    [SerializeField]
    private TextMeshProUGUI[] _statUpText_WIL;  //의지 스탯 상승 텍스트

    [SerializeField]
    private Sprite[] _spr_diceSide;

    [SerializeField]
    private Transform _eff_group;
    [SerializeField]
    private ParticleSystem _eff_statUp;

    public void Set_StatDisplay()   //레벨업 패널 초기 설정 (스탯별 주사위 이미지를 플레이어 스탯에 맞게 적용)
    {
        var stat_str = PlayerSys.STR;
        var stat_int = PlayerSys.INT;
        var stat_dex = PlayerSys.DEX;
        var stat_agi = PlayerSys.AGI;
        var stat_con = PlayerSys.CON;
        var stat_wil = PlayerSys.WIL;

        ButtonActive_STR(true);
        ActiveUI_STR(true);
        for (int i = 0; i < stat_str.Length; i++)   //힘
        {
            if (stat_str[i] >= 10)
                _img_STR[i].sprite = _spr_diceSide[10];
            else if (stat_str[i] <= 0)
                _img_STR[i].sprite = _spr_diceSide[0];
            else
                _img_STR[i].sprite = _spr_diceSide[stat_str[i]];

            _statUpCursor_STR[i].SetActive(false);
        }

        ButtonActive_INT(true);
        ActiveUI_INT(true);
        for (int i = 0; i < stat_int.Length; i++)   //지능
        {
            if (stat_int[i] >= 10)
                _img_INT[i].sprite = _spr_diceSide[10];
            else if (stat_int[i] <= 0)
                _img_INT[i].sprite = _spr_diceSide[0];
            else
                _img_INT[i].sprite = _spr_diceSide[stat_int[i]];

            _statUpCursor_INT[i].SetActive(false);
        }

        ButtonActive_DEX(true);
        ActiveUI_DEX(true);
        for (int i = 0; i < stat_dex.Length; i++)   //손재주
        {
            if (stat_dex[i] >= 10)
                _img_DEX[i].sprite = _spr_diceSide[10];
            else if (stat_dex[i] <= 0)
                _img_DEX[i].sprite = _spr_diceSide[0];
            else
                _img_DEX[i].sprite = _spr_diceSide[stat_dex[i]];

            _statUpCursor_DEX[i].SetActive(false);
        }

        ButtonActive_AGI(true);
        ActiveUI_AGI(true);
        for (int i = 0; i < stat_agi.Length; i++)   //민첩
        {
            if (stat_agi[i] >= 10)
                _img_AGI[i].sprite = _spr_diceSide[10];
            else if (stat_agi[i] <= 0)
                _img_AGI[i].sprite = _spr_diceSide[0];
            else
                _img_AGI[i].sprite = _spr_diceSide[stat_agi[i]];

            _statUpCursor_AGI[i].SetActive(false);
        }

        ButtonActive_CON(true);
        ActiveUI_CON(true);
        for (int i = 0; i < stat_con.Length; i++)   //건강
        {
            if (stat_con[i] >= 10)
                _img_CON[i].sprite = _spr_diceSide[10];
            else if (stat_con[i] <= 0)
                _img_CON[i].sprite = _spr_diceSide[0];
            else
                _img_CON[i].sprite = _spr_diceSide[stat_con[i]];

            _statUpCursor_CON[i].SetActive(false);
        }

        ButtonActive_WIL(true);
        ActiveUI_WIL(true);
        for (int i = 0; i < stat_wil.Length; i++)   //의지
        {
            if (stat_wil[i] >= 10)
                _img_WIL[i].sprite = _spr_diceSide[10];
            else if (stat_wil[i] <= 0)
                _img_WIL[i].sprite = _spr_diceSide[0];
            else
                _img_WIL[i].sprite = _spr_diceSide[stat_wil[i]];

            _statUpCursor_WIL[i].SetActive(false);
        }
    }

    public void Set_StatUpCursor(ICreature.Stats stat, int order, int value)    //해당 스탯의 해당 위치에 스탯 상승 커서 활성화
    {
        TextMeshProUGUI statText = null;
        Image[] diceImg = null;
        GameObject[] cursor = null;
        TextMeshProUGUI[] statUpText = null;
        int statValue = 0;

        switch (stat)
        {
            case ICreature.Stats.STR:
                statText = _text_STR;
                diceImg = _img_STR;
                cursor = _statUpCursor_STR;
                statUpText = _statUpText_STR;
                statValue = PlayerSys.STR[order];
                break;
            case ICreature.Stats.INT:
                statText = _text_INT;
                diceImg = _img_INT;
                cursor = _statUpCursor_INT;
                statUpText = _statUpText_INT;
                statValue = PlayerSys.INT[order];
                break;
            case ICreature.Stats.DEX:
                statText = _text_DEX;
                diceImg = _img_DEX;
                cursor = _statUpCursor_DEX;
                statUpText = _statUpText_DEX;
                statValue = PlayerSys.DEX[order];
                break;
            case ICreature.Stats.AGI:
                statText = _text_AGI;
                diceImg = _img_AGI;
                cursor = _statUpCursor_AGI;
                statUpText = _statUpText_AGI;
                statValue = PlayerSys.AGI[order];
                break;
            case ICreature.Stats.CON:
                statText = _text_CON;
                diceImg = _img_CON;
                cursor = _statUpCursor_CON;
                statUpText = _statUpText_CON;
                statValue = PlayerSys.CON[order];
                break;
            case ICreature.Stats.WIL:
                statText = _text_WIL;
                diceImg = _img_WIL;
                cursor = _statUpCursor_WIL;
                statUpText = _statUpText_WIL;
                statValue = PlayerSys.WIL[order];
                break;
        }

        //상승한 스탯명 텍스트 활성화
        statText.color = Color.white;

        //상승한 스탯의 주사위 이미지 변경
        diceImg[order].color = Color.white;

        if (statValue >= 10)
            diceImg[order].sprite = _spr_diceSide[10];
        else if (statValue <= 0)
            diceImg[order].sprite = _spr_diceSide[0];
        else
            diceImg[order].sprite = _spr_diceSide[statValue];


        //파티클 이펙트
        var effect = Instantiate(_eff_statUp, _eff_group);  //이펙트
        var uiParticle = effect.AddComponent<UIParticle>(); 
        var pos = diceImg[order].transform.position;
        var rect = diceImg[order].GetComponent<RectTransform>();
        effect.transform.position = new Vector2(pos.x + (rect.sizeDelta.x), pos.y);
        uiParticle.scale = 20f;

        //커서와 상승값 표시
        cursor[order].SetActive(true);
        statUpText[order].text = value.ToString();
    }

    public void ActiveUI_STR(bool b)  //힘 스탯 UI 활성화/비활성화
    {
        _text_STR.color = new Color(1, 1, 1, b ? 1 : 0.25f);

        foreach (Image img in _img_STR)
            img.color = new Color(1, 1, 1, b ? 1 : 0.25f);

        if (b == false)
            ButtonActive_STR(false);
    }

    public void ButtonActive_STR(bool b)    //힘 스탯 버튼 활성화/비활성화
    {
        _btn_STR.enabled = b;
        _trigger_STR.enabled = b;
    }

    public void ActiveUI_INT(bool b)  //지능 스탯 UI 활성화/비활성화
    {
        _text_INT.color = new Color(1, 1, 1, b ? 1 : 0.25f);

        foreach (Image img in _img_INT)
            img.color = new Color(1, 1, 1, b ? 1 : 0.25f);

        if (b == false)
            ButtonActive_INT(false);
    }

    public void ButtonActive_INT(bool b)    //지능 스탯 버튼 활성화/비활성화
    {
        _btn_INT.enabled = b;
        _trigger_INT.enabled = b;
    }

    public void ActiveUI_DEX(bool b)  //손재주 스탯 UI 활성화/비활성화
    {
        _text_DEX.color = new Color(1, 1, 1, b ? 1 : 0.25f);

        foreach (Image img in _img_DEX)
            img.color = new Color(1, 1, 1, b ? 1 : 0.25f);

        if (b == false)
            ButtonActive_DEX(false);
    }

    public void ButtonActive_DEX(bool b)    //손재주 스탯 버튼 활성화/비활성화
    {
        _btn_DEX.enabled = b;
        _trigger_DEX.enabled = b;
    }

    public void ActiveUI_AGI(bool b)  //민첩 스탯 UI 활성화/비활성화
    {
        _text_AGI.color = new Color(1, 1, 1, b ? 1 : 0.25f);

        foreach (Image img in _img_AGI)
            img.color = new Color(1, 1, 1, b ? 1 : 0.25f);

        if (b == false)
            ButtonActive_AGI(false);
    }

    public void ButtonActive_AGI(bool b)    //민첩 스탯 버튼 활성화/비활성화
    {
        _btn_AGI.enabled = b;
        _trigger_AGI.enabled = b;
    }

    public void ActiveUI_CON(bool b)  //건강 스탯 UI 활성화/비활성화
    {
        _text_CON.color = new Color(1, 1, 1, b ? 1 : 0.25f);

        foreach (Image img in _img_CON)
            img.color = new Color(1, 1, 1, b ? 1 : 0.25f);

        if (b == false)
            ButtonActive_CON(false);
    }

    public void ButtonActive_CON(bool b)    //건강 스탯 버튼 활성화/비활성화
    {
        _btn_CON.enabled = b;
        _trigger_CON.enabled = b;
    }

    public void ActiveUI_WIL(bool b)  //의지 스탯 UI 활성화/비활성화
    {
        _text_WIL.color = new Color(1, 1, 1, b ? 1 : 0.25f);

        foreach (Image img in _img_WIL)
            img.color = new Color(1, 1, 1, b ? 1 : 0.25f);

        if (b == false)
            ButtonActive_WIL(false);
    }

    public void ButtonActive_WIL(bool b)    //의지 스탯 버튼 활성화/비활성화
    {
        _btn_WIL.enabled = b;
        _trigger_WIL.enabled = b;
    }

    public void AllStatUpCursor_Off()  //스탯 상승 커서 전부 비활성화
    {
        foreach (GameObject obj in _statUpCursor_STR)
            obj.SetActive(false);

        foreach (GameObject obj in _statUpCursor_INT)
            obj.SetActive(false);

        foreach (GameObject obj in _statUpCursor_DEX)
            obj.SetActive(false);

        foreach (GameObject obj in _statUpCursor_AGI)
            obj.SetActive(false);

        foreach (GameObject obj in _statUpCursor_CON)
            obj.SetActive(false);

        foreach (GameObject obj in _statUpCursor_WIL)
            obj.SetActive(false);
    }
}
