using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashController: MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Additive);
    }
}

