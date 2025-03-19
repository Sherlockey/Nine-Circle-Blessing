using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentOfChaosPact : Pact
{
    private PlayerStats _playerStats;
    private float _manaMultiplier = 2;
    private float _cooldownMultiplier = 2;

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
