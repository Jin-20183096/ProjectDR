using System.Collections.Generic;
using UnityEngine;
using static ICreature;

public class PlayerSystem : MonoBehaviour, ICreature
{
    public static PlayerSystem PlayerSys = null;

    [SerializeField]
    private Vector2Int _grid;
    public int X
    {
        get { return _grid.x; }
        set { _grid.x = value; }
    }
    public int Y
    {
        get { return _grid.y; }
        set { _grid.y = value; }
    }
    [SerializeField]
    private int _sight; //시야
    public int SIGHT
    {
        get { return _sight; }
    }

    [Header("# Main Info")]
    [SerializeField]
    private string _name;   //이름
    public string NAME
    {
        get { return _name; }
    }
    [SerializeField]
    private int _lv;     //레벨
    public int LV
    {
        get { return _lv; }
    }
    [SerializeField]
    private int _exp;    //경험치
    public int EXP
    {
        get { return _exp; }
    }
    [SerializeField]
    private int _expMax;    //최대 경험치
    public int EXP_MAX
    {
        get { return _expMax; }
    }
    [SerializeField]
    private int _hp;        //HP
    public int HP
    {
        get { return _hp; }
    }
    [SerializeField]
    private int _hpMax;     //최대 HP
    public int HP_MAX
    {
        get { return _hpMax; }
    }
    [SerializeField]
    private int _ac;        //방어도
    public int AC
    {
        get { return _ac; }
    }
    [SerializeField]
    private int _ap;        //행동력
    public int AP
    {
        get { return _ap; }
    }
    [SerializeField]
    private int _apMax;     //최대 행동력
    public int AP_MAX
    {
        get { return _apMax; }
    }
    private int _apUse;     //사용할 행동력

    [Header("# Action Related")]
    [SerializeField]
    private int[] _stat_STR = { 0, 0, 0, 0, 0, 0 };     //힘 스탯
    public int[] STR
    {
        get { return _stat_STR; }
    }
    [SerializeField]
    private int[] _stat_INT = { 0, 0, 0, 0, 0, 0 };     //지능 스탯
    public int[] INT
    {
        get { return _stat_INT; }
    }
    [SerializeField]
    private int[] _stat_DEX = { 0, 0, 0, 0, 0, 0 };     //손재주 스탯
    public int[] DEX
    {
        get { return _stat_DEX; }
    }
    [SerializeField]
    private int[] _stat_AGI = { 0, 0, 0, 0, 0, 0 };     //민첩 스탯
    public int[] AGI
    {
        get { return _stat_AGI; }
    }
    [SerializeField]
    private int[] _stat_CON = { 0, 0, 0, 0, 0, 0 };     //건강 스탯
    public int[] CON
    {
        get { return _stat_CON; }
    }
    [SerializeField]
    private int[] _stat_WIL = { 0, 0, 0, 0, 0, 0 };     //건강 스탯
    public int[] WIL
    {
        get { return _stat_WIL; }
    }

    [SerializeField]
    private int[] _reroll = new int[7];     //No, 힘, 지능, 손재주, 민첩, 건강, 의지

    public int GetReroll(Stats stat)  //재굴림 반환
    {
        return _reroll[(int)stat];
    }

    [SerializeField]
    private List<BtlActClass> _actList; //행동 리스트
    public List<BtlActClass> ActList
    {
        get { return _actList; }
    }

    [Header("# UI Reference")]
    //플레이어 기본 정보 패널
    [SerializeField]
    private PlayerInfoPannel _infoScr;

    [Header("# Sprite Reference")]
    [SerializeField]
    private SpriteSystem _p_spr;
    [SerializeField]
    private SpriteSystem _p_spr_btl;

    [SerializeField]
    private GameObject _btn_fullRest;

    void Awake()
    {
        if (PlayerSys)
        {
            DestroyImmediate(gameObject);
            return;
        }
        else
        {
            PlayerSys = this;
            DontDestroyOnLoad(gameObject);
        }

        //초기 스테이터스 설정
        Change_Name("이름 없는 모험가");

        _lv = 1;
        _exp = 0;
        _expMax = 10;

        _hpMax = Random.Range(8, 15);
        _hp = _hpMax;
        Change_HpMax(true, 0);
        Change_Hp(true, 0);

        _ac = 0;
        Change_AC(true, 0);

        _apMax = 6;
        _ap = Random.Range(1, _apMax + 1);
        Change_ApMax(true, 0);
        Change_Ap(true, 0);
    }

    public void Move_PlayerPosition(Vector3 vec3, bool isLeft)
    {
        transform.position = vec3;
        _p_spr.Flip_X(isLeft);
    }

    public void Set_PlayerLocalPos(bool isCenter, bool isLeft)
    {
        if (isCenter)
            _p_spr.transform.localPosition = new Vector3(0, 0, -1.25f);
        else
        {
            if (isLeft)
                _p_spr.transform.localPosition = new Vector3(-1.25f, 0, -1.25f);
            else
                _p_spr.transform.localPosition = new Vector3(1.25f, 0, -1.25f);
        }
    }

    public bool IsPlayer() => true;

    public void Change_Name(string name) => _name = name;   //이름 변경

