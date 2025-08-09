// Assets/_Project/Scripts/UI/UIChestPanel.cs
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIChestPanel : MonoBehaviour
{
    [SerializeField] TMP_Text textSummary;

    public void Show(List<Reward> rewards)
    {
        if (rewards == null || rewards.Count == 0) { gameObject.SetActive(false); return; }
        var parts = new System.Text.StringBuilder();
        for (int i = 0; i < rewards.Count; i++)
        {
            parts.Append(rewards[i].kind).Append(" x").Append(rewards[i].amount);
            if (i < rewards.Count - 1) parts.Append("\n");
        }
        textSummary.text = parts.ToString();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
