using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class MessageHandler
{
    public delegate void OnJoin(List<Player> players);
    public delegate void OnNewPlayer(Player player);
    public delegate void OnLeave(int index);
    public delegate void OnCards(List<string> cards);
    public delegate void OnPlay(PlayData play);
    public delegate void OnStart(int index);
    public delegate void OnEliminated(List<int> disqualifiers);
    public delegate void OnError(int error);
    public delegate void OnResult(List<ResultMsg> results);
    public static event OnJoin OnJoinEvent;
    public static event OnNewPlayer OnNewPlayerEvent;
    public static event OnLeave OnLeaveEvent;
    public static event OnCards OnCardsEvent;
    public static event OnPlay OnPlayEvent;
    public static event OnStart OnStartEvent;
    public static event OnEliminated OnEliminatedEvent;
    public static event OnError OnErrorEvent;
    public static event OnResult OnResultEvent;

    public const string JOIN = "JOIN";
    public const string DEAL = "DEAL";
    public const string PLAYERS = "PLAYERS";
    public const string NEWPLAYER = "NEWPLAYER";
    public const string LEAVE = "LEAVE";
    public const string CARDS = "CARDS";
    public const string ELIMINATED = "ELIMINATED";
    public const string START = "START";
    public const string RESULT = "RESULT";
    public const string ERROR = "ERROR";
    public const string PLAY = "PLAY";
    public const string FOLD = "FOLD";
    private static string room;
    private static string user;
    private static int index;
    private static object lockObj;
    private static List<string> listMessage;

    public static void Init(string roomId, string userId)
    {
        room = roomId;
        user = userId;
        lockObj = new object();
        listMessage = new List<string>();
    }
    public static void SetIndex(int userIndex)
    {
        index = userIndex;
    }

    public static void HandleMessage(string message) {
        Debug.Log(message);
        string[] messages = message.Split('\n');
        for (int i = 0; i < messages.Length; i++)
        {
            if (messages[i] != "")
            {
                listMessage.Add(messages[i]);
            }
        }
    }

    public static void ProcessMessage(string message)
    { 
        Command command = JsonConvert.DeserializeObject<Command>(message);
        Debug.Log("Receive command " + command.action);
        switch (command.action)
        {
            case PLAYERS:
                List<Player> players = JsonConvert.DeserializeObject<List<Player>>(command.data);
                OnJoinEvent(players);
                break;
            case NEWPLAYER:
                Player player = JsonConvert.DeserializeObject<Player>(command.data);
                OnNewPlayerEvent(player);
                break;
            case LEAVE:
                OnLeaveEvent(Convert.ToInt32(command.data));
                break;
            case CARDS:
                List<string> cards = JsonConvert.DeserializeObject<List<string>>(command.data);
                OnCardsEvent(cards);
                break;
            case PLAY:
            case FOLD:
                PlayData playData = JsonConvert.DeserializeObject<PlayData>(command.data);
                playData.action = command.action;
                OnPlayEvent(playData);
                break;
            case START:
                OnStartEvent(Convert.ToInt32(command.data));
                break;
            case ELIMINATED:
                List<int> disqualifiers = JsonConvert.DeserializeObject<List<int>>(command.data);
                OnEliminatedEvent(disqualifiers);
                break;
            case ERROR:
                int error = Convert.ToInt32(command.data);
                OnErrorEvent(error);
                break;
            case RESULT:
                List<ResultMsg> results = JsonConvert.DeserializeObject<List<ResultMsg>>(command.data);
                OnResultEvent(results);
                break;
            default:
                Debug.Log("Received unhandled event " + command.action);
                break;
        }
    }

    public static void ProcessMessage()
    {
        if(listMessage == null)
            {
                return;
            }
            for (int i = 0; i < listMessage.Count; i++)
            {
                ProcessMessage(listMessage[i]);
            }
            listMessage.Clear();
    }

    public static void JoinRoom(PlayerInfo playerInfo)
    {
        string data = JsonConvert.SerializeObject(playerInfo);
        SendCommand command = new SendCommand();
        command.action = JOIN;
        command.room = room;
        command.data = data;
        string sendData = JsonConvert.SerializeObject(command) + "\n";
        GameClient.Send(System.Text.Encoding.UTF8.GetBytes(sendData));
    }

    public static void Deal(){
        SendCommand command = new SendCommand();
        command.action = DEAL;
        command.room = room;
        command.index = index;
        command.data = "";
        string sendData = JsonConvert.SerializeObject(command) + "\n";
        GameClient.Send(System.Text.Encoding.UTF8.GetBytes(sendData));
    }

    public static void Play(string card)
    {
        SendCommand command = new SendCommand();
        command.action = PLAY;
        command.room = room;
        command.index = index;
        command.data = card;
        string sendData = JsonConvert.SerializeObject(command) + "\n";
        GameClient.Send(System.Text.Encoding.UTF8.GetBytes(sendData));
    }

    public static void Fold(string card)
    {
        SendCommand command = new SendCommand();
        command.action = FOLD;
        command.room = room;
        command.index = index;
        command.data = card;
        string sendData = JsonConvert.SerializeObject(command) + "\n";
        GameClient.Send(System.Text.Encoding.UTF8.GetBytes(sendData));
    }
}
