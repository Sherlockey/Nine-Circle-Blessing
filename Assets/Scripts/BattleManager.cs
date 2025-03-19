using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleManager : MonoBehaviour
{
    public event EventHandler<GameObject> OnStatsDead;
    public event EventHandler OnCharactersSpawned;
    public static event EventHandler OnBattleStarted;
    public bool IsActive { get; private set; } = true;
    public bool IsSearchingForTarget { get; private set; } = false;
    private bool _isBattleOver = false;

    [SerializeField] private GameObject[] _characterBattlePrefabs;

    public GameObject Player { get; private set; }
    public List<GameObject> TargetList { get; private set; } = new List<GameObject>();
    public List<GameObject> SelectedChevrons { get; private set; } = new List<GameObject>();
    public List<GameObject> EnemyList { get; private set; } = new List<GameObject>();

    private float _enemyGlobalOffsetX = -0.75f;
    private float _enemyFirstSpawnOffsetX = -0.75f;
    private float _enemySpawnSpacingX = 1.5f;

    private State _state;

    private enum State
    {
        WaitingForPlayer,
        Busy,
        NotBusy,
    }

    public static BattleManager Instance;

    private void Awake()
    {
        Instance = this;

        SpawnCharacters();
        SetCharacterBattlesActionPoints();

        _state = State.NotBusy;
        IsActive = true;
        _isBattleOver = false;
    }

    private void Start()
    {
        BattleUIManager.Instance.OnFullScreenPopupOpened += BattleUIManager_OnFullScreenPopupOpened;
        BattleUIManager.Instance.OnFullScreenPopupClosed += BattleUIManager_OnFullScreenPopupClosed;

        if (GameManager.Instance.CurrentCircleNumber == 1 && GameManager.Instance.CurrentEncounterNumber == 1)
        {
            AudioManager.Instance.PlayNewMusic();
        }

        OnBattleStarted?.Invoke(this, EventArgs.Empty);
    }

    private void BattleUIManager_OnFullScreenPopupOpened(object sender, EventArgs e)
    {
        _state = State.WaitingForPlayer;
    }
    private void BattleUIManager_OnFullScreenPopupClosed(object sender, EventArgs e)
    {
        _state = State.NotBusy;
    }


    private void Update()
    {
        if (IsSearchingForTarget)
        {
            CheckForTarget();
        }

        if (_state == State.NotBusy)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (IsActive)
                {
                    BattleUIManager.Instance.PauseGame();
                }
                else if (!IsActive)
                {
                    BattleUIManager.Instance.UnpauseGame();
                }
            }
        }

        //TODO remove this soon when making actions
        if (_state == State.WaitingForPlayer)
        {
            
            if (Input.GetMouseButtonDown(0) && TargetList.Count != 0 && Player.GetComponent<CharacterBattle>().SelectedAction != null)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                IAction action = Player.GetComponent<CharacterBattle>().SelectedAction;
                action.PayCost();

                //could do a foreach here so that it does the selected action to each target in a list. single target actions would still only do the action to one target this way
                foreach (GameObject target in TargetList)
                {
                    action.Execute(Player.GetComponent<CharacterBattle>(), target.GetComponent<CharacterBattle>());
                }
                //Player.GetComponent<CharacterBattle>().Attack(Target.GetComponent<CharacterBattle>());
                TargetList.Clear();
                BattleUIManager.Instance.DestroySelectedChevrons();
                Player.GetComponent<CharacterBattle>().SetSelectedAction(null);
                IsSearchingForTarget = false;
            }

            if (Player.GetComponent<CharacterBattle>().SelectedAction != null)
            {
                if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
                {
                    Player.GetComponent<CharacterBattle>().SetSelectedAction(null);
                    TargetList.Clear();
                    BattleUIManager.Instance.DestroySelectedChevrons();
                    IsSearchingForTarget = false;
                }
            }
        }
    }

    //this calculation might want to be redone to add more considerations(like how many total combatants there are) and potentially randomization, but it works for now
    private void SetCharacterBattlesActionPoints()
    {
        float totalSpeed = 0f;

        float playerSpeed = Player.GetComponent<Stats>().Speed;
        totalSpeed += playerSpeed;

        foreach (GameObject enemy in EnemyList)
        {
            totalSpeed += enemy.GetComponent<Stats>().Speed;
        }

        //set each character battle's action points equal to their % of the total speed * 100 (the value required to get a turn)
        Player.GetComponent<CharacterBattle>().SetActionPoints(playerSpeed / totalSpeed * 100);

        foreach (GameObject enemy in EnemyList)
        {
            enemy.GetComponent<CharacterBattle>().SetActionPoints(enemy.GetComponent<Stats>().Speed / totalSpeed * 100);
        }
    }

    private void CheckForTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, 1 << 7))
        {
            if (raycastHit.transform.GetComponent<CharacterBattle>() != null)
            {
                if (!TargetList.Contains(raycastHit.transform.gameObject))
                {
                    TargetList.Clear();
                    BattleUIManager.Instance.DestroySelectedChevrons();
                    TargetList.Add(raycastHit.transform.gameObject);
                    BattleUIManager.Instance.CreateSelectedChevron(raycastHit.transform);
                }
            }
        }
    }

    private void SpawnCharacters()
    {
        //Spawn the player
        Player = GameManager.Instance.Player;

        CharacterBattle playerCharacterBattle = Player.GetComponent<CharacterBattle>();
        playerCharacterBattle.OnTurnReached += CharacterBattle_OnTurnReached;
        playerCharacterBattle.OnTurnEnded += CharacterBattle_OnTurnEnded;

        Stats playerStats = Player.GetComponent<Stats>();
        playerStats.OnDead += Stats_OnDead;

        IAction[] playerActions;
        playerActions = Player.GetComponents<IAction>();

        foreach (IAction action in playerActions)
        {
            action.OnOnClick += Action_OnOnClick;
        }

        //Spawn all enemies
        //TODO increase the likelyhood of spawning higher numbers of enemies as the circle and/or encounter number increase
        int numberToSpawn = UnityEngine.Random.Range(1, 6);

        for (int i = 0; i < numberToSpawn; i++)
        {
            GameObject enemy = Instantiate(_characterBattlePrefabs[UnityEngine.Random.Range(1, 3)],
                new Vector3(_enemyGlobalOffsetX + _enemyFirstSpawnOffsetX * (numberToSpawn - 1) + _enemySpawnSpacingX * (i), 0f, -8f),
                Quaternion.identity);
            enemy.transform.LookAt(Player.transform);

            EnemyList.Add(enemy);
            CharacterBattle enemyCharacterBattle = enemy.GetComponent<CharacterBattle>();
            enemyCharacterBattle.OnTurnReached += CharacterBattle_OnTurnReached;
            enemyCharacterBattle.OnTurnEnded += CharacterBattle_OnTurnEnded;

            Stats enemyStats = enemy.GetComponent<Stats>();
            enemyStats.OnDead += Stats_OnDead;
        }

        OnCharactersSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void Action_OnOnClick(object sender, bool doesSearchForTarget)
    {
        if (doesSearchForTarget == true)
        {
            TargetList.Clear();
            BattleUIManager.Instance.DestroySelectedChevrons();
            IsSearchingForTarget = true;
        }
        else if (sender.GetType() == typeof(PotionAction))
        {
            TargetList.Clear();
            BattleUIManager.Instance.DestroySelectedChevrons();
            TargetList.Add(Player);
            BattleUIManager.Instance.CreateSelectedChevron(Player.transform);
        }
        else
        {
            TargetList.Clear();
            BattleUIManager.Instance.DestroySelectedChevrons();
            foreach (GameObject enemy in Instance.EnemyList)
            {
                TargetList.Add(enemy);
                BattleUIManager.Instance.CreateSelectedChevron(enemy.transform);
            }
        }
    }

    private void Stats_OnDead(object sender, GameObject gameObject)
    {
        bool isPlayerDead = false;

        if (EnemyList.Contains(gameObject))
        {
            EnemyList.Remove(gameObject);
        }

        if (gameObject == Player)
        {
            //change this
            isPlayerDead = true;
        }
        //free the damagePopupClones from the gameObject parent if it is dead so the text popup doesnt instantly disappear
        int childCount = gameObject.transform.childCount;
        List<GameObject> toBeOrphanedList = new List<GameObject>();
        for (int i = 0; i < childCount; i++)
        {
            if (gameObject.transform.GetChild(i).name == "DamagePopup(Clone)")
            {
                toBeOrphanedList.Add(gameObject.transform.GetChild(i).gameObject);
            }
        }

        foreach (GameObject go in toBeOrphanedList)
        {
            go.transform.SetParent(null);
        }

        if (gameObject.tag == "Player")
        {
            gameObject.GetComponent<Renderer>().enabled = false;
            foreach(Transform child in gameObject.transform)
            {
                if (child.TryGetComponent<Renderer>(out Renderer renderer))
                {
                    renderer.enabled = false;
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }

        if (BattleWonCheck())
        {
            if (this != null)
            {
                StartCoroutine(WaitThenGoToNextBattle(1.25f));
            }
        }

        if (isPlayerDead)
        {
            if (this != null)
            {
                StartCoroutine(WaitThenGoToMainMenu(1.25f));
            }
        }
        else
        {
            OnStatsDead?.Invoke(this, gameObject);
        }
    }

    private void CharacterBattle_OnTurnReached(object sender, CharacterBattle characterBattle)
    {
        IsActive = false;

        if (characterBattle.gameObject == Player)
        {
            _state = State.WaitingForPlayer;
            BattleUIManager.Instance.HidePauseGamePopup();
        }
        else
        {
            //TODO: eventually make the enemies do their own actions (attacks) which are defined in classes which carry possible status effects and other information along
            characterBattle.Attack(characterBattle.GetComponent<Stats>(), Player.GetComponent<CharacterBattle>(), 1f, false);
        }
    }

    private void CharacterBattle_OnTurnEnded(object sender, CharacterBattle characterBattle)
    {
        _state = State.NotBusy;
        //DEBUG added temporarily to make it so enemies do not attack you back to back too quickly 
        //this null check is a temporary fix. something is actually going wrong with the static class and coroutines? not entirely sure
        if (this != null)
        {
            StartCoroutine(Timer(1.25f));
        }
        //IsActive = true;
        //_state = State.NotBusy;
    }

    private bool BattleWonCheck()
    {
        return (EnemyList.Count < 1);
    }

    private IEnumerator Timer(float duration)
    {
        IsActive = false;
        yield return new WaitForSeconds(duration);
        if (_isBattleOver == false)
        {
            IsActive = true;
        }
    }

    private IEnumerator WaitThenGoToNextBattle(float duration)
    {
        IsActive = false;
        _isBattleOver = true;
        yield return new WaitForSeconds(duration);
        GameManager.Instance.NextScene();
    }

    private IEnumerator WaitThenGoToMainMenu(float duration)
    {
        IsActive = false;
        _isBattleOver = true;
        yield return new WaitForSeconds(duration);
        GameManager.Instance.GameOver();
    }

    public void SetIsActive(bool value)
    {
        IsActive = value;
    }
}
