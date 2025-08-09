// Assets/_Project/Scripts/Economy/LootTable.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootTable", menuName = "Game/LootTable")]
public class LootTable : ScriptableObject
{
    public List<LootEntry> entries = new List<LootEntry>();
    public int rollsPerChest = 1;
}
