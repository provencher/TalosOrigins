using UnityEngine;
using System.Collections;

public class PauseControl : MonoBehaviour {

    public GameObject pausePanel;
    public bool paused;

	// Use this for initialization
	void Start () {
        paused = false;
        pausePanel.SetActive(false);
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
            pausePanel.SetActive(true);
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
        pausePanel.SetActive(false);
    }

    public void GoToMainMenu(int number)
    {
        Application.LoadLevel(number);
        pausePanel.SetActive(false);
    }
}
