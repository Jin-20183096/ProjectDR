using UnityEngine;
using static PlayerSystem;

public class CameraSystem : MonoBehaviour
{
    private Transform _p_transf;

    private float _smoothing = 0.5f;

    private float _cameraY = 15f;
    private float _cameraZ = 9.5f;

    void Start()
    {
        _p_transf = PlayerSys.transform;
    }

    void FixedUpdate()
    {
        var strX = _p_transf.position.x.ToString("0.0");
        var strZ = (_p_transf.position.z - _cameraZ).ToString("0.0");

        Vector3 targetPos = new Vector3(float.Parse(strX), _cameraY, float.Parse(strZ));
        transform.position = Vector3.Lerp(transform.position, targetPos, _smoothing);
    }
}
