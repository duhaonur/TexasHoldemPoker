using System;
using UnityEngine;
public class FirebaseAnalytics : MonoBehaviour
{
    private float _startTime;

    public static object EventAppOpen { get; private set; }
    public static object ParameterValue { get; private set; }


    void Start()
    {
        DontDestroyOnLoad(gameObject);
        // Oyuna giriş zamanını kaydet
        _startTime = Time.time;

    }

    void OnDestroy()
    {
        // Oyundan çıkış zamanını kaydet ve Firebase Analytics'e gönder
        float duration = Time.time - _startTime;
        LogGameDuration(duration);

    }

    void LogGameDuration(float duration)
    {
        // Firebase Analytics'e oyun süresini gönder
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventAppOpen,
        FirebaseAnalytics.ParameterValue, duration);
    }

    private static void LogEvent(object eventAppOpen, object parameterValue, float duration)
    {
        throw new NotImplementedException();
    }
}
