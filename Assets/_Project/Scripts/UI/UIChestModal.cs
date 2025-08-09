// Assets/_Project/Scripts/UI/UIChestModal.cs
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIChestModal : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text body;
    [SerializeField] private GameObject btnOpen;
    [SerializeField] private GameObject btnClaim;
    [SerializeField] private GameObject btnClaimX2;

    private List<Reward> lastRewards;
    private UIModalEndLevel owner;

    public void BindOwner(UIModalEndLevel modal) { owner = modal; }

    public void ShowReady()
    {
        if (root) root.SetActive(true);
        lastRewards = null;
        if (title) title.text = "Chest";
        if (body) body.text = ChestService.I != null ? $"Opens left today: {ChestService.I.RemainingToday()}" : "";
        if (btnOpen) btnOpen.SetActive(true);
        if (btnClaim) btnClaim.SetActive(false);
        if (btnClaimX2) btnClaimX2.SetActive(false);
    }

    public void Hide()
    {
        if (root) root.SetActive(false);
    }

    public void OnOpen()
    {
        if (ChestService.I == null) return;

        if (!ChestService.I.CanOpenToday())
        {
            Hide();
            owner?.EnableNext();
            return;
        }

        lastRewards = ChestService.I.OpenChest();
        if (lastRewards == null || lastRewards.Count == 0)
        {
            Hide();
            owner?.EnableNext();
            return;
        }

        var parts = lastRewards.ConvertAll(x => $"{x.kind} x{x.amount}");
        if (title) title.text = "You got";
        if (body) body.text = string.Join("\n", parts);
        if (btnOpen) btnOpen.SetActive(false);
        if (btnClaim) btnClaim.SetActive(true);
        if (btnClaimX2) btnClaimX2.SetActive(true);
    }


    public void OnClaim()
    {
        Hide();
        owner?.EnableNext();
    }

    public void OnClaimX2()
    {
        if (ChestService.I != null && lastRewards != null) ChestService.I.GrantDouble(lastRewards);
        Hide();
        owner?.EnableNext();
    }
}
