using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PactInventory : MonoBehaviour
{
    public event EventHandler<Pact> OnPactSet;

    [SerializeField] private Pact[] _pactArray;

    public void EnablePact(Pact pact, bool value)
    {
        if (pact.GetEnabled() != value)
        {
            pact.SetEnabled(value);
            pact.PactEffect();
            OnPactSet?.Invoke(this, pact);
        }
    }

    public Pact GetPactFromPactArrayByString(string s)
    {
        for (int i = 0; i < _pactArray.Length; i++)
        {
            if (s == _pactArray[i].GetName())
            {
                return _pactArray[i];
            }
        }
        return null;
    }

    public Pact[] GetNAvailablePacts(int n)
    {
        //Generate a list of possible strings from all unallocated ability upgrades
        List<Pact> possiblePacts = new List<Pact>();
        foreach (Pact pact in _pactArray)
        {
            if (pact.GetEnabled() == false)
            {
                possiblePacts.Add(pact);
            }
        }

        //fisher yates shuffle
        for (int i = 0; i < possiblePacts.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, possiblePacts.Count);
            Pact temp = possiblePacts[i];
            possiblePacts[i] = possiblePacts[randomIndex];
            possiblePacts[randomIndex] = temp;
        }

        //get first three from shuffled list
        Pact[] finalPactArray = new Pact[n];

        for (int i = 0; i < finalPactArray.Length; i++)
        {
            finalPactArray[i] = possiblePacts[i];
        }
        return finalPactArray;
    }

    public void RevertAllPacts()
    {
        foreach (Pact pact in _pactArray)
        {
            pact.SetEnabled(false);
            pact.RevertPactEffect();
        }
    }
}
