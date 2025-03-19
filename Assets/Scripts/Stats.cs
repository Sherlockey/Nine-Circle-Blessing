using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Stats : MonoBehaviour
{
    public event EventHandler<Stats> OnEnemyHealthChanged;
    public event EventHandler<OnActionResolvedEventArgs> OnActionResolved;
    public event EventHandler<GameObject> OnDead;

    public class OnActionResolvedEventArgs : EventArgs
    {
        public GameObject GameObject;
        public string String;
        public Color Color;
        public bool CanPopupStack;
    }

    public float DamageDealtScalar = 1;
    public float DamageTakenScalar = 1;
    protected float _damagedTargetScalarIncrease = 0f;
    protected float _undamagedTargetScalarIncrease = 0f;

    [SerializeField] protected float _baseHealth;
    [SerializeField] protected float _baseMana;
    [SerializeField] protected float _baseDamage;
    [SerializeField] protected float _baseSpeed;
    [SerializeField] protected float _defaultCooldownMax;
    [SerializeField] protected float _baseCooldownReduction;
    [SerializeField] protected float _baseArmor;
    [SerializeField] protected float _baseEvasion;
    [SerializeField] protected float _baseLeech;
    [SerializeField] protected float _baseArea;

    protected const float DEFAULT_COOLDOWN_RATE = 1f;
    protected const float EVASION_CAP = 80.0f;

    [SerializeField] protected int _baseLevel;

    public float Health { get; protected set; }
    public float MaxHealth { get; protected set; }
    public float Mana { get; protected set; }
    public float MaxMana { get; protected set; }
    public float Damage { get; protected set; }
    public float Speed { get; protected set; }
    public float Cooldown { get; protected set; }
    public float CooldownMax { get; protected set; }
    public float CooldownReduction { get; protected set; }
    public float Armor { get; protected set; }
    public float Evasion { get; protected set; }
    public float Leech { get; protected set; }
    public float Area { get; protected set; }

    public int Level { get; protected set; }

    protected virtual void Awake()
    {
        InitializeBaseStats();
    }

    //TODO speed might need to also be multiplied by basespeedscalar before setting it here
    protected virtual void InitializeBaseStats()
    {
        Health = _baseHealth;
        MaxHealth = _baseHealth;
        Mana = _baseMana;
        MaxMana = _baseMana;
        Damage = _baseDamage;
        Speed = _baseSpeed;
        CooldownMax = _defaultCooldownMax;
        CooldownReduction = _baseCooldownReduction;
        Armor = _baseArmor;
        Evasion = UnityEngine.Mathf.Min(_baseEvasion, EVASION_CAP);
        Leech = _baseLeech;
        Area = _baseArea;
    }

    public void ProcessAttack(Stats source, float damageAmount, bool canPopupStack)
    {
        if (CheckIfHit() == true)
        {
            TakeDamage(source, damageAmount, canPopupStack);
        }
        else
        {
            AudioManager.Instance.PlaySoundEffect("evaded", 0.25f);
            OnActionResolved?.Invoke(this, new OnActionResolvedEventArgs() 
            { GameObject = gameObject, String = "Evaded", Color = Color.white, CanPopupStack = canPopupStack });
        }
    }

    public bool CheckIfHit()
    {
        float hitRoll = UnityEngine.Random.Range(0, 100);

        return hitRoll >= Evasion;
    }

    public void TakeDamage(Stats source, float amount, bool canPopupStack)
    {
        //TODO added this int damage thing. not sure if i like the design
        float damage = (amount - Armor) * DamageTakenScalar;
        int intDamage = (int)MathF.Round(damage);
        if (intDamage < 1)
        {
            intDamage = 1;
        }

        Health -= intDamage;

        if (Health < 0)
        {
            Health = 0;
        }

        OnActionResolved?.Invoke(this, new OnActionResolvedEventArgs() 
        { GameObject = gameObject, String = intDamage.ToString(), Color = Color.white, CanPopupStack = canPopupStack });

        if (gameObject.CompareTag("Enemy"))
        {
            OnEnemyHealthChanged?.Invoke(this, this);
        }

        if (gameObject.CompareTag("Player"))
        {
            if (source.gameObject.name.StartsWith("Ghoul"))
            {
                AudioManager.Instance.PlaySoundEffect("ghoul_attack", 0.25f);
            }
            if (source.gameObject.name.StartsWith("Imp"))
            {
                AudioManager.Instance.PlaySoundEffect("imp_attack", 0.25f);
            }

        }

        if (source.Leech > 0)
        {
            source.Heal(intDamage * source.Leech/100.0f, canPopupStack);
        }

        if (Health <= 0)
        {
            Die();
        }
    }

    public void TakeNonLethalDamage(float amount, bool canPopupStack)
    {
        //TODO added this int damage thing. not sure if i like the design
        float damage = (amount - Armor) * DamageTakenScalar;
        int intDamage = (int)MathF.Round(damage);
        if (intDamage < 1)
        {
            intDamage = 1;
        }

        Health -= intDamage;

        if (Health <= 0)
        {
            Health = 1;
        }

        if (gameObject.CompareTag("Enemy"))
        {
            OnEnemyHealthChanged?.Invoke(this, this);
        }

        OnActionResolved?.Invoke(this, new OnActionResolvedEventArgs() 
        { GameObject = gameObject, String = intDamage.ToString(), Color = Color.white, CanPopupStack = canPopupStack });
    }

    protected virtual void Die()
    {
        OnDead?.Invoke(this, gameObject);
    }

    public void Heal(float healAmount, bool canPopupStack)
    {
        //TODO added this int heal thing. not sure if i like the design
        float healthBeforeHeal = Health;
        int intHeal = (int)MathF.Round(healAmount);
        Health += intHeal;
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }

        float amountHealed = Health - healthBeforeHeal;

        if (amountHealed > 0)
        {
            OnActionResolved?.Invoke(this, new OnActionResolvedEventArgs() 
            { GameObject = gameObject, String = Mathf.Round(amountHealed).ToString(), Color = Color.green, CanPopupStack = canPopupStack });
        }
    }

    public float GetDamagedTargetScalarIncrease()
    {
        return _damagedTargetScalarIncrease;
    }

    public float GetUndamagedTargetScalarIncrease()
    {
        return _undamagedTargetScalarIncrease;
    }

    public void SetDamagedTargetScalarIncrease(float amount)
    {
        _damagedTargetScalarIncrease = amount;
    }

    public void SetUndamagedTargetScalarIncrease(float amount)
    {
        _undamagedTargetScalarIncrease = amount;
    }
}
