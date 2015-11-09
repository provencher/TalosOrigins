using UnityEngine;
using System.Collections;

public class audioPrefabController : MonoBehaviour {

    float timeToLive = 0.3f;
    public float delay = -1;

	// Use this for initialization
	void Start () {
        GetComponent<AudioSource>().Play();
	}
	
	// Update is called once per frame
	void Update () {
        if(delay > 0)
        {
            timeToLive += delay;
            delay = -1;
        }

        timeToLive -= Time.deltaTime;

        if(timeToLive < 0)
        {
            Destroy(gameObject);
        }
	}
}
