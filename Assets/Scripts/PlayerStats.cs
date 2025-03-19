using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Stats
{
    public event EventHandler<OnCooldownChangedEventArgs> OnCooldownChanged;
    public class OnCooldownChangedEventArgs : EventArgs
    {
        public float Cooldown;
        public float CooldownMax;
    }

    public event EventHandler<PlayerStats> OnHealthChanged;
    public event EventHandler<PlayerStats> OnManaChanged;
    public event EventHandler<int> OnLevelChanged;
    public event EventHandler<Stats> OnStatsChanged;
    public event EventHandler<int> OnExperienceChanged;

    private float _baseMaxHealthScalar = 1;
    private float _baseMaxManaScalar = 1;
    private float _baseDamageScalar = 1;
    private float _baseSpeedScalar = 1;
    private float _baseCooldownMaxScalar = 1;
    private float _baseArmorScalar = 1;
    private float _baseEvasionScalar = 1;
    private float _baseLeechScalar = 1;
    private float _baseAreaScalar = 1;

    public float MaxHealthScalar = 1;
    public float MaxManaScalar = 1;
    public float DamageScalar = 1;
    public float SpeedScalar = 1;
    public float CooldownMaxScalar = 1;
    public float ArmorScalar = 1;
    public float EvasionScalar = 1;
    public float LeechScalar = 1;
    public float AreaScalar = 1;

    private float _gearHealth;
    private float _gearMana;
    private float _gearDamage;
    private float _gearSpeed;
    private float _gearCooldownReduction;
    private float _gearArmor;
    private float _gearEvasion;
    private float _gearLeech;
    private float _gearArea;

    [SerializeField] private GearInventory _gearInventory;

    [SerializeField] private int _baseExperienceToLevel;
    [SerializeField] private int _baseExperience;

    public int ExperienceToLevel { get; private set; }
    public int Experience { get; private set; }

    public float HealthRegen { get; private set; } = 0.01f;
    public float ManaRegen { get; private set; } = 0.05f;

    private const int EXPERIENCE_TO_LEVEL_SCALE_AMOUNT = 10;

    private const float HEALTH_PER_LEVEL = 0.5f;
    private const float MANA_PER_LEVEL = 0.1f;
    private const float DAMAGE_PER_LEVEL = 0.1f;
    private const float SPEED_PER_LEVEL = 0.1f;
    private const float COOLDOWN_REDUCTION_PER_LEVEL = 0.1f;
    private const float ARMOR_PER_LEVEL = 0.1f;
    private const float EVASION_PER_LEVEL = 0.1f;
    private const float LEECH_PER_LEVEL = 0.05f;
    private const float AREA_PER_LEVEL = 0.05f;

    protected override void Awake()
    {
        base.Awake();
        InitializePlayerStats();

        _gearInventory.OnGearEquipped += GearInventory_OnGearEquipped;
        _gearInventory.OnGearUnequipped += GearInventory_OnGearUnequipped;

        EnemyStats.OnEnemyDead += EnemyStats_OnEnemyDead;
    }

    private void InitializePlayerStats()
    {
        CooldownMax = (_defaultCooldownMax - ((CooldownReduction * 0.01f * _defaultCooldownMax))) * CooldownMaxScalar;
        Level = _baseLevel;
        ExperienceToLevel = _baseExperienceToLevel;
        Experience = _baseExperience;
    }

    public void RevertAllPlayerStats()
    {
        MaxHealthScalar = _baseMaxHealthScalar;
        MaxManaScalar = _baseMaxManaScalar;
        DamageScalar = _baseDamageScalar;
        SpeedScalar = _baseSpeedScalar;
        CooldownMaxScalar = _baseCooldownMaxScalar;
        ArmorScalar = _baseArmorScalar;
        EvasionScalar = _baseEvasionScalar;
        LeechScalar = _baseLeechScalar;
        AreaScalar = _baseAreaScalar;

        DamageDealtScalar = 1;
        DamageTakenScalar = 1;
        _damagedTargetScalarIncrease = 0f;
        _undamagedTargetScalarIncrease = 0f;

        Health = _baseHealth;
        MaxHealth = _baseHealth;
        Mana = _baseMana;
        MaxMana = _baseMana;
        Damage = _baseDamage;
        Speed = _baseSpeed;
        Cooldown = 0;
        CooldownMax = _defaultCooldownMax;
        CooldownReduction = _baseCooldownReduction;
        Armor = _baseArmor;
        Evasion = UnityEngine.Mathf.Min(_baseEvasion, EVASION_CAP);
        Leech = _baseLeech;
        Area = _baseArea;

        CooldownMax = (_defaultCooldownMax - ((CooldownReduction * 0.01f * _defaultCooldownMax))) * CooldownMaxScalar;
    }

    private bool CheckIfLevelThresholdMet()
    {
        if (Experience >= ExperienceToLevel)
        {
            return true;
        }
        return false;
    }

    private void LevelUp()
    {
        Level++;
        LevelUpStatIncrease();
        ProcessExperienceChanges();
        OnLevelChanged?.Invoke(this, Level);
    }

    private void LevelUpStatIncrease()
    {
        _baseHealth += HEALTH_PER_LEVEL;
        _baseMana += MANA_PER_LEVEL;
        _baseDamage += DAMAGE_PER_LEVEL;
        _baseSpeed += SPEED_PER_LEVEL;
        _baseCooldownReduction += COOLDOWN_REDUCTION_PER_LEVEL;
        _baseArmor += ARMOR_PER_LEVEL;
        _baseEvasion += EVASION_PER_LEVEL;
        _baseLeech += LEECH_PER_LEVEL;
        _baseArea += AREA_PER_LEVEL;

        RefreshPlayerStats();
    }

    public void GainExperience(int amount)
    {
        if (Level < 99)
        {
            Experience += amount;
            //maybe a while loop instead to account for cases where you gain enough experience in one kill to get multiple levels?
            if (CheckIfLevelThresholdMet())
            {
                LevelUp();
            }
            OnExperienceChanged?.Invoke(this, Experience);
        }
    }

    private void ProcessExperienceChanges()
    {
        Experience %= ExperienceToLevel;
        ExperienceToLevel += EXPERIENCE_TO_LEVEL_SCALE_AMOUNT;
        if (Level == 99)
        {
            ExperienceToLevel = 0;
        }
    }

    private void EnemyStats_OnEnemyDead(object sender, int experienceReward)
    {
        GainExperience(experienceReward);
    }

    private void Update()
    {
        if (BattleManager.Instance != null && BattleManager.Instance.IsActive)
        {
            RegenerateMana();
            UpdateCooldown();
            RegenerateHealth();
        }
    }

    private void RegenerateMana()
    {

        //TODO might want to round mana when it becomes your turn? or dole out a point value from the battlemanager to each stats every time the timer increases?
        if (Mana < MaxMana)
        {
            Mana += ManaRegen * MaxMana * Time.deltaTime;
            if (Mana > MaxMana)
            {
                Mana = MaxMana;
            }
            OnManaChanged?.Invoke(this, this);
        }
    }

    private void UpdateCooldown()
    {
        if (Cooldown > 0)
        {
            Cooldown -= DEFAULT_COOLDOWN_RATE * Time.deltaTime;
            if (Cooldown < 0)
            {
                Cooldown = 0;
            }
            OnCooldownChanged(this, new OnCooldownChangedEventArgs() { Cooldown = Cooldown, CooldownMax = CooldownMax });
        }
    }

    private void RegenerateHealth()
    {
        if (Health < MaxHealth)
        {
            Health += HealthRegen * MaxHealth * Time.deltaTime;
            if (Health > MaxHealth)
            {
                Health = MaxHealth;
            }
            OnHealthChanged?.Invoke(this, this);
        }
    }

    public void RestoreMana(float amount)
    {
        Mana += amount;

        if (Mana > MaxMana)
        {
            Mana = MaxMana;
        }
        OnManaChanged?.Invoke(this, this);
    }

    public void RemoveMana(float amount)
    {
        Mana -= amount;

        //DEBUG
        if (Mana < 0)
        {
            print("Mana is less than 0. Likely a bug");
        }
        OnManaChanged?.Invoke(this, this);
    }

    public void RestoreCooldown(float amount)
    {
        Cooldown -= amount;

        if (Cooldown < 0)
        {
            Cooldown = 0;
        }
        OnCooldownChanged(this, new OnCooldownChangedEventArgs() { Cooldown = Cooldown, CooldownMax = CooldownMax });
    }

    public void StartCooldown()
    {
        Cooldown = CooldownMax;
        OnCooldownChanged(this, new OnCooldownChangedEventArgs() { Cooldown = Cooldown, CooldownMax = CooldownMax });
    }

    public void SetCooldown(float value)
    {
        Cooldown = value;
        OnCooldownChanged(this, new OnCooldownChangedEventArgs() { Cooldown = Cooldown, CooldownMax = CooldownMax });
    }

    public void SetHealthRegen(float value)
    {
        HealthRegen = value;
    }

    private void GearInventory_OnGearEquipped(object sender, Gear gear)
    {
        _gearHealth += gear.Health;
        _gearMana += gear.Mana;
        _gearDamage += gear.Damage;
        _gearSpeed += gear.Speed;
        _gearCooldownReduction += gear.CooldownReduction;
        _gearArmor += gear.Armor;
        _gearEvasion += gear.Evasion;
        _gearLeech += gear.Leech;
        _gearArea += gear.Area;

        RefreshPlayerStats();
    }

    public void RefreshPlayerStats()
    {
        float oldMaxHealth = MaxHealth; //old max health was 10
        MaxHealth = (_baseHealth + _gearHealth) * MaxHealthScalar; //now max health is 5
        float healthScalar = oldMaxHealth / MaxHealth; //scalar is 2
        Health *= ((1 / healthScalar)); //if health was 8, health is now 4
        if (Health <= 0)
        {
            Health = 1;
        }

        float oldMaxMana = MaxMana;
        MaxMana = (_baseMana + _gearMana) * MaxManaScalar;
        float manaScalar = oldMaxMana / MaxMana;
        Mana *= (1 / manaScalar);
        if (Mana < 0)
        {
            Mana = 0;
        }

        Damage = (_baseDamage + _gearDamage) * DamageScalar;
        Speed = (_baseSpeed + _gearSpeed) * SpeedScalar;
        CooldownReduction = _baseCooldownReduction + _gearCooldownReduction;
        CooldownMax = (_defaultCooldownMax - ((CooldownReduction * 0.01f * _defaultCooldownMax))) * CooldownMaxScalar;
        Armor = (_baseArmor + _gearArmor) * ArmorScalar;
        Evasion = (_baseEvasion + _gearEvasion) * EvasionScalar;
        Evasion = UnityEngine.Mathf.Min(Evasion, EVASION_CAP);
        Leech = (_baseLeech + _gearLeech) * LeechScalar;
        Area = (_baseArea + _gearArea) * AreaScalar;

        OnStatsChanged?.Invoke(this, this);
    }

    private void GearInventory_OnGearUnequipped(object sender, Gear gear)
    {
        _gearHealth -= gear.Health;
        _gearMana -= gear.Mana;
        _gearDamage -= gear.Damage;
        _gearSpeed -= gear.Speed;
        _gearCooldownReduction -= gear.CooldownReduction;
        _gearArmor -= gear.Armor;
        _gearEvasion -= gear.Evasion;
        _gearLeech -= gear.Leech;
        _gearArea -= gear.Area;

        RefreshPlayerStats();
    }

    private void OnDestroy()
    {
        EnemyStats.OnEnemyDead -= EnemyStats_OnEnemyDead;
    }
}
