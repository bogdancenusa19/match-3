using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class SaveSystem
{
    private const string FileName = "save_m3.json";
    private const string Key = "m3_key_2025";
    public static string PathFull => Path.Combine(Application.persistentDataPath, FileName);

    public static void Save(SaveData data)
    {
        var json = JsonUtility.ToJson(data);
        var enc = Xor(json, Key);
        File.WriteAllText(PathFull, Convert.ToBase64String(Encoding.UTF8.GetBytes(enc)));
    }

    public static SaveData Load()
    {
        try
        {
            if (!File.Exists(PathFull)) return new SaveData();
            var b64 = File.ReadAllText(PathFull);
            var enc = Encoding.UTF8.GetString(Convert.FromBase64String(b64));
            var dec = Xor(enc, Key);
            return JsonUtility.FromJson<SaveData>(dec) ?? new SaveData();
        }
        catch { return new SaveData(); }
    }

    private static string Xor(string input, string key)
    {
        var sb = new StringBuilder(input.Length);
        for (int i = 0; i < input.Length; i++)
            sb.Append((char)(input[i] ^ key[i % key.Length]));
        return sb.ToString();
    }
}
