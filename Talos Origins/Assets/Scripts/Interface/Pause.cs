using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Pause : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        /*
        if (Input.GetKeyDown("return"))
        {
            Debug.Log("Enter");

            if (paused)
            {
                GetComponent<Text>().text = "";
                Time.timeScale = 1;
                resumeGame();
            }
            else
            {
                GetComponent<Text>().text = "PAUSE";
                Time.timeScale = 0.3f;
                pauseGame();
            }
        }       
        */
	}

    void pauseGame()
    {
        Object[] objects = FindObjectsOfType(typeof(GameObject));
        foreach (GameObject go in objects)
        {
            go.SendMessage("OnPauseGame", SendMessageOptions.DontRequireReceiver);
        }
        OnPauseGame();
    }

    public void resumeGame()
    {
        Object[] objects = FindObjectsOfType(typeof(GameObject));
        foreach (GameObject go in objects)
        {
            go.SendMessage("OnPauseGame", SendMessageOptions.DontRequireReceiver);
        }
    }

    protected bool paused;


    void OnPauseGame()
    {
        paused = true;
    }


    void OnResumeGame()
    {

        paused = false;
    }

}
