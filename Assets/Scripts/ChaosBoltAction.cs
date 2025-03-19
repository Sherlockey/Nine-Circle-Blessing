using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosBoltAction : MonoBehaviour, IAction
{
    public event EventHandler<bool> OnOnClick;

    [SerializeField] AbilityUpgrade _abilityUpgrade;
    [SerializeField] float _baseManaCost = 25f;

    private float _actionAttackScalar = 2.5f;
    private float _twinnedChaosAttackScalar = 0.5f;
    private float _manaCost;
    private bool _doesSearchForTarget = true;
    private float _chaosEnsuesManaCostScalar = 0.50f;
    private float _entropyFeastManaRestorePercentage = 0.50f;
    private bool _isPandemoniumEnabled = false;
    private float _pandemoniumAttackScalar = 0.30f;
    bool _canPopupStack = false;

    delegate void OnHit();
    private OnHit _onHit;

    delegate void OnKill();
    private OnKill _onKill;

    private void Start()
    {
        _abilityUpgrade.OnAbilityUpgradeSet += AbilityUpgrade_OnAbilityUpgradeSet;
        _abilityUpgrade.OnAbilityUpgradeReverted += AbilityUpgrade_OnAbilityUpgradeReverted;
        _manaCost = _baseManaCost;
        
    }

    private void AbilityUpgrade_OnAbilityUpgradeReverted(object sender, string upgradeName)
    {
        switch (upgradeName)
        {
            case "Twinned Chaos":
                _onHit = null;
                _canPopupStack = false;
                break;
            case "Chaos Ensues":
                RevertChaosEnsuesEffect();
                break;
            case "Entropy Feast":
                _onHit = null;
                break;
            case "Pandemonium":
                _isPandemoniumEnabled = false;
                break;
            default:
                break;
        }
    }

    private void AbilityUpgrade_OnAbilityUpgradeSet(object sender, string upgradeName)
    {
        switch (upgradeName)
        {
            case "Twinned Chaos":
                _onHit += TwinnedChaosEffect;
                _canPopupStack = true;
                break;
            case "Chaos Ensues":
                ChaosEnsuesEffect();
                break;
            case "Entropy Feast":
                _onKill += EntropyFeastEffect;
                break;
            case "Pandemonium":
                _isPandemoniumEnabled = true;
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
    }
    public void Execute(CharacterBattle sourceCharacterBattle, CharacterBattle targetCharacterBattle)
    {
        if (_isPandemoniumEnabled == true && IsTargetDamaged(targetCharacterBattle.Stats) == true)
        {
            sourceCharacterBattle.Attack(GetComponent<Stats>(), targetCharacterBattle, _actionAttackScalar * (1 + _pandemoniumAttackScalar), _canPopupStack);
        }
        else
        {
            sourceCharacterBattle.Attack(GetComponent<Stats>(), targetCharacterBattle, _actionAttackScalar, _canPopupStack);
        }

        if (_onHit != null)
        {
            _onHit();
        }
        if (DidExecutionKill(targetCharacterBattle.Stats))
        {
            if (_onKill != null)
            {
                _onKill();
            }
        }

        AudioManager.Instance.PlaySoundEffect("chaos_bolt", 0.25f);
    }

    private bool EvaluateAction()
    {
        if (BattleManager.Instance.Player.GetComponent<Stats>().Mana < _manaCost)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void PayCost()
    {
        gameObject.GetComponent<PlayerStats>().RemoveMana(_manaCost);
    }

    private bool DidExecutionKill(Stats targetStats)
    {
        return targetStats.Health <= 0;
    }

    private bool IsTargetDamaged(Stats stats)
    {
        return stats.Health < stats.MaxHealth;
    }

    private void TwinnedChaosEffect()
    {
        CharacterBattle playerCharacterBattle = GetComponent<CharacterBattle>();
        if (BattleManager.Instance.EnemyList.Count > 0)
        {
            GameObject secondaryTarget = BattleManager.Instance.EnemyList[UnityEngine.Random.Range(0, BattleManager.Instance.EnemyList.Count)];
            playerCharacterBattle.Attack(GetComponent<Stats>(), secondaryTarget.GetComponent<CharacterBattle>(), _actionAttackScalar * _twinnedChaosAttackScalar, _canPopupStack);
        }
    }

    private void ChaosEnsuesEffect()
    {
        _manaCost *= _chaosEnsuesManaCostScalar;
    }

    private void RevertChaosEnsuesEffect()
    {
        _manaCost /= _chaosEnsuesManaCostScalar;
    }

    private void EntropyFeastEffect()
    {
        PlayerStats playerStats = GetComponent<PlayerStats>();
        float manaToRestore = playerStats.MaxMana * _entropyFeastManaRestorePercentage;
        playerStats.RestoreMana(manaToRestore);
    }
}
