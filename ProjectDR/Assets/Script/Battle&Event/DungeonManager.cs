using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager DgnManager = null;

    public enum DungeonFloor
    {
        Floor1, Floor2
    }

    //현재 몇 층인지
    public DungeonFloor NowFloor;

    [SerializeField]
    private GameObject _btn_floorUp; //위층으로 이동 버튼
    [SerializeField]
    private GameObject _btn_floorDown;  //아래층으로 이동 버튼

    //각 층의 던전 데이터 캐싱
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

    //위층으로 이동
    public void Move_FloorUp()
    {
        var floor = (int)NowFloor - 1;

        if (floor < 0)
            Debug.Log("더 이상 올라갈 수 없음");
        else
        {
            SceneManager.LoadSceneAsync(floor, LoadSceneMode.Single);
            NowFloor = (DungeonFloor)floor;
        }
    }
    //아래층으로 이동
    public void Move_FloorDown()
    {
        var floor = (int)NowFloor + 1;

        if (floor >= SceneManager.sceneCountInBuildSettings)
            Debug.Log("더 이상 내려갈 수 없음");
        else
        {
            SceneManager.LoadSceneAsync(floor, LoadSceneMode.Single);
            NowFloor = (DungeonFloor)floor;
        }
    }
}
