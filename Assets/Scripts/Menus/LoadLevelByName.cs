using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelByName : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.LoadScene(AllValues.MenuSceneName);
    }
}
