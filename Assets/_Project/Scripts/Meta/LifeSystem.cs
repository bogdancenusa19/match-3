using System;
using UnityEngine;

public class LifeSystem : MonoBehaviour
{
    public static LifeSystem I { get; private set; }

    [SerializeField] private int maxLives = 5;
    [SerializeField] private int regenMinutes = 30;

    private SaveData data;

    public int MaxLives => maxLives;
    public int Lives => data.lives;
    public TimeSpan TimeToNext => GetTimeToNext();

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        data = SaveSystem.Load();
        data.lives = Mathf.Clamp(data.lives, 0, maxLives);
        if (data.lives >= maxLives) data.nextLifeUnix = 0;
    }

    private void Update()
    {
        if (data.lives >= maxLives) return;
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (data.nextLifeUnix == 0) data.nextLifeUnix = now + regenMinutes * 60;
        if (now >= data.nextLifeUnix)
        {
            data.lives = Mathf.Min(maxLives, data.lives + 1);
            if (data.lives < maxLives) data.nextLifeUnix = now + regenMinutes * 60;
            else data.nextLifeUnix = 0;
            SaveSystem.Save(data);
        }
    }

    public bool TryConsumeLife()
    {
        if (data.lives <= 0) return false;
        data.lives -= 1;
        if (data.lives < maxLives && data.nextLifeUnix == 0)
            data.nextLifeUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + regenMinutes * 60;
        SaveSystem.Save(data);
        return true;
    }

    public void AddLife(int count)
    {
        data.lives = Mathf.Clamp(data.lives + count, 0, maxLives);
        if (data.lives >= maxLives) data.nextLifeUnix = 0;
        SaveSystem.Save(data);
    }

    private TimeSpan GetTimeToNext()
    {
        if (data.lives >= maxLives || data.nextLifeUnix == 0) return TimeSpan.Zero;
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var remain = Math.Max(0, data.nextLifeUnix - now);
        return TimeSpan.FromSeconds(remain);
    }
}
