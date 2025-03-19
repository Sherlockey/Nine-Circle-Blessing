using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainOfTormentAction : MonoBehaviour, IAction
{
    public event EventHandler<bool> OnOnClick;

    [SerializeField] AbilityUpgrade _abilityUpgrade;

    private float _actionAttackScalar = 0.8f;
    private bool _doesSearchForTarget = false;
    private float _tormentorsThirstCooldownRestorePercentage = 0.25f;
    private bool _isConcentratedStormEnabled = false;
    private float _baseConcentratedStormAttackScalar = 0.20f;
    private float _concentratedStormAttackScalar = 0f;
    private int _concentratedStormEnemyCountTarget = 5;
    private float _hellfireAttackScalar = 0.50f;
    private float _hellfireSelfAttackScalar = 0.20f;
    private bool _isHellfireEnabled = false;
    private bool _isFirstRainEnabled = false;
    private float _firstRainAttackScalar = 0.30f;

    delegate void OnKill();
    private OnKill _onKill;

    delegate void OnCast();
    private OnCast _onCast;

    private void Start()
    {
        _abilityUpgrade.OnAbilityUpgradeSet += AbilityUpgrade_OnAbilityUpgradeSet;
        _abilityUpgrade.OnAbilityUpgradeReverted += AbilityUpgrade_OnAbilityUpgradeReverted;
    }

    private void AbilityUpgrade_OnAbilityUpgradeReverted(object sender, string upgradeName)
    {
        switch (upgradeName)
        {
            case "Tormentor's Thirst":
                _onKill = null;
                break;
            case "Concentrated Storm":
                _isConcentratedStormEnabled = false;
                break;
            case "Hellfire":
                _isHellfireEnabled = false;
                RevertHellfireUpgrade();
                _onCast = null;
                break;
            case "First Rain":
                _isFirstRainEnabled = false;
                break;
            default:
                break;
        }
    }

    private void AbilityUpgrade_OnAbilityUpgradeSet(object sender, string upgradeName)
    {
        switch (upgradeName)
        {
            case "Tormentor's Thirst":
                _onKill += TormentorsThirstEffect;
                break;
            case "Concentrated Storm":
                _isConcentratedStormEnabled = true;
                break;
            case "Hellfire":
                _isHellfireEnabled = true;
                ApplyHellfireUpgrade();
                _onCast += HellfireEffect;
                break;
            case "First Rain":
                _isFirstRainEnabled = true;
                break;
            default:
                break;
        }
    }

    public void OnClick()
    {
        if (EvaluateAction() == false)
        {
            return;
        }

        OnOnClick?.Invoke(this, _doesSearchForTarget);

        BattleManager.Instance.Player.GetComponent<CharacterBattle>().SetSelectedAction(this);
        _concentratedStormAttackScalar = _baseConcentratedStormAttackScalar * (_concentratedStormEnemyCountTarget - BattleManager.Instance.EnemyList.Count);
    }

    public void Execute(CharacterBattle sourceCharacterBattle, CharacterBattle targetCharacterBattle)
    {
        if (_isConcentratedStormEnabled)
        {
            if (_isFirstRainEnabled == true && IsTargetDamaged(targetCharacterBattle.Stats) == false)
            {
                sourceCharacterBattle.Attack(GetComponent<Stats>(), targetCharacterBattle, _actionAttackScalar * (1 + _firstRainAttackScalar + _concentratedStormAttackScalar), false);
            }
            else
            {
                sourceCharacterBattle.Attack(GetComponent<Stats>(), targetCharacterBattle, _actionAttackScalar * (1 + _concentratedStormAttackScalar), false);
            }
        }
        else
        {
            if (_isFirstRainEnabled == true && IsTargetDamaged(targetCharacterBattle.Stats) == false)
            {
                sourceCharacterBattle.Attack(GetComponent<Stats>(), targetCharacterBattle, _actionAttackScalar * (1 + _firstRainAttackScalar), false);
            }
            else
            {
                sourceCharacterBattle.Attack(GetComponent<Stats>(), targetCharacterBattle, _actionAttackScalar, false);
            }
        }

        if (DidExecutionKill(targetCharacterBattle.Stats))
        {
            if (_onKill != null)
            {
                _onKill();
            }
        }

        AudioManager.Instance.PlaySoundEffect("rain_of_torment", 0.25f);
    }


    private bool EvaluateAction()
    {
        if (BattleManager.Instance.Player.GetComponent<Stats>().Cooldown > 0)
        {
            return false;
        }
        if (_isHellfireEnabled)
        {
            if (WillHellfireKillPlayer())
            {
                return false;
            }
        }
        return true;
    }

    public void PayCost()
    {
        gameObject.GetComponent<PlayerStats>().StartCooldown();

        if (_onCast != null)
        {
            _onCast();
        }
    }
    private bool DidExecutionKill(Stats targetStats)
    {
        return targetStats.Health <= 0;
    }

    private bool IsTargetDamaged(Stats stats)
    {
        return stats.Health < stats.MaxHealth;
    }

    private void TormentorsThirstEffect()
    {
        PlayerStats playerStats = GetComponent<PlayerStats>();
        float cooldownToRestore = playerStats.CooldownMax * _tormentorsThirstCooldownRestorePercentage;
        playerStats.RestoreCooldown(cooldownToRestore);
    }

    private void ApplyHellfireUpgrade()
    {
        _actionAttackScalar *= (1 + _hellfireAttackScalar);
    }

    private void RevertHellfireUpgrade()
    {
        _actionAttackScalar /= (1 + _hellfireAttackScalar);
    }

    private void HellfireEffect()
    {
        CharacterBattle playerCharacterBattle = GetComponent<CharacterBattle>();
        playerCharacterBattle.Attack(GetComponent<Stats>(), playerCharacterBattle, _actionAttackScalar * _hellfireSelfAttackScalar, false);
    }

    private bool WillHellfireKillPlayer()
    {
        Stats stats = GetComponent<Stats>();
        float damage = stats.Damage * _actionAttackScalar * _hellfireSelfAttackScalar;
        int intDamage = (int)MathF.Round(damage);
        if (intDamage < 1)
        {
            intDamage = 1;
        }
        if (stats.Health <= intDamage)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
