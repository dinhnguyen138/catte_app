using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

static class ServiceClient
{
    static string host = Setting.GetHost();
    public static IEnumerator DoLogin(string username, string password, System.Action<bool> onLoginFinish)
    {
        LoginRegister login = new LoginRegister();
        login.username = username;
        login.password = password;
        string data = JsonConvert.SerializeObject(login);
        Debug.Log(data);
        UnityWebRequest request = UnityWebRequest.Post(host + "/login", "");
        request.SetRequestHeader("Content-Type", "application/json");
        if (data != null)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);

            UploadHandlerRaw upHandler = new UploadHandlerRaw(bytes);
            upHandler.contentType = "application/json";
            request.uploadHandler = upHandler;
        }
        yield return request.SendWebRequest();
        
        if (request.isNetworkError || request.isHttpError)
        {
            onLoginFinish(true);
        }
        else
        {
            string token = request.downloadHandler.text;
            SignInResult result = JsonConvert.DeserializeObject<SignInResult>(token);
            PlayerPrefHandler.SaveString(PlayerPrefHandler.TOKEN, result.token);
            onLoginFinish(false);
        }
    }

    public static IEnumerator DoRegister(string username, string password, System.Action<bool> onRegisterFinish)
    {
        LoginRegister login = new LoginRegister();
        login.username = username;
        login.password = password;
        string data = JsonConvert.SerializeObject(login);
        Debug.Log(data);
        UnityWebRequest request = UnityWebRequest.Post(host + "/register", "");
        request.SetRequestHeader("Content-Type", "application/json");
        if (data != null)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);

            UploadHandlerRaw upHandler = new UploadHandlerRaw(bytes);
            upHandler.contentType = "application/json";
            request.uploadHandler = upHandler;
        }
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            onRegisterFinish(true);
        }
        else
        {
            onRegisterFinish(false);
        }
    }

    public static IEnumerator DoLogin3rd(string username, string user3rdid, string source, string token, string image, System.Action<bool> onFinish)
    {
        Login3rd user = new Login3rd();
        user.username = username;
        user.user3rdid = user3rdid;
        user.token = token;
        user.source = source;
        user.image = image;
        string data = JsonConvert.SerializeObject(user);
        UnityWebRequest request = UnityWebRequest.Post(host + "/login3rd", "");
        request.SetRequestHeader("Content-Type", "application/json");
        if (data != null)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);

            UploadHandlerRaw upHandler = new UploadHandlerRaw(bytes);
            upHandler.contentType = "application/json";
            request.uploadHandler = upHandler;
        }
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            onFinish(true);
        }
        else
        {
            string tk = request.downloadHandler.text;
            SignInResult result = JsonConvert.DeserializeObject<SignInResult>(tk);
            PlayerPrefHandler.SaveString(PlayerPrefHandler.TOKEN, result.token);
            onFinish(false);
        }
    }

    public static IEnumerator GetUserInfo(System.Action<PlayerInfo> onFinish)
    {
        Debug.Log(host+ "/get-info");
        UnityWebRequest request = UnityWebRequest.Get(host + "/get-info");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefHandler.LoadString(PlayerPrefHandler.TOKEN));
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            onFinish(null);
        }
        else
        {
            string info = request.downloadHandler.text;
            Debug.Log(info);
            PlayerInfo result = JsonConvert.DeserializeObject<PlayerInfo>(info);
            onFinish(result);
        }
    }

    public static IEnumerator GetRooms(System.Action<List<RoomInfo>> onFinish)
    {
        UnityWebRequest request = UnityWebRequest.Get(host + "/get-rooms");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefHandler.LoadString(PlayerPrefHandler.TOKEN));
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            onFinish(null);
        }
        else
        {
            string info = request.downloadHandler.text;
            Debug.Log(info);
            List<RoomInfo> result = JsonConvert.DeserializeObject<List<RoomInfo>>(info);
            onFinish(result);
        }
    }

    public static IEnumerator QuickJoin(System.Action<RoomInfo> onFinish)
    {
        UnityWebRequest request = UnityWebRequest.Get(host + "/quick-join");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefHandler.LoadString(PlayerPrefHandler.TOKEN));
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            onFinish(null);
        }
        else
        {
            string info = request.downloadHandler.text;
            Debug.Log(info);
            RoomInfo result = JsonConvert.DeserializeObject<RoomInfo>(info);
            onFinish(result);
        }
    }

    public static IEnumerator GetImageTexture(string url, System.Action<Texture2D> onFinish)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            onFinish(null);
        }
        else
        {
            // Get downloaded asset bundle
            onFinish(DownloadHandlerTexture.GetContent(request));
        }
    }

    public static IEnumerator RefreshToken(string token, System.Action<bool> onFinish)
    {
        UnityWebRequest request = UnityWebRequest.Get(host + "/refresh-token");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + token);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            onFinish(true);
        }
        else
        {
            string newToken = request.downloadHandler.text;
            SignInResult result = JsonConvert.DeserializeObject<SignInResult>(newToken);
            PlayerPrefHandler.SaveString(PlayerPrefHandler.TOKEN, result.token);
            onFinish(false);
        }
    }

    public static IEnumerator CheckIn(System.Action<bool, System.Int64> onFinish)
    {
        UnityWebRequest request = UnityWebRequest.Get(host + "/checkin");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefHandler.LoadString(PlayerPrefHandler.TOKEN));
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            onFinish(true, 0);
        }
        else
        {
            string reward = request.downloadHandler.text;
            onFinish(false, System.Int64.Parse(reward));
        }
    }
}
