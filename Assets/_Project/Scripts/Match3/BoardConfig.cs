// Assets/_Project/Scripts/Match3/BoardConfig.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Match3/BoardConfig")]
public class BoardConfig : ScriptableObject
{
    public int rows = 9;
    public int cols = 9;
    public float tileSize = 1f;
    public Color[] colors = new Color[7];
    public float swapDuration = 0.15f;
    public float fallSpeed = 12f;
    public int startMoves = 20;
    public int scorePerTile = 10;
}
