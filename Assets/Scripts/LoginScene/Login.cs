using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public Button loginBtn;
    public Button registerBtn;
    // Start is called before the first frame update
    void Start()
    {
        loginBtn.onClick.AddListener(handleLogin);
        registerBtn.onClick.AddListener(handleRegister);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void handleLogin() {
        GameObject usernameField = GameObject.Find("Username");
        GameObject passwordField = GameObject.Find("Password");
        string username = usernameField.GetComponent<InputField>().text;
        string password = passwordField.GetComponent<InputField>().text;
        StartCoroutine(DoLogin(username, password));
    }

    private void handleRegister() {
        GameObject usernameField = GameObject.Find("Username1");
        GameObject passwordField = GameObject.Find("Password1");
        string username = usernameField.GetComponent<InputField>().text;
        string password = passwordField.GetComponent<InputField>().text;
        StartCoroutine(DoRegister(username, password));
    }

    IEnumerator DoLogin(string username, string password)
    {
        string jsonData = "";
        jsonData = "{Username:\"" + username + "\",Password:\"" + password + "\"}";
        UnityWebRequest request = UnityWebRequest.Post("localhost:8080/login", jsonData);
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            string token = request.downloadHandler.text;
            SignInResult result = JsonConvert.DeserializeObject<SignInResult>(token);
            PlayerPrefHandler.SaveString(PlayerPrefHandler.TOKEN, result.token);
            SceneManager.LoadSceneAsync("RoomScene");
        }
    }

    IEnumerator DoRegister(string username, string password)
    {
        string jsonData = "";
        jsonData = "{Username:\"" + username + "\",Password:\"" + password + "\"}";
        UnityWebRequest request = UnityWebRequest.Post("localhost:8080/register", jsonData);
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {

        }

    }
}
