using UnityEngine;

[CreateAssetMenu(fileName = "Character Manager", menuName = "Equipment/Equipment Manager")]
public class EquipmentManager : ScriptableObject
{
    public PlayerCharacter[] Characters;
    public PLayerWeapon[] Weapons;
    public int CharactersCount {
        get {
            return Characters.Length;
        }
    }
    public int WeaponsCount {
        get {
            return Weapons.Length;
        }
    }
}
