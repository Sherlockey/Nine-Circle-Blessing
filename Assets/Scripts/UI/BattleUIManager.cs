using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Stats;

public class BattleUIManager : MonoBehaviour
{
    public event EventHandler OnFullScreenPopupOpened;
    public event EventHandler OnFullScreenPopupClosed;

    [SerializeField] private GameObject _canvas;
    [SerializeField] private GameObject _pauseGamePopup;
    [SerializeField] private GameObject _damagePopupPrefab;
    [SerializeField] private GameObject _actionPointBarPrefab;
    [SerializeField] private GameObject _enemyHealthBarPrefab;
    [SerializeField] private GameObject _selectedChevronPrefab;
    [SerializeField] private GameObject _abilityUpgradePopupPrefab;
    [SerializeField] private GameObject _abilityUpgradePrefab;
    [SerializeField] private GameObject _pactPopupPrefab;
    [SerializeField] private GameObject _cannotPerformActionPopupPrefab;
    [SerializeField] private GameObject _pactPrefab;
    [SerializeField] private GameObject _itemActionUI;
    [SerializeField] private GameObject _gearEquipmentUI;
    [SerializeField] private GameObject _gearInventoryUI;
    [SerializeField] private GameObject _statsUI;
    [SerializeField] private GameObject _circleEncounterUI;
    [SerializeField] private GameObject _levelExperienceGoldUI;
    [SerializeField] private GameObject _skillTreePopup;
    [SerializeField] private GameObject _pactsPopup;
    [SerializeField] private Button _shadowBoltButton;
    [SerializeField] private Button _chaosBoltButton;
    [SerializeField] private Button _rainOfTormentButton;
    [SerializeField] private Button _itemButton;
    [SerializeField] private Button _potionButton;
    [SerializeField] private Button _explosiveButton;
    [SerializeField] private Button _skillTreeButton;
    [SerializeField] private Button _closeSkillTreeButton;
    [SerializeField] private Button _pactsButton;
    [SerializeField] private Button _closePactsButton;
    [SerializeField] private TMP_Text _maxHealthValueText;
    [SerializeField] private TMP_Text _maxManaValueText;
    [SerializeField] private TMP_Text _damageValueText;
    [SerializeField] private TMP_Text _speedValueText;
    [SerializeField] private TMP_Text _cooldownReductionValueText;
    [SerializeField] private TMP_Text _armorValueText;
    [SerializeField] private TMP_Text _evasionValueText;
    [SerializeField] private TMP_Text _leechValueText;
    [SerializeField] private TMP_Text _areaValueText;
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private TMP_Text _manaText;
    [SerializeField] private TMP_Text _cooldownText;
    [SerializeField] private TMP_Text _potionCountText;
    [SerializeField] private TMP_Text _explosiveCountText;
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _manaSlider;
    [SerializeField] private Slider _cooldownSlider;
    [SerializeField] private Image _trinketImage;
    [SerializeField] private Image _potionImage;
    [SerializeField] private Image _explosiveImage;
    [SerializeField] private Sprite _trinketSpriteBW;
    [SerializeField] private Sprite _potionSpriteBW;
    [SerializeField] private Sprite _explosiveSpriteBW;

    private Dictionary<GameObject, GameObject> _actionPointBarDictionary = new Dictionary<GameObject, GameObject>();
    private Dictionary<GameObject, GameObject> _enemyHealthBarDictionary = new Dictionary<GameObject, GameObject>();

    private GearInventory _gearInventory;
    private PlayerStats _playerStats;
    private AbilityUpgrade _abilityUpgrade;
    private MetaInventory _metaInventory;
    private PactInventory _pactInventory;
    private ItemInventory _itemInventory;

    private int _numberOfUpgradesToGenerate = 3;
    private int _numberOfPactsToGenerate = 3;

    public static BattleUIManager Instance;

    private int _numberOfShadowBoltUpgradesEnabled = 0;
    private int _numberOfChaosBoltUpgradesEnabled = 0;
    private int _numberOfRainOfTormentUpgradesEnabled = 0;
    private int _numberOfPactsEnabled = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        GetSpawnedPlayer();
        GetActionButtons();
        GetSkillTreeButtons();
        GetPactsButtons();

        GearGenerator.OnGearGenerated += GearGenerator_OnGearGenerated;
        BattleManager.OnBattleStarted += BattleManager_OnBattleStarted;
        GameManager.Instance.OnGameOver += GameManager_OnGameOver;

        //grab each item slot in gear equipment's gear slot container and listen to their OnGearEquipped event
        foreach (ItemSlot itemSlot in _gearEquipmentUI.transform.GetChild(0).GetChild(0).GetComponentsInChildren<ItemSlot>())
        {
            itemSlot.OnGearEquipped += ItemSlot_OnGearEquipped;
        }

