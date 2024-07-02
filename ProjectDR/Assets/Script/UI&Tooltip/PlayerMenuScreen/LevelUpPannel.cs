using UnityEngine;
using UnityEngine.UI;
using static PlayerSystem;

public class LevelUpPannel : MonoBehaviour
{
    [SerializeField]
    private Image[] _img_STR;   //�� �ֻ��� �̹���
    [SerializeField]
    private Image[] _img_INT;   //���� �ֻ��� �̹���
    [SerializeField]
    private Image[] _img_DEX;   //������ �ֻ��� �̹���
    [SerializeField]
    private Image[] _img_AGI;   //��ø �ֻ��� �̹���
    [SerializeField]
    private Image[] _img_CON;   //�ǰ� �ֻ��� �̹���
    [SerializeField]
    private Image[] _img_WIL;   //���� �ֻ��� �̹���

    [SerializeField]
    private Sprite[] _spr_diceSide;

    public void Set_StatDisplay()
    {
        var stat_str = PlayerSys.STR;
        var stat_int = PlayerSys.INT;
        var stat_dex = PlayerSys.DEX;
        var stat_agi = PlayerSys.AGI;
        var stat_con = PlayerSys.CON;
        var stat_wil = PlayerSys.WIL;

        for (int i = 0; i < stat_str.Length; i++)   //��
        {
            if (stat_str[i] >= 10)
                _img_STR[i].sprite = _spr_diceSide[10];
            else if (stat_str[i] <= 0)
                _img_STR[i].sprite = _spr_diceSide[0];
            else
                _img_STR[i].sprite = _spr_diceSide[stat_str[i]];
        }

        for (int i = 0; i < stat_int.Length; i++)   //����
        {
            if (stat_int[i] >= 10)
                _img_INT[i].sprite = _spr_diceSide[10];
            else if (stat_int[i] <= 0)
                _img_INT[i].sprite = _spr_diceSide[0];
            else
                _img_INT[i].sprite = _spr_diceSide[stat_int[i]];
        }

        for (int i = 0; i < stat_dex.Length; i++)   //������
        {
            if (stat_dex[i] >= 10)
                _img_DEX[i].sprite = _spr_diceSide[10];
            else if (stat_dex[i] <= 0)
                _img_DEX[i].sprite = _spr_diceSide[0];
            else
                _img_DEX[i].sprite = _spr_diceSide[stat_dex[i]];
        }

        for (int i = 0; i < stat_agi.Length; i++)   //��ø
        {
            if (stat_agi[i] >= 10)
                _img_AGI[i].sprite = _spr_diceSide[10];
            else if (stat_agi[i] <= 0)
                _img_AGI[i].sprite = _spr_diceSide[0];
            else
                _img_AGI[i].sprite = _spr_diceSide[stat_agi[i]];
        }

        for (int i = 0; i < stat_con.Length; i++)   //�ǰ�
        {
            if (stat_con[i] >= 10)
                _img_CON[i].sprite = _spr_diceSide[10];
            else if (stat_con[i] <= 0)
                _img_CON[i].sprite = _spr_diceSide[0];
            else
                _img_CON[i].sprite = _spr_diceSide[stat_con[i]];
        }

        for (int i = 0; i < stat_wil.Length; i++)   //����
        {
            if (stat_wil[i] >= 10)
                _img_WIL[i].sprite = _spr_diceSide[10];
            else if (stat_wil[i] <= 0)
                _img_WIL[i].sprite = _spr_diceSide[0];
            else
                _img_WIL[i].sprite = _spr_diceSide[stat_wil[i]];
        }
    }
}
