using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;

public class GoogleSignIn : MonoBehaviour
{
    public string Token;
    public string Error;
    public bool testBool = false;

    private object playGamesPlatformInstance;
    private MethodInfo authenticateMethod;
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
            Error = "Google Play Games plugin not found.";
            Debug.LogWarning(Error);
            return;
        }

        PropertyInfo instanceProperty = playGamesPlatformType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
        playGamesPlatformInstance = instanceProperty?.GetValue(null);

        authenticateMethod = playGamesPlatformType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(method =>
            {
                if (method.Name != "Authenticate")
                {
                    return false;
                }

                ParameterInfo[] parameters = method.GetParameters();
                return parameters.Length == 1 && typeof(Delegate).IsAssignableFrom(parameters[0].ParameterType);
            });

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
        if (playGamesPlatformInstance == null || authenticateMethod == null)
        {
            Error = "Authenticate API not found on installed Google Play Games plugin.";
            Debug.LogError(Error);
            return;
        }

        Type callbackType = authenticateMethod.GetParameters()[0].ParameterType;
        Delegate callbackDelegate = BuildAuthenticationCallback(callbackType);
        authenticateMethod.Invoke(playGamesPlatformInstance, new object[] { callbackDelegate });
    }

    private Delegate BuildAuthenticationCallback(Type callbackType)
    {
        Type[] genericArgs = callbackType.GenericTypeArguments;
        if (genericArgs.Length != 1)
        {
            throw new InvalidOperationException("Unexpected Authenticate callback signature.");
        }

        MethodInfo handler = GetType()
            .GetMethod(nameof(HandleAuthenticateResult), BindingFlags.Instance | BindingFlags.NonPublic)
            ?.MakeGenericMethod(genericArgs[0]);

        if (handler == null)
        {
            throw new InvalidOperationException("Unable to build Authenticate callback delegate.");
        }

        return Delegate.CreateDelegate(callbackType, this, handler);
    }

    private void HandleAuthenticateResult<T>(T authResult)
    {
        bool isSuccess = authResult is bool boolResult
            ? boolResult
            : string.Equals(authResult?.ToString(), "Success", StringComparison.OrdinalIgnoreCase);

        if (!isSuccess)
        {
            Error = "Failed to authenticate with Google Play Games. Status: " + authResult;
            Debug.LogError(Error);
            return;
        }

        Debug.Log("Login with Google Play Games successful.");
        RequestServerSideAccessCode();
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
