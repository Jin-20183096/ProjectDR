using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "ScrObj/AbilityData")]  //커스텀 메뉴 추가

public class AbilityData : ScriptableObject
{
    [Header("# Main Info")]
    [TextArea(1, 3)]
    public string NAME;         //능력 이름

    [TextArea(3, 8)]
    public string INFO;  //능력 설명
}
