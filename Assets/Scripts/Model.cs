using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SendCommand
{
    [JsonProperty("action")]
    public string action;

    [JsonProperty("room")]
    public string room;

    [JsonProperty("id")]
    public string userId;

    [JsonProperty("data")]
    public string data;
}

public class Command {
    [JsonProperty("action")]
    public string action;

    [JsonProperty("data")]
    public string data;
}

public class Player
{
    [JsonProperty("info")]
    public PlayerInfo playerInfo;

    [JsonProperty("numcard")]
    public int numCard;

    [JsonProperty("index")]
    public int index;

    [JsonProperty("ingame")]
    public bool inGame;

    [JsonProperty("finalist")]
    public bool finalist;

    [JsonProperty("ishost")]
    public bool isHost;

    [JsonProperty("disconnected")]
    public bool disconnected;

    public int mappedIndex;
    public List<string> cards;
}

public class PlayerInfo
{
    [JsonProperty("id")]
    public string userId;

    [JsonProperty("username")]
    public string userName;

    [JsonProperty("image")]
    public string image;

    [JsonProperty("amount")]
    public double amount;
}

public class PlayData {
    [JsonProperty("id")]
    public string id;

    [JsonProperty("data")]
    public string card;
}

public class Play {
    [JsonProperty("action")]
    public string action;

    [JsonProperty("id")]
    public string userid;

    [JsonProperty("card")]
    public string card;
}

public class Result {
    
}
