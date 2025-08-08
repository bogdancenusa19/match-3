using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private string nextScene = "MainMenu";
    private void Start()
    {
        if (string.IsNullOrEmpty(nextScene)) return;
        SceneManager.LoadScene(nextScene);
    }
}
