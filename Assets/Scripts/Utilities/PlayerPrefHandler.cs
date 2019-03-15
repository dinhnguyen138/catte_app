using System;
using UnityEngine;

public static class PlayerPrefHandler
{
    public static string TOKEN = "Token";

    public static void SaveString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    public static void LoadString(string key, ref string value)
    {
        PlayerPrefs.GetString(key, value);
    }
}
