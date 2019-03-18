using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoogleHandler : MonoBehaviour
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

    public string webClientId; 

    private GoogleSignInConfiguration configuration;

    // Start is called before the first frame update

    private void Awake()
    {
        webClientId = Setting.GetGoogleToken();
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestIdToken = true
        };
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        signin.onClick.AddListener(OnSignIn);
    }

    private void OnApplicationQuit()
    {
        signin.onClick.RemoveAllListeners();
    }

    public void OnSignIn()
    {
        loginErr.gameObject.SetActive(false);
        GameObject loading = Instantiate(loadingPanel, new Vector3(0, 0), Quaternion.identity);
        loading.transform.SetParent(canvas.transform, false);
        loading.name = "LoadingScreen";

        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        AddStatusText("Calling SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
          OnAuthenticationFinished);
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<System.Exception> enumerator =
                    task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error =
                            (GoogleSignIn.SignInException)enumerator.Current;
                    AddStatusText("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    AddStatusText("Got Unexpected Exception?!?" + task.Exception);
                }
            }
            OnLoginFinish(true);
        }
        else if (task.IsCanceled)
        {
            AddStatusText("Canceled");
            OnLoginFinish(true);
        }
        else
        {
            AddStatusText("Welcome: " + task.Result.DisplayName + "!");
            AddStatusText(task.Result.ImageUrl.OriginalString);
            StartCoroutine(ServiceClient.DoLogin3rd(task.Result.DisplayName,
                task.Result.UserId,
                "Google",
                task.Result.IdToken,
                task.Result.ImageUrl.OriginalString,
                OnLoginFinish));
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
