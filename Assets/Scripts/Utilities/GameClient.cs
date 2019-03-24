using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using UnityEngine;

public static class GameClient
{
    public delegate void OnConnect();
    public delegate void OnDisconnect();
    public static event OnConnect OnConnectEvent;
    public static event OnDisconnect OnDisconnectEvent;
    private static TcpClient tcpClient;
    private static SslStream stream;
    private static string serverHost;
    private static byte[] recvBuffer = new byte[4096];

    public static void Init(string host)
    {
        serverHost = host;
        if (host == "")
        {
            serverHost = Setting.GetGamehost();
        }
        tcpClient = new TcpClient();
        tcpClient.ReceiveBufferSize = 4096;
        tcpClient.SendBufferSize = 4096;
    }

    public static void Connect()
    {
        Thread connectThread = new Thread(new ThreadStart(() => {
            var result = tcpClient.BeginConnect(serverHost, 9999, null, null);
            var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));

            try
            {
                tcpClient.EndConnect(result);
            }
            catch { }
            if (!success)
            {
                OnDisconnectEvent();
            }
            else
            {
                if (tcpClient.Connected == false)
                {
                    Debug.Log("Connect failed");
                    OnDisconnectEvent();
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
                    Debug.Log("Success handshake to server");
                    tcpClient.NoDelay = false;
                    OnConnectEvent();
                    stream.BeginRead(recvBuffer, 0, recvBuffer.Length, ClientReceive, null);
                }
            }
        }));
        connectThread.Start();
    }

    private static void ClientReceive(IAsyncResult ar)
    {
        try
        {
            Debug.Log("Written");
            int length = stream.EndRead(ar);
            if (length < 0)
            {
                OnDisconnectEvent();
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
        if (stream != null)
        {
            stream.Write(buffer, 0, data.Length);
        }
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
