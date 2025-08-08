// Assets/_Project/Scripts/UI/UIHUD.cs
using UnityEngine;
using TMPro;

public class UIHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text movesText;
    [SerializeField] private BoardController board;

    private void OnEnable()
    {
        if (!board) return;
        board.ScoreChanged += OnScoreChanged;
        board.MovesChanged += OnMovesChanged;
        OnScoreChanged(board.Score);
        OnMovesChanged(board.Moves);
    }

    private void OnDisable()
    {
        if (!board) return;
        board.ScoreChanged -= OnScoreChanged;
        board.MovesChanged -= OnMovesChanged;
    }

    private void OnScoreChanged(int v) { if (scoreText) scoreText.text = $"Score: {v}"; }
    private void OnMovesChanged(int v) { if (movesText) movesText.text = $"Moves: {v}"; }
}
