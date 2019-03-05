using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public static class GameClient
{
    public delegate void OnConnect();
    public static event OnConnect OnConnectEvent;
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
                // Disconnected
                return;
            }
            byte[] buffer = new byte[length];
            Array.Copy(recvBuffer, buffer, length);
            Array.Clear(recvBuffer, 0, recvBuffer.Length);
            string message = System.Text.Encoding.UTF8.GetString(buffer);
            Debug.Log(message);
            MessageHandler.HandleMessage(message);
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
        stream.Write(buffer, 0, data.Length);
    }

    public static void Disconnect()
    {
        if (tcpClient != null)
        {
            tcpClient.Close();
        }
    }
}
