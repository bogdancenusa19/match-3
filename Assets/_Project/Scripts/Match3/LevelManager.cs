// Assets/_Project/Scripts/Match3/LevelManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private BoardController board;
    [SerializeField] private InputController inputCtrl;
    [SerializeField] private UIModalEndLevel modalEndLevel;
    [SerializeField] private LevelRuntime level;

    private DailyRunService dailyRun;
    private bool ended;
    private bool targetReached;
    private bool pendingEnd;

    private void Awake()
    {
        if (!board) board = Object.FindFirstObjectByType<BoardController>(FindObjectsInactive.Include);
        if (!inputCtrl) inputCtrl = Object.FindFirstObjectByType<InputController>(FindObjectsInactive.Include);
        if (!modalEndLevel) modalEndLevel = Object.FindFirstObjectByType<UIModalEndLevel>(FindObjectsInactive.Include);
        if (!level) level = Object.FindFirstObjectByType<LevelRuntime>(FindObjectsInactive.Include);
        if (!dailyRun) dailyRun = Object.FindFirstObjectByType<DailyRunService>(FindObjectsInactive.Include);
    }

    private void Start()
    {
        if (dailyRun) { dailyRun.EnsureToday(); dailyRun.DayRolledOver += OnMidnight; }
        if (board && level) board.SetMoves(level.GetMoves());
        Subscribe();
        targetReached = board && level && board.Score >= level.GetTargetScore();
    }

    private void OnDestroy()
    {
        if (dailyRun) dailyRun.DayRolledOver -= OnMidnight;
    }

    private void OnMidnight()
    {
        SceneManager.LoadScene("Game");
    }

    private void OnEnable() { Subscribe(); }
    private void OnDisable() { Unsubscribe(); }

    private void Subscribe()
    {
        if (!board) return;
        board.MovesChanged -= OnMovesChanged;
        board.ScoreChanged -= OnScoreChanged;
        board.MovesChanged += OnMovesChanged;
        board.ScoreChanged += OnScoreChanged;
    }

    private void Unsubscribe()
    {
        if (!board) return;
        board.MovesChanged -= OnMovesChanged;
        board.ScoreChanged -= OnScoreChanged;
    }

    private void OnMovesChanged(int v)
    {
        if (ended) return;
        if (v > 0) return;
        if (pendingEnd) return;
        pendingEnd = true;
        StartCoroutine(WaitBoardStableThenEnd());
    }

    private IEnumerator WaitBoardStableThenEnd()
    {
        yield return null;
        while (board != null && board.IsProcessing) yield return null;
        EndNow(targetReached);
    }

    private void OnScoreChanged(int score)
    {
        if (ended) return;
        if (level == null) return;
        int target = level.GetTargetScore();
        if (!targetReached && target > 0 && score >= target) targetReached = true;
    }

    private void EndNow(bool win)
    {
        ended = true;
        if (inputCtrl) inputCtrl.enabled = false;
        if (board) board.LockBoard();
        if (!modalEndLevel) return;
        if (win) modalEndLevel.ShowWin();
        else
        {
            bool canTry = LifeSystem.I != null && LifeSystem.I.Lives > 0;
            modalEndLevel.ShowLose(canTry);
        }
    }

    public void FinishNow()
    {
        if (ended) return;
        if (!targetReached) return;
        EndNow(true);
    }

    public void TryAgain()
    {
        if (LifeSystem.I == null) return;
        if (!LifeSystem.I.TryConsumeLife())
        {
            if (modalEndLevel) modalEndLevel.ShowLose(false);
            return;
        }
        SceneManager.LoadScene("Game");
    }

    public void GoHome()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void NextLevel()
    {
        if (dailyRun) dailyRun.AdvanceStage();
        SceneManager.LoadScene("Game");
    }
}
