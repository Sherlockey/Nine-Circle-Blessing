using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PridefulDestruction : Trinket
{
    private PlayerStats _playerStats;
    private float _healPercentage = 0.02f;

    protected override void Awake()
    {
        base.Awake();
        _playerStats = GetComponent<PlayerStats>();
    }

    protected override void InitializeTrinket()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnStatsDead += BattleManager_OnStatsDead;
        }
    }

    private void BattleManager_OnStatsDead(object sender, GameObject gameObject)
    {
        print(BattleManager.Instance);
        print("OnStatsDead");
        if (BattleManager.Instance != null)
        {
            if (gameObject.tag == "Enemy")
            {
                print("trinketeffect");
                TrinketEffect();
            }
        }
    }

    public override void TrinketEffect()
    {
        print("PridefulDestruction trinketEffect()");
        _playerStats.Heal(_playerStats.MaxHealth * _healPercentage, false);
    }
}
