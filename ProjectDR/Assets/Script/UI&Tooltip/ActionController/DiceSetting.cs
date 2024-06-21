using System.Collections;
using UnityEngine;

public class DiceSetting : MonoBehaviour
{
    [SerializeField]
    private ActionController _actController;
    [SerializeField]
    private Renderer[] _diceSide;   //�ֻ��� ���� ������

    private BoxCollider _collider;
    private Rigidbody _rigid;

    [SerializeField]
    private int _order; //���° �ֻ�������
    private Vector3 _originPos;     //�ֻ����� ���� ��ġ (���� ��ġ�� ���� ��ġ�� ���� ���� ���� �Ǵ�)
    private int _maxCounter = 80;   //�ֻ��� ���� �Ǵܿ� ���Ǵ� �ִ� ī���� (���� ī���Ͱ� �� ���� �����ϸ� ������ �Ǵ�)
    private int _stopCounter;       //���� ī����

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        _rigid = GetComponent<Rigidbody>();
    }

    public void DiceObject_OnOff(bool b)    //�ֻ��� ������Ʈ OnOff
    {
        foreach (Renderer r in _diceSide)   //�ֻ������� ������ OnOff
            r.gameObject.SetActive(b);

        _collider.enabled = b;  //�ݶ��̴� OnOff
        _rigid.useGravity = b;  //�߷¼��� OnOff
    }

    public void Change_DiceSide(Material[] mat)    //���ȿ� �°� �ֻ��� ������ ����
    {
        for (int i = 0; i < _diceSide.Length; i++)
            _diceSide[i].material = mat[i];
    }

    public void Set_DiceTransform(Vector3 boardVec) //�ֻ��� ȸ�� ����� ��ġ ������ ����
    {
        //�ֻ��� ��ġ ������ ��ġ (�־��� ���� ������)
        transform.position = new Vector3(boardVec.x, boardVec.y, boardVec.z - 2.0f);

        //������ �������� ȸ���� �ο�
        transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        _rigid.AddTorque(new Vector3(Random.value, Random.value, Random.value) * Random.Range(4, 12));
        _stopCounter = 0;
        StartCoroutine(Check_DiceStop());
    }

    IEnumerator Check_DiceStop()    //���� �ð����� �ֻ��� ��ġ�� ������ ������, �����ߴٰ� �Ǵ��ϰ� ������� actController���� ����
    {
        while (true)
        {
            if (_originPos != null)
            {
                if ((transform.position - _originPos).magnitude < 0.005f)
                {
                    _stopCounter++;
                    if (_stopCounter >= _maxCounter)    //�ִ� ���� ī���Ϳ� �����ϸ�, �� �ֻ����� ������ ��
                    {
                        _actController.DiceStop(_order, Check_DiceSide());  //����� ����
                        _actController.Add_StopDice();      //�ֻ��� �ϳ��� �����Ǿ��� �˸���
                        _actController.Check_DiceTotal();   //��� �ֻ����� �����Ǿ����� üũ(��� ���� ��, ���� ����)

                        _stopCounter = 0;   //���� �� ���� üũ�� ���� ī���� �ʱ�ȭ
                        StopCoroutine(Check_DiceStop());
                    }
                }
            }

            _originPos = transform.position;
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }

    //�ֻ��� ��� üũ
    public int Check_DiceSide()
    {
        int stopSide = -1;
        var nowZ = 1.0f;

        for (int i = 0; i < _diceSide.Length; i++)  //6�� �ֻ��� ���� ���鼭, ���� ���� y���� ���� ���� ã��
        {
            if (_diceSide[i].transform.position.z < nowZ)
            {
                stopSide = i;
                nowZ = _diceSide[i].transform.position.z;
            }
        }

        return stopSide;
    }
}
