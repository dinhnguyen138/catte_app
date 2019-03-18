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
    public Button registerShowBtn;
    public Button registerBtn;
    public GameObject loadingPanel;
    public Canvas canvas;
    Text loginErr;
    Text registerErr;
    // Start is called before the first frame update
    void Start()
    {
        loginBtn.onClick.AddListener(handleLogin);
        registerBtn.onClick.AddListener(handleRegister);
        registerShowBtn.onClick.AddListener(openRegister);
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        loginErr = GameObject.Find("Error").GetComponent<Text>();
        loginErr.gameObject.SetActive(false);
        registerErr = GameObject.Find("Error1").GetComponent<Text>();
        registerErr.gameObject.SetActive(false);
        string token = PlayerPrefHandler.LoadString(PlayerPrefHandler.TOKEN);
        Debug.Log(token);
        if (token != "")
        {
            loginErr.gameObject.SetActive(false);
            GameObject loading = Instantiate(loadingPanel, new Vector3(0, 0), Quaternion.identity);
            loading.transform.SetParent(canvas.transform, false);
            loading.name = "LoadingScreen";
            StartCoroutine(ServiceClient.RefreshToken(token, OnRefreshFinish));
        }
    }

    private void OnApplicationQuit()
    {
        loginBtn.onClick.RemoveAllListeners();
        registerShowBtn.onClick.RemoveAllListeners();
        registerBtn.onClick.RemoveAllListeners();
    }

    private void handleLogin() {
        loginErr.gameObject.SetActive(false);
        GameObject loading = Instantiate(loadingPanel, new Vector3(0, 0), Quaternion.identity);
        loading.transform.SetParent(canvas.transform, false);
        loading.name = "LoadingScreen";
        GameObject usernameField = GameObject.Find("Username");
        GameObject passwordField = GameObject.Find("Password");
        string username = usernameField.GetComponent<InputField>().text;
        string password = passwordField.GetComponent<InputField>().text;
        StartCoroutine(ServiceClient.DoLogin(username, password, OnLoginFinish));
    }

    void OnLoginFinish(bool error)
    {
        GameObject loading = GameObject.Find("LoadingScreen");
        GameObject.Destroy(loading);
        if (error == true)
        {
            loginErr.gameObject.SetActive(true);
        }
        else
        {
            SceneManager.LoadSceneAsync("RoomScene");
        }
    }

    private void openRegister()
    {
        RegisterController controller = GameObject.Find("RegisterPage").GetComponent<RegisterController>();
        controller.show = true;
    }

    private void handleRegister() {
        registerErr.gameObject.SetActive(false);
        GameObject loading = Instantiate(loadingPanel, new Vector3(0, 0), Quaternion.identity);
        loading.transform.SetParent(canvas.transform, false);
        loading.name = "LoadingScreen";
        GameObject usernameField = GameObject.Find("Username1");
        GameObject passwordField = GameObject.Find("Password1");
        string username = usernameField.GetComponent<InputField>().text;
        string password = passwordField.GetComponent<InputField>().text;
        StartCoroutine(ServiceClient.DoRegister(username, password, OnRegisterFinish));
    }

    void OnRegisterFinish(bool error)
    {
        GameObject loading = GameObject.Find("LoadingScreen");
        GameObject.Destroy(loading);
        if (error == true)
        {
            registerErr.gameObject.SetActive(true);
        }
        else
        {
            RegisterController registerController = GameObject.Find("RegisterPage").GetComponent<RegisterController>();
            registerController.show = false;
        }
    }

    void OnRefreshFinish(bool hasError)
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
}
