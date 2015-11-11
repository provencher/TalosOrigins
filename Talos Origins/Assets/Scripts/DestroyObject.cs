using UnityEngine;
using System.Collections;

public class DestroyObject : MonoBehaviour {
    public float timeToLive = 2.0f;   
    
		
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
