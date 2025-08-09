// Assets/_Project/Scripts/UI/UIModalEndLevel.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIModalEndLevel : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Button btnTryAgain;
    [SerializeField] private Button btnHome;
    [SerializeField] private Button btnNext;
    [SerializeField] private UIChestModal chestModal;

    private LevelManager lm;

    private void Start()
    {
        if (!lm) lm = Object.FindFirstObjectByType<LevelManager>(FindObjectsInactive.Include);
        if (btnTryAgain) { btnTryAgain.onClick.RemoveAllListeners(); btnTryAgain.onClick.AddListener(() => lm.TryAgain()); }
        if (btnHome)    { btnHome.onClick.RemoveAllListeners();    btnHome.onClick.AddListener(() => lm.GoHome());    }
        if (btnNext)    { btnNext.onClick.RemoveAllListeners();    btnNext.onClick.AddListener(() => lm.NextLevel()); }

        if (!chestModal) chestModal = Object.FindFirstObjectByType<UIChestModal>(FindObjectsInactive.Include);
        if (chestModal) chestModal.BindOwner(this);
    }

    public void ShowWin()
    {
        gameObject.SetActive(true);
        if (titleText) titleText.text = "Level Complete!";
        if (btnTryAgain) btnTryAgain.gameObject.SetActive(false);
        if (btnHome) btnHome.gameObject.SetActive(true);
        if (btnNext) { btnNext.gameObject.SetActive(true); btnNext.interactable = false; }

        bool canOpen = ChestService.I != null && ChestService.I.CanOpenToday();
        if (canOpen && chestModal)
        {
            chestModal.gameObject.SetActive(true);
            chestModal.ShowReady();
        }
        else
        {
            if (chestModal) chestModal.Hide();
            if (btnNext) btnNext.interactable = true;
        }
    }


    public void ShowLose(bool canTryAgain)
    {
        gameObject.SetActive(true);
        if (titleText) titleText.text = "Out of moves";
        if (btnTryAgain) { btnTryAgain.gameObject.SetActive(true); btnTryAgain.interactable = canTryAgain; }
        if (btnNext) btnNext.gameObject.SetActive(false);
        if (btnHome) btnHome.gameObject.SetActive(true);
        if (chestModal) chestModal.Hide();
    }

    public void EnableNext()
    {
        if (btnNext) btnNext.interactable = true;
    }
}
