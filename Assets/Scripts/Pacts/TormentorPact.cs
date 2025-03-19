using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TormentorPact : Pact
{
    private PlayerStats _playerStats;
    private float _manaMultiplier = 0.5f;
    private float _cooldownMultiplier = 0.5f;

    private void Awake()
    {
        _playerStats = GetComponent<PlayerStats>();
    }

    public override void PactEffect()
    {
        _playerStats.MaxManaScalar *= _manaMultiplier;
        _playerStats.CooldownMaxScalar *= _cooldownMultiplier;
        _playerStats.SetCooldown(_playerStats.Cooldown * _cooldownMultiplier);
        _playerStats.RefreshPlayerStats();
    }

    public override void RevertPactEffect()
    {
        _playerStats.MaxManaScalar /= _manaMultiplier;
        _playerStats.CooldownMaxScalar /= _cooldownMultiplier;
        _playerStats.SetCooldown(_playerStats.Cooldown / _cooldownMultiplier);
        _playerStats.RefreshPlayerStats();
    }
}
