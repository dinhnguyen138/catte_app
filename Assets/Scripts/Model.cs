using Newtonsoft.Json;
using System.Collections.Generic;

public class SendCommand
{
    [JsonProperty("action")]
    public string action;

    [JsonProperty("room")]
    public string room;

    [JsonProperty("index")]
    public int index;

    [JsonProperty("data")]
    public string data;
}

public class Command {
    [JsonProperty("action")]
    public string action;

    [JsonProperty("data")]
    public string data;
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

public class WinnerData
{
    [JsonProperty("index")]
    public int winnerIndex;

    [JsonProperty("lastplays")]
    public List<PlayData> lastPlays;
}

public class PlayData {

    public string action;

    [JsonProperty("index")]
    public int index;

    [JsonProperty("row")]
    public int row;

    [JsonProperty("nextturn")]
    public int nextTurn;

    [JsonProperty("newrow")]
    public bool newRow;

    [JsonProperty("data")]
    public string card;
}

public class Play {
    [JsonProperty("action")]
    public string action;

    [JsonProperty("index")]
    public int index;

    [JsonProperty("card")]
    public string card;
}

public class Result {
    
}
