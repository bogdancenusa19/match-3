// Assets/_Project/Scripts/Economy/ChestService.cs
using System;
using System.Collections.Generic;
using UnityEngine;

public class ChestService : MonoBehaviour
{
    public static ChestService I { get; private set; }

    [SerializeField] private LootTable lootTable;
    [SerializeField] private int dailyLimit = 3;

    private SaveData data;
    private string today => DateTime.Now.ToString("yyyyMMdd");

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        data = SaveSystem.Load();
        if (data == null) data = new SaveData();
        EnsureDay();
        SaveSystem.Save(data);
    }

    private void EnsureDay()
    {
        if (data.chestState == null) data.chestState = new DailyChestState();
        if (data.chestState.yyyymmdd != today)
        {
            data.chestState.yyyymmdd = today;
            data.chestState.pityCounter = 0;
            data.chestState.openedToday = 0;
        }
    }

    public bool CanOpenToday()
    {
        EnsureDay();
        return data.chestState.openedToday < dailyLimit;
    }

    public int RemainingToday()
    {
        EnsureDay();
        return Mathf.Max(0, dailyLimit - data.chestState.openedToday);
    }

    public List<Reward> OpenChest(int seedExtra = 0)
    {
        EnsureDay();
        var results = new List<Reward>();
        if (!CanOpenToday()) return results;
        if (lootTable == null || lootTable.entries.Count == 0) return results;

        int seed = GetBaseSeed() + seedExtra;
        var rnd = new System.Random(seed);

        for (int i = 0; i < Mathf.Max(1, lootTable.rollsPerChest); i++)
        {
            var pick = PickEntryWithPity(rnd);
            int min = Mathf.Min(pick.amountRange.x, pick.amountRange.y);
            int max = Mathf.Max(pick.amountRange.x, pick.amountRange.y) + 1;
            int amt = rnd.Next(min, max);
            var r = new Reward { kind = pick.kind, amount = Mathf.Max(1, amt) };
            results.Add(r);
            InventoryService.I?.Add(r);
        }

        data.chestState.openedToday++;
        SaveSystem.Save(data);

        var parts = results.ConvertAll(x => $"{x.kind} x{x.amount}");
        Debug.Log($"[CHEST] {string.Join(", ", parts)} | Pity={data.chestState.pityCounter} | Left={RemainingToday()}");

        return results;
    }

    public void GrantDouble(List<Reward> rewards)
    {
        if (rewards == null) return;
        foreach (var r in rewards) InventoryService.I?.Add(r);
        SaveSystem.Save(data);
        Debug.Log("[CHEST] Claim x2");
    }

    private LootEntry PickEntryWithPity(System.Random rnd)
    {
        int total = 0;
        for (int i = 0; i < lootTable.entries.Count; i++) total += Mathf.Max(0, lootTable.entries[i].weight);

        int roll = rnd.Next(0, Mathf.Max(1, total));
        int acc = 0;
        for (int i = 0; i < lootTable.entries.Count; i++)
        {
            acc += Mathf.Max(0, lootTable.entries[i].weight);
            if (roll < acc)
            {
                if (lootTable.entries[i].pityThreshold > 0 && data.chestState.pityCounter >= lootTable.entries[i].pityThreshold)
                {
                    data.chestState.pityCounter = 0;
                    return lootTable.entries[i];
                }
                else
                {
                    data.chestState.pityCounter++;
                    return lootTable.entries[i];
                }
            }
        }
        data.chestState.pityCounter++;
        return lootTable.entries[lootTable.entries.Count - 1];
    }

    private int GetBaseSeed()
    {
        int s = DateTime.Now.Year * 10000 + DateTime.Now.Month * 100 + DateTime.Now.Day;
        if (DailyRunService.I != null) s += DailyRunService.I.Stage * 733;
        return s;
    }
}