        _abilityUpgrade.OnAbilityUpgradeSet += SetUpgradeInSkillTreePopup;
        _pactInventory.OnPactSet += SetPactInPactPopup;
        _metaInventory.OnGoldChanged += MetaInventory_OnGoldChanged;
        _playerStats.OnLevelChanged += PlayerStats_OnLevelChanged;
        _playerStats.OnExperienceChanged += PlayerStats_OnExperienceChanged;
        _playerStats.OnActionResolved += PlayerStats_OnActionResolved;
        _playerStats.OnHealthChanged += PlayerStats_OnHealthChanged;
        _playerStats.OnManaChanged += PlayerStats_OnManaChanged;
        _playerStats.OnCooldownChanged += PlayerStats_OnCooldownChanged;
        _itemInventory.OnPotionCountChanged += ItemInventory_OnPotionCountChanged;
        _itemInventory.OnExplosiveCountChanged += ItemInventory_OnExplosiveCountChanged;

    }
    
    private void ItemInventory_OnPotionCountChanged(object sender, int potionCount)
    {
        _potionCountText.text = potionCount.ToString();
    }
    private void ItemInventory_OnExplosiveCountChanged(object sender, int explosiveCount)
    {
        _explosiveCountText.text = explosiveCount.ToString();
    }


    public void CreateCannotPerformActionPopup(string message, float lifetime = 1.5f)
    {
        GameObject notEnoughItemsPopup = Instantiate(_cannotPerformActionPopupPrefab, _canvas.transform);
        notEnoughItemsPopup.transform.Find("CannotPerformActionText").GetComponent<TMP_Text>().text = message;
        Destroy(notEnoughItemsPopup, lifetime);
    }

    public void CreateSelectedChevron(Transform target)
    {
        GameObject selectedChevron = Instantiate(_selectedChevronPrefab, target);
        selectedChevron.transform.GetChild(0).GetChild(0).LookAt(Camera.main.transform);
        /*Quaternion lookAt = selectedChevron.transform.rotation;
        selectedChevron.transform.rotation = Quaternion.Euler(0, lookAt.y, lookAt.z);*/
        BattleManager.Instance.SelectedChevrons.Add(selectedChevron);
    }

    public void DestroySelectedChevrons()
    {
        foreach (GameObject gameObject in BattleManager.Instance.SelectedChevrons)
        {
            Destroy(gameObject);
        }
    }

    private void PlayerStats_OnHealthChanged(object sender, PlayerStats e)
    {
        UpdateHealthBar(_playerStats.Health, _playerStats.MaxHealth);
    }

    private void PlayerStats_OnActionResolved(object sender, OnActionResolvedEventArgs e)
    {
        UpdateHealthBar(_playerStats.Health, _playerStats.MaxHealth);
    }

    private void PlayerStats_OnManaChanged(object sender, PlayerStats e)
    {
        UpdateManaBar(e.Mana, e.MaxMana);
    }

    private void PlayerStats_OnCooldownChanged(object sender, PlayerStats.OnCooldownChangedEventArgs e)
    {
        UpdateCooldownBar(e.Cooldown, e.CooldownMax);
    }

    private void UpdateHealthBar(float health, float maxHealth)
    {
        _healthSlider.maxValue = maxHealth;
        _healthSlider.value = health;
        if (health < 1 && health > 0)
        {
            health = 1;
        }
        Mathf.Round(health);
        _healthText.text = "Health: " + health.ToString("0") + "/" + maxHealth.ToString("0");
    }

    private void UpdateManaBar(float mana, float maxMana)
    {
        _manaSlider.maxValue = maxMana;
        _manaSlider.value = mana;
        if (mana < 1 && mana > 0)
        {
            mana = 1;
        }
        Mathf.Round(mana);
        _manaText.text = "Mana: " + mana.ToString("0") + "/" + maxMana.ToString("0");
    }

    private void UpdateCooldownBar(float cooldown, float cooldownMax)
    {
        _cooldownSlider.maxValue = cooldownMax;
        _cooldownSlider.value = cooldown;

        if (cooldown < 1 && cooldown > 0)
        {
            cooldown = 1;
        }
        Mathf.Round(cooldown);
        _cooldownText.text = "Cooldown: " + cooldown.ToString("0") + "/" + cooldownMax.ToString("0");
    }

    private void GameManager_OnGameOver(object sender, System.EventArgs e)
    {
        StopAllCoroutines();
    }

    private void BattleManager_OnBattleStarted(object sender, System.EventArgs e)
    {
        UpdateStats(_playerStats);
        UpdateHealthBar(_playerStats.Health, _playerStats.MaxHealth);
        UpdateManaBar(_playerStats.Mana, _playerStats.MaxMana);
        UpdateCooldownBar(_playerStats.Cooldown, _playerStats.CooldownMax);

        UpdateCircleText();
        UpdateEncounterText();
        UpdateLevelText(_playerStats.Level.ToString());
        UpdateExperienceText(_playerStats.Experience.ToString());
        UpdateGoldText(_metaInventory.GetGoldOwned().ToString());

        if (GameManager.Instance.CurrentEncounterNumber == 1)
        {
            CreateAbilityUpgradePopup();
        }

        if (GameManager.Instance.CurrentEncounterNumber == 4)
        {
            CreatePactPopup();
        }

        GetSpawnedEnemies();
        SetPlayerActionBarWhite();
        SetTrinketSprite();
        SetPotionBagSprite();
        SetExplosiveBagSprite();
    }

    private void SetTrinketSprite()
    {
        Trinket trinket = GameManager.Instance.Player.GetComponent<TrinketInventory>().GetEquippedTrinket();
        if (trinket != null)
        {
            _trinketImage.sprite = trinket.GetSprite();
            _trinketImage.transform.parent.Find("Background").GetComponent<Image>().color = new Color(0.65f, 0.65f, 0.65f);
        }
        else
        {
            _trinketImage.sprite = _trinketSpriteBW;
        }
    }

    private void SetPotionBagSprite()
    {
        PotionBagSO potionBagSO = GameManager.Instance.Player.GetComponent<ItemInventory>().GetPotionBagSO();
        if (potionBagSO != null)
        {
            _potionImage.sprite = potionBagSO.Sprite;
            _potionImage.transform.parent.Find("Background").GetComponent<Image>().color = new Color(0.65f, 0.65f, 0.65f);
        }
        else
        {
            _potionImage.sprite = _potionSpriteBW;
        }
    }

    private void SetExplosiveBagSprite()
    {
        ExplosiveBagSO explosiveBagSO = GameManager.Instance.Player.GetComponent<ItemInventory>().GetExplosiveBagSO();
        if (explosiveBagSO != null)
        {
            _explosiveImage.sprite = explosiveBagSO.Sprite;
            _explosiveImage.transform.parent.Find("Background").GetComponent<Image>().color = new Color(0.65f, 0.65f, 0.65f);
        }
        else
        {
            _explosiveImage.sprite = _explosiveSpriteBW;
        }
    }

    private void PlayerStats_OnLevelChanged(object sender, int level)
    {
        UpdateLevelText(level.ToString());
    }

    private void PlayerStats_OnExperienceChanged(object sender, int experience)
    {
        UpdateExperienceText(experience.ToString());
    }

    private void MetaInventory_OnGoldChanged(object sender, int gold)
    {
        UpdateGoldText(gold.ToString());
    }
    private void ItemSlot_OnGearEquipped(object sender, System.EventArgs e)
    {
        OrganizeGear();
    }

    private void GearGenerator_OnGearGenerated(object sender, Gear gear)
    {
        //TODO each time you get a piece of gear, shift all other pieces of gear one slot down
        if (_gearInventory.GearList.Count > 0)
        {
            OrganizeGear(gear);
        }

        //set the gear's parent to GearInventoryUI>Background
        gear.transform.SetParent(_gearInventoryUI.transform.GetChild(0));

        //set the gear's anchored position to the first GearSlot in GearInventoryUI>Background>GearSlotContainer
        gear.gameObject.GetComponent<RectTransform>().anchoredPosition =
            gear.transform.parent.GetChild(0).GetChild(0).GetComponent<RectTransform>().anchoredPosition;

        gear.transform.localScale = Vector3.one;
        gear.gameObject.GetComponent<DragDrop>().SetLastSlottedPosition(gear.transform.localPosition);
    }
    private void PlayerStats_OnStatsChanged(object sender, Stats stats)
    {
        UpdateStats(stats);
        UpdateHealthBar(_playerStats.Health, _playerStats.MaxHealth);
        UpdateManaBar(_playerStats.Mana, _playerStats.MaxMana);
        UpdateCooldownBar(_playerStats.Cooldown, _playerStats.CooldownMax);
    }


    private void CreateAbilityUpgradePopup()
    {
        PauseGame();
        OnFullScreenPopupOpened?.Invoke(this, EventArgs.Empty);

        GameObject abilityUpgradePopup = Instantiate(_abilityUpgradePopupPrefab, _canvas.transform);
        //call a function which gives an array of three ability upgrade strings which are not yet enabled
        string[] generatedAbilityUpgradeNames = _abilityUpgrade.GetNAvailableAbilityUpgrades(_numberOfUpgradesToGenerate);

        //call a function which does the following given a gameobject which will be abilityUpgradePopup

        for (int i = 0; i < _numberOfUpgradesToGenerate; i++)
        {
            //Instantiate an _abilityUpgradePrefab with the parent of AbilityUpgradeLayoutGroup in abilityUpgradePopup and set its text fields
            GameObject abilityUpgrade = Instantiate(_abilityUpgradePrefab, abilityUpgradePopup.transform.GetChild(0).GetChild(0));
            AbilityUpgradeSO abilityUpgradeSO = _abilityUpgrade.GetAbilityUpgradeSOFromString(generatedAbilityUpgradeNames[i]);
            abilityUpgrade.transform.GetChild(0).GetComponent<TMP_Text>().text = abilityUpgradeSO.AbilityName;
            abilityUpgrade.transform.GetChild(1).GetComponent<TMP_Text>().text = abilityUpgradeSO.UpgradeName;
            abilityUpgrade.transform.GetChild(2).GetComponent<TMP_Text>().text = abilityUpgradeSO.Description;

            //hookup the button in that abilityUpgrade to the appropriate functions
            abilityUpgrade.GetComponent<Button>().onClick.AddListener(() =>
            {
                _abilityUpgrade.SetAbilityUpgradeEnabled(abilityUpgradeSO.UpgradeName, true);
                UnpauseGame();
                DestroyGameObject(abilityUpgradePopup);
                OnFullScreenPopupClosed?.Invoke(this, EventArgs.Empty);
            });
        }
    }

    private void CreatePactPopup()
    {
        PauseGame();
        OnFullScreenPopupOpened?.Invoke(this, EventArgs.Empty);

        GameObject pactPopup = Instantiate(_pactPopupPrefab, _canvas.transform);
        //call a function which gives an array of three ability upgrade strings which are not yet enabled
        Pact[] generatedPacts = _pactInventory.GetNAvailablePacts(_numberOfPactsToGenerate);

        //call a function which does the following given a gameobject which will be abilityUpgradePopup

        for (int i = 0; i < _numberOfPactsToGenerate; i++)
        {
            //Instantiate an _pactPopupPrefab with the parent of PactLayoutGroup in pactPopup and set its text fields
            GameObject pactUI = Instantiate(_pactPrefab, pactPopup.transform.GetChild(0).GetChild(0));
            Pact pact = _pactInventory.GetPactFromPactArrayByString(generatedPacts[i].GetName());
            pactUI.transform.GetChild(0).GetComponent<TMP_Text>().text = pact.GetName();
            pactUI.transform.GetChild(1).GetComponent<TMP_Text>().text = pact.GetDescription();

            //hookup the button in that abilityUpgrade to the appropriate functions
            pactUI.GetComponent<Button>().onClick.AddListener(() =>
            {
                _pactInventory.EnablePact(pact, true);
                UnpauseGame();
                DestroyGameObject(pactPopup);
                OnFullScreenPopupClosed?.Invoke(this, EventArgs.Empty);
            });
        }
    }

    private void OrganizeGear(Gear newestGear)
    {
        int i = 1;
        foreach (Gear gear in _gearInventory.GearList)
        {
            if (gear != newestGear)
            {
                //set the gear's anchored position to the appropriate GearSlot in GearInventoryUI>Background>GearSlotContainer
                gear.gameObject.GetComponent<RectTransform>().anchoredPosition =
                gear.transform.parent.GetChild(0).GetChild(_gearInventory.GearList.Count - i).GetComponent<RectTransform>().anchoredPosition;
                gear.gameObject.GetComponent<DragDrop>().SetLastSlottedPosition(gear.transform.localPosition);
                i++;
            }
        }
    }

    public void OrganizeGear()
    {
        int i = 1;
        foreach (Gear gear in _gearInventory.GearList)
        {
            //set the gear's anchored position to the appropriate GearSlot in GearInventoryUI>Background>GearSlotContainer
            gear.gameObject.GetComponent<RectTransform>().anchoredPosition =
                gear.transform.parent.GetChild(0).GetChild(_gearInventory.GearList.Count - i).GetComponent<RectTransform>().anchoredPosition;
            gear.gameObject.GetComponent<DragDrop>().SetLastSlottedPosition(gear.transform.localPosition);
            i++;
        }
    }

    private void GetSpawnedPlayer()
    {
        _playerStats = GameManager.Instance.Player.GetComponent<PlayerStats>();
        _playerStats.OnActionResolved += Stats_OnActionResolved;
        _playerStats.OnStatsChanged += PlayerStats_OnStatsChanged;

        _gearInventory = _playerStats.gameObject.GetComponent<GearInventory>();
        _abilityUpgrade = GameManager.Instance.Player.GetComponent<AbilityUpgrade>();
        _pactInventory = GameManager.Instance.Player.GetComponent<PactInventory>();
        _metaInventory = GameManager.Instance.Player.GetComponent<MetaInventory>();
        _itemInventory = GameManager.Instance.Player.GetComponent<ItemInventory>();

        CharacterBattle playerCharacterBattle = GameManager.Instance.Player.GetComponent<CharacterBattle>();
        playerCharacterBattle.OnTurnEnded += CharacterBattle_OnTurnEnded;
        playerCharacterBattle.OnTurnReached += CharacterBattle_OnTurnReached;
        playerCharacterBattle.OnActionPointsUpdated += CharacterBattle_OnActionPointsUpdated;

        CreateActionPointBar(playerCharacterBattle.transform, _playerStats.gameObject);
    }

    private void GetSpawnedEnemies()
    {
        foreach (GameObject enemy in BattleManager.Instance.EnemyList)
        {
            Stats enemyStats = enemy.GetComponent<Stats>();
            enemyStats.OnActionResolved += Stats_OnActionResolved;

            CharacterBattle enemyCharacterBattle = enemy.GetComponent<CharacterBattle>();
            enemyCharacterBattle.OnTurnReached += CharacterBattle_OnTurnReached;
            enemyCharacterBattle.OnTurnEnded += CharacterBattle_OnTurnEnded;
            enemyCharacterBattle.OnActionPointsUpdated += CharacterBattle_OnActionPointsUpdated;
            enemyStats.OnEnemyHealthChanged += Stats_OnEnemyHealthChanged;

            CreateActionPointBar(enemyCharacterBattle.transform, enemyStats.gameObject);
            CreateEnemyHealthBar(enemyCharacterBattle.transform, enemyStats.gameObject);
        }
    }

    private void GetActionButtons()
    {
        _shadowBoltButton.onClick.AddListener(() => {
            BattleManager.Instance.Player.GetComponent<ShadowBoltAction>().OnClick();
        });
        _chaosBoltButton.onClick.AddListener(() => {
            BattleManager.Instance.Player.GetComponent<ChaosBoltAction>().OnClick();
        });
        _rainOfTormentButton.onClick.AddListener(() => {
            BattleManager.Instance.Player.GetComponent<RainOfTormentAction>().OnClick();
        });
        _itemButton.onClick.AddListener(() => {
            ToggleItemMenu();
        });
        _potionButton.onClick.AddListener(() => {
            BattleManager.Instance.Player.GetComponent<PotionAction>().OnClick();
        });
        _explosiveButton.onClick.AddListener(() => {
            BattleManager.Instance.Player.GetComponent<ExplosiveAction>().OnClick();
        });
    }

    private void GetSkillTreeButtons()
    {
        _skillTreeButton.onClick.AddListener(() => {
            Show(_skillTreePopup);
        });
        _skillTreeButton.onClick.AddListener(() => {
            PauseGame();
        });
        _closeSkillTreeButton.onClick.AddListener(() => {
            Hide(_skillTreePopup);
        });
        _closeSkillTreeButton.onClick.AddListener(() => {
            UnpauseGame();
        });
    }

    private void GetPactsButtons()
    {
        _pactsButton.onClick.AddListener(() => {
            Show(_pactsPopup);
        });
        _pactsButton.onClick.AddListener(() => {
            PauseGame();
        });
        _closePactsButton.onClick.AddListener(() => {
            Hide(_pactsPopup);
        });
        _closePactsButton.onClick.AddListener(() => {
            UnpauseGame();
        });
    }

    private void ToggleItemMenu()
    {
        Toggle(_itemActionUI);
    }

    private void CreateActionPointBar(Transform transform, GameObject gameObject)
    {
        GameObject actionPointBar = Instantiate(_actionPointBarPrefab, transform);
        actionPointBar.transform.LookAt(Camera.main.transform);
        Quaternion lookAt = actionPointBar.transform.rotation;
        actionPointBar.transform.rotation = Quaternion.Euler(0, lookAt.y, lookAt.z);

        _actionPointBarDictionary.Add(gameObject, actionPointBar);
    }

    private void CreateEnemyHealthBar(Transform transform, GameObject gameObject)
    {
        GameObject enemyHealthBar = Instantiate(_enemyHealthBarPrefab, transform);
        enemyHealthBar.transform.LookAt(Camera.main.transform);
        Quaternion lookAt = enemyHealthBar.transform.rotation;
        enemyHealthBar.transform.rotation = Quaternion.Euler(0, lookAt.y, lookAt.z);

        _enemyHealthBarDictionary.Add(gameObject, enemyHealthBar);
    }
    
    private void CharacterBattle_OnActionPointsUpdated(object sender, CharacterBattle characterBattle)
    {
        _actionPointBarDictionary[characterBattle.gameObject].transform.GetChild(1).GetComponent<Image>().fillAmount = characterBattle.ActionPoints / 100f;
    }
    private void Stats_OnEnemyHealthChanged(object sender, Stats stats)
    {
        _enemyHealthBarDictionary[stats.gameObject].transform.GetChild(1).GetComponent<Image>().fillAmount = stats.Health / stats.MaxHealth;
    }

    private void Stats_OnActionResolved(object sender, OnActionResolvedEventArgs onActionResolvedEventArgs)
    {
        float yOffsetAmount = 0.5f;
        int offsetCount = 0;

        if (onActionResolvedEventArgs.CanPopupStack == true)
        {
            foreach (Transform child in onActionResolvedEventArgs.GameObject.transform)
            {
                if (child.name == "DamagePopup(Clone)")
                {
                    offsetCount++;
                }
            }
            CreateDamagePopupPrefab(onActionResolvedEventArgs, yOffsetAmount, offsetCount);
        }
        else
        {
            foreach (Transform child in onActionResolvedEventArgs.GameObject.transform)
            {
                if (child.name == "DamagePopup(Clone)")
                {
                    string oldStringValue = child.GetComponentInChildren<TextMeshPro>().text;
                    string newStringValue = onActionResolvedEventArgs.String;

                    if (float.TryParse(oldStringValue, out float oldStringResult) && float.TryParse(newStringValue, out float newStringResult))
                    {
                        oldStringResult += newStringResult;
                        child.GetComponentInChildren<TextMeshPro>().text = oldStringResult.ToString();
                        return;
                    }
                    else
                    {
                        offsetCount++;
                        CreateDamagePopupPrefab(onActionResolvedEventArgs, yOffsetAmount, offsetCount);
                        return;
                    }
                }
            }
            CreateDamagePopupPrefab(onActionResolvedEventArgs, yOffsetAmount, offsetCount);
        }
    }

    private void CreateDamagePopupPrefab(OnActionResolvedEventArgs onActionResolvedEventArgs, float yOffsetAmount, int offsetCount)
    {
        GameObject damagePopupPrefab = Instantiate(_damagePopupPrefab, onActionResolvedEventArgs.GameObject.transform);
        damagePopupPrefab.transform.position =
            new Vector3(damagePopupPrefab.transform.position.x, damagePopupPrefab.transform.position.y + yOffsetAmount * offsetCount, damagePopupPrefab.transform.position.z);
        damagePopupPrefab.transform.GetChild(0).LookAt(Camera.main.transform);
        Quaternion lookAt = damagePopupPrefab.transform.GetChild(0).transform.rotation;
        damagePopupPrefab.transform.GetChild(0).transform.rotation = Quaternion.Euler(lookAt.x, lookAt.y + 180, lookAt.z);
        TextMeshPro damagePopupPrefabTextMeshPro = damagePopupPrefab.GetComponentInChildren<TextMeshPro>();
        damagePopupPrefabTextMeshPro.text = onActionResolvedEventArgs.String;
        damagePopupPrefabTextMeshPro.color = onActionResolvedEventArgs.Color;

        if (onActionResolvedEventArgs.GameObject == GameManager.Instance.Player)
        {
            damagePopupPrefab.transform.GetChild(0).transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            if (offsetCount == 0)
            {
                damagePopupPrefab.transform.GetChild(0).transform.localPosition = new Vector3(0, 2.275f, 0);
            }
            if (offsetCount == 1)
            {
                damagePopupPrefab.transform.localPosition = Vector3.zero;
                damagePopupPrefab.transform.GetChild(0).transform.localPosition = new Vector3(0, 2.65f, 0);
            }
        }

        Destroy(damagePopupPrefab, 1f);
    }


    private void CharacterBattle_OnTurnEnded(object sender, CharacterBattle characterBattle)
    {
        Image actionBarImage = _actionPointBarDictionary[characterBattle.gameObject].transform.GetChild(1).GetComponent<Image>();
        StartCoroutine(WaitForBattleManagerIsActiveThenImageWhite(actionBarImage));
    }

    private void CharacterBattle_OnTurnReached(object sender, CharacterBattle characterBattle)
    {
        Image actionBarImage = _actionPointBarDictionary[characterBattle.gameObject].transform.GetChild(1).GetComponent<Image>();
        actionBarImage.fillAmount = 1f;
        actionBarImage.color = Color.yellow;
    }

    private void SetPlayerActionBarWhite()
    {
        _actionPointBarDictionary[GameManager.Instance.Player].transform.GetChild(1).GetComponent<Image>().color = Color.white;
    }

    private IEnumerator WaitForBattleManagerIsActiveThenImageWhite(Image image)
    {
        yield return new WaitUntil(() => BattleManager.Instance.IsActive == true);
        image.color = Color.white;
    }

    private void UpdateStats(Stats stats)
    {
        _maxHealthValueText.text = stats.MaxHealth.ToString("0.##");
        _maxManaValueText.text = stats.MaxMana.ToString("0.##");
        _damageValueText.text = stats.Damage.ToString("0.##");
        _speedValueText.text = stats.Speed.ToString("0.##");
        _cooldownReductionValueText.text = stats.CooldownReduction.ToString("0.##");
        _armorValueText.text = stats.Armor.ToString("0.##");
        _evasionValueText.text = stats.Evasion.ToString("0.##");
        _leechValueText.text = stats.Leech.ToString("0.##");
        _areaValueText.text = stats.Area.ToString("0.##");
    }

    private void SetUpgradeInSkillTreePopup(object sender, string s)
    {
        AbilityUpgradeSO abilityUpgradeSO = _abilityUpgrade.GetAbilityUpgradeSOFromString(s);
        string abilityName = abilityUpgradeSO.AbilityName;
        string upgradeName = abilityUpgradeSO.UpgradeName;
        string description = abilityUpgradeSO.Description;

        switch (abilityName)
        {
            case "Shadow Bolt":
                //+1 because ability name text is in first slot
                GameObject shadowBoltAbilityUpgrade = _skillTreePopup.transform.GetChild(0).GetChild(0).GetChild(_numberOfShadowBoltUpgradesEnabled + 1).gameObject;
                shadowBoltAbilityUpgrade.SetActive(true);
                shadowBoltAbilityUpgrade.transform.GetChild(0).GetComponent<TMP_Text>().text = abilityName;
                shadowBoltAbilityUpgrade.transform.GetChild(1).GetComponent<TMP_Text>().text = upgradeName;
                shadowBoltAbilityUpgrade.transform.GetChild(2).GetComponent<TMP_Text>().text = description;
                _numberOfShadowBoltUpgradesEnabled++;
                break;
            case "Chaos Bolt":
                GameObject chaosBoltAbilityUpgrade = _skillTreePopup.transform.GetChild(0).GetChild(1).GetChild(_numberOfChaosBoltUpgradesEnabled + 1).gameObject;
                chaosBoltAbilityUpgrade.SetActive(true);
                chaosBoltAbilityUpgrade.transform.GetChild(0).GetComponent<TMP_Text>().text = abilityName;
                chaosBoltAbilityUpgrade.transform.GetChild(1).GetComponent<TMP_Text>().text = upgradeName;
                chaosBoltAbilityUpgrade.transform.GetChild(2).GetComponent<TMP_Text>().text = description;
                _numberOfChaosBoltUpgradesEnabled++;
                break;
            case "Rain of Torment":
                GameObject rainOfTormentAbilityUpgrade = _skillTreePopup.transform.GetChild(0).GetChild(2).GetChild(_numberOfRainOfTormentUpgradesEnabled + 1).gameObject;
                rainOfTormentAbilityUpgrade.SetActive(true);
                rainOfTormentAbilityUpgrade.transform.GetChild(0).GetComponent<TMP_Text>().text = abilityName;
                rainOfTormentAbilityUpgrade.transform.GetChild(1).GetComponent<TMP_Text>().text = upgradeName;
                rainOfTormentAbilityUpgrade.transform.GetChild(2).GetComponent<TMP_Text>().text = description;
                _numberOfRainOfTormentUpgradesEnabled++;
                break;
        }
    }

    private void SetPactInPactPopup(object sender, Pact pact)
    {
        GameObject pactUI = _pactsPopup.transform.GetChild(0).GetChild(0).GetChild(_numberOfPactsEnabled).gameObject;
        pactUI.SetActive(true);
        pactUI.transform.GetChild(0).GetComponent<TMP_Text>().text = pact.GetName();
        pactUI.transform.GetChild(1).GetComponent<TMP_Text>().text = pact.GetDescription();
        _numberOfPactsEnabled++;
    }
    public void ResetAbilityUpgrades()
    {
        _numberOfShadowBoltUpgradesEnabled = 0;
        _numberOfChaosBoltUpgradesEnabled = 0;
        _numberOfRainOfTormentUpgradesEnabled = 0;

        //start at index 1 for each child in each ability layout group and set the gameObject to inactive
        for (int i = 1; i < _skillTreePopup.transform.GetChild(0).GetChild(0).childCount; i++)
        {
            Destroy(_skillTreePopup.transform.GetChild(0).GetChild(0).GetChild(i).gameObject);
        }
        for (int i = 1; i < _skillTreePopup.transform.GetChild(0).GetChild(1).childCount; i++)
        {
            Destroy(_skillTreePopup.transform.GetChild(0).GetChild(1).GetChild(i).gameObject);
        }
        for (int i = 1; i < _skillTreePopup.transform.GetChild(0).GetChild(2).childCount; i++)
        {
            Destroy(_skillTreePopup.transform.GetChild(0).GetChild(2).GetChild(i).gameObject);
        }

        for (int i = 0; i < 4; i++)
        {
            GameObject blankAbilityUpgrade = Instantiate(_abilityUpgradePrefab, _skillTreePopup.transform.GetChild(0).GetChild(0));
            Destroy(blankAbilityUpgrade.GetComponent<Button>());
            blankAbilityUpgrade.SetActive(false);
        }
        for (int i = 0; i < 4; i++)
        {
            GameObject blankAbilityUpgrade = Instantiate(_abilityUpgradePrefab, _skillTreePopup.transform.GetChild(0).GetChild(1));
            Destroy(blankAbilityUpgrade.GetComponent<Button>());
            blankAbilityUpgrade.SetActive(false);
        }
        for (int i = 0; i < 4; i++)
        {
            GameObject blankAbilityUpgrade = Instantiate(_abilityUpgradePrefab, _skillTreePopup.transform.GetChild(0).GetChild(2));
            Destroy(blankAbilityUpgrade.GetComponent<Button>());
            blankAbilityUpgrade.SetActive(false);
        }
    }

    public void ResetPacts()
    {
        _numberOfPactsEnabled = 0;

        foreach (Transform child in _pactsPopup.transform.GetChild(0).GetChild(0))
        {
            child.gameObject.SetActive(false);
        }
    }


    private void UpdateCircleText()
    {
        _circleEncounterUI.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = 
            "Circle: " + GameManager.Instance.CurrentCircleNumber.ToString() + "/" + GameManager.Instance.MAX_CIRCLE_NUMBER.ToString();
    }

    private void UpdateEncounterText()
    {
        _circleEncounterUI.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text =
            "Encounter: " + GameManager.Instance.CurrentEncounterNumber.ToString() + "/" + GameManager.Instance.MAX_ENCOUNTER_NUMBER.ToString();
    }

    private void UpdateLevelText(string s)
    {
        _levelExperienceGoldUI.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = s + "/99";
    }

    private void UpdateExperienceText(string s)
    {
        _levelExperienceGoldUI.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<TMP_Text>().text = s + "/" + _playerStats.ExperienceToLevel;
    }

    private void UpdateGoldText(string s)
    {
        _levelExperienceGoldUI.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<TMP_Text>().text = s;
    }

    private void Show(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }

    private void Hide(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    private void Toggle(GameObject gameObject)
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    private void DestroyGameObject(GameObject gameObject)
    {
        Destroy(gameObject);
    }

    public void PauseGame()
    {
        BattleManager.Instance.SetIsActive(false);
        Show(_pauseGamePopup);
    }

    public void UnpauseGame()
    {
        BattleManager.Instance.SetIsActive(true);
        Hide(_pauseGamePopup);
    }

    public void HidePauseGamePopup()
    {
        Hide(_pauseGamePopup);
    }

    private void OnDestroy()
    {
        GearGenerator.OnGearGenerated -= GearGenerator_OnGearGenerated;
        /*_abilityUpgrade.OnAbilityUpgradeSet -= SetUpgradeInSkillTreePopup;
        _metaInventory.OnGoldChanged -= MetaInventory_OnGoldChanged;
        _playerStats.OnLevelChanged -= PlayerStats_OnLevelChanged;
        _playerStats.OnExperienceChanged -= PlayerStats_OnExperienceChanged;
        _playerStats.OnActionResolved -= Stats_OnActionResolved;
        _playerStats.OnStatsChanged -= PlayerStats_OnStatsChanged;
        CharacterBattle playerCharacterBattle = GameManager.Instance.Player.GetComponent<CharacterBattle>();
        playerCharacterBattle.OnTurnEnded -= CharacterBattle_OnTurnEnded;
        playerCharacterBattle.OnTurnReached -= CharacterBattle_OnTurnReached;
        playerCharacterBattle.OnActionPointsUpdated -= CharacterBattle_OnActionPointsUpdated;*/
    }
}
