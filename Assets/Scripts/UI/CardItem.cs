using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardItem : MonoBehaviour
{
    public Image thumb;
    public TextMeshProUGUI nameItem, buyButtonText, selectButtonText;
    public Button buyButton, selectButton;
    public AudioClip audioClip;
    [Range(0,1)] public float volumeScale;
    public TypeItem typeItem;
    public int price;
    public int id;
    private GameManager gameManager;
    private SoundManager soundManager;
    private bool boughtItem;
    public static event Action OnBuyFailed;
    private void Awake() {
        gameManager = GameManager.Instance;
        soundManager = SoundManager.Instance;
    }

    private void OnEnable() {
        gameManager.OnSelectItem.AddListener(OnSelect);

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
        soundManager.PlayOneShot(audioClip, volumeScale);
        if(bought) {
            buyButton.interactable = false;
            buyButtonText.text = "Owned";
        } else {
            OnBuyFailed?.Invoke();
        }
    }

    private void SelectItem() {
        bool selected = gameManager.SelectItem(id, typeItem);
        soundManager.PlayOneShot(audioClip, volumeScale);
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

    private void OnDisable() {
        gameManager.OnSelectItem.RemoveListener(OnSelect);
        buyButton.onClick.RemoveListener(BuyItem);
        buyButton.onClick.RemoveListener(SelectItem);
    }
}
