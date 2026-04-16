using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Achievements : MonoBehaviour
{
    private object playGamesPlatformInstance;
    private MethodInfo incrementAchievementMethod;

    void Start()
    {
        CachePlayGamesPlatformApi();
    }

    private void CachePlayGamesPlatformApi()
    {
        Type playGamesPlatformType = Type.GetType("GooglePlayGames.PlayGamesPlatform, GooglePlayGames");
        if (playGamesPlatformType == null)
        {
            return;
        }

        PropertyInfo instanceProperty = playGamesPlatformType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
        playGamesPlatformInstance = instanceProperty?.GetValue(null);
        incrementAchievementMethod = playGamesPlatformType.GetMethod("IncrementAchievement", new[] { typeof(string), typeof(int), typeof(System.Action<bool>) });
    }

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
                Debug.Log("no achievements loaded");
                return;
            }

            var achievement = achievements.FirstOrDefault(a => a.id == achievementID);
            if (achievement == null || achievement.completed)
            {
                Debug.Log("incremental achievement already unlocked");
                return;
            }

            if (playGamesPlatformInstance != null && incrementAchievementMethod != null)
            {
                incrementAchievementMethod.Invoke(playGamesPlatformInstance, new object[]
                {
                    achievementID,
                    1,
                    new System.Action<bool>(_ => { })
                });
                return;
            }

            Debug.LogWarning("IncrementAchievement API is unavailable. Falling back to full unlock progress.");
            Social.ReportProgress(achievementID, 100.0f, _ => { });
        });
    }

    public void VerifyAchievement(string achievementID)
    {
        if (Social.localUser.authenticated)
        {
            Debug.Log("The local user is already authenticated.");
            UnlockAchievement(achievementID);
            return;
        }

        Social.localUser.Authenticate(success =>
        {
            if (success)
            {
                UnlockAchievement(achievementID);
            }
        });
    }

    public void UnlockAchievement(string achievementID)
    {
        Social.LoadAchievements(achievements =>
        {
            if (achievements == null)
            {
                Debug.Log("no achievements loaded");
                return;
            }

            var achievement = achievements.FirstOrDefault(a => a.id == achievementID);
            if (achievement == null || achievement.completed)
            {
                Debug.Log("one off achievement already unlocked");
                return;
            }

            Social.ReportProgress(achievementID, 100.0f, _ => { });
        });
    }

    public void ShowAchievements()
    {
        Debug.Log("Show Achievements");
        Social.ShowAchievementsUI();
    }
}
