using System.Collections;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    Color ColorStart = new Color(1, 1, 1, 1);
    Color ColorEnd = new Color(1, 1, 1, 0);

    [SerializeField]
    private TextMeshPro _txt_outline;
    [SerializeField]
    private TextMeshPro _txt;

    private void Start()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSecondsRealtime(0.005f);

        if (_txt.color.a > 0)
        {
            _txt_outline.color = new Color(_txt_outline.color.r, _txt_outline.color.g, _txt_outline.color.b, _txt_outline.color.a - 0.01f);
            _txt.color = new Color(_txt.color.r, _txt.color.g, _txt.color.b, _txt.color.a - 0.005f);

            transform.position = new Vector3(transform.position.x, transform.position.y + 0.007f, transform.position.z + 0.01f);
            StartCoroutine(FadeOut()); //코루틴 재실행
        }
        else
            Destroy(gameObject);
    }
}
