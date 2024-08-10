using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoPannel : MonoBehaviour
{
    [Header("# HP")]
    [SerializeField]
    private Image _hpMask;         //HP�� ����ũ
    [SerializeField]
    private Image _hpMeter;        //HP�� ����
    [SerializeField]
    private TextMeshProUGUI _txt_hp;    //HP �ؽ�Ʈ           
    [SerializeField]
    private TextMeshProUGUI _txt_hpMax; //�ִ� HP �ؽ�Ʈ

    [Header("# AC")]
    [SerializeField]
    private GameObject _icon_ac;        //�� ������
    [SerializeField]
    private TextMeshProUGUI _txt_ac;    //�� �ؽ�Ʈ
    [SerializeField]
    private TextMeshProUGUI _txt_acMax; //�ִ� �� �ؽ�Ʈ

    [Header("# AP")]
    [SerializeField]
    private Transform[] _meter_ap;     //�ൿ�� ����(�����ܵ�)
    [SerializeField]
    private int _cursor_nowAp;          //���� �ൿ�� �������� ���� �ε����� ����Ű�� Ŀ��

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

    public void Change_HpBar(int hp, int hpMax) //�ڷ�ƾ ���� HP��, HP�� ����ũ, ������ ������ ��� ����
    {
        _txt_hp.text = hp.ToString();
        _txt_hpMax.text = hpMax.ToString();

        _hpMask.fillAmount = hp / (float)hpMax;
        _hpMeter.fillAmount = hp / (float)hpMax;
    }

    IEnumerator HpMask_Down()   //�پ�� ���͸�ŭ ����ũ �����
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

    IEnumerator HpMask_Up()     //�þ ����ũ��ŭ ���� �ø���
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
            if (_meter_ap[i].gameObject.activeSelf) //Ȱ��ȭ�� ������Ʈ�� ��� (�ִ� �ൿ�¿� �ش��ϴ� ����)
            {
                _meter_ap[i].gameObject.SetActive(true);    //������Ʈ Ȱ��ȭ
                if (i < newAp)  //���� �ൿ�¿� �ش��ϴ� �����̸�
                    _meter_ap[i].GetChild(0).gameObject.SetActive(true);    //���� ������ Ȱ��ȭ
                else
                {
                    _meter_ap[i].GetChild(0).gameObject.SetActive(false);   //���� ������ ��Ȱ��ȭ
                    _meter_ap[i].GetChild(1).gameObject.SetActive(false);   //��뿹�� ������ ��Ȱ��ȭ
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

            for (int i = _cursor_nowAp - 1; i >= 0; i--)                  //���� Ŀ�� �̳��� �ൿ�� ����
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
