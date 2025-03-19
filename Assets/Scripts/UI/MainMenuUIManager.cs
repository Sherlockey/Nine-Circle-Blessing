using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] private GameObject _canvas;
    [SerializeField] private GameObject _shop;
    [SerializeField] private GameObject _items;
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _itemsButton;
    [SerializeField] private Button _quitGameButton;

    private void Awake()
    {
        _startGameButton.onClick.AddListener(() => {
            SceneManager.LoadScene(1);
        });
        _shopButton.onClick.AddListener(() => {
            Show(_shop);
        });
        _itemsButton.onClick.AddListener(() => {
            Show(_items);
        });
        _quitGameButton.onClick.AddListener(() => {
            if (GameManager.Instance != null) GameManager.Instance.QuitGame();
        });
    }

    private void Show(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }
}
