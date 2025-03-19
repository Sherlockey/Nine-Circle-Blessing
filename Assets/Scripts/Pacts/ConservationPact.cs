using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConservationPact : Pact
{
    private CharacterBattle _characterBattle;
    private PlayerStats _playerStats;
    private float _manaRemainingDamageScalarIncrease = 0;
    private float _manaRemainingScalar = 0.5f;


    private void Awake()
    {
        _characterBattle = GetComponent<CharacterBattle>();
        _playerStats = GetComponent<PlayerStats>();
    }
    public override void PactEffect()
    {
        _characterBattle.OnTurnReached += CharacterBattle_OnTurnReached;
    }

    public override void RevertPactEffect()
    {
        _characterBattle.OnTurnReached -= CharacterBattle_OnTurnReached;
    }

    private void CharacterBattle_OnTurnReached(object sender, CharacterBattle characterBattle)
    {
        _playerStats.DamageDealtScalar -= _manaRemainingDamageScalarIncrease;
        _manaRemainingDamageScalarIncrease = _playerStats.Mana / _playerStats.MaxMana * _manaRemainingScalar;
        _playerStats.DamageDealtScalar += _manaRemainingDamageScalarIncrease;
    }

    
}
