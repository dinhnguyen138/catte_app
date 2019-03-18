using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class Setting
{
    static string host = "";
    static string googleToken = "";

    class Config
    {
        [JsonProperty("host")]
        public string host;

        [JsonProperty("googletoken")]
        public string token;
    }

    static void LoadSetting()
    {
        TextAsset data = Resources.Load<TextAsset>("env");
        Config config = JsonConvert.DeserializeObject<Config>(data.text);
        host = config.host;
        googleToken = config.token;
    }

    public static string GetGoogleToken()
    {
        if (googleToken == "")
        {
            LoadSetting();
        }
        return googleToken;
    }

    public static string GetHost()
    {
        if (host == "")
        {
            LoadSetting();
        }
        return host;
    }
}
