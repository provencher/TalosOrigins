using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrbController : MonoBehaviour {
    public GameObject attachedParent; 

    public GameObject blueOrbPrefab;   

    public void SpawnOrb(int colorNum)
    {
        Vector2 offSet = new Vector2(Random.Range(-1.2f, 1.2f), Random.Range(-1.2f, 1.2f));
        switch (colorNum)
        {
            case 0:
                Instantiate(blueOrbPrefab, (Vector2)gameObject.GetComponentInParent<Transform>().position + offSet, Quaternion.identity);
                break;
            case 1:

                break;
            default:
                Instantiate(blueOrbPrefab, (Vector2)gameObject.GetComponentInParent<Transform>().position + offSet, Quaternion.identity);
                break;
        }
    }   
          
    
}
