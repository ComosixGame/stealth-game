using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardItem : MonoBehaviour
{
    public Image thumb;
    public TextMeshProUGUI nameItem, buyButtonText, selectButtonText;
    public Button buyButton, selectButton;
    public TypeItem typeItem;
    public int price;
    public int id;
    private GameManager gameManager;

    private void Awake() {
        gameManager = GameManager.Instance;
    }

    private void OnEnable() {
        gameManager.OnSelectItem.AddListener(OnSelect);

        buyButton.onClick.AddListener(BuyItem);
        selectButton.onClick.AddListener(SelectItem);
    }

    public void SetCardItem(Player playerCharacter, int idItems, bool bought, bool selected) {
        typeItem = TypeItem.Character;
        thumb.sprite = playerCharacter.thumb;
        nameItem.text = playerCharacter.name;
        buyButton.interactable = !bought;
        selectButton.interactable = !selected && bought;
        buyButtonText.text = bought ? "Owned" : "$" + playerCharacter.price.ToString();
        selectButtonText.text = selected ? "Selected" : "Select";
        price = playerCharacter.price;
        id = idItems;
    }

    private void BuyItem() {
        bool bought = gameManager.BuyItem(id, price);
        if(bought) {
            buyButton.interactable = false;
            buyButtonText.text = "Owned";
        }
    }

    private void SelectItem() {
        gameManager.SelectItem(id);
        selectButton.interactable = false;
        selectButtonText.text = "Selected";
    }

    private void OnSelect(int _id) {
        if(id != _id) {
            selectButton.interactable = true;
            selectButtonText.text = "Select";
        }
    }

    private void OnDisable() {
        gameManager.OnSelectItem.RemoveListener(OnSelect);
        buyButton.onClick.RemoveListener(BuyItem);
        buyButton.onClick.RemoveListener(SelectItem);
    }
}
