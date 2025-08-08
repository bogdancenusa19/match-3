using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Match3/Level Config", order = 0)]
public class LevelConfig : ScriptableObject
{
    public int levelID = 1;
    public int boardRows = 9;
    public int boardCols = 9;
    public int moves = 20;

    public int targetScore = 1000;
    public int score2Stars = 1500;
    public int score3Stars = 2000;

    public ObjectiveType objectiveType = ObjectiveType.ScoreOnly;
    public int objectiveTarget = 1000;
    public int objectiveColorIndex = 0;

    public enum ObjectiveType
    {
        ScoreOnly,
        CollectColor,
        ClearObstacles
    }
}
