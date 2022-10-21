using System.Collections.Generic;
using UnityEngine;

public enum TypeItem {
    Character,
    Weapon
}
public class Shop : MonoBehaviour
{
    [SerializeField] private EquipmentManager equipmentManager;
    public GameObject cardItem;
    public Transform CharactersContainer;
    public Transform WeaponsContainer;

    // Start is called before the first frame update

    private void Awake() {
        PlayerData playerData = PlayerData.Load();
        List<int> charactersOwned = playerData.characters;
        int selectedCharacter = playerData.selectedCharacter;
        List<int> weaponsOwned = playerData.weapons;
        int selectedWeapon = playerData.selectedWeapon;

        PlayerCharacter[] playerCharacters = equipmentManager.Characters;
        RenderCardItem(
            playerCharacters,
            CharactersContainer,
            equipmentManager.CharactersCount,
            charactersOwned,
            selectedCharacter
        );
        
        PLayerWeapon[] pLayerWeapons = equipmentManager.Weapons;
        RenderCardItem(
            pLayerWeapons,
            WeaponsContainer,
            equipmentManager.WeaponsCount,
            weaponsOwned,
            selectedWeapon
        );

    }

    private void RenderCardItem(PlayerEquipment[] listItem, Transform parent, int itemCount, List<int> ItemOwned, int currentItem) {
        for(int i = 0; i < itemCount; i++) {
            CardItem cardItemCopy = Instantiate(cardItem, parent.position, parent.rotation).GetComponent<CardItem>();
            cardItemCopy.transform.SetParent(parent, false);
            bool bought = ItemOwned.IndexOf(i) != -1;
            bool selected = i == currentItem;
            cardItemCopy.SetCardItem(listItem[i], i, bought, selected);
        }
    }


}
