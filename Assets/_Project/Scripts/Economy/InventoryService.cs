// Assets/_Project/Scripts/Economy/InventoryService.cs
using System;
using UnityEngine;

public class InventoryService : MonoBehaviour
{
    public static InventoryService I { get; private set; }
    private SaveData data;

    public event Action WalletChanged;
    public event Action InventoryChanged;

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        data = SaveSystem.Load();
        if (data == null) data = new SaveData();
        SaveSystem.Save(data);
    }

    public int Coins => data.wallet.coins;
    public int Gems => data.wallet.gems;
    public int Hammer => data.inv.boosterHammer;
    public int Shuffle => data.inv.boosterShuffle;
    public int ColorBomb => data.inv.boosterColorBomb;

    public void Add(Reward r)
    {
        switch (r.kind)
        {
            case RewardKind.Coins: data.wallet.coins += r.amount; WalletChanged?.Invoke(); break;
            case RewardKind.Gems: data.wallet.gems += r.amount; WalletChanged?.Invoke(); break;
            case RewardKind.BoosterHammer: data.inv.boosterHammer += r.amount; InventoryChanged?.Invoke(); break;
            case RewardKind.BoosterShuffle: data.inv.boosterShuffle += r.amount; InventoryChanged?.Invoke(); break;
            case RewardKind.BoosterColorBomb: data.inv.boosterColorBomb += r.amount; InventoryChanged?.Invoke(); break;
        }
        SaveSystem.Save(data);
        Debug.Log($"[INV+] {r.kind} x{r.amount} | Coins={data.wallet.coins} Gems={data.wallet.gems} H={data.inv.boosterHammer} S={data.inv.boosterShuffle} CB={data.inv.boosterColorBomb}");
    }


    public void SpendCoins(int amount)
    {
        if (amount <= 0) return;
        if (data.wallet.coins < amount) return;
        data.wallet.coins -= amount;
        SaveSystem.Save(data);
        WalletChanged?.Invoke();
    }

    public void SpendGems(int amount)
    {
        if (amount <= 0) return;
        if (data.wallet.gems < amount) return;
        data.wallet.gems -= amount;
        SaveSystem.Save(data);
        WalletChanged?.Invoke();
    }

    public bool ConsumeBooster(RewardKind kind, int count = 1)
    {
        if (count <= 0) return false;
        switch (kind)
        {
            case RewardKind.BoosterHammer:
                if (data.inv.boosterHammer < count) return false;
                data.inv.boosterHammer -= count;
                SaveSystem.Save(data);
                InventoryChanged?.Invoke();
                return true;
            case RewardKind.BoosterShuffle:
                if (data.inv.boosterShuffle < count) return false;
                data.inv.boosterShuffle -= count;
                SaveSystem.Save(data);
                InventoryChanged?.Invoke();
                return true;
            case RewardKind.BoosterColorBomb:
                if (data.inv.boosterColorBomb < count) return false;
                data.inv.boosterColorBomb -= count;
                SaveSystem.Save(data);
                InventoryChanged?.Invoke();
                return true;
        }
        return false;
    }
}
