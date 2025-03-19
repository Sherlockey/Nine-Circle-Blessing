using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public event EventHandler OnGameOver;
    public event EventHandler SceneAboutToBeChanged;
    public int CurrentCircleNumber { get; private set; } = 1;
    public readonly int MAX_CIRCLE_NUMBER = 9;
    public int CurrentEncounterNumber { get; private set; } = 1;
    public readonly int MAX_ENCOUNTER_NUMBER = 9;

    public GameObject Player;

    [SerializeField] private GameObject _velfirithPrefab;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        Player = Instantiate(_velfirithPrefab, new Vector3(-0.75f, 0, -3f), Quaternion.Euler(new Vector3(0, 180, 0)));
        DontDestroyOnLoad(Player);
    }

    private void ResetGameState()
    {
        //reset everything that needs to be for the next run to begin

        //sell items
        Player.GetComponent<GearInventory>().SellAllGear();
        //BattleUI reset
        BattleUIManager.Instance.ResetAbilityUpgrades();
        BattleUIManager.Instance.ResetPacts();
        ResetPlayer();
    }

    private void ResetPlayer()
    {
        //pacts
        Player.GetComponent<PactInventory>().RevertAllPacts();
        //abilityUpgrades
        Player.GetComponent<AbilityUpgrade>().RevertAllAbilityUpgrades();
        //PlayerStats
        Player.GetComponent<PlayerStats>().RevertAllPlayerStats();

        //unhide player
        Player.GetComponent<Renderer>().enabled = true;
        foreach (Transform child in Player.transform)
        {
            if (child.TryGetComponent<Renderer>(out Renderer renderer))
            {
                renderer.enabled = true;
            }
        }
    }

    public void GameOver()
    {
        CurrentEncounterNumber = 1;
        CurrentCircleNumber = 1;
        ResetGameState();
        SceneManager.LoadSceneAsync(0); //0 is Main Menu Scene
        AudioManager.Instance.PlayMusic("the_heron");
        OnGameOver?.Invoke(this, EventArgs.Empty);
    }

    public void NextScene()
    {
        SceneAboutToBeChanged?.Invoke(this, EventArgs.Empty);

        CurrentEncounterNumber++;
        if (CurrentEncounterNumber > MAX_ENCOUNTER_NUMBER)
        {
            CurrentEncounterNumber = 1;
            CurrentCircleNumber++;
            if (CurrentCircleNumber > MAX_CIRCLE_NUMBER)
            {
                CurrentEncounterNumber = 1;
                CurrentCircleNumber = 1;
                SceneManager.LoadSceneAsync(0); //0 is Main Menu Scene
                AudioManager.Instance.PlayMusic("the_heron");
            }
            else
            {
                SceneManager.LoadSceneAsync(1); //1 is Battle Scene
            }
        }
        else
        {
            SceneManager.LoadSceneAsync(1); //1 is Battle Scene
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
