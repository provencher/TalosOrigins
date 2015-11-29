using UnityEngine;
using System.Collections;

public class ContinueScript : MonoBehaviour {

    public void LoadScene(int reset)
    {
        PlayerPrefs.SetInt("resetGame", reset);
        Application.LoadLevel(0);
    }
}
