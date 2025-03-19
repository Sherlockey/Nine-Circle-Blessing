using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindDancerPact : Pact
{
    private float _evasionScalar = 0.5f;
    private float _armorScalar = 0f;

    public override void PactEffect()
    {
        PlayerStats playerStats = GetComponent<PlayerStats>();
        playerStats.EvasionScalar += _evasionScalar;
        playerStats.ArmorScalar = _armorScalar;
        playerStats.RefreshPlayerStats();
    }

    public override void RevertPactEffect()
    {
        PlayerStats playerStats = GetComponent<PlayerStats>();
        playerStats.EvasionScalar -= _evasionScalar;
        playerStats.ArmorScalar = 1;
        playerStats.RefreshPlayerStats();
    }
}
