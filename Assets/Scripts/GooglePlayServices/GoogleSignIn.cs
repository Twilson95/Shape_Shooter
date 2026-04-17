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

        if (!IsPlayGamesAuthenticationSupportedOnCurrentPlatform())
        {
            Debug.LogWarning("Google Play Games authentication is only supported on Android device builds. Editor/PC uses DummyClient and will not sign in.");
            return;
        }

        LoginGooglePlayGames();
    }

    private void CachePlayGamesPlatformApi()
    {
        Type playGamesPlatformType = ResolvePlayGamesPlatformType();
        if (playGamesPlatformType == null)
        {
            Error = "Google Play Games plugin not found.";
            Debug.LogWarning(Error);
            return;
        }

        PropertyInfo instanceProperty = playGamesPlatformType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
        playGamesPlatformInstance = instanceProperty?.GetValue(null);

        authenticateMethod = playGamesPlatformType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .FirstOrDefault(method =>
            {
                if (method.Name != "Authenticate")
                {
                    return false;
                }

                ParameterInfo[] parameters = method.GetParameters();
                return parameters.Length > 0 && typeof(Delegate).IsAssignableFrom(parameters[0].ParameterType);
            });

        requestServerSideAccessMethod = playGamesPlatformType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .FirstOrDefault(method =>
            {
                if (method.Name != "RequestServerSideAccess")
                {
                    return false;
                }

                ParameterInfo[] parameters = method.GetParameters();
                return parameters.Length > 1
                    && parameters[0].ParameterType == typeof(bool)
                    && typeof(Delegate).IsAssignableFrom(parameters[1].ParameterType);
            });
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
        object[] arguments = BuildMethodArguments(authenticateMethod, callbackDelegate);
        object target = authenticateMethod.IsStatic ? null : playGamesPlatformInstance;
        authenticateMethod.Invoke(target, arguments);
    }

    private object[] BuildMethodArguments(MethodInfo method, Delegate firstArgument)
    {
        ParameterInfo[] parameters = method.GetParameters();
        object[] arguments = new object[parameters.Length];
        arguments[0] = firstArgument;

        for (int i = 1; i < parameters.Length; i++)
        {
            ParameterInfo parameter = parameters[i];
            if (parameter.HasDefaultValue)
            {
                arguments[i] = parameter.DefaultValue;
                continue;
            }

            Type parameterType = parameter.ParameterType;
            arguments[i] = parameterType.IsValueType ? Activator.CreateInstance(parameterType) : null;
        }

        return arguments;
    }

    private Type ResolvePlayGamesPlatformType()
    {
        string[] possibleTypeNames =
        {
            "GooglePlayGames.PlayGamesPlatform",
            "Google.Play.Games.PlayGamesPlatform"
        };

        foreach (string typeName in possibleTypeNames)
        {
            Type resolvedType = Type.GetType(typeName);
            if (resolvedType != null)
            {
                return resolvedType;
            }
        }

        Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assembly in loadedAssemblies)
        {
            foreach (string typeName in possibleTypeNames)
            {
                Type resolvedType = assembly.GetType(typeName);
                if (resolvedType != null)
                {
                    return resolvedType;
                }
            }
        }

        return null;
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
        string statusText = authResult?.ToString() ?? "Unknown";
        bool isSuccess = authResult is bool boolResult
            ? boolResult
            : string.Equals(statusText, "Success", StringComparison.OrdinalIgnoreCase);

        if (!isSuccess)
        {
            if (string.Equals(statusText, "Canceled", StringComparison.OrdinalIgnoreCase))
            {
                Debug.LogWarning("Google Play Games sign-in was canceled. In Unity Editor/PC this is expected because DummyClient is used. Test on an Android device with configured credentials.");
                return;
            }

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
        
        Action<string> serverAuthCodeCallback = code =>
        {
            Debug.Log("Authorization code: " + code);
            Token = code;
        };

        object[] arguments = BuildMethodArguments(
            requestServerSideAccessMethod,
            serverAuthCodeCallback
        );
        arguments[0] = true;

        object target = requestServerSideAccessMethod.IsStatic ? null : playGamesPlatformInstance;
        requestServerSideAccessMethod.Invoke(target, arguments);
    }

    private bool IsPlayGamesAuthenticationSupportedOnCurrentPlatform()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return true;
#else
        return false;
#endif
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
        catch (Exception ex)
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
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}
