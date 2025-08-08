using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIModalEndLevel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button btnTryAgain;
    [SerializeField] private Button btnHome;
    [SerializeField] private Button btnNext;
    [SerializeField] private LevelManager levelManager;

    private void Awake()
    {
        if (!levelManager) levelManager = Object.FindFirstObjectByType<LevelManager>(FindObjectsInactive.Include);
        if (btnTryAgain) btnTryAgain.onClick.AddListener(levelManager.TryAgain);
        if (btnHome) btnHome.onClick.AddListener(levelManager.GoHome);
        if (btnNext) btnNext.onClick.AddListener(levelManager.NextLevel);
    }

    public void ShowWin()
    {
        gameObject.SetActive(true);
        if (titleText) titleText.text = "Level Complete!";
        if (btnTryAgain) btnTryAgain.gameObject.SetActive(false);
        if (btnNext) btnNext.gameObject.SetActive(true);
    }

    public void ShowLose(bool canTryAgain)
    {
        gameObject.SetActive(true);
        if (titleText) titleText.text = "Game Over";
        if (btnTryAgain)
        {
            btnTryAgain.gameObject.SetActive(true);
            btnTryAgain.interactable = canTryAgain;
        }
        if (btnNext) btnNext.gameObject.SetActive(false);
    }
}
