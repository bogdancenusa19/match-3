// Assets/_Project/Scripts/Match3/BoardController.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BoardController : MonoBehaviour
{
    [SerializeField] private BoardConfig config;
    [SerializeField] private GameObject tilePrefab;

    private TileView[,] tiles;
    private Vector2 origin;
    private bool isBusy;

    public event Action<int> ScoreChanged;
    public event Action<int> MovesChanged;
    public int Score { get; private set; }
    public int Moves { get; private set; }

    private void Awake()
    {
        if (!config || !tilePrefab) return;
        tiles = new TileView[config.rows, config.cols];
        GenerateBoard();
        Moves = config.startMoves;
        ScoreChanged?.Invoke(Score);
        MovesChanged?.Invoke(Moves);
    }

    private void GenerateBoard()
    {
        float w = config.cols * config.tileSize;
        float h = config.rows * config.tileSize;
        origin = new Vector2(-w * 0.5f + config.tileSize * 0.5f, h * 0.5f - config.tileSize * 0.5f);

        for (int r = 0; r < config.rows; r++)
        {
            for (int c = 0; c < config.cols; c++)
            {
                var go = Instantiate(tilePrefab, transform);
                go.transform.position = GetWorldPos(r, c);
                var view = go.GetComponent<TileView>();
                int colorIndex = UnityEngine.Random.Range(0, Mathf.Max(1, config.colors.Length));
                view.Init(r, c, colorIndex, config.colors[colorIndex], config.tileSize);
                tiles[r, c] = view;
            }
        }
    }

    public Vector3 GetWorldPos(int row, int col)
    {
        return new Vector3(origin.x + col * config.tileSize, origin.y - row * config.tileSize, 0f);
    }

    public bool InBounds(int row, int col)
    {
        return row >= 0 && row < config.rows && col >= 0 && col < config.cols;
    }

    public bool AreAdjacent(int r1, int c1, int r2, int c2)
    {
        int dr = Mathf.Abs(r1 - r2);
        int dc = Mathf.Abs(c1 - c2);
        return (dr == 1 && dc == 0) || (dr == 0 && dc == 1);
    }

    public void TrySwap(int r1, int c1, int r2, int c2)
    {
        if (isBusy) return;
        if (!InBounds(r1, c1) || !InBounds(r2, c2)) return;
        if (!AreAdjacent(r1, c1, r2, c2)) return;
        StartCoroutine(SwapRoutine(r1, c1, r2, c2));
    }

    private IEnumerator SwapRoutine(int r1, int c1, int r2, int c2)
    {
        isBusy = true;
        var a = tiles[r1, c1];
        var b = tiles[r2, c2];

        Vector3 pa = GetWorldPos(r1, c1);
        Vector3 pb = GetWorldPos(r2, c2);

        yield return Move(a.transform, pb, config.swapDuration);
        yield return Move(b.transform, pa, config.swapDuration);

        tiles[r1, c1] = b;
        tiles[r2, c2] = a;
        a.SetIndex(r2, c2);
        b.SetIndex(r1, c1);

        bool valid = HasMatchAt(r1, c1) || HasMatchAt(r2, c2);
        if (!valid)
        {
            yield return Move(a.transform, pa, config.swapDuration);
            yield return Move(b.transform, pb, config.swapDuration);
            tiles[r1, c1] = a;
            tiles[r2, c2] = b;
            a.SetIndex(r1, c1);
            b.SetIndex(r2, c2);
            isBusy = false;
            yield break;
        }

        Moves = Mathf.Max(0, Moves - 1);
        MovesChanged?.Invoke(Moves);

        yield return ResolveBoard();
        isBusy = false;
    }

    private IEnumerator ResolveBoard()
    {
        while (true)
        {
            var matches = FindAllMatches();
            if (matches.Count == 0) yield break;

            foreach (var p in matches)
            {
                var t = tiles[p.r, p.c];
                if (t)
                {
                    Score += config.scorePerTile;
                    Destroy(t.gameObject);
                    tiles[p.r, p.c] = null;
                }
            }
            ScoreChanged?.Invoke(Score);

            yield return CollapseColumns();
            yield return RefillBoard();
            yield return null;
        }
    }

    private List<(int r, int c)> FindAllMatches()
    {
        var set = new HashSet<(int r, int c)>();
        for (int r = 0; r < config.rows; r++)
        {
            int count = 1;
            for (int c = 1; c < config.cols; c++)
            {
                bool same = tiles[r, c] && tiles[r, c - 1] && tiles[r, c].ColorIndex == tiles[r, c - 1].ColorIndex;
                if (same) count++;
                if (!same || c == config.cols - 1)
                {
                    if (count >= 3)
                    {
                        int start = c - count;
                        for (int k = 0; k < count; k++) set.Add((r, start + k));
                    }
                    count = 1;
                }
            }
        }
        for (int c = 0; c < config.cols; c++)
        {
            int count = 1;
            for (int r = 1; r < config.rows; r++)
            {
                bool same = tiles[r, c] && tiles[r - 1, c] && tiles[r, c].ColorIndex == tiles[r - 1, c].ColorIndex;
                if (same) count++;
                if (!same || r == config.rows - 1)
                {
                    if (count >= 3)
                    {
                        int start = r - count;
                        for (int k = 0; k < count; k++) set.Add((start + k, c));
                    }
                    count = 1;
                }
            }
        }
        return new List<(int r, int c)>(set);
    }

    private bool HasMatchAt(int r, int c)
    {
        var center = tiles[r, c];
        if (!center) return false;
        int color = center.ColorIndex;

        int countRow = 1;
        int cc = c - 1;
        while (cc >= 0 && tiles[r, cc] && tiles[r, cc].ColorIndex == color) { countRow++; cc--; }
        cc = c + 1;
        while (cc < config.cols && tiles[r, cc] && tiles[r, cc].ColorIndex == color) { countRow++; cc++; }
        if (countRow >= 3) return true;

        int countCol = 1;
        int rr = r - 1;
        while (rr >= 0 && tiles[rr, c] && tiles[rr, c].ColorIndex == color) { countCol++; rr--; }
        rr = r + 1;
        while (rr < config.rows && tiles[rr, c] && tiles[rr, c].ColorIndex == color) { countCol++; rr++; }
        return countCol >= 3;
    }

    private IEnumerator CollapseColumns()
    {
        for (int c = 0; c < config.cols; c++)
        {
            int writeRow = config.rows - 1;
            for (int r = config.rows - 1; r >= 0; r--)
            {
                if (tiles[r, c] != null)
                {
                    if (writeRow != r)
                    {
                        var t = tiles[r, c];
                        tiles[writeRow, c] = t;
                        tiles[r, c] = null;
                        t.SetIndex(writeRow, c);
                        yield return Move(t.transform, GetWorldPos(writeRow, c), DistanceTime(t.transform.position, GetWorldPos(writeRow, c)));
                    }
                    writeRow--;
                }
            }
        }
    }

    private IEnumerator RefillBoard()
    {
        for (int c = 0; c < config.cols; c++)
        {
            for (int r = 0; r < config.rows; r++)
            {
                if (tiles[r, c] == null)
                {
                    var go = Instantiate(tilePrefab, transform);
                    int colorIndex = UnityEngine.Random.Range(0, Mathf.Max(1, config.colors.Length));
                    var view = go.GetComponent<TileView>();
                    view.Init(r, c, colorIndex, config.colors[colorIndex], config.tileSize);
                    Vector3 start = GetWorldPos(-1, c);
                    go.transform.position = start;
                    tiles[r, c] = view;
                    yield return Move(go.transform, GetWorldPos(r, c), DistanceTime(start, GetWorldPos(r, c)));
                }
            }
        }
    }

    private IEnumerator Move(Transform t, Vector3 target, float duration)
    {
        Vector3 start = t.position;
        float t0 = 0f;
        while (t0 < duration)
        {
            t0 += Time.deltaTime;
            float k = Mathf.Clamp01(t0 / duration);
            t.position = Vector3.Lerp(start, target, k);
            yield return null;
        }
        t.position = target;
    }

    private float DistanceTime(Vector3 a, Vector3 b)
    {
        float d = Vector3.Distance(a, b);
        return Mathf.Max(0.05f, d / Mathf.Max(0.01f, config.fallSpeed));
    }

    public bool TryGetTileFromCollider(Collider col, out TileView tile)
    {
        tile = col ? col.GetComponent<TileView>() : null;
        return tile != null;
    }
}
