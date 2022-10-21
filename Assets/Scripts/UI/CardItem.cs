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

    public void SetCardItem(PlayerEquipment playerEquipment, int idItems, bool bought, bool selected) {
        System.Type type = playerEquipment.GetType();
        typeItem = type.Equals(typeof(PlayerCharacter)) ? TypeItem.Character : TypeItem.Weapon;
        thumb.sprite = playerEquipment.thumb;
        nameItem.text = playerEquipment.nameEquipment;
        buyButton.interactable = !bought;
        selectButton.interactable = !selected && bought;
        buyButtonText.text = bought ? "Owned" : "$" + playerEquipment.price.ToString();
        selectButtonText.text = selected ? "Selected" : "Select";
        price = playerEquipment.price;
        id = idItems;
        boughtItem = bought;
    }

    private void BuyItem() {
        bool bought = gameManager.BuyItem(id, price, typeItem);
        if(bought) {
            buyButton.interactable = false;
            buyButtonText.text = "Owned";
        }
    }

    private void SelectItem() {
       bool selected = gameManager.SelectItem(id, typeItem);
       if(selected) {
            selectButton.interactable = false;
            selectButtonText.text = "Selected";
       }
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
