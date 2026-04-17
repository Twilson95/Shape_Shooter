using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class Achievements : MonoBehaviour
{
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
        if (IsAuthenticated())
        {
            UnlockAchievement(achievementID);
            return;
        }

        PlayGamesPlatform.Instance.Authenticate(signInStatus =>
        {
            if (signInStatus == SignInStatus.Success)
            {
                UnlockAchievement(achievementID);
            }
            else
            {
                Debug.LogWarning("User authentication failed while verifying achievement. Status: " + signInStatus);
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
        if (!IsAuthenticated())
        {
            Debug.LogWarning("ShowAchievements skipped. User is not authenticated.");
            return;
        }

        PlayGamesPlatform.Instance.ShowAchievementsUI();
    }

    bool IsAuthenticated()
    {
        return PlayGamesPlatform.Instance != null && PlayGamesPlatform.Instance.IsAuthenticated();
    }
}
