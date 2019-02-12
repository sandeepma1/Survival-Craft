using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Bronz.Ui
{
    public class UiIgmMenuCanvas : MonoBehaviour
    {
        [SerializeField] private Button backToGameButton;
        [SerializeField] private Button backToMenuButton;
        [SerializeField] private Button quitGameButton;
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private GameObject igmMenuPanel;

        private void Start()
        {
            backToGameButton.onClick.AddListener(HideIgmMenu);
            backToMenuButton.onClick.AddListener(LoadMenuLevel);
            quitGameButton.onClick.AddListener(QuitGame);
            igmMenuPanel.SetActive(false);
        }

        private void Update()
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor)
            {
                if (Input.GetKey(KeyCode.Escape))
                {
                    if (SceneManager.GetActiveScene().name == "Menu")
                    {
                        Application.Quit();
                    }
                    else
                    {
                        ShowIgmMenu();
                    }
                }
            }
        }

        private void ShowIgmMenu()
        {
            igmMenuPanel.SetActive(true);
        }

        private void HideIgmMenu()
        {
            igmMenuPanel.SetActive(false);
        }

        private void LoadMainLevel()
        {
            loadingScreen.SetActive(true);
            SceneManager.LoadScene("Main");
        }

        private void LoadMenuLevelDeleteSaves()
        {
            loadingScreen.SetActive(true);
            ES2.DeleteDefaultFolder();
            SceneManager.LoadScene("Menu");
        }

        private void LoadMenuLevel()
        {
            loadingScreen.SetActive(true);
            SceneManager.LoadScene("Menu");
        }

        private void QuitGame()
        {
            Application.Quit();
        }
    }
}