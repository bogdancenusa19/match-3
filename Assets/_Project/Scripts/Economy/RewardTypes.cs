using System;
using UnityEngine;

public enum RewardKind { Coins, Gems, BoosterHammer, BoosterShuffle, BoosterColorBomb }

[Serializable]
public struct Reward
{
    public RewardKind kind;
    public int amount;
}

[Serializable]
public struct LootEntry
{
    public RewardKind kind;
    public Vector2Int amountRange;
    public int weight;
    public int pityThreshold;
}
