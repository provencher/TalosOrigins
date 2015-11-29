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
            if(paused)
            {
                paused = false;              
            }
            else
            {
                paused = true;                
            }           
        }
        pausePanel.SetActive(paused);
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
        Cursor.visible = true;
        Application.LoadLevel(number);
        pausePanel.SetActive(false);
    }
}
