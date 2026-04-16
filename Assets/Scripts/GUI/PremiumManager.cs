using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PremiumManager : MonoBehaviour
{
    public TextMeshProUGUI premiumText;
    public int premium = 0;

    public void UpdatePremium(int value)
    {
        premium += value;
        // premiumText.text = "Premium: " + premium.ToString();
        premiumText.text = premium.ToString();
    }

    public void ResetPremium()
    {
        premium = 0;
        // premiumText.text = "Premium: " + premium.ToString();
        premiumText.text = premium.ToString(); 
    }

    public void SetSaveData(GameData gameData)
    {
        UpdatePremium(gameData.premiumScore);
    }
    
}