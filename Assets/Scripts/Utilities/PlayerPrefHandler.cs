using System;
using UnityEngine;

public static class PlayerPrefHandler
{
    public static string TOKEN = "Token";

    public static void SaveString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    public static string LoadString(string key)
    {
        return PlayerPrefs.GetString(key, "");
    }
}
