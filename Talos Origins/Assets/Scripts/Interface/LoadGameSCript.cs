using UnityEngine;
using System.Collections;

public class LoadGameSCript : MonoBehaviour {

    public void Reset(int reset)
    {
        //PlayerPrefs.SetInt("resetGame", reset);
        
        if(reset == 1)
        {
            PlayerPrefs.SetInt("Total Orbs", 200);
            //PlayerPrefs.SetInt("resetGame", 0);
            PlayerPrefs.SetInt("Grapple", 0);
            PlayerPrefs.SetInt("Big Bullets", 0);
            PlayerPrefs.SetInt("Rate of Fire", 0);
            PlayerPrefs.SetInt("Spray Bullets", 0);
            PlayerPrefs.SetInt("Jump", 0);
            PlayerPrefs.SetInt("Breadcrumbs", 0);
            PlayerPrefs.SetInt("Health Pack", 0);
            PlayerPrefs.SetInt("Shield", 0);
            PlayerPrefs.SetInt("Portal Distance", 0);
            PlayerPrefs.SetInt("Portal Cooldown", 0);            
            PlayerPrefs.SetInt("currentLevel", 1);            
        }       
        else
        {
            PlayerPrefs.SetInt("Total Orbs", 0);
        }

        Application.LoadLevel(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
