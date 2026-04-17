using System.Collections;
using System.Linq;
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
        Social.LoadAchievements(achievements =>
        {
            if (achievements == null)
            {
                Debug.LogWarning("Achievements could not be loaded.");
                return;
            }

            var achievement = achievements.FirstOrDefault(a => a.id == achievementID);
            if (achievement == null)
            {
                Debug.LogWarning("Achievement not found: " + achievementID);
                return;
            }

            if (achievement.completed)
            {
                Debug.Log("Increment skipped. Achievement already completed: " + achievementID);
                return;
            }

            // Cross-platform API: increase progress by 1 percentage point.
            // For step-based achievements, configure total steps accordingly in Play Console.
            double nextProgress = Mathf.Clamp((float)achievement.percentCompleted + 1f, 0f, 100f);
            Social.ReportProgress(achievementID, nextProgress, success =>
            {
                if (!success)
                {
                    Debug.LogWarning("IncrementAchievement failed for: " + achievementID);
                }
            });
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
        Social.LoadAchievements(achievements =>
        {
            if (achievements == null)
            {
                Debug.LogWarning("Achievements could not be loaded.");
                return;
            }

            var achievement = achievements.FirstOrDefault(a => a.id == achievementID);
            if (achievement == null)
            {
                Debug.LogWarning("Achievement not found: " + achievementID);
                return;
            }

            if (achievement.completed)
            {
                Debug.Log("Unlock skipped. Achievement already completed: " + achievementID);
                return;
            }

            Social.ReportProgress(achievementID, 100.0f, success =>
            {
                if (!success)
                {
                    Debug.LogWarning("UnlockAchievement failed for: " + achievementID);
                }
            });
        });
    }

    public void ShowAchievements()
    {
        Social.ShowAchievementsUI();
    }
}
