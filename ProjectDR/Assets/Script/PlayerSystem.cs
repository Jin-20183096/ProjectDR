using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static ICreature;
using static ItemSystem;
using static SingleToneCanvas;

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
        get
        {
            var actList = _actList.ToList();
            actList.Add(_waitAct);
            return actList;
        }
    }

    [SerializeField]
    private BtlActClass _waitAct;   //대기 행동

    [SerializeField]
    private List<BtlActClass> _actList_unarm;   //맨손 행동 리스트

    [Header("# Menu Screen")]
    [Header("   > Info Pannel")]
    [SerializeField]
    private bool _isOn_infoPannel;
    [SerializeField]
    private PlayerInfoPannel _infoPannel;

    [Header("   > Status Screen")]
    [SerializeField]
    private bool _isOn_statusScr;   //스테이터스창 활성화 여부
    [SerializeField]
    private PlayerMenuButton _btn_statusScr;    //스테이터스창 버튼
    [SerializeField]
    private StatusScreen _statusScr;    //스테이터스창 스크립트

    [Header("   > Inventory Screen")]
    [SerializeField]
    private bool _isOn_inventoryScr;    //인벤토리창 활성화 여부
    [SerializeField]
    private PlayerMenuButton _btn_inventoryScr; //인벤토리창 버튼
    [SerializeField]
    private GameObject _inventoryScr;  //인벤토리창 스크립트

    [Header("   > Action Screen")]
    [SerializeField]
    private bool _isOn_actScr;  //행동창 활성화 여부
    [SerializeField]
    private bool _isActChange;  //보유 행동의 변경 여부 (변경점이 있다면, 행동목록 창이 활성화될 때 목록 정보를 갱신해줌)
    [SerializeField]
    private PlayerMenuButton _btn_actScr;   //행동창 버튼
    [SerializeField]
    private ActionScreen _actScr;   //행동창 스크립트

    [Header("# Sprite Reference")]
    [SerializeField]
    private SpriteSystem _p_spr;
    [SerializeField]
    private SpriteSystem _p_spr_btl;

    [Header("# Effect")]
    [SerializeField]
    private Transform _effect_group;
    [SerializeField]
    private ParticleSystem _effect_blood;

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
        Debug.Log("플레이어 중앙에 있는가" + isCenter + " /\n 플레이어 좌측에 있는가" + isLeft);
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

    public void Change_Name(string name)
    {
        _name = name;   //이름 변경

        if (_isOn_statusScr)    //스테이터스창에 값 적용
            _statusScr.Change_Name(_name);
    }

    public void LvUp(int value)     //레벨 업
    {
        _exp = 0;   //경험치 초기화
        var val = value;

        while (val > 0)
        {
            val--;  //상승할 레벨 1 계산
            _lv++;  //레벨 1 상승

            //레벨대에 따른 최대 경험치량 증가
            if (_lv >= 1 && _lv <= 2)
            {
                Debug.Log(_expMax + "+" + (_expMax * 1.6f));
                _expMax = (int)(_expMax + (_expMax * 1.6f));
            }
            else if (_lv >= 3 && _lv <= 4)
            {
                Debug.Log(_expMax + "+" + (_expMax * 1.3f));
                _expMax = (int)(_expMax + (_expMax * 1.3f));
            }
            else if (_lv >= 5 && _lv <= 7)
            {
                Debug.Log(_expMax + "+" + (_expMax * 0.9f));
                _expMax = (int)(_expMax + (_expMax * 0.9f));
            }
            else if (_lv >= 8 && _lv <= 10)
            {
                Debug.Log(_expMax + "+" + (_expMax * 0.5f));
                _expMax = (int)(_expMax + (_expMax * 0.5f));
            }
            else
            {
                Debug.Log(_expMax + "+" + (_expMax * 0.2f));
                _expMax = (int)(_expMax + (_expMax * 0.2f));
            }
        }

        if (_isOn_statusScr)
        {
            _statusScr.Change_Lv(_lv);
            _statusScr.Change_Exp(_exp);
            _statusScr.Change_ExpMax(_expMax);
            _statusScr.Change_ExpMeter(_exp / (float)_expMax);
        }
    }

    public void Change_Exp(bool plus, int value)    //경험치 획득, 감소
    {
        if (plus)
            _exp += value;
        else
        {
            if (_exp <= value)
                _exp = 0;
            else
                _exp -= value;
        }

        if (_isOn_statusScr)    //스테이터스창에 값 적용
        {
            _statusScr.Change_Lv(_lv);
            _statusScr.Change_Exp(_exp);
            _statusScr.Change_ExpMax(_expMax);
            _statusScr.Change_ExpMeter(_exp / (float)_expMax);
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

        _infoPannel.Change_TextHp(_hp);

        if (_hp > old_hp)   //HP 회복 시
            _infoPannel.Change_HpMask(_hp / (float)_hpMax);    //HP 마스크 변경
        else                //HP 피해받았을 시
            _infoPannel.Change_HpMeter(_hp / (float)_hpMax);   //HP 미터 변경

        if (_isOn_statusScr)    //스테이터스창에 값 적용
            _statusScr.Change_Hp(_hp);
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
        _infoPannel.Change_TextHp(_hp);
        _infoPannel.Change_TextHpMax(_hpMax);

        if (_hpMax > old_hpMax)
            _infoPannel.Change_HpMeter(_hp / (float)_hpMax);
        else
            _infoPannel.Change_HpMask(_hp / (float)_hpMax);

        if (_isOn_statusScr)    //스테이터스창에 값 적용
        {
            _statusScr.Change_Hp(_hp);
            _statusScr.Change_HpMax(_hpMax);
        }
    }

    public void Change_AC(bool plus, int value) //방어도 변경
    {
        if (plus)
            _ac += value;
        else
            _ac -= value;

        //방어도 아이콘과 ui수치변경
        _infoPannel.Change_Ac(_ac);

        if (_isOn_statusScr)    //스테이터스창에 값 적용
            _statusScr.Change_AC(_ac);
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
        _infoPannel.Change_ApMeter(_ap);
        _infoPannel.Change_Ap_UsePreview(_apUse);

        if (_isOn_statusScr)
            _statusScr.Change_Ap(_ap);
    }

    public void Change_Ap_UsePreview(int use)
    {
        _apUse = use;
        _infoPannel.Change_Ap_UsePreview(_apUse);
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
        _infoPannel.Change_ApMeter(_ap);
        _infoPannel.Change_Ap_UsePreview(_apUse);
        _infoPannel.Change_ApMeterMax(_apMax);

        if (_isOn_statusScr)
        {
            _statusScr.Change_Ap(_ap);
            _statusScr.Change_ApMax(_apMax);
        }
    }

    public void Change_ActionStat(bool plus, Stats stat, int[] stat_arr)
    {
        int[] changingStat = { };

        switch (stat)
        {
            case Stats.STR:
                changingStat = _stat_STR;
                break;
            case Stats.INT:
                changingStat = _stat_INT;
                break;
            case Stats.DEX:
                changingStat = _stat_DEX;
                break;
            case Stats.AGI:
                changingStat = _stat_AGI;
                break;
            case Stats.CON:
                changingStat = _stat_CON;
                break;
            case Stats.WIL:
                changingStat = _stat_WIL;
                break;
        }

        for (int i = 0; i < changingStat.Length; i++)
        {
            if (plus)
                changingStat[i] += stat_arr[i];
            else
                changingStat[i] -= stat_arr[i];
        }

        if (_isOn_statusScr)
            _statusScr.Change_ActionStat(stat, changingStat);
    }

    public void TakeDamage(int dmg, BtlActData.DamageType dmgType)
    {
        Change_Hp(false, dmg);

        var eff = Instantiate(_effect_blood, _effect_group);
        var pos = _p_spr_btl.transform.position;
        var sizeY = _p_spr_btl.GetComponent<SpriteRenderer>().bounds.size.y;
        eff.transform.position = new Vector3(pos.x, pos.y + sizeY / 2, pos.z);
        eff.transform.localScale = new Vector3(0.75f, 0.75f, 1f);

        //데미지 타입에 따른 피격음
    }

    public void Change_Reroll(bool plus, Stats stat, int value)
    {
        var reroll_stat = Stats.No;

        switch (stat)
        {
            case Stats.RE_STR:
                reroll_stat = Stats.STR;
                break;
            case Stats.RE_INT:
                reroll_stat = Stats.INT;
                break;
            case Stats.RE_DEX:
                reroll_stat = Stats.DEX;
                break;
            case Stats.RE_AGI:
                reroll_stat = Stats.AGI;
                break;
            case Stats.RE_CON:
                reroll_stat = Stats.CON;
                break;
            case Stats.RE_WIL:
                reroll_stat = Stats.WIL;
                break;
        }

        if (plus)
            _reroll[(int)reroll_stat] += value;
        else
            _reroll[(int)reroll_stat] -= value;

        //스테이터스 창에 정보 적용
        if (_isOn_statusScr)
            _statusScr.Change_Reroll(reroll_stat, _reroll[(int)reroll_stat]);
    }

    public void Change_BtlAct(bool plus, BtlActClass btlAct)
    {
        if (plus)   //행동 추가
            _actList.Add(btlAct);
        else
            _actList.Remove(btlAct);

        //행동들을 행동 타입에 맞게 정렬
        _actList.Sort((x, y) => string.Compare(x.Data.Type.ToString(), y.Data.Type.ToString()));

        //행동목록 창에 정보 연동
        if (_isOn_actScr)
            _actScr.Change_BtlActList();
        else
            _isActChange = true;
    }

    public void Armed_OnOff(bool b) //무장 or 맨손
    {
        if (b)  //무장했을 경우
        {
            foreach (BtlActClass act in _actList_unarm)
                Change_BtlAct(false, act);  //맨손 행동을 전부 제거
        }
        else
        {
            foreach (BtlActClass act in _actList_unarm)
                Change_BtlAct(true, act);   //맨손 행동을 전부 추가
        }
    }

    public void InfoPannel_OnOff(bool b)    //플레이어 기본 정보창 OnOff
    {
        _isOn_infoPannel = b;
        _infoPannel.gameObject.SetActive(b);

        if (_isOn_infoPannel)
        {
            //플레이어 기본 정보 동기화
            //HP바
            _infoPannel.Change_HpBar(_hp, _hpMax);
            //행동력
            _infoPannel.Change_ApMeterMax(_apMax);
            _infoPannel.Change_ApMeter(_ap);
            //방어도
            _infoPannel.Change_Ac(_ac);
        }
    }

    public void StatusScreen_OnOff()    //스테이터스창 OnOff
    {
        if (STCanvas.DRAG == false)
        {
            _isOn_statusScr = !_isOn_statusScr;
            _statusScr.gameObject.SetActive(_isOn_statusScr);

            if (_isOn_statusScr)
            {
                //스테이터창의 UI 우선순위를 최상으로
                _statusScr.transform.SetAsLastSibling();

                //스테이터스 값 동기화
                _statusScr.Change_Name(_name);  //이름
                _statusScr.Change_Lv(_lv);      //레벨
                _statusScr.Change_Exp(_exp);    //경험치
                _statusScr.Change_ExpMax(_expMax);  //최대 경험치
                _statusScr.Change_ExpMeter((float)_exp / _expMax);
                _statusScr.Change_Hp(_hp);          //HP
                _statusScr.Change_HpMax(_hpMax);    //최대 HP
                _statusScr.Change_Ap(_ap);          //행동력
                _statusScr.Change_ApMax(_apMax);    //최대 행동력
                _statusScr.Change_AC(_ac);          //방어도

                //힘 스탯
                _statusScr.Change_Reroll(Stats.STR, _reroll[(int)Stats.STR]);
                _statusScr.Change_ActionStat(Stats.STR, _stat_STR);
                //지능 스탯
                _statusScr.Change_Reroll(Stats.INT, _reroll[(int)Stats.INT]);
                _statusScr.Change_ActionStat(Stats.INT, _stat_INT);
                //손재주 스탯
                _statusScr.Change_Reroll(Stats.DEX, _reroll[(int)Stats.DEX]);
                _statusScr.Change_ActionStat(Stats.DEX, _stat_DEX);
                //민첩 스탯
                _statusScr.Change_Reroll(Stats.AGI, _reroll[(int)Stats.AGI]);
                _statusScr.Change_ActionStat(Stats.AGI, _stat_AGI);
                //건강 스탯
                _statusScr.Change_Reroll(Stats.CON, _reroll[(int)Stats.CON]);
                _statusScr.Change_ActionStat(Stats.CON, _stat_CON);
                //의지 스탯
                _statusScr.Change_Reroll(Stats.WIL, _reroll[(int)Stats.WIL]);
                _statusScr.Change_ActionStat(Stats.WIL, _stat_WIL);
            }
        }
    }

    public void InventoryScreen_OnOff() //인벤토리창 OnOff
    {
        if (STCanvas.DRAG == false)
        {
            _isOn_inventoryScr = !_isOn_inventoryScr;
            _inventoryScr.SetActive(_isOn_inventoryScr);

            if (_isOn_inventoryScr)
            {
                _inventoryScr.transform.SetAsLastSibling(); //인벤토리 UI 우선순위를 최상으로

                ItemSys.Set_EquipIcon();        //장착한 장비 아이콘 할당

                ItemSys.Set_InventoryIcon();    //인벤토리 아이콘 할당
            }

            ItemSys.ON_INVENTORY = _isOn_inventoryScr;
        }
    }

    public void ActionScreen_OnOff()    //행동창 OnOff
    {
        if (STCanvas.DRAG == false)
        {
            _isOn_actScr = !_isOn_actScr;
            _actScr.gameObject.SetActive(_isOn_actScr);

            if (_isOn_actScr)
            {
                //인벤토리 UI 우선순위 최상위
                _actScr.transform.SetAsLastSibling();

                if (_isActChange)   //보유한 행동에 변화가 생겼을 경우
                {
                    _actScr.Change_BtlActList();    //행동 목록 동기화
                    _isActChange = false;           //행동 목록 변화 없음으로 처리
                }
            }
        }
    }

    public void MenuButton_OnOff_Status(bool b)  //스테이터스창 버튼 OnOff
    {
        //스테이터스 버튼을 비활성화 될때, 스테이터스창이 활성화 중이라면
        if (b == false && _isOn_statusScr)
        {
            _btn_statusScr.Button_OnOff();   //버튼 Off 상태
            StatusScreen_OnOff();               //스테이터스창 off 상태
        }

        _btn_statusScr.gameObject.SetActive(b);
    }

    public void MenuButton_OnOff_Inventory(bool b)  //인벤토리창 버튼 OnOff
    {
        //인벤토리 버튼을 비활성화할 때, 인벤토리가 활성화 중이라면
        if (b == false && _isOn_inventoryScr)
        {
            _btn_inventoryScr.Button_OnOff();    //버튼 Off 상태
            InventoryScreen_OnOff();                //인벤토리창 Off 상태
        }

        _btn_inventoryScr.gameObject.SetActive(b);
    }

    public void MenuButton_OnOff_ActList(bool b)    //행동창 버튼 OnOff
    {
        //행동목록 버튼을 비활성화할 때, 행동목록이 활성화 중이라면
        if (b == false && _isOn_actScr)
        {
            _btn_actScr.Button_OnOff();  //버튼 Off 상태
            ActionScreen_OnOff();          //행동목록창 Off 상태
        }

        _btn_actScr.gameObject.SetActive(b);
    }

    public void EquipItem(ItemData item)    //아이템 장비
    {
        if (item.Type == ItemData.ItemType.Weapon)
            Armed_OnOff(true);  //무기를 장비함

        //무기, 방어구, 보조무기 타입의 장비의 경우 외형 변경
        if (item.Type <= ItemData.ItemType.SubWp)
        {
            _p_spr.Change_Item(item, item.Type);
            _p_spr_btl.Change_Item(item, item.Type);
        }
    }

    public void UnequipItem(ItemData item)  //아이템 해제
    {
        if (item.Type == ItemData.ItemType.Weapon)
            Armed_OnOff(true);  //무기를 해제함

        //무기, 방어구, 보조무기 타입의 장비의 경우 외형 변경
        if (item.Type <= ItemData.ItemType.SubWp)
        {
            _p_spr.Change_Item(item, item.Type);
            _p_spr_btl.Change_Item(item, item.Type);
        }
    }
}
