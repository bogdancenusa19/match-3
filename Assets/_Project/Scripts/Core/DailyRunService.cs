// Assets/_Project/Scripts/Core/DailyRunService.cs
using System;
using System.IO;
using UnityEngine;

[Serializable]
public class DailyRunData
{
    public string yyyymmdd;
    public int stage;
    public int seed;
    public int bestStageToday;
    public int target;
    public int moves;
}

public class DailyRunService : MonoBehaviour
{
    public static DailyRunService I { get; private set; }

    [SerializeField] private int baseTarget = 1000;
    [SerializeField] private int targetPerStage = 150;
    [SerializeField] private int targetVariance = 120;
    [SerializeField] private int baseMoves = 20;
    [SerializeField] private int movesEveryNStages = 3;
    [SerializeField] private int movesPenalty = 1;
    [SerializeField] private int minMoves = 10;

    private DailyRunData data;
    private string path;
    private string Today => DateTime.Now.ToString("yyyyMMdd");

    public int Stage => data.stage;
    public int Target => data.target;
    public int Moves => data.moves;
    public int Seed => data.seed;
    public int BestStageToday => data.bestStageToday;

    public event Action DayRolledOver;
    public event Action ParamsChanged;

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        path = Path.Combine(Application.persistentDataPath, "daily_run.json");
        LoadOrInit();
        EnsureToday();
    }

    private void Update()
    {
        if (data.yyyymmdd != Today)
        {
            ResetForNewDay();
            DayRolledOver?.Invoke();
        }
    }

    public void EnsureToday()
    {
        if (data.yyyymmdd != Today)
        {
            ResetForNewDay();
        }
        Recalc();
        Save();
        ParamsChanged?.Invoke();
    }

    public void AdvanceStage()
    {
        data.stage = Mathf.Max(1, data.stage + 1);
        if (data.stage > data.bestStageToday) data.bestStageToday = data.stage;
        Recalc();
        Save();
        ParamsChanged?.Invoke();
    }

    public int ActiveColors(int maxColors)
    {
        int inc = 1 + (data.stage - 1) / 3;
        return Mathf.Clamp(3 + inc, 3, Mathf.Max(3, maxColors));
    }


    private void ResetForNewDay()
    {
        data.yyyymmdd = Today;
        data.seed = DateTime.Now.Year * 10000 + DateTime.Now.Month * 100 + DateTime.Now.Day;
        data.stage = 1;
        data.bestStageToday = 1;
        Recalc();
        Save();
        ParamsChanged?.Invoke();
    }

    private void Recalc()
    {
        var rnd = new System.Random(data.seed + data.stage * 7919);
        int variance = rnd.Next(-targetVariance, targetVariance + 1);
        data.target = Mathf.Max(300, baseTarget + targetPerStage * (data.stage - 1) + variance);
        int steps = (data.stage - 1) / Mathf.Max(1, movesEveryNStages);
        data.moves = Mathf.Max(minMoves, baseMoves - steps * movesPenalty);
    }

    private void LoadOrInit()
    {
        if (File.Exists(path))
        {
            try { data = JsonUtility.FromJson<DailyRunData>(File.ReadAllText(path)); }
            catch { data = new DailyRunData(); }
        }
        else { data = new DailyRunData(); }
        if (string.IsNullOrEmpty(data.yyyymmdd)) data.yyyymmdd = Today;
        if (data.stage < 1) data.stage = 1;
        if (data.bestStageToday < 1) data.bestStageToday = data.stage;
        if (data.seed == 0) data.seed = DateTime.Now.Year * 10000 + DateTime.Now.Month * 100 + DateTime.Now.Day;
    }

    private void Save()
    {
        try { File.WriteAllText(path, JsonUtility.ToJson(data)); }
        catch (Exception e) { Debug.LogError(e.Message); }
    }
}
