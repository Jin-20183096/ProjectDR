using UnityEngine;
using UnityEngine.SceneManagement;

public class FloorButton : MonoBehaviour
{
    [SerializeField]
    private int floorNum;

    public void FloorMove()
    {
        SceneManager.LoadSceneAsync("Dungeon" + floorNum);
    }
}
