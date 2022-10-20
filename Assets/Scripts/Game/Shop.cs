using System.Collections.Generic;
using UnityEngine;

public enum TypeItem {
    Character,
    Weapon
}
public class Shop : MonoBehaviour
{
    [SerializeField] private CharacterManager characterManager;
    public GameObject cardItem;
    public Transform contentContainer;

    // Start is called before the first frame update

    private void Awake() {
        PlayerData playerData = PlayerData.Load();
        List<int> charactersOwned = playerData.characters;
        int selectedCharacter = playerData.selectedCharacter;

        Player[] playerCharacters = characterManager.Characters;
        for(int i = 0; i < characterManager.CharactersCount; i++) {
            CardItem cardItemCopy = Instantiate(cardItem, contentContainer.position, contentContainer.rotation).GetComponent<CardItem>();
            cardItemCopy.transform.SetParent(contentContainer, false);
            bool bought = charactersOwned.IndexOf(i) != -1;
            bool selected = i == selectedCharacter;
            cardItemCopy.SetCardItem(playerCharacters[i], i, bought, selected);
        }
    }


}
