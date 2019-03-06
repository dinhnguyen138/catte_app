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
    public delegate void OnLeave(string id);
    public delegate void OnCards(List<string> cards);
    public delegate void OnPlay(Play play);
    public delegate void OnStartRow(string id);
    public delegate void OnEliminated(List<string> disqualifiers);
    public delegate void OnResult();
    public static event OnJoin OnJoinEvent;
    public static event OnNewPlayer OnNewPlayerEvent;
    public static event OnLeave OnLeaveEvent;
    public static event OnCards OnCardsEvent;
    public static event OnPlay OnPlayEvent;
    public static event OnStartRow OnStartRowEvent;
    public static event OnEliminated OnEliminatedEvent;
    public static event OnResult OnResultEvent;

    public const string JOIN = "JOIN";
    public const string DEAL = "DEAL";
    public const string PLAYERS = "PLAYERS";
    public const string NEWPLAYER = "NEWPLAYER";
    public const string LEAVE = "LEAVE";
    public const string CARDS = "CARDS";
    public const string ELIMINATED = "ELIMINATED";
    public const string STARTROW = "STARTROW";
    public const string WINNER = "WINNER";
    public const string PLAY = "PLAY";
    public const string FOLD = "FOLD";
    private static string room;
    private static string user;

    public static void Init(string roomId, string userId)
    {
        room = roomId;
        user = userId;
    }
    public static void HandleMessage(string message) {
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
                OnLeaveEvent(command.data);
                break;
            case CARDS:
                List<string> cards = JsonConvert.DeserializeObject<List<string>>(command.data);
                OnCardsEvent(cards);
                break;
            case PLAY:
            case FOLD:
                PlayData playData = JsonConvert.DeserializeObject<PlayData>(command.data);
                Play play = new Play();
                play.action = command.action;
                play.userid = playData.id;
                play.card = playData.card;
                OnPlayEvent(play);
                break;
            case STARTROW:
                string id = JsonConvert.DeserializeObject<string>(command.data);
                OnStartRowEvent(id);
                break;
            case ELIMINATED:
                List<string> disqualifiers = JsonConvert.DeserializeObject<List<string>>(command.data);
                OnEliminatedEvent(disqualifiers);
                break; 
            default:
                Debug.Log("Received unhandled event " + command.action);
                break;
        }
    }

    public static void JoinRoom(PlayerInfo playerInfo)
    {
        string data = JsonConvert.SerializeObject(playerInfo);
        SendCommand command = new SendCommand();
        command.action = JOIN;
        command.room = room;
        command.userId = user;
        command.data = data;
        string sendData = JsonConvert.SerializeObject(command) + "\n";
        GameClient.Send(System.Text.Encoding.UTF8.GetBytes(sendData));
    }

    public static void Deal(){
        SendCommand command = new SendCommand();
        command.action = DEAL;
        command.room = room;
        command.userId = user;
        command.data = "";
        string sendData = JsonConvert.SerializeObject(command) + "\n";
        GameClient.Send(System.Text.Encoding.UTF8.GetBytes(sendData));
    }

    public static void Play(string card)
    {
        SendCommand command = new SendCommand();
        command.action = PLAY;
        command.room = room;
        command.userId = user;
        command.data = card;
        string sendData = JsonConvert.SerializeObject(command) + "\n";
        GameClient.Send(System.Text.Encoding.UTF8.GetBytes(sendData));
    }

    public static void Fold(string card)
    {
        SendCommand command = new SendCommand();
        command.action = FOLD;
        command.room = room;
        command.userId = user;
        command.data = card;
        string sendData = JsonConvert.SerializeObject(command) + "\n";
        GameClient.Send(System.Text.Encoding.UTF8.GetBytes(sendData));
    }
}
