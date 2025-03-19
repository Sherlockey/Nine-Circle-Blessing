using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Items : MonoBehaviour
{
    [SerializeField] private Button _closeItemsButton;
    [SerializeField] private PotionBagSO[] _potionBagSOArray;
    [SerializeField] private ExplosiveBagSO[] _explosiveBagSOArray;
    [SerializeField] private Transform _equippedPotionBagLayoutGroup;
    [SerializeField] private Transform _equippedExplosiveBagLayoutGroup;
    [SerializeField] private Transform _equippedTrinketLayoutGroup;
    [SerializeField] private Transform _ownedTrinketsLayoutGroup;
    [SerializeField] private GameObject _itemPrefab;

    private void Awake()
    {
        _closeItemsButton.onClick.AddListener(() => { Hide(gameObject); });
    }

    private void OnEnable()
    {
        SetEquippedPotionBag();
        SetEquippedExplosiveBag();
        SetEquippedTrinket();
        SetOwnedTrinkets();
    }

    private void SetEquippedPotionBag()
    {
        if (_equippedPotionBagLayoutGroup.childCount > 1)
        {
            Destroy(_equippedPotionBagLayoutGroup.GetChild(1).gameObject);
        }

        PotionBagSO bestPotionBagSO = null;
        foreach (PotionBagSO potionBagSO in _potionBagSOArray)
        {
            if (potionBagSO.IsOwned == true)
            {
                bestPotionBagSO = potionBagSO;
            }
        }

        GameObject potionBag = Instantiate(_itemPrefab, _equippedPotionBagLayoutGroup);
        Destroy(potionBag.GetComponent<Button>());

        if (bestPotionBagSO != null)
        {
            potionBag.transform.GetChild(0).GetComponent<TMP_Text>().text = bestPotionBagSO.ObjectName; //set name
            potionBag.transform.GetChild(1).GetComponent<TMP_Text>().text = ""; //set cost to empty
            potionBag.transform.GetChild(2).GetComponent<TMP_Text>().text = bestPotionBagSO.Description; //set description
        }
        else
        {
            potionBag.transform.GetChild(0).GetComponent<TMP_Text>().text = ""; //set name to empty
            potionBag.transform.GetChild(1).GetComponent<TMP_Text>().text = ""; //set cost to Error Message
            potionBag.transform.GetChild(2).GetComponent<TMP_Text>().text = "No Potion Bag Owned"; //set description to empty
        }
    }

    private void SetEquippedExplosiveBag()
    {
        if (_equippedExplosiveBagLayoutGroup.childCount > 1)
        {
            Destroy(_equippedExplosiveBagLayoutGroup.GetChild(1).gameObject);
        }

        ExplosiveBagSO bestExplosiveBagSO = null;
        foreach (ExplosiveBagSO explosiveBagSO in _explosiveBagSOArray)
        {
            if (explosiveBagSO.IsOwned == true)
            {
                bestExplosiveBagSO = explosiveBagSO;
            }
        }

        GameObject explosiveBag = Instantiate(_itemPrefab, _equippedExplosiveBagLayoutGroup);
        Destroy(explosiveBag.GetComponent<Button>());

        if (bestExplosiveBagSO != null)
        {
            explosiveBag.transform.GetChild(0).GetComponent<TMP_Text>().text = bestExplosiveBagSO.ObjectName; //set name
            explosiveBag.transform.GetChild(1).GetComponent<TMP_Text>().text = ""; //set cost to empty
            explosiveBag.transform.GetChild(2).GetComponent<TMP_Text>().text = bestExplosiveBagSO.Description; //set description
        }
        else
        {
            explosiveBag.transform.GetChild(0).GetComponent<TMP_Text>().text = ""; //set name to empty
            explosiveBag.transform.GetChild(1).GetComponent<TMP_Text>().text = ""; //set cost to Error Message
            explosiveBag.transform.GetChild(2).GetComponent<TMP_Text>().text = "No Explosive Bag Owned"; //set description to empty
        }
    }

    private void SetEquippedTrinket()
    {
        if (_equippedTrinketLayoutGroup.childCount > 1)
        {
            Destroy(_equippedTrinketLayoutGroup.GetChild(1).gameObject);
        }

        Trinket enabledTrinket = null;
        foreach (Trinket trinket in GameManager.Instance.Player.GetComponent<TrinketInventory>().GetTrinketArray())
        {
            if (trinket.GetEnabled() == true)
            {
                enabledTrinket = trinket;
            }
        }

        GameObject trinketUI = Instantiate(_itemPrefab, _equippedTrinketLayoutGroup);
        Destroy(trinketUI.GetComponent<Button>());

        if (enabledTrinket != null)
        {
            trinketUI.transform.GetChild(0).GetComponent<TMP_Text>().text = "Trinket of " + enabledTrinket.GetSuffix(); //set name
            trinketUI.transform.GetChild(1).GetComponent<TMP_Text>().text = ""; //set cost to empty
            trinketUI.transform.GetChild(2).GetComponent<TMP_Text>().text = enabledTrinket.GetDescription(); //set description
        }
        else
        {
            trinketUI.transform.GetChild(0).GetComponent<TMP_Text>().text = ""; //set name to empty
            trinketUI.transform.GetChild(1).GetComponent<TMP_Text>().text = ""; //set cost to Error Message
            trinketUI.transform.GetChild(2).GetComponent<TMP_Text>().text = "No Trinket Owned"; //set description to empty
        }
    }

    private void SetEquippedTrinket(Trinket trinket)
    {
        if (_equippedTrinketLayoutGroup.childCount > 1)
        {
            Destroy(_equippedTrinketLayoutGroup.GetChild(1).gameObject);
        }

        GameObject trinketUI = Instantiate(_itemPrefab, _equippedTrinketLayoutGroup);
        Destroy(trinketUI.GetComponent<Button>());

        trinketUI.transform.GetChild(0).GetComponent<TMP_Text>().text = "Trinket of " + trinket.GetSuffix(); //set name
        trinketUI.transform.GetChild(1).GetComponent<TMP_Text>().text = ""; //set cost to empty
        trinketUI.transform.GetChild(2).GetComponent<TMP_Text>().text = trinket.GetDescription(); //set description
    }

    private void SetOwnedTrinkets()
    {
        foreach (Transform child in _ownedTrinketsLayoutGroup)
        {
            Destroy(child.gameObject);
        }

        foreach (Trinket trinket in GameManager.Instance.Player.GetComponent<TrinketInventory>().GetTrinketArray())
        {
            if (trinket.GetOwned() == true)
            {
                GameObject trinketUI = Instantiate(_itemPrefab, _ownedTrinketsLayoutGroup);
                trinketUI.transform.GetChild(0).GetComponent<TMP_Text>().text = "Trinket of " + trinket.GetSuffix(); //set name
                trinketUI.transform.GetChild(1).GetComponent<TMP_Text>().text = ""; //set cost to empty
                trinketUI.transform.GetChild(2).GetComponent<TMP_Text>().text = trinket.GetDescription(); //set description
                trinketUI.GetComponent<Button>().onClick.AddListener(() => { EquipTrinket(trinket); });
            }
        }
    }

    private void EquipTrinket(Trinket trinket)
    {
        TrinketInventory trinketInventory = GameManager.Instance.Player.GetComponent<TrinketInventory>();

        for (int i = 0; i < trinketInventory.GetTrinketArray().Length; i++)
        {
            trinketInventory.SetTrinketEnabled(trinketInventory.GetTrinketArray()[i], false);
        }

        //enable the trinket just clicked on
        trinketInventory.SetTrinketEnabled(trinket, true);
        SetEquippedTrinket(trinket);
    }

    private void Hide(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
}