    public void Change_Exp(bool plus, int value)    //경험치 획득, 감소
    {
        if (plus)
        {
            if (_expMax <= _exp + value)    //획득한 경험치로 레벨업이 발생할 경우
            {
                while (_expMax <= _exp + value) //레벨업 종료까지 경험치 체크
                {
                    _exp = (_exp + value) - _expMax;    //경험치는 레벨업 종료 후 경험치만 남김
                    _lv += 1;   //레벨 1 증가
                    _expMax += (int)(_expMax * 1.75);   //최대 경험치량 증가
                }
            }
            else
                _exp += value;
        }
        else
        {
            if (_exp <= value)
                _exp = 0;
            else
                _exp -= value;
        }
    }

    public void Change_Hp(bool plus, int value)     //HP 변경
    {
        var old_hp = _hp;

        if (value >= 0) //변경값이 0 이상일때만 처리
        {
            if (plus)   //회복
            {
                if (_hpMax <= _hp + value)  //회복량이 최대 HP를 넘을 경우
                    _hp = _hpMax;   //최대 HP로 회복
                else
                    _hp += value;   //값만큼 회복
            }
            else        //피해
            {
                if (_hp <= value)   //피해량이 남은 HP를 넘을 경우
                    _hp = 0;        //HP가 0으로
                else
                    _hp -= value;   //값만큼 피해
            }
        }

        _infoScr.Change_TextHp(_hp);

        if (_hp > old_hp)   //HP 회복 시
            _infoScr.Change_HpMask(_hp / (float)_hpMax);    //HP 마스크 변경
        else                //HP 피해받았을 시
            _infoScr.Change_HpMeter(_hp / (float)_hpMax);   //HP 미터 변경
    }

    public void Change_HpMax(bool plus, int value)  //최대 HP 변경
    {
        var old_hpMax = _hpMax;

        //값만큼 증가
        if (plus)
            _hpMax += value;
        else
            _hpMax -= value;

        if (_hp >= _hpMax)      //최대 HP가 현재 HP보다 낮아졌을 경우
            _hp = _hpMax;       //낮아진 최대 HP만큼 현재 HP를 설정

        //변경된 비율만큼 HP바 변경
        _infoScr.Change_TextHp(_hp);
        _infoScr.Change_TextHpMax(_hpMax);

        if (_hpMax > old_hpMax)
            _infoScr.Change_HpMeter(_hp / (float)_hpMax);
        else
            _infoScr.Change_HpMask(_hp / (float)_hpMax);
    }

    public void Change_AC(bool plus, int value) //방어도 변경
    {
        if (plus)
            _ac += value;
        else
            _ac -= value;

        //방어도 아이콘과 ui수치변경
        _infoScr.Change_Ac(_ac);
    }

    public void Change_Ap(bool plus, int value) //행동력 변경
    {
        var old_ap = _ap;

        if (value >= 0) //변경값이 0 이상일떄만 처리
        {
            if (plus)   //추가
            {
                if (_apMax <= _ap + value)  //추가량이 최대 행동력을 넘을 경우
                    _ap = _apMax;
                else
                    _ap += value;
            }
            else        //소모
            {
                if (_ap <= value)           //소모량이 남은 행동력을 넘을 경우
                    _ap = 0;
                else
                    _ap -= value;
            }
        }

        if (old_ap == 0)    //행동력이 0이었다가
        {
            if (_ap > 0)    //0이 아니게 되면, 방어도 회복
                Change_AC(true, 5);
        }
        else if (_ap == 0)  //행동력이 0이 아니었다가 0이 되면, 방어도 감소
        {
            Change_AC(false, 5);
        }

        //미터 갱신
        _infoScr.Change_ApMeter(_ap);
        _infoScr.Change_Ap_UsePreview(_apUse);
    }

    public void Change_Ap_UsePreview(int use)
    {
        _apUse = use;
        _infoScr.Change_Ap_UsePreview(_apUse);
    }

    public void Change_ApMax(bool plus, int value)
    {
        if (plus)
            _apMax += value;
        else
            _apMax -= value;

        if (_ap >= _apMax)  //최대 행동력이 현재 행동력보다 낮아졌을 경우
            _ap = _apMax;   //현재 행동력을 낮아진 최대 행동력만큼 변경

        //미터 갱신
        _infoScr.Change_ApMeter(_ap);
        _infoScr.Change_Ap_UsePreview(_apUse);
        _infoScr.Change_ApMeterMax(_apMax);
    }

    public void Change_ActionStat(bool plus, Stats stat, int[] stat_arr)
    {
        int[] temp_stat = { };

        switch (stat)
        {
            case Stats.STR:
                temp_stat = _stat_STR;
                break;
            case Stats.INT:
                temp_stat = _stat_INT;
                break;
            case Stats.DEX:
                temp_stat = _stat_DEX;
                break;
            case Stats.AGI:
                temp_stat = _stat_AGI;
                break;
            case Stats.CON:
                temp_stat = _stat_CON;
                break;
            case Stats.WIL:
                temp_stat = _stat_WIL;
                break;
        }

        for (int i = 0; i < temp_stat.Length; i++)
        {
            if (plus)
                temp_stat[i] += stat_arr[i];
            else
                temp_stat[i] -= stat_arr[i];
        }
    }

    public void TakeDamage(int dmg, BtlActData.DamageType dmgType)
    {
        Change_Hp(false, dmg);
        //데미지 타입에 따른 피격음
    }

    public void FullRest_OnOff(bool isOn)
    {
        _btn_fullRest.SetActive(isOn);
    }
}
