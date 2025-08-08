// Assets/_Project/Scripts/Core/ProgressService.cs
using UnityEngine;
using System;

public class ProgressService : MonoBehaviour
{
    public static ProgressService I { get; private set; }
    private SaveData data;

    public event Action<int,int,int> LevelSaved;

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        data = SaveSystem.Load();
    }

    public (int best,int stars) GetLevel(int levelID)
    {
        var e = data.levels.Find(x => x.levelID == levelID);
        if (e == null) return (0,0);
        return (e.bestScore, e.stars);
    }

    public int ComputeStars(int score, LevelConfig cfg)
    {
        if (cfg == null) return 0;
        if (score >= cfg.score3Stars) return 3;
        if (score >= cfg.score2Stars) return 2;
        if (score >= cfg.targetScore) return 1;
        return 0;
    }

    public void SaveLevelResult(int levelID, int score, LevelConfig cfg)
    {
        int stars = ComputeStars(score, cfg);
        var e = data.levels.Find(x => x.levelID == levelID);
        if (e == null)
        {
            e = new LevelEntry { levelID = levelID, bestScore = score, stars = stars };
            data.levels.Add(e);
        }
        else
        {
            if (score > e.bestScore) e.bestScore = score;
            if (stars > e.stars) e.stars = stars;
        }
        SaveSystem.Save(data);

        Debug.Log($"[SAVE] Level {levelID} -> BestScore: {e.bestScore}, Stars: {e.stars}");
        Debug.Log(JsonUtility.ToJson(data, true));

        LevelSaved?.Invoke(levelID, e.bestScore, e.stars);
    }

}
