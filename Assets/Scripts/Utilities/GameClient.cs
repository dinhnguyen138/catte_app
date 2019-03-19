using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public static class GameClient
{
    public delegate void OnConnect();
    public static event OnConnect OnConnectEvent;
    private static TcpClient tcpClient;
    private static SslStream stream;
    private static byte[] recvBuffer = new byte[4096];

    public static void Init(string host)
    {
        tcpClient = new TcpClient();
        tcpClient.ReceiveBufferSize = 4096;
        tcpClient.SendBufferSize = 4096;
        tcpClient.BeginConnect(host, 9999, new AsyncCallback(ClientConnect), tcpClient);
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
            stream = new SslStream(
                tcpClient.GetStream(),
                false,
                new RemoteCertificateValidationCallback(ValidateServerCertificate),
                null
            );
            stream.AuthenticateAsClient("");
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

    // TODO: Cheat to bypass untrusted certificate, can remove this on production
    public static bool ValidateServerCertificate(
          object sender,
          X509Certificate certificate,
          X509Chain chain,
          SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }
}
