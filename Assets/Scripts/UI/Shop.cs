using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField] private Button _closeShopButton;
    [SerializeField] private PotionBagSO[] _potionBagSOArray;
    [SerializeField] private ExplosiveBagSO[] _explosiveBagSOArray;
    [SerializeField] private Transform _potionBagLayoutGroup;
    [SerializeField] private Transform _explosiveBagLayoutGroup;
    [SerializeField] private Transform _trinketLayoutGroup;
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private GameObject _notEnoughGoldPrefab;
    [SerializeField] private GameObject _insufficientRequisitePrefab;
    [SerializeField] private TMP_Text _goldText;

    private void Awake()
    {
        _closeShopButton.onClick.AddListener(() => { Hide(gameObject); });
    }

    private void Start()
    {
        GameManager.Instance.Player.GetComponent<MetaInventory>().OnGoldChanged += MetaInventory_OnGoldChanged;
        SetGoldText(GameManager.Instance.Player.GetComponent<MetaInventory>().GetGoldOwned());

        CreatePotionBags();
        CreateExplosiveBags();
        CreateTrinkets();
    }

    private void MetaInventory_OnGoldChanged(object sender, int goldOwned)
    {
        SetGoldText(goldOwned);
    }

    private void SetGoldText(int amount)
    {
        _goldText.text = "Gold: " + amount.ToString() + "g";
    }

    private void BuyPotionBag(PotionBagSO potionBagSO)
    {
        if (EvaluateOwnedPotionBags(potionBagSO) == false)
        {
            string previousRankPotionBagSOName = GetPreviousRankPotionBagSO(potionBagSO).ObjectName;
            string message = "Cannot buy " + potionBagSO.ObjectName + " until " + previousRankPotionBagSOName + " is owned";
            CreateInsufficientRequisitePrefab(message, 2);

            return;
        }

        if (CanPayCost(potionBagSO.Cost))
        {
            MetaInventory metaInventory = GameManager.Instance.Player.GetComponent<MetaInventory>();
            metaInventory.RemoveGold(potionBagSO.Cost);
            SetGoldText(metaInventory.GetGoldOwned());

            GameManager.Instance.Player.GetComponent<ItemInventory>().SetPotionBagSO(potionBagSO);
            DestroyPotionBags();
            CreatePotionBags();
        }
        else
        {
            CreateNotEnoughGoldPrefab(2);
        }
    }

    private bool EvaluateOwnedPotionBags(PotionBagSO potionBagSO)
    {
        if (GameManager.Instance.Player.GetComponent<ItemInventory>().GetPotionBagSO() == null)
        {
            if (potionBagSO.Rank == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        PotionBagSO bestPotionBagSO = null;
        int bestRank = 0;

        foreach (PotionBagSO evaluatedPotionBagSO in _potionBagSOArray)
        {
            if (evaluatedPotionBagSO.IsOwned == true && evaluatedPotionBagSO.Rank >= bestRank)
            {
                bestPotionBagSO = evaluatedPotionBagSO;
                bestRank = bestPotionBagSO.Rank;
            }
        }
        return potionBagSO.Rank == bestPotionBagSO.Rank + 1;
    }

    private PotionBagSO GetPreviousRankPotionBagSO(PotionBagSO potionBagSO)
    {
        foreach (PotionBagSO evaluatedPotionBagSO in _potionBagSOArray)
        {
            if (evaluatedPotionBagSO.Rank == potionBagSO.Rank - 1)
            {
                return evaluatedPotionBagSO;
            }
        }
        return null;
    }

    private void BuyExplosiveBag(ExplosiveBagSO explosiveBagSO)
    {
        if (EvaluateOwnedExplosiveBags(explosiveBagSO) == false)
        {
            string previousRankExplosiveBagSOName = GetPreviousRankExplosiveBagSO(explosiveBagSO).ObjectName;
            string message = "Cannot buy " + explosiveBagSO.ObjectName + " until " + previousRankExplosiveBagSOName + " is owned";
            CreateInsufficientRequisitePrefab(message, 2);

            return;
        }

        if (CanPayCost(explosiveBagSO.Cost))
        {
            MetaInventory metaInventory = GameManager.Instance.Player.GetComponent<MetaInventory>();
            metaInventory.RemoveGold(explosiveBagSO.Cost);
            SetGoldText(metaInventory.GetGoldOwned());

            GameManager.Instance.Player.GetComponent<ItemInventory>().SetExplosiveBagSO(explosiveBagSO);
            DestroyExplosiveBags();
            CreateExplosiveBags();
        }
        else
        {
            CreateNotEnoughGoldPrefab(2);
        }
    }

    private bool EvaluateOwnedExplosiveBags(ExplosiveBagSO explosiveBagSO)
    {
        if (GameManager.Instance.Player.GetComponent<ItemInventory>().GetExplosiveBagSO() == null)
        {
            if (explosiveBagSO.Rank == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        ExplosiveBagSO bestExplosiveBagSO = null;
        int bestRank = 0;

        foreach (ExplosiveBagSO evaluatedExplosiveBagSO in _explosiveBagSOArray)
        {
            if (evaluatedExplosiveBagSO.IsOwned == true && evaluatedExplosiveBagSO.Rank >= bestRank)
            {
                bestExplosiveBagSO = evaluatedExplosiveBagSO;
                bestRank = bestExplosiveBagSO.Rank;
            }
        }
        return explosiveBagSO.Rank == bestExplosiveBagSO.Rank + 1;
    }

    private ExplosiveBagSO GetPreviousRankExplosiveBagSO(ExplosiveBagSO explosiveBagSO)
    {
        foreach (ExplosiveBagSO evaluatedExplosiveBagSO in _explosiveBagSOArray)
        {
            if (evaluatedExplosiveBagSO.Rank == explosiveBagSO.Rank - 1)
            {
                return evaluatedExplosiveBagSO;
            }
        }
        return null;
    }

    private void CreateInsufficientRequisitePrefab(string message, int lifetime)
    {
        GameObject insufficientRequisite = Instantiate(_insufficientRequisitePrefab, this.transform);
        insufficientRequisite.transform.Find("InsufficientRequisiteText").GetComponent<TMP_Text>().text = message;
        Destroy(insufficientRequisite, lifetime);
    }

    private void BuyTrinket(Trinket trinket)
    {
        if (CanPayCost(trinket.GetCost()))
        {
            MetaInventory metaInventory = GameManager.Instance.Player.GetComponent<MetaInventory>();
            metaInventory.RemoveGold(trinket.GetCost());
            SetGoldText(metaInventory.GetGoldOwned());

            TrinketInventory trinketInventory = GameManager.Instance.Player.GetComponent<TrinketInventory>();

            trinketInventory.SetTrinketOwned(trinket, true);

            //disable all other trinkets
            for (int i = 0; i < trinketInventory.GetTrinketArray().Length; i++)
            {
                trinketInventory.SetTrinketEnabled(trinketInventory.GetTrinketArray()[i], false);
            }
            
            //enable the trinket just bought
            trinketInventory.SetTrinketEnabled(trinket, true);

            //redraw TrinketUI in shop
            DestroyTrinkets();
            CreateTrinkets();
        }
        else
        {
            CreateNotEnoughGoldPrefab(2);
        }
    }

    private bool CanPayCost(int cost)
    {
        return cost <= GameManager.Instance.Player.GetComponent<MetaInventory>().GetGoldOwned();
    }    

    private void CreateNotEnoughGoldPrefab(int lifetime)
    {
        GameObject notEnoughGoldGameObject = Instantiate(_notEnoughGoldPrefab, this.transform);
        Destroy(notEnoughGoldGameObject, lifetime);
    }

    private void CreatePotionBags()
    {
        foreach (PotionBagSO potionBagSO in _potionBagSOArray)
        {
            if (potionBagSO.IsOwned == false)
            {
                GameObject potionBag = Instantiate(_itemPrefab, _potionBagLayoutGroup);
                potionBag.transform.GetChild(0).GetComponent<TMP_Text>().text = potionBagSO.ObjectName; //set name
                potionBag.transform.GetChild(1).GetComponent<TMP_Text>().text = potionBagSO.Cost.ToString() + "g"; //set cost
                potionBag.transform.GetChild(2).GetComponent<TMP_Text>().text = potionBagSO.Description; //set description
                potionBag.GetComponent<Button>().onClick.AddListener(() => { BuyPotionBag(potionBagSO); });
            }
        }
    }

    private void CreateExplosiveBags()
    {
        foreach (ExplosiveBagSO explosiveBagSO in _explosiveBagSOArray)
        {
            if (explosiveBagSO.IsOwned == false)
            {
                GameObject explosiveBag = Instantiate(_itemPrefab, _explosiveBagLayoutGroup);
                explosiveBag.transform.GetChild(0).GetComponent<TMP_Text>().text = explosiveBagSO.ObjectName; //set name
                explosiveBag.transform.GetChild(1).GetComponent<TMP_Text>().text = explosiveBagSO.Cost.ToString() + "g"; //set cost
                explosiveBag.transform.GetChild(2).GetComponent<TMP_Text>().text = explosiveBagSO.Description; //set description
                explosiveBag.GetComponent<Button>().onClick.AddListener(() => { BuyExplosiveBag(explosiveBagSO); });
            }
        }
    }


    private void CreateTrinkets()
    {
        //same for TrinketPrefabArray TODO need to fix the potionBagSO references here
        foreach (Trinket trinket in GameManager.Instance.Player.GetComponent<TrinketInventory>().GetTrinketArray())
        {
            if (trinket.GetOwned() == false)
            {
                GameObject trinketUI = Instantiate(_itemPrefab, _trinketLayoutGroup);
                trinketUI.transform.GetChild(0).GetComponent<TMP_Text>().text = "Trinket of " + trinket.GetSuffix(); //set name
                trinketUI.transform.GetChild(1).GetComponent<TMP_Text>().text = trinket.GetCost() + "g"; //set cost
                trinketUI.transform.GetChild(2).GetComponent<TMP_Text>().text = trinket.GetDescription(); //set description
                trinketUI.GetComponent<Button>().onClick.AddListener(() => { BuyTrinket(trinket); });
            }
        }
    }

    private void DestroyPotionBags()
    {
        //loop through layoutgroup and destroy all but the first child (which is the text label for that group)
        for (int i = 1; i < +_potionBagLayoutGroup.childCount; i++)
        {
            Destroy(_potionBagLayoutGroup.GetChild(i).gameObject);
        }
    }

    private void DestroyExplosiveBags()
    {
        //loop through layoutgroup and destroy all but the first child (which is the text label for that group)
        for (int i = 1; i < +_explosiveBagLayoutGroup.childCount; i++)
        {
            Destroy(_explosiveBagLayoutGroup.GetChild(i).gameObject);
        }
    }

    private void DestroyTrinkets()
    {
        //loop through layoutgroup and destroy all but the first child (which is the text label for that group)
        for (int i = 1; i < +_trinketLayoutGroup.childCount; i++)
        {
            Destroy(_trinketLayoutGroup.GetChild(i).gameObject);
        }
    }

    private void Hide(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
}
