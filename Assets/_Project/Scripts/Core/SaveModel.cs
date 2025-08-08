// Assets/_Project/Scripts/Core/SaveModel.cs
using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int lives = 5;
    public long nextLifeUnix = 0;
    public List<LevelEntry> levels = new List<LevelEntry>();
}

[Serializable]
public class LevelEntry
{
    public int levelID;
    public int bestScore;
    public int stars;
}
