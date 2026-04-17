using System.Collections;
using GooglePlayGames;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Achievements : MonoBehaviour
{
    public IEnumerator WaitAndUnlock(string achievementID)
    {
        yield return new WaitForSeconds(2f);
        UnlockAchievement(achievementID);
    }

    public void IncrementAchievement(string achievementID)
    {
        if (!Social.localUser.authenticated)
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
        if (Social.localUser.authenticated)
        {
            UnlockAchievement(achievementID);
            return;
        }

        Social.localUser.Authenticate(success =>
        {
            if (success)
            {
                UnlockAchievement(achievementID);
            }
            else
            {
                Debug.LogWarning("User authentication failed while verifying achievement.");
            }
        });
    }

    public void UnlockAchievement(string achievementID)
    {
        if (!Social.localUser.authenticated)
        {
            Debug.LogWarning("UnlockAchievement skipped. User is not authenticated.");
            return;
        }

        Social.ReportProgress(achievementID, 100.0f, success =>
        {
            if (!success)
            {
                Debug.LogWarning("UnlockAchievement failed for: " + achievementID);
            }
        });
    }

    public void ShowAchievements()
    {
        if (!Social.localUser.authenticated)
        {
            Debug.LogWarning("ShowAchievements skipped. User is not authenticated.");
            return;
        }

        Social.ShowAchievementsUI();
    }
}
