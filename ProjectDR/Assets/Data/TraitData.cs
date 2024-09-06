using UnityEngine;

[CreateAssetMenu(fileName = "Trait", menuName = "ScrObj/TraitData")]  //Ŀ���� �޴� �߰�

public class TraitData : ScriptableObject
{
    [Header("# Main Info")]
    [TextArea(1, 3)]
    public string NAME;     //Ư�� �̸�

    [TextArea(3, 8)]
    public string INFO;     //Ư�� ����
}
