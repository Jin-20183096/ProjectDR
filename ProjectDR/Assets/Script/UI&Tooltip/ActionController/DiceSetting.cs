using System.Collections;
using UnityEngine;
using static AudioSystem;

public class DiceSetting : MonoBehaviour
{
    [SerializeField]
    private ActionController _actController;
    [SerializeField]
    private Renderer[] _diceSide;   //주사위 면의 렌더링

    private BoxCollider _collider;
    private Rigidbody _rigid;

    [SerializeField]
    private int _order; //몇번째 주사위인지
    private Vector3 _originPos;     //주사위의 직전 위치 (현재 위치와 직전 위치를 비교해 정지 여부 판단)
    private int _maxCounter = 80;   //주사위 정지 판단에 사용되는 최대 카운터 (현재 카운터가 이 값에 도달하면 정지로 판단)
    private int _stopCounter;       //정지 카운터

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        _rigid = GetComponent<Rigidbody>();
    }

    public void DiceObject_OnOff(bool b)    //주사위 오브젝트 OnOff
    {
        foreach (Renderer r in _diceSide)   //주사위면의 렌더링 OnOff
            r.gameObject.SetActive(b);

        _collider.enabled = b;  //콜라이더 OnOff
        _rigid.useGravity = b;  //중력설정 OnOff
    }

    public void Change_DiceSide(Material[] mat)    //스탯에 맞게 주사위 렌더링 변경
    {
        for (int i = 0; i < _diceSide.Length; i++)
            _diceSide[i].material = mat[i];
    }

    public void Set_DiceTransform(Vector3 boardVec) //주사위 회전 방향과 위치 무작위 설정
    {
        //주사위 위치 무작위 배치 (주어진 범위 내에서)
        transform.position = new Vector3(boardVec.x, boardVec.y, boardVec.z - 2.0f);

        //무작위 방향으로 회전력 부여
        transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        _rigid.AddTorque(new Vector3(Random.value, Random.value, Random.value) * Random.Range(4, 12));
        _stopCounter = 0;
        StartCoroutine("Check_DiceStop");
    }
    
    IEnumerator Check_DiceStop()    //주사위 구르는 도중의 값을 족족 전달
    {
        while (true)
        {
            if (_originPos != null)
            {
                if ((transform.position - _originPos).magnitude < 0.005f)
                {
                    _stopCounter++;
                    _actController.Get_DiceValue(_order, Check_DiceSide());  //결과값 전달
                    
                    if (_stopCounter >= _maxCounter)    //최대 정지 카운터에 도달하면, 이 주사위는 정지한 것
                    {
                        _actController.Add_StopDice(_order);      //주사위 하나가 정지되었다 알리기
                        _actController.Check_AllDiceStop();   //모든 주사위가 정지되었는지 체크(모두 정지 시, 총합 산출)

                        _stopCounter = 0;   //다음 번 정지 체크를 위한 카운터 초기화
                        StopCoroutine("Check_DiceStop");
                    }
                }
            }

            _originPos = transform.position;
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }

    /*
    IEnumerator Check_DiceStop()    //주사위가 멈췄을 떄 값 전달
    {
        while (true)
        {
            if (_originPos != null)
            {
                if ((transform.position - _originPos).magnitude < 0.005f)
                {
                    _stopCounter++;
                    if (_stopCounter >= _maxCounter)    //최대 정지 카운터에 도달하면, 이 주사위는 정지한 것
                    {
                        _actController.Get_DiceValue(_order, Check_DiceSide()); //결과값 전달
                        _actController.Add_StopDice(_order);    //주사위 하나가 정지되었다 알리기
                        _actController.Check_AllDiceStop();     //모든 주사위가 정지되었는지 체크

                        _stopCounter = 0;   //다음 번 정지 체크를 위한 카운터 초기화
                        StopCoroutine("Check_DiceStop");
                    }
                }
            }

            _originPos = transform.position;
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
    */

    //주사위 결과 체크
    public int Check_DiceSide()
    {
        int stopSide = -1;
        var nowZ = 1.0f;

        for (int i = 0; i < _diceSide.Length; i++)  //6개 주사위 면을 돌면서, 가장 높은 y값을 가진 면을 찾기
        {
            if (_diceSide[i].transform.position.z < nowZ)
            {
                stopSide = i;
                nowZ = _diceSide[i].transform.position.z;
            }
        }

        return stopSide;
    }

    private void OnCollisionEnter(Collision collision)
    {
        AudioSys.Play_Sfx(Sfx.DiceRoll);
    }
}
