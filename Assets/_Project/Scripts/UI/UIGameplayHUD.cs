// Assets/_Project/Scripts/UI/UIGameplayHUD.cs
using UnityEngine;

public class UIGameplayHUD : MonoBehaviour
{
    [SerializeField] private BoardController board;
    [SerializeField] private UIFloatingTextSpawner popSpawner;
    [SerializeField] private UINoMovesToast toast;

    private void Awake()
    {
        if (!board) board = Object.FindFirstObjectByType<BoardController>(FindObjectsInactive.Include);
    }
void Update()
{
    if (Input.GetKeyDown(KeyCode.P))
        if (popSpawner) popSpawner.SpawnScreenCenter("+999");
}

    private void OnEnable()
    {
        if (!board) return;
        board.ScorePop += OnScorePop;
        board.NoMovesReshuffle += OnReshuffle;
    }

    private void OnDisable()
    {
        if (!board) return;
        board.ScorePop -= OnScorePop;
        board.NoMovesReshuffle -= OnReshuffle;
    }

    private void OnScorePop(int delta, Vector3 worldPos)
    {
        if (popSpawner) popSpawner.SpawnWorld(worldPos, "+" + delta.ToString());
    }

    private void OnReshuffle()
    {
        if (toast) toast.Show("No moves → Shuffle");
    }
}
