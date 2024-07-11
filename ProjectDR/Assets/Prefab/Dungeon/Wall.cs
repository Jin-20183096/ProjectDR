using UnityEngine;
using UnityEngine.Rendering;

public class Wall : MonoBehaviour
{
    Color ColorUnknown = new Color32(0, 0, 0, 255);  //�� ���� ��
    Color ColorDark = new Color32(120, 120, 120, 255);  //�þ� ���� ���� ��
    Color ColorSightAround = new Color32(210, 210, 210, 255);   //�����ڸ� �þ��� ��
    Color ColorSight = new Color32(255, 255, 255, 255); //�þ� ���� ��

    [SerializeField]
    private SpriteRenderer _ceiling;
    [SerializeField]
    private SpriteRenderer[] _wall;

    [SerializeField]
    private Sprite _spr_wall;

    [SerializeField]
    private bool[] _isExplored = new bool[6];   //6�� ���� �߰� ���� (12_1_2���� �ð� ����)
    [SerializeField]
    private bool[] _isVisible = new bool[6];    //6�� ���� �þ� ���� (12_1_2���� �ð� ����)

    void Awake()
    {
        for (int i = 0; i < _isVisible.Length; i++)
            SetVisible(i, false, false);
    }

    public void SetY(int y)
    {
        for (int i = 0; i < _wall.Length; i++)
        {
            if (i == 2 || i == 3)
                _wall[i].transform.parent.GetComponent<SortingGroup>().sortingOrder = y * -1;
            else
                _wall[i].transform.parent.GetComponent<SortingGroup>().sortingOrder = (y + 1) * -1;
        }
    }

    public void SetWallSprite(int i) => _wall[i].sprite = _spr_wall;

    public bool GetExplored(int i)
    {
        return _isExplored[i];
    }
    public void SetExplored(int i, bool b)
    {
        _isExplored[i] = b;

        if (b && _isVisible[i] == false)    //�þ߹ۿ��� �߰߸� �� ���
            _wall[i].color = ColorDark;
    }

    public bool GetVisible(int i)
    {
        return _isVisible[i];
    }
    public void SetVisible(int i, bool b, bool isCenter) //i���� ���� �þ� �� ����
    {
        //�þ߿� ���� ���� �߰��� ������ ó��
        if (b && _isExplored[i] == false)
            SetExplored(i, true);

        _isVisible[i] = b;

        if (b)
        {
            /*
            if (isCenter)
                _wall[i].color = ColorSight;
            else
            */
                _wall[i].color = ColorSightAround;
        }
        else
            _wall[i].color = _isExplored[i] ? ColorDark : ColorUnknown;
    }

    public void InvinsibleWall(bool b)    //õ�� ����ȭ OnOff (�÷��̾� �þ� Ȯ���� ����)
    {
        if (b)
            _ceiling.color = new Color32((byte)_ceiling.color.r, (byte)_ceiling.color.g, (byte)_ceiling.color.b, 120);
        else
            _ceiling.color = new Color32((byte)_ceiling.color.r, (byte)_ceiling.color.g, (byte)_ceiling.color.b, 255);

        _wall[0].enabled = !b;
        _wall[1].enabled = !b;
        _wall[4].enabled = !b;
        _wall[5].enabled = !b;
    }
}
