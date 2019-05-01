using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Bronz.Ui
{
    public class UiMainMenuCanvas : MonoBehaviour
    {
        public static Action OnNewGameButtonClick;
        public static Action OnLoadMainScene;
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueGameButton;
        [SerializeField] private Button quitGameButton;
        [SerializeField] private GameObject loadingScreen;

        private void Awake()
        {
            string[] ass = ES2.GetFiles("");
            if (ass.Length >= 2)
            {
                continueGameButton.interactable = true;
            }
            else
            {
                continueGameButton.interactable = false;
            }
        }

        private void Start()
        {
            OnLoadMainScene += LoadMainScene;
            newGameButton.onClick.AddListener(() => OnNewGameButtonClick?.Invoke());
            continueGameButton.onClick.AddListener(LoadMainScene);
            quitGameButton.onClick.AddListener(QuitGame);
        }

        private void LoadMainScene()
        {
            loadingScreen.SetActive(true);
            SceneManager.LoadScene(AllValues.MainSceneName);
        }

        private void QuitGame()
        {
            Application.Quit();
        }
    }
}