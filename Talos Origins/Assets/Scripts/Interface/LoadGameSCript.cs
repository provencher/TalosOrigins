using UnityEngine;
using System.Collections;

public class LoadGameSCript : MonoBehaviour {

    public void Reset(int reset)
    {
        PlayerPrefs.SetInt("resetGame", reset);
        
        if(reset == 1)
        {
            PlayerPrefs.SetInt("Total Orbs", 0);
            PlayerPrefs.SetInt("currentLevel", 1);
        }       

        Application.LoadLevel(1);
    }
}
