using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityUpgrade : MonoBehaviour
{
    public event EventHandler<string> OnAbilityUpgradeSet;
    public event EventHandler<string> OnAbilityUpgradeReverted;

    [SerializeField] private List<AbilityUpgradeSO> _abilityUpgradeSOList;
    public Dictionary<string, bool> AbilityUpgradeDictionary { get; private set; } = new Dictionary<string, bool>();

    private void Awake()
    {
        foreach (AbilityUpgradeSO abilityUpgradeSO in _abilityUpgradeSOList)
        {
            AbilityUpgradeDictionary.Add(abilityUpgradeSO.UpgradeName, abilityUpgradeSO.Enabled);
        }
    }

    public void SetAbilityUpgradeEnabled(string abilityUpgradeName, bool value)
    {
        if (AbilityUpgradeDictionary.ContainsKey(abilityUpgradeName))
        {
            AbilityUpgradeDictionary[abilityUpgradeName] = value;
            if (value == true)
            {
                OnAbilityUpgradeSet?.Invoke(this, abilityUpgradeName);
            }
        }
    }

    public string[] GetNAvailableAbilityUpgrades(int n)
    {
        //Generate a list of possible strings from all unallocated ability upgrades
        List<string> possibleStrings = new List<string>();
        foreach (KeyValuePair<string, bool> entry in AbilityUpgradeDictionary)
        {
            if (entry.Value == false)
            {
                possibleStrings.Add(entry.Key);
            }
        }

        //fisher yates shuffle
        for (int i = 0; i < possibleStrings.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, possibleStrings.Count);
            string temp = possibleStrings[i];
            possibleStrings[i] = possibleStrings[randomIndex];
            possibleStrings[randomIndex] = temp;
        }

        //get first three from shuffled list
        string[] finalStringArray = new string[n];

        for (int i = 0; i < finalStringArray.Length; i++)
        {
            finalStringArray[i] = possibleStrings[i];
        }
        return finalStringArray;
    }

    public AbilityUpgradeSO GetAbilityUpgradeSOFromString(string s)
    {
        foreach (AbilityUpgradeSO abilityUpgradeSO in _abilityUpgradeSOList)
        {
            if (s == abilityUpgradeSO.UpgradeName)
            {
                return abilityUpgradeSO;
            }
        }
        print("Error. Failed to find AbilityUpgradeSO from given string: " + s);
        return null;
    }

    public void RevertAllAbilityUpgrades()
    {
        Dictionary<string, bool> toBeDisabledDicionary = new Dictionary<string, bool>();
        foreach (KeyValuePair<string, bool> entry in AbilityUpgradeDictionary)
        {
            if (entry.Value == true)
            {
                toBeDisabledDicionary.Add(entry.Key, entry.Value);
            }
        }
        foreach (KeyValuePair<string, bool> entry in toBeDisabledDicionary)
        {
            string abilityUpgradeName = entry.Key;
            SetAbilityUpgradeEnabled(abilityUpgradeName, false);
            OnAbilityUpgradeReverted?.Invoke(this, abilityUpgradeName);
        }
    }
}
