// Assets/_Project/Scripts/Core/SaveModel.cs
using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int lives = 5;
    public long nextLifeUnix = 0;
    public List<LevelEntry> levels = new List<LevelEntry>();
    public Wallet wallet = new Wallet();
    public Inventory inv = new Inventory();
    public DailyChestState chestState = new DailyChestState();
}

[Serializable]
public class LevelEntry
{
    public int levelID;
    public int bestScore;
    public int stars;
}

[Serializable]
public class Wallet
{
    public int coins;
    public int gems;
}

[Serializable]
public class Inventory
{
    public int boosterHammer;
    public int boosterShuffle;
    public int boosterColorBomb;
}

[Serializable]
public class DailyChestState
{
    public string yyyymmdd;
    public int pityCounter;
    public int openedToday;
}
