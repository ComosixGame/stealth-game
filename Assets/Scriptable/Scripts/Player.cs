using UnityEngine;

[CreateAssetMenu(fileName = "New Player", menuName = "Character/Player")]
public class Player : ScriptableObject
{
    public string nameCharacter;
    public GameObject character, radoll;
}
