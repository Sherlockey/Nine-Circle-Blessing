using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pact : MonoBehaviour
{
    [SerializeField] private string _pactName;
    [SerializeField] private string _description;
    [SerializeField] private bool _isEnabled;

    public void SetEnabled(bool value)
    {
        _isEnabled = value;
    }

    public string GetName()
    {
        return _pactName;
    }

    public string GetDescription()
    {
        return _description;
    }

    public bool GetEnabled()
    {
        return _isEnabled;
    }

    public abstract void PactEffect();

    public abstract void RevertPactEffect();
}
