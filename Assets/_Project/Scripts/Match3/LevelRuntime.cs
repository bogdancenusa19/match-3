// Assets/_Project/Scripts/Match3/LevelRuntime.cs
using UnityEngine;

public class LevelRuntime : MonoBehaviour
{
    public enum SourceMode { StaticAsset, DailyRun }

    [SerializeField] private SourceMode source = SourceMode.DailyRun;
    [SerializeField] private LevelConfig staticLevel;
    [SerializeField] private BoardController board;

    private DailyRunService daily;

    public SourceMode Mode => source;
    public LevelConfig Level => staticLevel;

    private void Awake()
    {
        daily = Object.FindFirstObjectByType<DailyRunService>(FindObjectsInactive.Include);
    }

    public void Init(BoardController b)
    {
        board = b;
    }

    public int GetMoves()
    {
        if (source == SourceMode.DailyRun && daily != null) return daily.Moves;
        return staticLevel ? staticLevel.moves : 20;
    }

    public int GetTargetScore()
    {
        if (source == SourceMode.DailyRun && daily != null) return daily.Target;
        return staticLevel ? staticLevel.targetScore : 1000;
    }

    public int GetStar2()
    {
        if (source == SourceMode.DailyRun) return Mathf.RoundToInt(GetTargetScore() * 1.4f);
        return staticLevel ? staticLevel.score2Stars : 1500;
    }

    public int GetStar3()
    {
        if (source == SourceMode.DailyRun) return Mathf.RoundToInt(GetTargetScore() * 1.8f);
        return staticLevel ? staticLevel.score3Stars : 2000;
    }
    public void RefreshDaily() { }
}
