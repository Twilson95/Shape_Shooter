using System;
using System.Reflection;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GoogleSignIn : MonoBehaviour
{
    public string Token;
    public string Error;
    public bool testBool = false;

    private object playGamesPlatformInstance;
    private MethodInfo requestServerSideAccessMethod;

    void Awake()
    {
        testBool = true;
        Debug.Log("GoogleSignInAwakes");

        CachePlayGamesPlatformApi();
        ActivatePlayGamesPlatformIfAvailable();

        LoginGooglePlayGames();
    }

    private void CachePlayGamesPlatformApi()
    {
        Type playGamesPlatformType = Type.GetType("GooglePlayGames.PlayGamesPlatform, GooglePlayGames");
        if (playGamesPlatformType == null)
        {
            Error = "Google Play Games plugin not found. Sign-in will use Social API only.";
            Debug.LogWarning(Error);
            return;
        }

        PropertyInfo instanceProperty = playGamesPlatformType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
        playGamesPlatformInstance = instanceProperty?.GetValue(null);
        requestServerSideAccessMethod = playGamesPlatformType.GetMethod("RequestServerSideAccess", new[] { typeof(bool), typeof(Action<string>) });
    }

    private void ActivatePlayGamesPlatformIfAvailable()
    {
        if (playGamesPlatformInstance == null)
        {
            return;
        }

        MethodInfo activateMethod = playGamesPlatformInstance.GetType().GetMethod("Activate", BindingFlags.Public | BindingFlags.Static);
        activateMethod?.Invoke(null, null);
        Debug.Log("GoogleSignIn Activated");
    }

    public void LoginGooglePlayGames()
    {
        Social.localUser.Authenticate(success =>
        {
            if (!success)
            {
                Error = "Failed to authenticate with Google Play Games.";
                Debug.LogError(Error);
                return;
            }

            Debug.Log("Login with Google Play Games successful.");
            RequestServerSideAccessCode();
        });
    }

    private void RequestServerSideAccessCode()
    {
        if (playGamesPlatformInstance == null || requestServerSideAccessMethod == null)
        {
            Error = "RequestServerSideAccess API not found on installed Google Play Games plugin.";
            Debug.LogWarning(Error);
            return;
        }

        requestServerSideAccessMethod.Invoke(playGamesPlatformInstance, new object[]
        {
            true,
            new Action<string>(code =>
            {
                Debug.Log("Authorization code: " + code);
                Token = code;
            })
        });
    }

    // sign in a returning player or create new player
    async Task SignInWithGooglePlayGamesAsync(string authCode)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(authCode);
            Debug.Log("SignIn is successful.");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    // updating a player from anonymous to a google play games account
    async Task LinkWithGooglePlayGamesAsync(string authCode)
    {
        try
        {
            await AuthenticationService.Instance.LinkWithGooglePlayGamesAsync(authCode);
            Debug.Log("Link is successful.");
        }
        catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
        {
            Debug.LogError("This user is already linked with another account. Log in instead.");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }
}
