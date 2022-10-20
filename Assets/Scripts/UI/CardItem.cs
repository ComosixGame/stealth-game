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
    private bool boughtItem;

    private void Awake() {
        gameManager = GameManager.Instance;
    }

    private void OnEnable() {
        gameManager.OnSelectItem.AddListener(OnSelect);
        gameManager.OnBuyItem.AddListener(OnBuy);

        buyButton.onClick.AddListener(BuyItem);
        selectButton.onClick.AddListener(SelectItem);
    }

    public void SetCardItem(Player playerCharacter, int idItems, bool bought, bool selected) {
        typeItem = TypeItem.Character;
        thumb.sprite = playerCharacter.thumb;
        nameItem.text = playerCharacter.nameCharacter;
        buyButton.interactable = !bought;
        selectButton.interactable = !selected && bought;
        buyButtonText.text = bought ? "Owned" : "$" + playerCharacter.price.ToString();
        selectButtonText.text = selected ? "Selected" : "Select";
        price = playerCharacter.price;
        id = idItems;
        boughtItem = bought;
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
            selectButton.interactable = true && boughtItem;
            selectButtonText.text = "Select";
        }
    }

    private void OnBuy(int _id) {
        if(id == _id) {
            boughtItem = true;
            selectButton.interactable = true;
        }
    }

    private void OnDisable() {
        gameManager.OnSelectItem.RemoveListener(OnSelect);
        gameManager.OnBuyItem.RemoveListener(OnBuy);
        buyButton.onClick.RemoveListener(BuyItem);
        buyButton.onClick.RemoveListener(SelectItem);
    }
}
