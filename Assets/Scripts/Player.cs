using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
