using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trinket : MonoBehaviour
{
    [SerializeField] private string _trinketSuffix;
    [SerializeField] private string _description;
    [SerializeField] private bool _isEnabled;
    [SerializeField] private bool _isOwned;
    [SerializeField] private int _cost;
    [SerializeField] private Sprite _sprite;

    protected virtual void Awake()
    {
        BattleManager.OnBattleStarted += BattleManager_OnBattleStarted;
    }

    private void BattleManager_OnBattleStarted(object sender, System.EventArgs e)
    {
        if (!_isEnabled)
        {
            return;
        }

        if (GameManager.Instance.CurrentCircleNumber == 1 && GameManager.Instance.CurrentEncounterNumber == 1)
        {
            InitializeTrinket();
        }
    }

    public string GetSuffix()
    {
        return _trinketSuffix;
    }

    public string GetDescription()
    {
        return _description;
    }

    public int GetCost()
    { 
        return _cost; 
    }

    public void SetEnabled(bool value)
    {
        _isEnabled = value;
    }

    public bool GetEnabled()
    {
        return _isEnabled;
    }

    public void SetOwned(bool value)
    {
        _isOwned = value;
    }

    public bool GetOwned()
    {
        return _isOwned;
    }

    public Sprite GetSprite()
    {
        return _sprite;
    }

    protected abstract void InitializeTrinket();

    public abstract void TrinketEffect();

    protected void OnDestroy()
    {
        BattleManager.OnBattleStarted -= BattleManager_OnBattleStarted;
    }
}
