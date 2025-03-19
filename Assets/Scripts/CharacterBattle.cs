using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBattle : MonoBehaviour
{
    public event EventHandler<CharacterBattle> OnTurnReached;
    public event EventHandler<CharacterBattle> OnTurnEnded;
    public event EventHandler<CharacterBattle> OnActionPointsUpdated;

    public IAction SelectedAction { get; private set; }
    public GameObject Target { get; private set; }
    public Stats Stats { get; private set; }
    public float ActionPoints { get; private set; }

    private void Awake()
    {
        Stats = GetComponent<Stats>();
    }

    private void Update()
    {
        if (BattleManager.Instance != null && BattleManager.Instance.IsActive)
        {
            ActionPoints += Stats.Speed * Time.deltaTime;

            OnActionPointsUpdated?.Invoke(this, this);

            if (ActionPoints >= 100)
            {
                OnTurnReached?.Invoke(this, this);
            }
        }
    }

    public void SetActionPoints(float actionPoints)
    {
        ActionPoints = actionPoints;
        OnActionPointsUpdated?.Invoke(this, this);
    }

    public void Attack(Stats sourceStats, CharacterBattle targetCharacterBattle, float actionDamageScalar, bool canPopupStack)
    {
        float damage = 0;
        List<CharacterBattle> areaTargetCharacterBattleList = new List<CharacterBattle>();

        if (sourceStats.Area > 0)
        {
            foreach (GameObject enemy in BattleManager.Instance.EnemyList)
            {
                CharacterBattle enemyCharacterBattle = enemy.GetComponent<CharacterBattle>();
                if (enemyCharacterBattle != targetCharacterBattle)
                {
                    areaTargetCharacterBattleList.Add(enemyCharacterBattle);
                }
            }
        }

        //if target is damaged use this calculation
        if (targetCharacterBattle.Stats.Health < targetCharacterBattle.Stats.MaxHealth)
        {
            float damageDealtScalar = Stats.DamageDealtScalar + Stats.GetDamagedTargetScalarIncrease();
            damage = Stats.Damage * actionDamageScalar * damageDealtScalar;
        }
        //if target is undamaged use this calculation
        else if (targetCharacterBattle.Stats.Health == targetCharacterBattle.Stats.MaxHealth)
        {
            float damageDealtScalar = Stats.DamageDealtScalar + Stats.GetUndamagedTargetScalarIncrease();
            damage = Stats.Damage * actionDamageScalar * damageDealtScalar;
        }

        targetCharacterBattle.Stats.ProcessAttack(Stats, damage, canPopupStack);

        foreach (CharacterBattle characterBattle in areaTargetCharacterBattleList) 
        {
            characterBattle.Stats.ProcessAttack(Stats, damage * (sourceStats.Area/100.0f), false);
        }

        EndTurn();
    }

    public void Heal(CharacterBattle targetCharacterBattle, float amount, bool canPopupStack)
    {
        targetCharacterBattle.Stats.Heal(amount, canPopupStack);
    }

    private void EndTurn()
    {
        ActionPoints = 0;

        OnTurnEnded?.Invoke(this, this);
    }

    public void SetSelectedAction(IAction action)
    {
        SelectedAction = action;
    }
}
