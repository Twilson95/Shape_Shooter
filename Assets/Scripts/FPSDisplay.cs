using UnityEngine;
using UnityEngine.UI;
using TMPro;
 
public class FPSDisplay : MonoBehaviour
{
    public int avgFrameRate;
    public TextMeshProUGUI display_Text;
    // private float timer = 0f;
    public float calculateRate = 1f;

    private int frameCount = 0;
    private float elapsedTime = 0f;
 
    public void Update()
    {
        // timer += Time.deltaTime;
        // if(timer > calculateRate)
        // {
        //     CalculateFPS();
        //     timer = 0f;
        // }  


        frameCount++;
        elapsedTime += Time.deltaTime;
        if(elapsedTime >= calculateRate)
        {
            CalculateFPS();
        }
    }

    // private void CalculateFPS()
    // {
    //     float current = 0;
    //     current = (int)(1f / Time.unscaledDeltaTime);
    //     avgFrameRate = (int)current;
    //     display_Text.text = avgFrameRate.ToString() + " FPS";
    // }

    private void CalculateFPS()
    {
        float averageFPS = frameCount / elapsedTime;
        int avgFrameRate = (int) averageFPS;
        display_Text.text = avgFrameRate.ToString() + " FPS";
        frameCount = 0;
        elapsedTime = 0f;
    }
}