using UnityEngine;
using System.Collections;

public class audioPrefabController : MonoBehaviour {

    float timeToLive = 2.0f;
    public float delay = -1;

    bool triggered = false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if(delay < 0 && !triggered)
        {
            triggered = true;
            GetComponent<AudioSource>().Play();
        }

        if (delay > 0)
        {
            delay -= Time.deltaTime;
        }
        else
        {
            timeToLive -= Time.deltaTime;

            if (timeToLive < 0)
            {
                Destroy(gameObject);
            }
        }        
	}
}
