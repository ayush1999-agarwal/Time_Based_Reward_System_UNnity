using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneManager : MonoBehaviour
{
    public void ClaimReward() 
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("DailyReward");
    }

    public void GoBack()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Start Menu");
    }
   public void OnQuitClick()
    {
        Application.Quit();
    }
}
