using UnityEngine;

[CreateAssetMenu(fileName = "Character Manager", menuName = "Character Manager")]
public class CharacterManager : ScriptableObject
{
    public Player[] Characters;
    public int CharactersCount {
        get {
            return Characters.Length;
        }
    }
}
