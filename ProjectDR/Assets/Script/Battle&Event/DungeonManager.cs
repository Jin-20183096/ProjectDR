using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager DgnManager = null;

    public enum DungeonFloor
    {
        Floor1, Floor2
    }

    //���� �� ������
    public DungeonFloor NowFloor;

    [SerializeField]
    private GameObject _btn_floorUp; //�������� �̵� ��ư
    [SerializeField]
    private GameObject _btn_floorDown;  //�Ʒ������� �̵� ��ư

    //�� ���� ���� ������ ĳ��
    public DungeonData Data_Floor1;
    public DungeonData Data_Floor2;

    void Awake()
    {
        if (DgnManager)
        {
            DestroyImmediate(gameObject);
            return;
        }
        else
        {
            DgnManager = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void FloorUpButton_OnOff(bool b)
        => _btn_floorUp.SetActive(b);

    public void FloorDownButton_OnOff(bool b)
        => _btn_floorDown.SetActive(b);

    //�������� �̵�
    public void Move_FloorUp()
    {
        var floor = (int)NowFloor - 1;

        if (floor < 0)
            Debug.Log("�� �̻� �ö� �� ����");
        else
        {
            SceneManager.LoadSceneAsync(floor, LoadSceneMode.Single);
            NowFloor = (DungeonFloor)floor;
        }
    }
    //�Ʒ������� �̵�
    public void Move_FloorDown()
    {
        var floor = (int)NowFloor + 1;

        if (floor >= SceneManager.sceneCountInBuildSettings)
            Debug.Log("�� �̻� ������ �� ����");
        else
        {
            SceneManager.LoadSceneAsync(floor, LoadSceneMode.Single);
            NowFloor = (DungeonFloor)floor;
        }
    }
}
