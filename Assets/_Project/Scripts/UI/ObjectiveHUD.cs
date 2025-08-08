// Assets/_Project/Scripts/UI/ObjectiveHUD.cs
using UnityEngine;
using TMPro;

public class ObjectiveHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text starsText;
    [SerializeField] private LevelRuntime level;
    [SerializeField] private BoardController board;

    private void OnEnable()
    {
        if (board) board.ScoreChanged += OnScore;
        UpdateTexts();
    }

    private void OnDisable()
    {
        if (board) board.ScoreChanged -= OnScore;
    }

    private void Start()
    {
        UpdateTexts();
    }

    private void OnScore(int _)
    {
        UpdateTexts();
    }

    private void UpdateTexts()
{
    if (!statusText || !starsText || level == null || board == null) return;

    int target = level.GetTargetScore();
    int s2 = level.GetStar2();
    int s3 = level.GetStar3();

    bool reached = target > 0 && board.Score >= target;
    // Schimbăm textul aici
    statusText.text = reached ? $"Target reached" : $"Target: {target}";

    int stars = 0;
    if (s3 > 0 && board.Score >= s3) stars = 3;
    else if (s2 > 0 && board.Score >= s2) stars = 2;
    else if (target > 0 && board.Score >= target) stars = 1;

    starsText.text = $"{stars}/3";
}

}
