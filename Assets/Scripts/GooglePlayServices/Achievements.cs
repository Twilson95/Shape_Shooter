using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class Achievements : MonoBehaviour
{
    private bool isAuthenticating;
    private readonly List<System.Action<bool>> pendingAuthCallbacks = new List<System.Action<bool>>();

    public IEnumerator WaitAndUnlock(string achievementID)
    {
        yield return new WaitForSeconds(2f);
        UnlockAchievement(achievementID);
    }

    public void IncrementAchievement(string achievementID)
    {
        if (!IsAuthenticated())
        {
            Debug.LogWarning("IncrementAchievement skipped. User is not authenticated.");
            return;
        }

        PlayGamesPlatform.Instance.IncrementAchievement(achievementID, 1, success =>
        {
            if (!success)
            {
                Debug.LogWarning("IncrementAchievement failed for: " + achievementID);
            }
        });
    }

    public void VerifyAchievement(string achievementID)
    {
        EnsureAuthenticated(isAuthenticated =>
        {
            if (isAuthenticated)
            {
                UnlockAchievement(achievementID);
            }
        });
    }

    public void UnlockAchievement(string achievementID)
    {
        if (!IsAuthenticated())
        {
            Debug.LogWarning("UnlockAchievement skipped. User is not authenticated.");
            return;
        }

        PlayGamesPlatform.Instance.ReportProgress(achievementID, 100.0f, success =>
        {
            if (!success)
            {
                Debug.LogWarning("UnlockAchievement failed for: " + achievementID);
            }
        });
    }

    public void ShowAchievements()
    {
        EnsureAuthenticated(isAuthenticated =>
        {
            if (!isAuthenticated)
            {
                Debug.LogWarning("ShowAchievements skipped. User is not authenticated.");
                return;
            }

            PlayGamesPlatform.Instance.ShowAchievementsUI();
        });
    }

    bool IsAuthenticated()
    {
        return PlayGamesPlatform.Instance != null && PlayGamesPlatform.Instance.IsAuthenticated();
    }

    private void EnsureAuthenticated(System.Action<bool> onComplete)
    {
        if (IsAuthenticated())
        {
            onComplete?.Invoke(true);
            return;
        }

        if (PlayGamesPlatform.Instance == null)
        {
            Debug.LogWarning("Google Play Games platform is not initialized.");
            onComplete?.Invoke(false);
            return;
        }

        if (isAuthenticating)
        {
            if (onComplete != null)
            {
                pendingAuthCallbacks.Add(onComplete);
            }
            return;
        }

        if (onComplete != null)
        {
            pendingAuthCallbacks.Add(onComplete);
        }

        isAuthenticating = true;
        PlayGamesPlatform.Instance.Authenticate(signInStatus =>
        {
            isAuthenticating = false;
            bool success = signInStatus == SignInStatus.Success;
            if (!success)
            {
                Debug.LogWarning("User authentication failed. Status: " + signInStatus);
            }

            for (int i = 0; i < pendingAuthCallbacks.Count; i++)
            {
                pendingAuthCallbacks[i]?.Invoke(success);
            }

            pendingAuthCallbacks.Clear();
        });
    }
}
