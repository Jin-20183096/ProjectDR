using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoPannel : MonoBehaviour
{
    [Header("# HP")]
    [SerializeField]
    private Image _hpMask;         //HP바 마스크
    [SerializeField]
    private Image _hpMeter;        //HP바 미터
    [SerializeField]
    private TextMeshProUGUI _txt_hp;    //HP 텍스트           
    [SerializeField]
    private TextMeshProUGUI _txt_hpMax; //최대 HP 텍스트

    [Header("# AC")]
    [SerializeField]
    private GameObject _icon_ac;        //방어도 아이콘
    [SerializeField]
    private TextMeshProUGUI _txt_ac;    //방어도 텍스트
    [SerializeField]
    private TextMeshProUGUI _txt_acMax; //최대 방어도 텍스트

    [Header("# AP")]
    [SerializeField]
    private Transform[] _meter_ap;     //행동력 미터(아이콘들)
    [SerializeField]
    private int _cursor_nowAp;          //현재 행동력 아이콘의 다음 인덱스를 가리키는 커서

    public void Change_TextHp(int hp)
    {
        _txt_hp.text = hp.ToString();
    }

    public void Change_TextHpMax(int hpMax)
    {
        _txt_hpMax.text = hpMax.ToString();
    }

    public void Change_HpMask(float amount)
    {
        _hpMask.fillAmount = amount;

        StopCoroutine("HpMask_Down");
        StartCoroutine("HpMask_Up");
    }

    public void Change_HpMeter(float amount)
    {
        _hpMeter.fillAmount = amount;

        StopCoroutine("HpMask_Up");
        StartCoroutine("HpMask_Down");
    }

    public void Change_HpBar(int hp, int hpMax) //코루틴 없이 HP값, HP바 마스크, 미터의 비율을 즉시 변경
    {
        _txt_hp.text = hp.ToString();
        _txt_hpMax.text = hpMax.ToString();

        _hpMask.fillAmount = hp / (float)hpMax;
        _hpMeter.fillAmount = hp / (float)hpMax;
    }

    IEnumerator HpMask_Down()   //줄어든 미터만큼 마스크 지우기
    {
        while (true)
        {
            if (_hpMask.fillAmount > _hpMeter.fillAmount)
                _hpMask.fillAmount -= 0.01f;
            else
                StopCoroutine("HpMask_Down");

            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator HpMask_Up()     //늘어난 마스크만큼 미터 늘리기
    {
        while (true)
        {
            if (_hpMask.fillAmount > _hpMeter.fillAmount)
                _hpMeter.fillAmount += 0.01f;
            else
                StopCoroutine("HpMask_Up");

            yield return new WaitForSeconds(0.01f);
        }
    }

    public void Change_Ac(int newAc)
    {
        _txt_ac.text = newAc.ToString();
    }

    public void Change_AcMax(int newAcMax)
    {
        _icon_ac.SetActive(newAcMax != 0);

        if (newAcMax != 0)
            _txt_acMax.text = newAcMax.ToString();
    }

    public void Change_ApMeter(int newAp)
    {
        for (int i = 0; i < _meter_ap.Length; i++)
        {
            if (_meter_ap[i].gameObject.activeSelf) //활성화된 오브젝트의 경우 (최대 행동력에 해당하는 미터)
            {
                _meter_ap[i].gameObject.SetActive(true);    //오브젝트 활성화
                if (i < newAp)  //현재 행동력에 해당하는 미터이면
                    _meter_ap[i].GetChild(0).gameObject.SetActive(true);    //미터 아이콘 활성화
                else
                {
                    _meter_ap[i].GetChild(0).gameObject.SetActive(false);   //미터 아이콘 비활성화
                    _meter_ap[i].GetChild(1).gameObject.SetActive(false);   //사용예정 아이콘 비활성화
                }
            }
        }

        _cursor_nowAp = newAp;
    }

    public void Change_ApMeterMax(int newApMax)
    {
        for (int i = 0; i < _meter_ap.Length; i++)
            _meter_ap[i].gameObject.SetActive(i < newApMax);
    }

    public void Change_Ap_UsePreview(int useAp)
    {
        if (useAp >= 0)
        {
            var tempAp = 0;

            for (int i = _cursor_nowAp - 1; i >= 0; i--)                  //현재 커서 이내의 행동력 미터
            {
                if (useAp > tempAp)
                    _meter_ap[i].transform.GetChild(1).gameObject.SetActive(true);
                else
                    _meter_ap[i].transform.GetChild(1).gameObject.SetActive(false);

                tempAp++;
            }
        }
    }
}
