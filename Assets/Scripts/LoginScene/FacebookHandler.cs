using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FacebookHandler : MonoBehaviour
{
    public Text statusText;
    public Button signin;
    public GameObject loadingPanel;
    public Canvas canvas;
    string token;
    string id;
    string username;
    string image;
    public Text loginErr;
    // Start is called before the first frame update
    void Awake()
    {
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        signin.onClick.AddListener(OnSignIn);
    }

    private void Start()
    {
        loginErr.gameObject.SetActive(false);
    }

    private void OnApplicationQuit()
    {
        signin.onClick.RemoveAllListeners();
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    private void OnSignIn()
    {
        loginErr.gameObject.SetActive(false);
        GameObject loading = Instantiate(loadingPanel, new Vector3(0, 0), Quaternion.identity);
        loading.transform.SetParent(canvas.transform, false);
        loading.name = "LoadingScreen";
        var perms = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            token = aToken.TokenString;
            // Print current access token's granted permissions
            FB.API("me?fields=id,name,email", HttpMethod.GET, UserCallBack);
            FB.API("/me/picture?redirect=false", HttpMethod.GET, ProfileCallBack);
        }
        else
        {
            Debug.Log("User cancelled login");
            OnLoginFinish(true);
        }
    }

    void UserCallBack(IResult result)
    {
        if (result.Error == null)
        {
            if (result.ResultDictionary["id"] != null)
            {
                id = result.ResultDictionary["id"] as string;
            }
            if (result.ResultDictionary["name"] != null)
            {
                username = result.ResultDictionary["name"] as string;
            }

            if (id != "" && username != "" && image != "")
            {
                StartCoroutine(ServiceClient.DoLogin3rd(username, id, "Facebook", token, image, OnLoginFinish));
            }
        }
        else
        {
            OnLoginFinish(true);
        }
    }

    void ProfileCallBack(IResult result)
    {
        if (result.Error == null)
        {
            if (result.ResultDictionary["data"] != null)
            {
                IDictionary data = result.ResultDictionary["data"] as IDictionary;
                image = data["url"] as string;
            }
            if (id != "" && username != "" && image != "")
            {
                StartCoroutine(ServiceClient.DoLogin3rd(username, id, "Facebook", token, image, OnLoginFinish));
            }
        }
        else
        {
            OnLoginFinish(true);
        }
    }

    void OnLoginFinish(bool hasError)
    {
        GameObject loading = GameObject.Find("LoadingScreen");
        GameObject.Destroy(loading);
        if (hasError == true)
        {
            loginErr.gameObject.SetActive(true);
        }
        else
        {
            SceneManager.LoadSceneAsync("RoomScene");
        }
    }

    private List<string> messages = new List<string>();
    void AddStatusText(string text)
    {
        if (messages.Count == 5)
        {
            messages.RemoveAt(0);
        }
        messages.Add(text);
        string txt = "";
        foreach (string s in messages)
        {
            txt += "\n" + s;
        }
        statusText.text = txt;
    }
}
