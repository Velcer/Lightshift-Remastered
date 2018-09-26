using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour {

    public static LoginUI loginUI;

    PlayerIONetwork network => PlayerIONetwork.net;

    [Header("Login Form")]
    public GameObject loginForm;
    public InputField tb_email;
    public InputField tb_password;
    public Toggle t_rememberMe;

    [Header("Register Form")]
    public GameObject registerForm;
    public InputField tb_regEmail;
    public InputField tb_regPassword;
    public InputField tb_regConfirmPassword;

    [Header("Forgot Password Form")]
    public GameObject forgotForm;
    public InputField tb_forgotPassword;

    [Header("Status")]
    public Text errorText;
    public Text[] statusText;

    void Start()
    {
        loginUI = this;

        if (PlayerPrefs.GetString("remember", "").ToLower() == "true")
        {
            tb_email.text = PlayerPrefs.GetString("email", "");
            tb_password.text = PlayerPrefs.GetString("password", "");
            t_rememberMe.isOn = true;
        }

        if (tb_email.text == "")
            tb_email.ActivateInputField();
        else if (tb_password.text == "") tb_password.ActivateInputField();
    }

    private string currentForm = "login";
    void Update()
    {
        #region Handle Textbox Focus
        if (Input.GetKeyDown("tab") || Input.GetKeyDown("down") || Input.GetKeyDown("right"))
        {
            switch (currentForm)
            {
                case "login":
                    if (tb_email.isFocused)
                    {
                        tb_password.ActivateInputField();
                    }
                    else
                        tb_email.ActivateInputField();
                    break;
                case "register":
                    if (tb_regEmail.isFocused)
                    {
                        tb_regPassword.ActivateInputField();
                    }
                    else if (tb_regPassword.isFocused)
                    {
                        tb_regConfirmPassword.ActivateInputField();
                    }
                    else tb_regEmail.ActivateInputField();
                    break;
            }
        }
        if (Input.GetKeyDown("return") || Input.GetKeyDown("enter"))
        {
            if (currentForm == "login")
            {
                Connect();
            }
            if (currentForm == "register")
            {
                Register();
            }
            if (currentForm == "forgot")
            {
                forgotPassword();
            }
        }
        #endregion
    }

    public void Connect()
    {
        if (tb_email.text == "")
        {
            showError("Email field cannot be empty.");
            return;
        }

        if (tb_password.text == "")
        {
            showError("Password field cannot be empty.");
            return;
        }

        loginForm.SetActive(false);
        network.Authenticate(tb_email.text, tb_password.text, loginCallback);
    }

    private void loginCallback(bool successful, string error)
    {
        if (!successful)
        {
            showError($"{error}");
            loginForm.SetActive(true);
        }
        else ConnectedSuccessfully("login");
    }

    public void Register()
    {
        if (tb_regPassword.text != tb_regConfirmPassword.text)
        {
            showError("Passwords do not match !");
            tb_regConfirmPassword.text = "";
            tb_regPassword.text = "";
            return;
        }

        if (tb_regPassword.text.Length < 6)
        {
            showError("Password too small! Please use 6 or more characters.");
            tb_regConfirmPassword.text = "";
            tb_regPassword.text = "";
            return;
        }

        registerForm.SetActive(false);
        network.Register(tb_regEmail.text, tb_regPassword.text, registerCallback);
    }

    private void registerCallback(bool successful, string error)
    {
        if (!successful)
        {
            registerForm.SetActive(true);
            showError($"{error}");
        }
        else ConnectedSuccessfully("register");
    }

    public void forgotPassword()
    {
        network.ForgotPassword(tb_forgotPassword.text, delegate 
        {
            showError("If there is an email registered for  " + tb_forgotPassword.text + " you should recieve an email shortly.");
            showLoginForm();
        });
    }

    public void ConnectedSuccessfully(string type)
    {
        if (type == "login" && t_rememberMe.isOn)
        {
            PlayerPrefs.SetString("email", tb_email.text);
            PlayerPrefs.SetString("password", tb_password.text);
        }
        else PlayerPrefs.SetString("email", tb_regEmail.text);

        PlayerPrefs.SetString("remember", t_rememberMe.isOn.ToString());
        if (!t_rememberMe.isOn)
        {
            PlayerPrefs.SetString("email", "");
            PlayerPrefs.SetString("password", "");
        }
        PlayerPrefs.Save();
    }

    #region Handle Forms
    public void showForgotForm()
    {
        currentForm = "forgot";
        forgotForm.SetActive(true);
        loginForm.SetActive(false);
        tb_forgotPassword.ActivateInputField();
    }

    public void showRegisterForm()
    {
        currentForm = "register";
        registerForm.SetActive(true);
        loginForm.SetActive(false);
        tb_regEmail.ActivateInputField();
    }

    public void showLoginForm()
    {
        currentForm = "login";
        registerForm.SetActive(false);
        forgotForm.SetActive(false);
        loginForm.SetActive(true);

        if (tb_email.text == "")
            tb_email.ActivateInputField();
        else tb_password.ActivateInputField();
    }

    public void showError(string error)
    {
        errorText.CrossFadeAlpha(1, 0, false);
        errorText.text = error;
        errorText.CrossFadeAlpha(0, 5, false);
    }

    public void toggleUsernameScreen(bool type)
    {
        //currentForm = "username";
        //usernameForm.SetActive(type);
        //tb_username.ActivateInputField();
    }
    #endregion
}
