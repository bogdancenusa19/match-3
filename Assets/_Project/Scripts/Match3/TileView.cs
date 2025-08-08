// Assets/_Project/Scripts/Match3/TileView.cs
using UnityEngine;

public class TileView : MonoBehaviour
{
    [SerializeField] private Renderer rend;
    public int Row { get; private set; }
    public int Col { get; private set; }
    public int ColorIndex { get; private set; }

    public void Init(int row, int col, int colorIndex, Color color, float size)
    {
        Row = row;
        Col = col;
        ColorIndex = colorIndex;
        if (!rend) rend = GetComponent<Renderer>();
        rend.material.color = color;
        transform.localScale = new Vector3(size, size, 1f);
        name = $"Tile_{row}_{col}";
    }

    public void SetIndex(int row, int col)
    {
        Row = row;
        Col = col;
        name = $"Tile_{row}_{col}";
    }

    public void SetColorIndex(int colorIndex, Color color)
    {
        ColorIndex = colorIndex;
        if (!rend) rend = GetComponent<Renderer>();
        rend.material.color = color;
    }
}
