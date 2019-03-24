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
    public System.Int64 amount;
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

public class LoginRegister
{
    [JsonProperty("username")]
    public string username;

    [JsonProperty("password")]
    public string password;
}

public class Login3rd
{
    [JsonProperty("username")]
    public string username;

    [JsonProperty("user3rdid")]
    public string user3rdid;

    [JsonProperty("token")]
    public string token;

    [JsonProperty("source")]
    public string source;

    [JsonProperty("image")]
    public string image;
}

public class RoomInfo {
    [JsonProperty("id")]
    public string roomid;

    [JsonProperty("noplayer")]
    public int numplayer;

    [JsonProperty("maxplayer")]
    public int maxplayer;

    [JsonProperty("amount")]
    public int amount;

    [JsonProperty("host")]
    public string host;
}

public class SignInResult
{
    [JsonProperty("token")]
    public string token;
}

public class ResultMsg {
    [JsonProperty("index")]
    public int index;

    [JsonProperty("change")]
    public int change;

    [JsonProperty("amount")]
    public System.Int64 amount;
}

public class CreateRoomMsg
{
    [JsonProperty("amount")]
    public long amount;

    [JsonProperty("numplayer")]
    public int maxPlayer;
}

public class LeaveMsg
{
    [JsonProperty("index")]
    public int index;

    [JsonProperty("host")]
    public int host;
}
