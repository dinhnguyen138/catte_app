using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class Setting
{
    static Config config;
    class Config
    {
        [JsonProperty("servicehost")]
        public string serviceHost;

        [JsonProperty("gamehost")]
        public string gameHost;

        [JsonProperty("googletoken")]
        public string token;
    }

    static void LoadSetting()
    {
        TextAsset data = Resources.Load<TextAsset>("env");
        config = JsonConvert.DeserializeObject<Config>(data.text);
        
    }

    public static string GetGoogleToken()
    {
        if (config == null)
        {
            LoadSetting();
        }
        return config.token;
    }

    public static string GetServiceHost()
    {
        if (config == null)
        {
            LoadSetting();
        }
        return config.serviceHost;
    }

    public static string GetGamehost()
    {
        if (config == null)
        {
            LoadSetting();
        }
        return config.gameHost;
    }
}
