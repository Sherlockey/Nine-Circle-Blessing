using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : Stats
{
    public static event EventHandler<int> OnEnemyDead;

    [SerializeField] private float _maxLevelHealth;
    [SerializeField] private float _maxLevelDamage;
    [SerializeField] private float _maxLevelSpeed;
    [SerializeField] private float _maxLevelArmor;

    private int _experienceReward;
    private const int EXPERIENCE_SCALAR = 10;

    protected override void Awake()
    {
        base.Awake();
        InitializeEnemyStats();
    }

    private void InitializeEnemyStats()
    {
        Level = GameManager.Instance.CurrentCircleNumber;
        _experienceReward = GameManager.Instance.CurrentCircleNumber * EXPERIENCE_SCALAR;

        int encounters = GameManager.Instance.CurrentCircleNumber * GameManager.Instance.MAX_ENCOUNTER_NUMBER
            - GameManager.Instance.MAX_ENCOUNTER_NUMBER + GameManager.Instance.CurrentEncounterNumber;

        float healthPerEncounter = (_maxLevelHealth - _baseHealth) / (GameManager.Instance.MAX_ENCOUNTER_NUMBER * GameManager.Instance.MAX_CIRCLE_NUMBER);
        Health = Mathf.Ceil(_baseHealth + (healthPerEncounter * encounters -1));
        MaxHealth = Health;
        float damagePerEncounter = (_maxLevelDamage - _baseDamage) / (GameManager.Instance.MAX_ENCOUNTER_NUMBER * GameManager.Instance.MAX_CIRCLE_NUMBER);
        Damage = Mathf.Ceil(_baseDamage + (damagePerEncounter * encounters -1));
        float speedPerEncounter = (_maxLevelSpeed - _baseSpeed) / (GameManager.Instance.MAX_ENCOUNTER_NUMBER * GameManager.Instance.MAX_CIRCLE_NUMBER);
        Speed = Mathf.Ceil(_baseSpeed + (speedPerEncounter * encounters - 1));
        float armorPerEncounter = (_maxLevelArmor - _baseArmor) / (GameManager.Instance.MAX_ENCOUNTER_NUMBER * GameManager.Instance.MAX_CIRCLE_NUMBER);
        Armor = Mathf.Ceil(_baseArmor + (armorPerEncounter * encounters - 1));
    }

    protected override void Die()
    {
        base.Die();
        OnEnemyDead?.Invoke(this, _experienceReward);
    }
}
