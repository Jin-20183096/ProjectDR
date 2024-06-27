using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "ScrObj/AbilityData")]  //Ŀ���� �޴� �߰�

public class AbilityData : ScriptableObject
{
    [Header("# Main Info")]
    [TextArea(1, 3)]
    public string NAME;         //�ɷ� �̸�

    [TextArea(3, 8)]
    public string INFO;  //�ɷ� ����
}
