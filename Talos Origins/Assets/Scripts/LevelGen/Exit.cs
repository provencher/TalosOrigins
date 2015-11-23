using UnityEngine;
using System.Collections;

public class Exit : MonoBehaviour {
	
    
    void NewExit(Vector3 position)
    {
        
        transform.position = position;
        transform.Find("Stars_Flyby_Fast").position = transform.position;

    }
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll != null)
        {
            if (coll.gameObject.tag == "Player")
            {
                GameObject.Find("MapGenerator").SendMessage("NextLevel");
            }
        }
    }     
}
