using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public static class GameClient
{
    public delegate void OnConnect();
    public delegate void OnJoin(List<Player> players);
    public delegate void OnCards(List<string> cards);
    public delegate void OnPlay(Play play);
    public delegate void OnResult();
    public static event OnJoin OnJoinEvent;
    public static event OnCards OnCardsEvent;
    public static event OnConnect OnConnectEvent;
    public static event OnPlay OnPlayEvent;
    public static event OnResult OnResultEvent;

    public const string PLAYERS = "PLAYERS";
    public const string CARDS = "CARDS";
    public const string ELIMINATED = "ELIMINATED";
    public const string STARTROW = "STARTROW";
    public const string SHOWBACK = "SHOWBACK";
    public const string WINNER = "WINNER";
    public const string PLAY = "PLAY";
    public const string FOLD = "FOLD";


    private static TcpClient tcpClient;
    private static NetworkStream stream
    {
        get
        {
            return tcpClient.GetStream();
        }
    }
    private static byte[] recvBuffer = new byte[4096];

    public static void Init()
    {
        tcpClient = new TcpClient();
        tcpClient.ReceiveBufferSize = 4096;
        tcpClient.SendBufferSize = 4096;
        tcpClient.BeginConnect("127.0.0.1", 9999, new AsyncCallback(ClientConnect), tcpClient);
    }

    private static void ClientConnect(IAsyncResult ar)
    {
        tcpClient.EndConnect(ar);
        if (tcpClient.Connected == false)
        {
            return;
        }
        else
        {
            tcpClient.NoDelay = false;
            OnConnectEvent();
            stream.BeginRead(recvBuffer, 0, recvBuffer.Length, ClientReceive, null);
        }
    }

    private static void ClientReceive(IAsyncResult ar)
    {
        try
        {
            Debug.Log("Written");
            int length = stream.EndRead(ar);
            if (length < 0)
            {
                return;
            }
            byte[] buffer = new byte[length];
            Array.Copy(recvBuffer, buffer, length);
            Array.Clear(recvBuffer, 0, recvBuffer.Length);
            string message = System.Text.Encoding.UTF8.GetString(buffer);
            Debug.Log(message);
            HandleMessage(message);
            stream.BeginRead(recvBuffer, 0, recvBuffer.Length, ClientReceive, null);
        }
        catch (Exception)
        {
            return;
        }
    }

    public static void Send(byte[] data)
    {
        byte[] buffer = new byte[data.Length];
        Array.Copy(data, buffer, data.Length);
        Debug.Log("send " + data.Length);
        stream.BeginWrite(buffer, 0, data.Length, ClientRead, null);
    }

    private static void ClientRead(IAsyncResult ar) {
        Debug.Log("sent");
        stream.EndWrite(ar);
    }

    public static void Disconnect()
    {
        if (tcpClient != null)
        {
            tcpClient.Close();
        }
    }

    private static void HandleMessage(string message) {
        Command command = JsonConvert.DeserializeObject<Command>(message);
        Debug.Log("Receive command " + command.action);
        switch (command.action) {
            case PLAYERS:
                List<Player> players = JsonConvert.DeserializeObject<List<Player>>(command.data);
                OnJoinEvent(players);
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
            default:
                Debug.Log("Received unhandled event " + command.action);
                break;
        }
    }
}
