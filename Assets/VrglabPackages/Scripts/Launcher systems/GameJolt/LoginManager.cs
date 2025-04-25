using GameJolt.API;
using GameJolt.API.Objects;
using GameJolt.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginManager : Instancable<LoginManager>
{
    public bool UseGJUI = true;

    public bool SignedIn { get; internal set; }
    public User ActiveUser { get; internal set; }

    void Start()
    {
        if (GameJoltAPI.Instance.HasSignedInUser)
        {
            ActiveUser = GameJoltAPI.Instance.CurrentUser;
            SignedIn = true;
        }
        else
        {
            if (UseGJUI)
            {
                GameJoltUI.Instance.ShowSignIn((bool signInSuccess) => {
                    if (signInSuccess)
                    {
                        ActiveUser = GameJoltAPI.Instance.CurrentUser;
                        SignedIn = true;
                    }
                });
            }
        }

    }

    /// <summary>
    /// Log's a user into GameJolt
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="userToken">Password</param>
    /// <returns>True if we logged in successfully</returns>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public bool Login(string username, string userToken)
    {
        bool SignedIn = false;
        ActiveUser = new GameJolt.API.Objects.User(username, userToken);
        ActiveUser.SignIn((bool signInSuccess) =>
        {
            SignedIn = signInSuccess;
        });
        this.SignedIn = SignedIn;
        return SignedIn;
    }

    /// <summary>
    /// If we have a user logged in we log them out
    /// </summary>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void LogOut()
    {
        if (SignedIn)
        {
            ActiveUser.SignOut();
            SignedIn = false;
        }
    }
}