using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlessedPact : Pact
{
    private PlayerStats _playerStats;
    private float _healthRegenIncrease = 0.01f;

    private void Awake()
    {
        _playerStats = GetComponent<PlayerStats>();
    }

    public override void PactEffect()
    {
        _playerStats.SetHealthRegen(_playerStats.HealthRegen + _healthRegenIncrease);
        _playerStats.RefreshPlayerStats();
    }

    public override void RevertPactEffect()
    {
        _playerStats.SetHealthRegen(_playerStats.HealthRegen - _healthRegenIncrease);
        _playerStats.RefreshPlayerStats();
    }
}
