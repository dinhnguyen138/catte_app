using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public GameObject usernameField;
    public GameObject passwordField;
    public Button loginBtn;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        string username = usernameField.GetComponent<InputField>().text;
        string password = passwordField.GetComponent<InputField>().text;

        loginBtn.onClick.AddListener(handleLogin);
    }

    private void handleLogin() {
        SceneManager.LoadSceneAsync("GameScene");
    }
}
