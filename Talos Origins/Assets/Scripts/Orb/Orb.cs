using UnityEngine;
using System.Collections;

public class Orb : MonoBehaviour {

    float timeToLive;
    float lifeTime;

	// Use this for initialization
	void Start () {
        lifeTime = Random.Range(1.5f,3.0f);
        timeToLive = lifeTime;
	}
	
	// Update is called once per frame
	void Update () {
	    if(timeToLive > 0)
        {
            timeToLive -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
	}


}
