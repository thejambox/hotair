using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;

public class WorldTime : MonoBehaviour
{
    public float timeSunrise = 6.4f; // 6:24AM
    public float timeSunset = 16.53f; // 4:32PM

    public bool overrideTime = false;
    public float timeOverride = 0f;

    public static WorldTime Instance = null;

    public float timeCurrent
    {
        get
        {
            if (overrideTime)
            {
                return Mathf.Clamp(timeOverride % 24f, 0f, 24f);
            }
            else
            {
                DateTime dt = DateTime.Now;
                return dt.Hour + (dt.Minute / 60f) + (dt.Second / 3600f);
            }
        }
    }

    public bool isDayTime
    {
        get
        {
            return timeCurrent >= timeSunrise && timeCurrent <= timeSunset;
        }
    }

    public float dayTimeLeft
    {
        get
        {
            return Mathf.Clamp(timeSunset - timeCurrent, 0f, totalDayTime);
        }
    }

    public float nightTimeLeft
    {
        get
        {
            return (timeCurrent < timeSunrise) ? timeSunrise - timeCurrent : totalNightTime - (timeCurrent - timeSunset);
        }
    }

    public float totalDayTime
    {
        get
        {
            return timeSunset - timeSunrise;
        }
    }

    public float totalNightTime
    {
        get
        {
            return 24f - totalDayTime;
        }
    }

    public float phaseComplete
    {
        get
        {
            return 1f - (isDayTime ? (dayTimeLeft / totalDayTime) : (nightTimeLeft / totalNightTime));
        }
    }

    private void Start()
    {
        if (Instance == null)
            Instance = this;

        if (Instance != null && Instance != this)
            Destroy(gameObject);

#if UNITY_EDITOR
        overrideTime = false;
#endif
    }
}
