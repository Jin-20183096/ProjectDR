using UnityEngine;

[CreateAssetMenu(fileName = "Trait", menuName = "ScrObj/TraitData")]  //커스텀 메뉴 추가

public class TraitData : ScriptableObject
{
    [Header("# Main Info")]
    [TextArea(1, 3)]
    public string NAME;     //특성 이름

    [TextArea(3, 8)]
    public string INFO;     //특성 설명
}
