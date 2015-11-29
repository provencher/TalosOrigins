using UnityEngine;
using System.Collections;

public class PauseControl : MonoBehaviour {

    public bool paused;

	// Use this for initialization
	void Start () {
        paused = false;

	}
	
	// Update is called once per frame
	void Update () {
        PauseGame();

	}

    void PauseGame()
    {
        if (Input.GetButtonDown("Pause"))
        {
            paused = true;
        }

        if (paused)
        {
            Time.timeScale = 0;
        }

        if (!paused)
        {
            Time.timeScale = 1;
        }
    }

    public void ResumeGame()
    {
        paused = false;
    }

    public void GoToMainMenu(int number)
    {
        Application.LoadLevel(number);
    }
}
