using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;
using System.Linq;

public class Achievements : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
 
    }

    
    public IEnumerator WaitAndUnlock(string achievementID)
    {
        yield return new WaitForSeconds(2f);
        UnlockAchievement(achievementID);
    }


    public void IncrementAchievement(string achievementID)
    {
        // load achievements
        Social.LoadAchievements(achievements =>
        {
            if (achievements == null)
            {
                Debug.Log("no achievements loaded");
                return;
            }
            // find the specified achievement
            var achievement = achievements.FirstOrDefault(a => a.id == achievementID);

            // check if the achievement is unlocked
            if (achievement == null || achievement.completed)
            {
                Debug.Log("incremental achievement already unlocked");
                return;
            }
            // increment the achievement
            PlayGamesPlatform.Instance.IncrementAchievement(
                achievementID,
                1,
                success =>
                {
                    // handle success or failure
                }
            );
        });
    }

    public void VerifyAchievement(string achievementID)
    {
        if (Social.localUser.authenticated)
        {
            // The local user is already authenticated
            Debug.Log("The local user is already authenticated.");
            UnlockAchievement(achievementID);
            return;
        }

        // Authenticate the local user
        Social.localUser.Authenticate(success => {
            if (success)
            {
                // Check which Weapons have been purchased and verify the achievement
                UnlockAchievement(achievementID);
            }
        });
    }


    public void UnlockAchievement(string achievementID)
    {
        // load achievements
        Social.LoadAchievements(achievements =>
        {
            if (achievements == null)
            {
                Debug.Log("no achievements loaded");
                return;
            }
            // find the specified achievement
            var achievement = achievements.FirstOrDefault(a => a.id == achievementID);

            // check if the achievement is unlocked
            if (achievement == null || achievement.completed)
            {
                Debug.Log("one off achievement already unlocked");
                return;
            }
            // unlock achievement (achievement ID "Cfjewijawiu_QA")
            Social.ReportProgress(achievementID, 100.0f, (bool success) =>
            {
                // handle success or failure
            }
            );
        });
    }


    public void ShowAchievements()
    {
        // show achievements UI
        Debug.Log("Show Achievements");
        Social.ShowAchievementsUI();
    }

}
